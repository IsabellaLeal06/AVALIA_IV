using Avalia.Desktop.Core.Excecoes;
using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Core.Servicos;

namespace Avalia.Desktop.Core.Controles;

/// <summary>
/// Controle ControleTurma: listar, consultarPorId, incluir, editar,
/// desativar e reativar as turmas e a grade de disciplinas (RF02, RN06, RN12).
/// </summary>
public class ControleTurma(ITurmaRepository turmas, IDisciplinaRepository disciplinas)
{
    public const int TamanhoMaximoNome = 100;
    public const int AnoMinimo = 2000;
    public const int AnoMaximo = 2100;

    public Task<IReadOnlyList<Turma>> Listar(FiltroCadastro filtro) => turmas.Listar(filtro);

    /// <summary>consultarPorId(id) → Turma.consultar(id)</summary>
    public Task<Turma?> ConsultarPorId(int id) => turmas.Consultar(id);

    /// <summary>Todas as turmas (inclusive desativadas, identificadas), para filtros.</summary>
    public Task<IReadOnlyList<OpcaoSelecao>> ListarTodas() => turmas.ListarOpcoes(somenteAtivas: false);

    /// <summary>
    /// Turmas ativas para o ComboBox do aluno. Se o aluno já está em uma turma
    /// desativada, ela também aparece, para não ser trocada sem querer.
    /// </summary>
    public async Task<IReadOnlyList<OpcaoSelecao>> ListarAtivas(int? idTurmaAtual = null)
    {
        var todas = await turmas.ListarOpcoes(somenteAtivas: false);
        return todas.Where(t => t.Ativo || t.Id == idTurmaAtual).ToList();
    }

    /// <summary>
    /// Disciplinas para o CheckedListBox (somente ativas) e as já vinculadas.
    /// Vínculos com disciplinas desativadas não aparecem para marcação, mas são mantidos.
    /// </summary>
    public async Task<VinculoDisciplinasTurma> ConsultarDisciplinas(int? idTurma)
    {
        var todas = await disciplinas.ListarOpcoes(somenteAtivas: false);
        var vinculadas = idTurma is > 0
            ? (await turmas.ListarIdsDisciplinasVinculadas(idTurma.Value)).ToHashSet()
            : [];

        return new VinculoDisciplinasTurma
        {
            DisciplinasAtivas = todas.Where(d => d.Ativo).ToList(),
            IdsVinculados = vinculadas,
            VinculadasInativas = todas.Where(d => !d.Ativo && vinculadas.Contains(d.Id)).ToList()
        };
    }

    /// <summary>incluir(turma, disciplinas) → Turma.incluir(turma, disciplinas)</summary>
    public async Task<ResultadoOperacao<int>> Incluir(Turma dados, IEnumerable<int> disciplinasMarcadas)
    {
        var turma = Normalizar(dados);
        var (erros, idsFinais) = await ValidarAsync(turma, null, disciplinasMarcadas);
        if (erros.PossuiErros)
            return ResultadoOperacao<int>.Falha(erros);

        try
        {
            var id = await turmas.Incluir(turma, idsFinais);
            return ResultadoOperacao<int>.Ok(id, "Turma cadastrada com sucesso.");
        }
        catch (ViolacaoUnicidadeException)
        {
            return FalhaDuplicada(turma);
        }
    }

    /// <summary>editar(id, turma, disciplinas) → Turma.editar(id, turma, disciplinas). Turma inativa não é editada.</summary>
    public async Task<ResultadoOperacao<int>> Editar(int id, Turma dados, IEnumerable<int> disciplinasMarcadas)
    {
        var atual = await turmas.Consultar(id);
        if (atual is null)
            return ResultadoOperacao<int>.Falha("Turma não encontrada.");
        if (!atual.Ativo)
            return ResultadoOperacao<int>.Falha("A turma está inativa e não pode ser editada. Reative-a antes.");

        var turma = Normalizar(dados);
        turma.Id = id;
        var (erros, idsFinais) = await ValidarAsync(turma, id, disciplinasMarcadas);
        if (erros.PossuiErros)
            return ResultadoOperacao<int>.Falha(erros);

        try
        {
            await turmas.Editar(id, turma, idsFinais);
            return ResultadoOperacao<int>.Ok(id, "Turma atualizada com sucesso.");
        }
        catch (ViolacaoUnicidadeException)
        {
            return FalhaDuplicada(turma);
        }
    }

