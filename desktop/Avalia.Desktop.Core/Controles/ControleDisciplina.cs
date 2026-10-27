using Avalia.Desktop.Core.Excecoes;
using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Core.Servicos;

namespace Avalia.Desktop.Core.Controles;

/// <summary>
/// Controle ControleDisciplina: listar, consultar, incluir, editar, desativar
/// e reativar as disciplinas e o professor responsável (RF02, RN07, RN12, RN27).
/// </summary>
public class ControleDisciplina(IDisciplinaRepository disciplinas, IUsuarioRepository usuarios)
{
    public const int TamanhoMaximoNome = 100;
    public const int TamanhoMaximoDescricao = 500;

    public Task<IReadOnlyList<Disciplina>> Listar(FiltroCadastro filtro) => disciplinas.Listar(filtro);

    /// <summary>consultar(id) → Disciplina.consultar(id)</summary>
    public Task<Disciplina?> Consultar(int id) => disciplinas.Consultar(id);

    /// <summary>Todas as disciplinas (inclusive desativadas, identificadas), para filtros.</summary>
    public Task<IReadOnlyList<OpcaoSelecao>> ListarTodas() => disciplinas.ListarOpcoes(somenteAtivas: false);

    /// <summary>
    /// Disciplinas ativas para o cadastro de questões. Na edição de uma questão
    /// de disciplina desativada, a disciplina atual também aparece.
    /// </summary>
    public async Task<IReadOnlyList<OpcaoSelecao>> ListarAtivas(int? idDisciplinaAtual = null)
    {
        var todas = await disciplinas.ListarOpcoes(somenteAtivas: false);
        return todas.Where(d => d.Ativo || d.Id == idDisciplinaAtual).ToList();
    }

    /// <summary>incluir(disciplina) → Disciplina.incluir(disciplina)</summary>
    public async Task<ResultadoOperacao<int>> Incluir(Disciplina dados)
    {
        var disciplina = Normalizar(dados);
        var erros = await ValidarAsync(disciplina, null);
        if (erros.PossuiErros)
            return ResultadoOperacao<int>.Falha(erros);

        try
        {
            var id = await disciplinas.Incluir(disciplina);
            return ResultadoOperacao<int>.Ok(id, "Disciplina cadastrada com sucesso.");
        }
        catch (ViolacaoUnicidadeException)
        {
            return ResultadoOperacao<int>.Falha("Já existe uma disciplina com este nome.", nameof(Disciplina.Nome));
        }
    }

    /// <summary>editar(id, disciplina) → Disciplina.editar(id, disciplina). Disciplina inativa não é editada.</summary>
    public async Task<ResultadoOperacao<int>> Editar(int id, Disciplina dados)
    {
        var atual = await disciplinas.Consultar(id);
        if (atual is null)
            return ResultadoOperacao<int>.Falha("Disciplina não encontrada.");
        if (!atual.Ativo)
            return ResultadoOperacao<int>.Falha("A disciplina está inativa e não pode ser editada. Reative-a antes.");

        var disciplina = Normalizar(dados);
        disciplina.Id = id;
        var erros = await ValidarAsync(disciplina, atual);
        if (erros.PossuiErros)
            return ResultadoOperacao<int>.Falha(erros);

        try
        {
            await disciplinas.Editar(id, disciplina);
            return ResultadoOperacao<int>.Ok(id, "Disciplina atualizada com sucesso.");
        }
        catch (ViolacaoUnicidadeException)
        {
            return ResultadoOperacao<int>.Falha("Já existe uma disciplina com este nome.", nameof(Disciplina.Nome));
        }
    }

    /// <summary>Texto de solicitarDesativacao(mensagem).</summary>
    public async Task<string> MensagemDesativacao(int id)
    {
        var disciplina = await disciplinas.Consultar(id);
        var questoes = await disciplinas.ContarQuestoesAtivas(id);

        return $"Deseja desativar a disciplina \"{disciplina?.Nome}\"?\n\n" +
               $"As {questoes} questão(ões) ativa(s) desta disciplina deixarão de entrar no sorteio de novos simulados " +
               "e ela não aparecerá no cadastro de novas questões. O histórico é mantido e é possível reativá-la depois.";
    }

    /// <summary>desativar(id) → Disciplina.desativar(id)</summary>
    public async Task<ResultadoOperacao> Desativar(int id)
    {
        var disciplina = await disciplinas.Consultar(id);
        if (disciplina is null)
            return ResultadoOperacao.Falha("Disciplina não encontrada.");
        if (!disciplina.Ativo)
            return ResultadoOperacao.Falha("A disciplina já está desativada.");

        await disciplinas.Desativar(id);
        return ResultadoOperacao.Ok(
            $"Disciplina \"{disciplina.Nome}\" desativada. As questões dela saíram do sorteio de simulados.");
    }

    public async Task<ResultadoOperacao> Reativar(int id)
    {
        var disciplina = await disciplinas.Consultar(id);
        if (disciplina is null)
            return ResultadoOperacao.Falha("Disciplina não encontrada.");
        if (disciplina.Ativo)
            return ResultadoOperacao.Falha("A disciplina já está ativa.");

        await disciplinas.Reativar(id);
        return ResultadoOperacao.Ok($"Disciplina \"{disciplina.Nome}\" reativada.");
    }

    private static Disciplina Normalizar(Disciplina dados) => new()
    {
        Id = dados.Id,
        Nome = string.Join(' ', (dados.Nome ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries)),
        Descricao = string.IsNullOrWhiteSpace(dados.Descricao) ? null : dados.Descricao.Trim(),
        IdProfessorResponsavel = dados.IdProfessorResponsavel is > 0 ? dados.IdProfessorResponsavel : null
    };

    private async Task<ListaErros> ValidarAsync(Disciplina disciplina, Disciplina? atual)
    {
        var erros = new ListaErros();
        if (disciplina.Nome.Length == 0)
            erros.Adicionar(nameof(Disciplina.Nome), "Informe o nome da disciplina.");
        else if (disciplina.Nome.Length > TamanhoMaximoNome)
            erros.Adicionar(nameof(Disciplina.Nome), $"O nome deve ter no máximo {TamanhoMaximoNome} caracteres.");

        if (disciplina.Descricao?.Length > TamanhoMaximoDescricao)
            erros.Adicionar(nameof(Disciplina.Descricao), $"A descrição deve ter no máximo {TamanhoMaximoDescricao} caracteres.");

        await ValidarResponsavelAsync(disciplina, atual, erros);

        if (!erros.Any(e => e.Campo == nameof(Disciplina.Nome))
            && await disciplinas.ExisteNome(disciplina.Nome, atual?.Id))
            erros.Adicionar(nameof(Disciplina.Nome), "Já existe uma disciplina com este nome.");

        return erros;
    }

    /// <summary>RN07: no máximo um responsável, que deve ser um Professor ativo (ou nenhum).</summary>
    private async Task ValidarResponsavelAsync(Disciplina disciplina, Disciplina? atual, ListaErros erros)
    {
        if (disciplina.IdProfessorResponsavel is not int idResponsavel)
            return;

        var professor = await usuarios.Consultar(idResponsavel);
        if (professor is null || professor.Perfil != PerfilUsuario.Professor)
            erros.Adicionar(nameof(Disciplina.IdProfessorResponsavel), "O responsável deve ser um usuário com perfil Professor.");
        else if (!professor.Ativo && atual?.IdProfessorResponsavel != idResponsavel)
            erros.Adicionar(nameof(Disciplina.IdProfessorResponsavel), "O professor selecionado está desativado.");
    }
}
