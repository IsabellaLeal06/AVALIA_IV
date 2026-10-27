using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Data.Conexao;
using Dapper;

namespace Avalia.Desktop.Data.Repositorios;

public class UsuarioRepository(IFabricaConexao fabrica) : RepositorioBase(fabrica), IUsuarioRepository
{
    private const string SelecaoCompleta = """
        SELECT u.Id_Usuario             AS Id,
               u.Nome_Usuario           AS Nome,
               u.CPF_Usuario            AS Cpf,
               u.Email_Usuario          AS Email,
               u.Senha_Usuario          AS SenhaHash,
               u.FotoUrl_Usuario        AS FotoUrl,
               u.DataIngresso_Usuario   AS DataIngresso,
               u.Ativo_Usuario          AS Ativo,
               u.PontuacaoTotal_Usuario AS PontuacaoTotal,
               u.Id_Perfil              AS Perfil,
               u.Id_Turma               AS IdTurma,
               t.Nome_Turma             AS NomeTurma
        FROM dbo.Usuario u
        LEFT JOIN dbo.Turma t ON t.Id_Turma = u.Id_Turma
        """;

    public Task<Usuario?> Consultar(int id) =>
        ExecutarAsync(c => c.QuerySingleOrDefaultAsync<Usuario>(
            SelecaoCompleta + " WHERE u.Id_Usuario = @Id;", new { Id = id }));

    public Task<Usuario?> ConsultarPorEmail(string email) =>
        ExecutarAsync(c => c.QuerySingleOrDefaultAsync<Usuario>(
            SelecaoCompleta + " WHERE u.Email_Usuario = @Email;", new { Email = Email(email) }));

    public Task<IReadOnlyList<UsuarioResumo>> Listar(FiltroUsuarios filtro) =>
        ExecutarAsync(async c => (IReadOnlyList<UsuarioResumo>)(await c.QueryAsync<UsuarioResumo>("""
            SELECT u.Id_Usuario    AS Id,
                   u.Nome_Usuario  AS Nome,
                   u.Email_Usuario AS Email,
                   u.Id_Perfil     AS Perfil,
                   CASE WHEN t.Id_Turma IS NULL THEN NULL
                        ELSE CONCAT(t.Nome_Turma, N' (', t.AnoLetivo_Turma, N')') END AS NomeTurma,
                   u.Ativo_Usuario AS Ativo
            FROM dbo.Usuario u
            LEFT JOIN dbo.Turma t ON t.Id_Turma = u.Id_Turma
            WHERE (@Perfil IS NULL OR u.Id_Perfil = @Perfil)
              AND (@IdTurma IS NULL OR u.Id_Turma = @IdTurma)
              AND (@Ativo IS NULL OR u.Ativo_Usuario = @Ativo)
              AND (@Busca IS NULL OR u.Nome_Usuario LIKE @Busca ESCAPE '\' OR u.Email_Usuario LIKE @Busca ESCAPE '\')
            ORDER BY u.Nome_Usuario;
            """,
            new
            {
                Perfil = (int?)filtro.Perfil,
                filtro.IdTurma,
                Ativo = ParametroAtivo(filtro.Situacao),
                Busca = ParametroBusca(filtro.Busca)
            })).AsList());

    public Task<bool> ExisteEmail(string email, int? ignorarId = null) =>
        ExecutarAsync(c => c.ExecuteScalarAsync<bool>("""
            SELECT CAST(CASE WHEN EXISTS (
                SELECT 1 FROM dbo.Usuario
                WHERE Email_Usuario = @Email AND (@IgnorarId IS NULL OR Id_Usuario <> @IgnorarId)
            ) THEN 1 ELSE 0 END AS BIT);
            """, new { Email = Email(email), IgnorarId = ignorarId }));

    public Task<bool> ExisteCpf(string cpf, int? ignorarId = null) =>
        ExecutarAsync(c => c.ExecuteScalarAsync<bool>("""
            SELECT CAST(CASE WHEN EXISTS (
                SELECT 1 FROM dbo.Usuario
                WHERE CPF_Usuario = @Cpf AND (@IgnorarId IS NULL OR Id_Usuario <> @IgnorarId)
            ) THEN 1 ELSE 0 END AS BIT);
            """, new { Cpf = Cpf(cpf), IgnorarId = ignorarId }));