    /// <summary>
    /// Texto de solicitarDesativacao(mensagem). RN12: se houver alunos ativos, alerta
    /// a quantidade, mas a desativação é permitida e os alunos não são desativados.
    /// </summary>
    public async Task<string> MensagemDesativacao(int id)
    {
        var turma = await turmas.Consultar(id);
        var nome = turma?.NomeCompleto ?? "selecionada";
        var alunos = await turmas.ContarAlunosAtivos(id);

        var texto = $"Deseja desativar a turma {nome}?\n\n" +
                    "Ela deixará de aparecer no cadastro de novos alunos. É possível reativá-la depois.";

        if (alunos > 0)
            texto += $"\n\nAtenção: há {alunos} aluno(s) ativo(s) vinculado(s) a esta turma. " +
                     "Eles NÃO serão desativados e continuarão com acesso ao sistema.";

        return texto;
    }

    /// <summary>desativar(id) → Turma.desativar(id)</summary>
    public async Task<ResultadoOperacao> Desativar(int id)
    {
        var turma = await turmas.Consultar(id);
        if (turma is null)
            return ResultadoOperacao.Falha("Turma não encontrada.");
        if (!turma.Ativo)
            return ResultadoOperacao.Falha("A turma já está desativada.");

        // Somente a turma muda de situação; os alunos permanecem como estão (RN12)
        await turmas.Desativar(id);
        var alunos = await turmas.ContarAlunosAtivos(id);

        return ResultadoOperacao.Ok(alunos > 0
            ? $"Turma {turma.NomeCompleto} desativada. Os {alunos} aluno(s) vinculado(s) continuam ativos."
            : $"Turma {turma.NomeCompleto} desativada.");
    }

    public async Task<ResultadoOperacao> Reativar(int id)
    {
        var turma = await turmas.Consultar(id);
        if (turma is null)
            return ResultadoOperacao.Falha("Turma não encontrada.");
        if (turma.Ativo)
            return ResultadoOperacao.Falha("A turma já está ativa.");

        await turmas.Reativar(id);
        return ResultadoOperacao.Ok($"Turma {turma.NomeCompleto} reativada.");
    }

    private static Turma Normalizar(Turma dados) => new()
    {
        Id = dados.Id,
        Nome = string.Join(' ', (dados.Nome ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries)),
        AnoLetivo = dados.AnoLetivo
    };

    private static ResultadoOperacao<int> FalhaDuplicada(Turma turma) =>
        ResultadoOperacao<int>.Falha($"Já existe a turma \"{turma.Nome}\" no ano letivo {turma.AnoLetivo}.", nameof(Turma.Nome));

    private async Task<(ListaErros Erros, List<int> IdsFinais)> ValidarAsync(Turma turma, int? id, IEnumerable<int> marcadasInformadas)
    {
        var erros = new ListaErros();
        if (turma.Nome.Length == 0)
            erros.Adicionar(nameof(Turma.Nome), "Informe o nome da turma (o período faz parte do nome, ex.: ADS 3º Semestre A).");
        else if (turma.Nome.Length > TamanhoMaximoNome)
            erros.Adicionar(nameof(Turma.Nome), $"O nome deve ter no máximo {TamanhoMaximoNome} caracteres.");

        if (turma.AnoLetivo is < AnoMinimo or > AnoMaximo)
            erros.Adicionar(nameof(Turma.AnoLetivo), $"Informe um ano letivo entre {AnoMinimo} e {AnoMaximo}.");

        var vinculo = await ConsultarDisciplinas(id);
        var marcadas = marcadasInformadas.Distinct().ToList();
        var idsAtivas = vinculo.DisciplinasAtivas.Select(d => d.Id).ToHashSet();
        if (marcadas.Any(x => !idsAtivas.Contains(x)))
            erros.Adicionar("Disciplinas", "Somente disciplinas ativas podem ser vinculadas à turma.");

        if (!erros.PossuiErros && await turmas.Existe(turma.Nome, turma.AnoLetivo, id))
            erros.Adicionar(nameof(Turma.Nome), $"Já existe a turma \"{turma.Nome}\" no ano letivo {turma.AnoLetivo}.");

        // Mantém os vínculos com disciplinas desativadas, que não aparecem para marcação
        var idsFinais = marcadas.Union(vinculo.VinculadasInativas.Select(d => d.Id)).ToList();
        return (erros, idsFinais);
    }
}
