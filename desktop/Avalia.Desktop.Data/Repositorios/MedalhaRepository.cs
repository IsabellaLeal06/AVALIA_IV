using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Data.Conexao;
using Dapper;

namespace Avalia.Desktop.Data.Repositorios;

/// <summary>Faixas de medalha (RN22). A medalha do aluno não é gravada: é apurada pela pontuação.</summary>
public class MedalhaRepository(IFabricaConexao fabrica) : RepositorioBase(fabrica), IMedalhaRepository
{
    private const string Selecao = """
        SELECT Id_Medalha            AS Id,
               Nome_Medalha          AS Nome,
               PontosMinimos_Medalha AS PontosMinimos,
               ImagemUrl_Medalha     AS ImagemUrl
        FROM dbo.Medalha
        """;

    public Task<IReadOnlyList<Medalha>> Listar() =>
        ExecutarAsync(async c => (IReadOnlyList<Medalha>)(await c.QueryAsync<Medalha>("""
            WITH Faixas AS (
                SELECT Id_Medalha, Nome_Medalha, PontosMinimos_Medalha, ImagemUrl_Medalha,
                       LEAD(PontosMinimos_Medalha) OVER (ORDER BY PontosMinimos_Medalha) AS PontosProximaFaixa
                FROM dbo.Medalha
            )
            SELECT f.Id_Medalha            AS Id,
                   f.Nome_Medalha          AS Nome,
                   f.PontosMinimos_Medalha AS PontosMinimos,
                   f.ImagemUrl_Medalha     AS ImagemUrl,
                   f.PontosProximaFaixa,
                   (SELECT COUNT(*) FROM dbo.Usuario u
                     WHERE u.Id_Perfil = @Aluno AND u.Ativo_Usuario = 1
                       AND u.PontuacaoTotal_Usuario >= f.PontosMinimos_Medalha
                       AND (f.PontosProximaFaixa IS NULL OR u.PontuacaoTotal_Usuario < f.PontosProximaFaixa)) AS QtdAlunos
            FROM Faixas f
            ORDER BY f.PontosMinimos_Medalha;
            """, new { Aluno = (int)PerfilUsuario.Aluno })).AsList());

    public Task<Medalha?> Consultar(int id) =>
        ExecutarAsync(c => c.QuerySingleOrDefaultAsync<Medalha>(Selecao + " WHERE Id_Medalha = @Id;", new { Id = id }));

    public Task<bool> ExisteNome(string nome, int? ignorarId = null) =>
        ExecutarAsync(c => c.ExecuteScalarAsync<bool>("""
            SELECT CAST(CASE WHEN EXISTS (
                SELECT 1 FROM dbo.Medalha
                WHERE Nome_Medalha = @Nome AND (@IgnorarId IS NULL OR Id_Medalha <> @IgnorarId)
            ) THEN 1 ELSE 0 END AS BIT);
            """, new { Nome = nome, IgnorarId = ignorarId }));

    public Task<bool> ExistePontosMinimos(int pontosMinimos, int? ignorarId = null) =>
        ExecutarAsync(c => c.ExecuteScalarAsync<bool>("""
            SELECT CAST(CASE WHEN EXISTS (
                SELECT 1 FROM dbo.Medalha
                WHERE PontosMinimos_Medalha = @Pontos AND (@IgnorarId IS NULL OR Id_Medalha <> @IgnorarId)
            ) THEN 1 ELSE 0 END AS BIT);
            """, new { Pontos = pontosMinimos, IgnorarId = ignorarId }));

    public Task<int> Incluir(Medalha medalha) =>
        ExecutarAsync(c => c.ExecuteScalarAsync<int>("""
            INSERT INTO dbo.Medalha (Nome_Medalha, PontosMinimos_Medalha, ImagemUrl_Medalha)
            VALUES (@Nome, @PontosMinimos, @ImagemUrl);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """, new { medalha.Nome, medalha.PontosMinimos, ImagemUrl = Url(medalha.ImagemUrl) }));

    public Task Editar(int id, Medalha medalha) =>
        ExecutarAsync(c => c.ExecuteAsync("""
            UPDATE dbo.Medalha
               SET Nome_Medalha          = @Nome,
                   PontosMinimos_Medalha = @PontosMinimos,
                   ImagemUrl_Medalha     = @ImagemUrl
             WHERE Id_Medalha = @Id;
            """, new { Id = id, medalha.Nome, medalha.PontosMinimos, ImagemUrl = Url(medalha.ImagemUrl) }));

    public Task Excluir(int id) =>
        ExecutarAsync(c => c.ExecuteAsync("DELETE FROM dbo.Medalha WHERE Id_Medalha = @Id;", new { Id = id }));

    public Task<int> QtdAlunosSemMedalha() =>
        ExecutarAsync(c => c.ExecuteScalarAsync<int>("""
            SELECT COUNT(*) FROM dbo.Usuario u
            WHERE u.Id_Perfil = @Aluno AND u.Ativo_Usuario = 1
              AND u.PontuacaoTotal_Usuario < COALESCE((SELECT MIN(PontosMinimos_Medalha) FROM dbo.Medalha), 2147483647);
            """, new { Aluno = (int)PerfilUsuario.Aluno }));

    // ImagemUrl_Medalha é VARCHAR no banco
    private static DbString Url(string url) => new() { Value = url, IsAnsi = true, Length = 500 };
}
