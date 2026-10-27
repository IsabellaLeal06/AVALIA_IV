using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Data.Conexao;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Avalia.Desktop.Data.Repositorios;

public class QuestaoRepository(IFabricaConexao fabrica) : RepositorioBase(fabrica), IQuestaoRepository
{
    public Task<Questao?> Consultar(int id) =>
        ExecutarAsync(async c =>
        {
            using var resultado = await c.QueryMultipleAsync("""
                SELECT q.Id_Questao                 AS Id,
                       q.Enunciado_Questao          AS Enunciado,
                       q.ImagemUrl_Questao          AS ImagemUrl,
                       q.ExplicacaoResposta_Questao AS Explicacao,
                       q.Ativo_Questao              AS Ativo,
                       q.Id_Disciplina              AS IdDisciplina,
                       q.Id_Usuario                 AS IdUsuarioCadastro,
                       d.Nome_Disciplina            AS NomeDisciplina,
                       u.Nome_Usuario               AS NomeUsuarioCadastro
                FROM dbo.Questao q
                JOIN dbo.Disciplina d ON d.Id_Disciplina = q.Id_Disciplina
                JOIN dbo.Usuario u    ON u.Id_Usuario = q.Id_Usuario
                WHERE q.Id_Questao = @Id;

                SELECT Id_Alternativa      AS Id,
                       Letra_Alternativa   AS Letra,
                       Texto_Alternativa   AS Texto,
                       Correta_Alternativa AS Correta,
                       Id_Questao          AS IdQuestao
                FROM dbo.Alternativa
                WHERE Id_Questao = @Id
                ORDER BY Letra_Alternativa;
                """, new { Id = id });

            var questao = await resultado.ReadSingleOrDefaultAsync<Questao>();
            if (questao is not null)
                questao.Alternativas = (await resultado.ReadAsync<Alternativa>()).AsList();
            return questao;
        });

    public Task<IReadOnlyList<QuestaoResumo>> Listar(FiltroQuestoes filtro) =>
        ExecutarAsync(async c => (IReadOnlyList<QuestaoResumo>)(await c.QueryAsync<QuestaoResumo>("""
            SELECT q.Id_Questao               AS Id,
                   LEFT(q.Enunciado_Questao, 300) AS Enunciado,
                   d.Nome_Disciplina          AS NomeDisciplina,
                   d.Ativo_Disciplina         AS DisciplinaAtiva,
                   u.Nome_Usuario             AS NomeUsuarioCadastro,
                   q.Ativo_Questao            AS Ativo,
                   CAST(CASE WHEN EXISTS (SELECT 1 FROM dbo.SimuladoQuestao sq
                                          WHERE sq.Id_Questao = q.Id_Questao)
                             THEN 1 ELSE 0 END AS BIT) AS Utilizada
            FROM dbo.Questao q
            JOIN dbo.Disciplina d ON d.Id_Disciplina = q.Id_Disciplina
            JOIN dbo.Usuario u    ON u.Id_Usuario = q.Id_Usuario
            WHERE (@IdDisciplina IS NULL OR q.Id_Disciplina = @IdDisciplina)
              AND (@Ativo IS NULL OR q.Ativo_Questao = @Ativo)
              AND (@Busca IS NULL OR q.Enunciado_Questao LIKE @Busca ESCAPE '\')
            ORDER BY d.Nome_Disciplina, q.Id_Questao;
            """,
            new
            {
                filtro.IdDisciplina,
                Ativo = ParametroAtivo(filtro.Situacao),
                Busca = ParametroBusca(filtro.Busca)
            })).AsList());

    public Task<int> Incluir(Questao questao, IReadOnlyList<Alternativa> alternativas) =>
        ExecutarEmTransacaoAsync(async (c, tx) =>
        {
            var id = await c.ExecuteScalarAsync<int>("""
                INSERT INTO dbo.Questao
                    (Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario)
                VALUES
                    (@Enunciado, @ImagemUrl, @Explicacao, 1, @IdDisciplina, @IdUsuarioCadastro);
                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """,
                new
                {
                    questao.Enunciado,
                    questao.ImagemUrl,
                    questao.Explicacao,
                    questao.IdDisciplina,
                    questao.IdUsuarioCadastro
                }, tx);

            await c.ExecuteAsync("""
                INSERT INTO dbo.Alternativa (Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao)
                VALUES (@Letra, @Texto, @Correta, @IdQuestao);
                """,
                alternativas.Select(a => new { a.Letra, a.Texto, a.Correta, IdQuestao = id }), tx);

            return id;
        });

    public Task Editar(int id, Questao questao, IReadOnlyList<Alternativa> alternativas) =>
        ExecutarEmTransacaoAsync(async (c, tx) =>
        {
            await c.ExecuteAsync("""
                UPDATE dbo.Questao
                   SET Enunciado_Questao          = @Enunciado,
                       ImagemUrl_Questao          = @ImagemUrl,
                       ExplicacaoResposta_Questao = @Explicacao,
                       Id_Disciplina              = @IdDisciplina
                 WHERE Id_Questao = @Id;
                """,
                new { Id = id, questao.Enunciado, questao.ImagemUrl, questao.Explicacao, questao.IdDisciplina }, tx);

            foreach (var alternativa in alternativas)
                await AtualizarAlternativaAsync(c, tx, id, alternativa);

            return true;
        });

    public Task Desativar(int id) => AlterarSituacao(id, false);

    public Task Reativar(int id) => AlterarSituacao(id, true);

    private Task AlterarSituacao(int id, bool ativo) =>
        ExecutarAsync(c => c.ExecuteAsync(
            "UPDATE dbo.Questao SET Ativo_Questao = @Ativo WHERE Id_Questao = @Id;", new { Id = id, Ativo = ativo }));

    public Task<bool> FoiUtilizadaEmSimulado(int idQuestao) =>
        ExecutarAsync(c => c.ExecuteScalarAsync<bool>("""
            SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM dbo.SimuladoQuestao WHERE Id_Questao = @Id)
                        THEN 1 ELSE 0 END AS BIT);
            """, new { Id = idQuestao }));

    public Task<int> ContarAtivos() =>
        ExecutarAsync(c => c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.Questao WHERE Ativo_Questao = 1;"));

    /// <summary>Atualiza a alternativa pela letra; se ela não existir (dados antigos), insere.</summary>
    private static async Task AtualizarAlternativaAsync(SqlConnection c, SqlTransaction tx, int idQuestao, Alternativa alternativa)
    {
        var parametros = new { alternativa.Letra, alternativa.Texto, alternativa.Correta, IdQuestao = idQuestao };

        var linhas = await c.ExecuteAsync("""
            UPDATE dbo.Alternativa
               SET Texto_Alternativa = @Texto, Correta_Alternativa = @Correta
             WHERE Id_Questao = @IdQuestao AND Letra_Alternativa = @Letra;
            """, parametros, tx);

        if (linhas == 0)
            await c.ExecuteAsync("""
                INSERT INTO dbo.Alternativa (Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao)
                VALUES (@Letra, @Texto, @Correta, @IdQuestao);
                """, parametros, tx);
    }
}
