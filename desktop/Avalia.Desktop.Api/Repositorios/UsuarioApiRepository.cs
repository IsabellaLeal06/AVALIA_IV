using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Modelos;

namespace Avalia.Desktop.Api.Repositorios;

/// <summary>Classe Usuario do diagrama, pela API (/usuarios). A API nunca devolve o CPF na listagem (RNF02).</summary>
public class UsuarioApiRepository(ClienteApi api) : RepositorioApiBase(api), IUsuarioRepository
{
    // Campos da API → campos das telas do Desktop
    private static readonly Dictionary<string, string> Campos = new()
    {
        ["nome"] = nameof(Usuario.Nome),
        ["cpf"] = nameof(Usuario.Cpf),
        ["email"] = nameof(Usuario.Email),
        ["idPerfil"] = nameof(Usuario.Perfil),
        ["idTurma"] = nameof(Usuario.IdTurma),
        ["dataIngresso"] = nameof(Usuario.DataIngresso)
    };

    public async Task<Usuario?> Consultar(int id)
    {
        var u = await Ler(() => Api.Get<UsuarioDto>($"usuarios/{id}"));
        return u is null ? null : new Usuario
        {
            Id = u.Id,
            Nome = u.Nome,
            Cpf = u.Cpf ?? string.Empty,
            Email = u.Email,
            DataIngresso = u.DataIngresso ?? DateTime.Today,
            Ativo = u.Ativo,
            Perfil = (PerfilUsuario)u.IdPerfil,
            IdTurma = u.IdTurma,
            NomeTurma = u.Turma
        };
    }

    public async Task<IReadOnlyList<UsuarioResumo>> Listar(FiltroUsuarios filtro)
    {
        var lista = await Buscar(filtro.Perfil, filtro.IdTurma, Situacao(filtro.Situacao), filtro.Busca);
        return lista.Select(u => new UsuarioResumo
        {
            Id = u.Id,
            Nome = u.Nome,
            Email = u.Email,
            Perfil = (PerfilUsuario)u.IdPerfil,
            NomeTurma = u.Turma,
            Ativo = u.Ativo
        }).ToList();
    }

    private async Task<List<UsuarioDto>> Buscar(PerfilUsuario? perfil, int? idTurma, string situacao, string? busca) =>
        await Ler(() => Api.Get<List<UsuarioDto>>("usuarios",
            ("perfil", perfil is null ? null : ((int)perfil.Value).ToString()),
            ("turma", idTurma?.ToString()),
            ("situacao", situacao),
            ("busca", busca))) ?? [];

    public async Task<bool> ExisteEmail(string email, int? ignorarId = null)
    {
        var encontrados = await Buscar(null, null, "todos", email);
        return encontrados.Any(u => u.Id != ignorarId && string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// A API não devolve CPF nas listagens (RNF02), então não há como conferir antes de gravar.
    /// A conferência é da API, que recusa o CPF repetido (400, campo cpf): ver <see cref="Incluir"/>.
    /// </summary>
    public Task<bool> ExisteCpf(string cpf, int? ignorarId = null) => Task.FromResult(false);

    private static object Corpo(Usuario u) => new
    {
        nome = u.Nome,
        cpf = u.Cpf,
        email = u.Email,
        idPerfil = (int)u.Perfil,
        idTurma = u.IdTurma,
        dataIngresso = u.DataIngresso.ToString("yyyy-MM-dd")
    };

    /// <summary>POST /usuarios. A senha inicial é o CPF: quem grava o hash BCrypt é a API (RN02).</summary>
    public async Task<int> Incluir(Usuario usuario)
    {
        var resposta = await Escrever(() => Api.Post<IncluidoDto>("usuarios", Corpo(usuario)), Campos);
        return resposta!.Id;
    }

    public async Task Editar(int id, Usuario usuario) =>
        await Escrever(() => Api.Put<IncluidoDto>($"usuarios/{id}", Corpo(usuario)), Campos);

    public Task Desativar(int id) => Acao(() => Api.Patch<object>($"usuarios/{id}/situacao", new { ativo = false }));

    public Task Reativar(int id) => Acao(() => Api.Patch<object>($"usuarios/{id}/situacao", new { ativo = true }));

    public async Task<IReadOnlyList<OpcaoSelecao>> ListarProfessores(bool somenteAtivos)
    {
        var professores = await Buscar(PerfilUsuario.Professor, null, somenteAtivos ? "ativo" : "todos", null);
        return professores.Select(p => new OpcaoSelecao(p.Id, p.Nome, p.Ativo)).ToList();
    }

    public async Task<int> ContarDisciplinasSobResponsabilidade(int idUsuario)
    {
        var disciplinas = await Ler(() => Api.Get<List<DisciplinaDto>>("disciplinas", ("situacao", "todos"))) ?? [];
        return disciplinas.Count(d => d.IdResponsavel == idUsuario);
    }

    public async Task<int> ContarAtivos(PerfilUsuario? perfil = null) =>
        perfil is null
            ? (await Painel()).Usuarios
            : (await Buscar(perfil, null, "ativo", null)).Count;
}
