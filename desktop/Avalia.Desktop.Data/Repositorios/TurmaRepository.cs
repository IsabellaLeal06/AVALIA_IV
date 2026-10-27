using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Data.Conexao;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Avalia.Desktop.Data.Repositorios;

public class TurmaRepository(IFabricaConexao fabrica) : RepositorioBase(fabrica), ITurmaRepository
{
    private const string Selecao = """
        SELECT t.Id_Turma        AS Id,
               t.Nome_Turma      AS Nome,
               t.AnoLetivo_Turma AS AnoLetivo,
               t.Ativo_Turma     AS Ativo,
               (SELECT COUNT(*) FROM dbo.Usuario u
                 WHERE u.Id_Turma = t.Id_Turma AND u.Ativo_Usuario = 1)      AS QuantidadeAlunosAtivos,
               (SELECT COUNT(*) FROM dbo.TurmaDisciplina td
                 WHERE td.Id_Turma = t.Id_Turma)                             AS QuantidadeDisciplinas
        FROM dbo.Turma t
        """;

    public Task<Turma?> Consultar(int id) =>
        ExecutarAsync(c => c.QuerySingleOrDefaultAsync<Turma>(Selecao + " WHERE t.Id_Turma = @Id;", new { Id = id }));

    public Task<IReadOnlyList<Turma>> Listar(FiltroCadastro filtro) =>
        ExecutarAsync(async c => (IReadOnlyList<Turma>)(await c.QueryAsync<Turma>(Selecao + """
             WHERE (@Ativo IS NULL OR t.Ativo_Turma = @Ativo)
               AND (@Busca IS NULL OR t.Nome_Turma LIKE @Busca ESCAPE '\')
             ORDER BY t.AnoLetivo_Turma DESC, t.Nome_Turma;
            """, new { Ativo = ParametroAtivo(filtro.Situacao), Busca = ParametroBusca(filtro.Busca) })).AsList());

    public Task<IReadOnlyList<OpcaoSelecao>> ListarOpcoes(bool somenteAtivas) =>
        ExecutarAsync(async c => (IReadOnlyList<OpcaoSelecao>)(await c.QueryAsync<(int Id, string Nome, bool Ativo)>("""
            SELECT Id_Turma, CONCAT(Nome_Turma, N' (', AnoLetivo_Turma, N')'), Ativo_Turma
            FROM dbo.Turma
            WHERE (@SomenteAtivas = 0 OR Ativo_Turma = 1)
            ORDER BY AnoLetivo_Turma DESC, Nome_Turma;
            """, new { SomenteAtivas = somenteAtivas }))
            .Select(t => new OpcaoSelecao(t.Id, t.Nome, t.Ativo, "(inativa)"))
            .ToList());

    public Task<bool> Existe(string nome, int anoLetivo, int? ignorarId = null) =>
        ExecutarAsync(c => c.ExecuteScalarAsync<bool>("""
            SELECT CAST(CASE WHEN EXISTS (
                SELECT 1 FROM dbo.Turma
                WHERE Nome_Turma = @Nome AND AnoLetivo_Turma = @AnoLetivo
                  AND (@IgnorarId IS NULL OR Id_Turma <> @IgnorarId)
            ) THEN 1 ELSE 0 END AS BIT);
            """, new { Nome = nome, AnoLetivo = anoLetivo, IgnorarId = ignorarId }));

    public Task<int> Incluir(Turma turma, IEnumerable<int> disciplinas) =>
        ExecutarEmTransacaoAsync(async (c, tx) =>
        {
            var id = await c.ExecuteScalarAsync<int>("""
                INSERT INTO dbo.Turma (Nome_Turma, AnoLetivo_Turma, Ativo_Turma)
                VALUES (@Nome, @AnoLetivo, 1);
                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """, new { turma.Nome, turma.AnoLetivo }, tx);

            await VincularDisciplinasAsync(c, tx, id, disciplinas.ToList());
            return id;
        });

    public Task Editar(int id, Turma turma, IEnumerable<int> disciplinas) =>
        ExecutarEmTransacaoAsync(async (c, tx) =>
        {
            await c.ExecuteAsync("""
                UPDATE dbo.Turma
                   SET Nome_Turma = @Nome, AnoLetivo_Turma = @AnoLetivo
                 WHERE Id_Turma = @Id;
                """, new { Id = id, turma.Nome, turma.AnoLetivo }, tx);

            await VincularDisciplinasAsync(c, tx, id, disciplinas.ToList());
            return true;
        });

    public Task Desativar(int id) => AlterarSituacao(id, false);

    public Task Reativar(int id) => AlterarSituacao(id, true);

    private Task AlterarSituacao(int id, bool ativo) =>
        // Somente a turma muda: os alunos vinculados não são alterados (RN12)
        ExecutarAsync(c => c.ExecuteAsync(
            "UPDATE dbo.Turma SET Ativo_Turma = @Ativo WHERE Id_Turma = @Id;", new { Id = id, Ativo = ativo }));

    public Task<int> ContarAlunosAtivos(int idTurma) =>
        ExecutarAsync(c => c.ExecuteScalarAsync<int>("""
            SELECT COUNT(*) FROM dbo.Usuario
            WHERE Id_Turma = @Id AND Id_Perfil = @Aluno AND Ativo_Usuario = 1;
            """, new { Id = idTurma, Aluno = (int)PerfilUsuario.Aluno }));

    public Task<IReadOnlyList<int>> ListarIdsDisciplinasVinculadas(int idTurma) =>
        ExecutarAsync(async c => (IReadOnlyList<int>)(await c.QueryAsync<int>(
            "SELECT Id_Disciplina FROM dbo.TurmaDisciplina WHERE Id_Turma = @Id;", new { Id = idTurma })).AsList());

    public Task<int> ContarAtivos() =>
        ExecutarAsync(c => c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.Turma WHERE Ativo_Turma = 1;"));

    /// <summary>Deixa a turma vinculada exatamente às disciplinas informadas.</summary>
    private static async Task VincularDisciplinasAsync(SqlConnection c, SqlTransaction tx, int idTurma, List<int> ids)
    {
        await c.ExecuteAsync(ids.Count == 0
                ? "DELETE FROM dbo.TurmaDisciplina WHERE Id_Turma = @IdTurma;"
                : "DELETE FROM dbo.TurmaDisciplina WHERE Id_Turma = @IdTurma AND Id_Disciplina NOT IN @Ids;",
            new { IdTurma = idTurma, Ids = ids }, tx);

        // O Dapper executa o comando uma vez para cada disciplina da lista
        await c.ExecuteAsync("""
            INSERT INTO dbo.TurmaDisciplina (Id_Turma, Id_Disciplina)
            SELECT @IdTurma, @IdDisciplina
            WHERE NOT EXISTS (SELECT 1 FROM dbo.TurmaDisciplina
                              WHERE Id_Turma = @IdTurma AND Id_Disciplina = @IdDisciplina);
            """, ids.Select(id => new { IdTurma = idTurma, IdDisciplina = id }), tx);
    }
}