    public Task<int> Incluir(Usuario usuario) =>
        ExecutarAsync(c => c.ExecuteScalarAsync<int>("""
            INSERT INTO dbo.Usuario
                (Nome_Usuario, CPF_Usuario, Email_Usuario, Senha_Usuario, FotoUrl_Usuario,
                 DataIngresso_Usuario, Ativo_Usuario, PontuacaoTotal_Usuario, Id_Perfil, Id_Turma)
            VALUES
                (@Nome, @Cpf, @Email, @SenhaHash, @FotoUrl,
                 @DataIngresso, 1, 0, @Perfil, @IdTurma);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """,
            new
            {
                usuario.Nome,
                Cpf = Cpf(usuario.Cpf),
                Email = Email(usuario.Email),
                usuario.SenhaHash,
                usuario.FotoUrl,
                usuario.DataIngresso,
                Perfil = (int)usuario.Perfil,
                usuario.IdTurma
            }));

    public Task Editar(int id, Usuario usuario) =>
        ExecutarAsync(c => c.ExecuteAsync("""
            UPDATE dbo.Usuario
               SET Nome_Usuario         = @Nome,
                   CPF_Usuario          = @Cpf,
                   Email_Usuario        = @Email,
                   DataIngresso_Usuario = @DataIngresso,
                   Id_Perfil            = @Perfil,
                   Id_Turma             = @IdTurma
             WHERE Id_Usuario = @Id;
            """,
            new
            {
                Id = id,
                usuario.Nome,
                Cpf = Cpf(usuario.Cpf),
                Email = Email(usuario.Email),
                usuario.DataIngresso,
                Perfil = (int)usuario.Perfil,
                usuario.IdTurma
            }));

    public Task Desativar(int id) => AlterarSituacao(id, false);

    public Task Reativar(int id) => AlterarSituacao(id, true);

    private Task AlterarSituacao(int id, bool ativo) =>
        ExecutarAsync(c => c.ExecuteAsync(
            "UPDATE dbo.Usuario SET Ativo_Usuario = @Ativo WHERE Id_Usuario = @Id;",
            new { Id = id, Ativo = ativo }));

    public Task AlterarSenha(int id, string hash) =>
        // A troca de senha também invalida um código de recuperação pendente
        ExecutarAsync(c => c.ExecuteAsync("""
            UPDATE dbo.Usuario
               SET Senha_Usuario            = @SenhaHash,
                   TokenRecuperacao_Usuario = NULL,
                   ValidadeToken_Usuario    = NULL
             WHERE Id_Usuario = @Id;
            """, new { Id = id, SenhaHash = hash }));

    public Task<IReadOnlyList<OpcaoSelecao>> ListarProfessores(bool somenteAtivos) =>
        ExecutarAsync(async c => (IReadOnlyList<OpcaoSelecao>)(await c.QueryAsync<(int Id, string Nome, bool Ativo)>("""
            SELECT Id_Usuario, Nome_Usuario, Ativo_Usuario
            FROM dbo.Usuario
            WHERE Id_Perfil = @Professor AND (@SomenteAtivos = 0 OR Ativo_Usuario = 1)
            ORDER BY Nome_Usuario;
            """, new { Professor = (int)PerfilUsuario.Professor, SomenteAtivos = somenteAtivos }))
            .Select(p => new OpcaoSelecao(p.Id, p.Nome, p.Ativo))
            .ToList());

    public Task<int> ContarDisciplinasSobResponsabilidade(int idUsuario) =>
        ExecutarAsync(c => c.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM dbo.Disciplina WHERE Id_Usuario = @Id;", new { Id = idUsuario }));

    public Task<int> ContarAtivos(PerfilUsuario? perfil = null) =>
        ExecutarAsync(c => c.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM dbo.Usuario WHERE Ativo_Usuario = 1 AND (@Perfil IS NULL OR Id_Perfil = @Perfil);",
            new { Perfil = (int?)perfil }));

    // E-mail e CPF são VARCHAR/CHAR no banco: parâmetros ANSI evitam conversão implícita e usam os índices
    private static DbString Email(string email) => new() { Value = email, IsAnsi = true, Length = 150 };

    private static DbString Cpf(string cpf) => new() { Value = cpf, IsAnsi = true, IsFixedLength = true, Length = 11 };
}
