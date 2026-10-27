using Avalia.Desktop.Core.Excecoes;
using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Core.Servicos;
using Avalia.Desktop.Core.Validacoes;

namespace Avalia.Desktop.Core.Controles;

/// <summary>
/// Controle ControleUsuario: listar, consultarPorId, incluir, editar,
/// desativar, reativar e listarProfessoresAtivos
/// (RF02, RN01, RN02, RN05, RN12, RN27).
/// </summary>
public class ControleUsuario(
    IUsuarioRepository usuarios,
    ITurmaRepository turmas,
    IHashSenha hashSenha,
    SessaoUsuario sessao)
{
    public const int TamanhoMaximoNome = 150;

    public Task<IReadOnlyList<UsuarioResumo>> Listar(FiltroUsuarios filtro) => usuarios.Listar(filtro);

    /// <summary>consultarPorId(id) → Usuario.consultar(id). Devolve os dados com CPF e sem o hash da senha.</summary>
    public async Task<Usuario?> ConsultarPorId(int id)
    {
        var usuario = await usuarios.Consultar(id);
        if (usuario is not null)
            usuario.SenhaHash = string.Empty;
        return usuario;
    }

    /// <summary>
    /// Professores ativos para o ComboBox de responsável pela disciplina. Se o
    /// responsável atual estiver desativado, ele também aparece, para não ser trocado sem querer.
    /// </summary>
    public async Task<IReadOnlyList<OpcaoSelecao>> ListarProfessoresAtivos(int? idResponsavelAtual = null)
    {
        var professores = await usuarios.ListarProfessores(somenteAtivos: false);
        return professores.Where(p => p.Ativo || p.Id == idResponsavelAtual).ToList();
    }

    /// <summary>incluir(usuario) → Usuario.incluir(usuario). A senha inicial é o CPF (RN02).</summary>
    public async Task<ResultadoOperacao<int>> Incluir(Usuario dados)
    {
        var usuario = Normalizar(dados);
        var erros = await ValidarAsync(usuario, null);
        if (erros.PossuiErros)
            return ResultadoOperacao<int>.Falha(erros);

        usuario.SenhaHash = hashSenha.Gerar(usuario.Cpf);
        usuario.Ativo = true;
        usuario.PontuacaoTotal = 0;
        try
        {
            var id = await usuarios.Incluir(usuario);
            return ResultadoOperacao<int>.Ok(id, "Usuário cadastrado com sucesso.\nA senha inicial é o CPF (somente números).");
        }
        catch (ViolacaoUnicidadeException ex)
        {
            return FalhaUnicidade(ex);
        }
    }

    /// <summary>editar(id, usuario) → Usuario.editar(id, usuario). Usuário inativo não pode ser editado.</summary>
    public async Task<ResultadoOperacao<int>> Editar(int id, Usuario dados)
    {
        var atual = await usuarios.Consultar(id);
        if (atual is null)
            return ResultadoOperacao<int>.Falha("Usuário não encontrado.");
        if (!atual.Ativo)
            return ResultadoOperacao<int>.Falha("O usuário está inativo e não pode ser editado. Reative-o antes.");

        var usuario = Normalizar(dados);
        usuario.Id = id;
        var erros = await ValidarAsync(usuario, atual);
        if (erros.PossuiErros)
            return ResultadoOperacao<int>.Falha(erros);

        try
        {
            await usuarios.Editar(id, usuario);
            return ResultadoOperacao<int>.Ok(id, "Usuário atualizado com sucesso.");
        }
        catch (ViolacaoUnicidadeException ex)
        {
            return FalhaUnicidade(ex);
        }
    }

    /// <summary>Texto de solicitarDesativacao(mensagem), com avisos quando necessário.</summary>
    public async Task<string> MensagemDesativacao(int id)
    {
        var usuario = await usuarios.Consultar(id);
        if (usuario is null)
            return "Deseja desativar este usuário?";

        var texto = $"Deseja desativar o usuário \"{usuario.Nome}\"?\n\n" +
                    "Ele não conseguirá mais entrar no sistema, mas o histórico é mantido. " +
                    "É possível reativá-lo depois.";

        if (usuario.Perfil == PerfilUsuario.Professor)
        {
            var disciplinas = await usuarios.ContarDisciplinasSobResponsabilidade(id);
            if (disciplinas > 0)
                texto += $"\n\nAtenção: este professor é responsável por {disciplinas} disciplina(s). " +
                         "Lembre-se de definir outro responsável em Disciplinas.";
        }

        return texto;
    }

    /// <summary>desativar(id) → Usuario.desativar(id). RN12: desativação lógica, nunca DELETE.</summary>
    public async Task<ResultadoOperacao> Desativar(int id)
    {
        if (sessao.Usuario?.Id == id)
            return ResultadoOperacao.Falha("Você não pode desativar a sua própria conta.");

        var usuario = await usuarios.Consultar(id);
        if (usuario is null)
            return ResultadoOperacao.Falha("Usuário não encontrado.");
        if (!usuario.Ativo)
            return ResultadoOperacao.Falha("O usuário já está desativado.");

        await usuarios.Desativar(id);
        return ResultadoOperacao.Ok($"Usuário \"{usuario.Nome}\" desativado.");
    }

    public async Task<ResultadoOperacao> Reativar(int id)
    {
        var usuario = await usuarios.Consultar(id);
        if (usuario is null)
            return ResultadoOperacao.Falha("Usuário não encontrado.");
        if (usuario.Ativo)
            return ResultadoOperacao.Falha("O usuário já está ativo.");

        await usuarios.Reativar(id);
        return ResultadoOperacao.Ok($"Usuário \"{usuario.Nome}\" reativado.");
    }

    private static ResultadoOperacao<int> FalhaUnicidade(ViolacaoUnicidadeException ex) =>
        // Outro administrador gravou o mesmo e-mail/CPF entre a validação e a gravação
        ex.Restricao?.Contains("CPF", StringComparison.OrdinalIgnoreCase) == true
            ? ResultadoOperacao<int>.Falha("Já existe um usuário com este CPF.", nameof(Usuario.Cpf))
            : ResultadoOperacao<int>.Falha("Já existe um usuário com este e-mail.", nameof(Usuario.Email));

    private async Task<ListaErros> ValidarAsync(Usuario usuario, Usuario? atual)
    {
        var erros = new ListaErros();
        ValidarCampos(usuario, erros);
        await ValidarTurmaAsync(usuario, atual, erros);
        await ValidarAlteracaoDePerfilAsync(usuario, atual, erros);

        if (!erros.Any(e => e.Campo == nameof(Usuario.Email))
            && await usuarios.ExisteEmail(usuario.Email, atual?.Id))
            erros.Adicionar(nameof(Usuario.Email), "Já existe um usuário com este e-mail.");

        if (!erros.Any(e => e.Campo == nameof(Usuario.Cpf))
            && await usuarios.ExisteCpf(usuario.Cpf, atual?.Id))
            erros.Adicionar(nameof(Usuario.Cpf), "Já existe um usuário com este CPF.");

        return erros;
    }

    private static Usuario Normalizar(Usuario dados) => new()
    {
        Id = dados.Id,
        Nome = string.Join(' ', (dados.Nome ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries)),
        Cpf = ValidadorCpf.Normalizar(dados.Cpf),
        Email = ValidadorEmail.Normalizar(dados.Email),
        Perfil = dados.Perfil,
        // RN05: somente Aluno possui turma
        IdTurma = dados.Perfil == PerfilUsuario.Aluno ? dados.IdTurma : null,
        DataIngresso = dados.DataIngresso.Date,
        FotoUrl = dados.FotoUrl
    };

    private static void ValidarCampos(Usuario usuario, ListaErros erros)
    {
        if (usuario.Nome.Length < 3)
            erros.Adicionar(nameof(Usuario.Nome), "Informe o nome completo.");
        else if (usuario.Nome.Length > TamanhoMaximoNome)
            erros.Adicionar(nameof(Usuario.Nome), $"O nome deve ter no máximo {TamanhoMaximoNome} caracteres.");

        if (usuario.Cpf.Length == 0)
            erros.Adicionar(nameof(Usuario.Cpf), "Informe o CPF.");
        else if (!ValidadorCpf.EhValido(usuario.Cpf))
            erros.Adicionar(nameof(Usuario.Cpf), "CPF inválido. Confira os dígitos.");

        if (usuario.Email.Length == 0)
            erros.Adicionar(nameof(Usuario.Email), "Informe o e-mail.");
        else if (!ValidadorEmail.EhValido(usuario.Email))
            erros.Adicionar(nameof(Usuario.Email), "E-mail inválido.");

        if (!Enum.IsDefined(usuario.Perfil))
            erros.Adicionar(nameof(Usuario.Perfil), "Selecione o perfil.");

        if (usuario.DataIngresso.Year < 2000 || usuario.DataIngresso > DateTime.Today.AddYears(1))
            erros.Adicionar(nameof(Usuario.DataIngresso), "Data de ingresso inválida.");
    }

    /// <summary>RN05: turma obrigatória somente para Aluno, e apenas turmas ativas.</summary>
    private async Task ValidarTurmaAsync(Usuario usuario, Usuario? atual, ListaErros erros)
    {
        if (usuario.Perfil != PerfilUsuario.Aluno)
            return;

        if (usuario.IdTurma is null)
        {
            erros.Adicionar(nameof(Usuario.IdTurma), "Selecione a turma do aluno.");
            return;
        }

        var turma = await turmas.Consultar(usuario.IdTurma.Value);
        if (turma is null)
            erros.Adicionar(nameof(Usuario.IdTurma), "Turma não encontrada.");
        else if (!turma.Ativo && atual?.IdTurma != turma.Id)
            erros.Adicionar(nameof(Usuario.IdTurma), "A turma selecionada está desativada. Escolha uma turma ativa.");
    }

    private async Task ValidarAlteracaoDePerfilAsync(Usuario usuario, Usuario? atual, ListaErros erros)
    {
        if (atual is null || atual.Perfil == usuario.Perfil)
            return;

        if (sessao.Usuario?.Id == atual.Id)
        {
            erros.Adicionar(nameof(Usuario.Perfil), "Você não pode alterar o seu próprio perfil.");
            return;
        }

        if (atual.Perfil == PerfilUsuario.Professor)
        {
            var disciplinas = await usuarios.ContarDisciplinasSobResponsabilidade(atual.Id);
            if (disciplinas > 0)
                erros.Adicionar(nameof(Usuario.Perfil),
                    $"Este professor é responsável por {disciplinas} disciplina(s). " +
                    "Defina outro responsável antes de alterar o perfil.");
        }
    }
}
