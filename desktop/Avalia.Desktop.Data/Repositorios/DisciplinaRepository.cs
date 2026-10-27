using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Data.Conexao;
using Dapper;

namespace Avalia.Desktop.Data.Repositorios;

public class DisciplinaRepository(IFabricaConexao fabrica) : RepositorioBase(fabrica), IDisciplinaRepository
{
    private const string Selecao = """
        SELECT d.Id_Disciplina        AS Id,
               d.Nome_Disciplina      AS Nome,
               d.Descricao_Disciplina AS Descricao,
               d.Ativo_Disciplina     AS Ativo,
               d.Id_Usuario           AS IdProfessorResponsavel,
               CASE WHEN p.Ativo_Usuario = 0 THEN CONCAT(p.Nome_Usuario, N' (inativo)')
                    ELSE p.Nome_Usuario END AS NomeProfessorResponsavel,
               (SELECT COUNT(*) FROM dbo.Questao q
                 WHERE q.Id_Disciplina = d.Id_Disciplina AND q.Ativo_Questao = 1) AS QuantidadeQuestoesAtivas
        FROM dbo.Disciplina d
        LEFT JOIN dbo.Usuario p ON p.Id_Usuario = d.Id_Usuario
        """;

    public Task<Disciplina?> Consultar(int id) =>
        ExecutarAsync(c => c.QuerySingleOrDefaultAsync<Disciplina>(
            Selecao + " WHERE d.Id_Disciplina = @Id;", new { Id = id }));

    public Task<IReadOnlyList<Disciplina>> Listar(FiltroCadastro filtro) =>
        ExecutarAsync(async c => (IReadOnlyList<Disciplina>)(await c.QueryAsync<Disciplina>(Selecao + """
             WHERE (@Ativo IS NULL OR d.Ativo_Disciplina = @Ativo)
               AND (@Busca IS NULL OR d.Nome_Disciplina LIKE @Busca ESCAPE '\')
             ORDER BY d.Nome_Disciplina;
            """, new { Ativo = ParametroAtivo(filtro.Situacao), Busca = ParametroBusca(filtro.Busca) })).AsList());

    public Task<IReadOnlyList<OpcaoSelecao>> ListarOpcoes(bool somenteAtivas) =>
        ExecutarAsync(async c => (IReadOnlyList<OpcaoSelecao>)(await c.QueryAsync<(int Id, string Nome, bool Ativo)>("""
            SELECT Id_Disciplina, Nome_Disciplina, Ativo_Disciplina
            FROM dbo.Disciplina
            WHERE (@SomenteAtivas = 0 OR Ativo_Disciplina = 1)
            ORDER BY Nome_Disciplina;
            """, new { SomenteAtivas = somenteAtivas }))
            .Select(d => new OpcaoSelecao(d.Id, d.Nome, d.Ativo, "(inativa)"))
            .ToList());

    public Task<bool> ExisteNome(string nome, int? ignorarId = null) =>
        ExecutarAsync(c => c.ExecuteScalarAsync<bool>("""
            SELECT CAST(CASE WHEN EXISTS (
                SELECT 1 FROM dbo.Disciplina
                WHERE Nome_Disciplina = @Nome AND (@IgnorarId IS NULL OR Id_Disciplina <> @IgnorarId)
            ) THEN 1 ELSE 0 END AS BIT);
            """, new { Nome = nome, IgnorarId = ignorarId }));

    public Task<int> Incluir(Disciplina disciplina) =>
        ExecutarAsync(c => c.ExecuteScalarAsync<int>("""
            INSERT INTO dbo.Disciplina (Nome_Disciplina, Descricao_Disciplina, Ativo_Disciplina, Id_Usuario)
            VALUES (@Nome, @Descricao, 1, @IdProfessorResponsavel);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """, new { disciplina.Nome, disciplina.Descricao, disciplina.IdProfessorResponsavel }));

    public Task Editar(int id, Disciplina disciplina) =>
        ExecutarAsync(c => c.ExecuteAsync("""
            UPDATE dbo.Disciplina
               SET Nome_Disciplina      = @Nome,
                   Descricao_Disciplina = @Descricao,
                   Id_Usuario           = @IdProfessorResponsavel
             WHERE Id_Disciplina = @Id;
            """, new { Id = id, disciplina.Nome, disciplina.Descricao, disciplina.IdProfessorResponsavel }));

    public Task Desativar(int id) => AlterarSituacao(id, false);

    public Task Reativar(int id) => AlterarSituacao(id, true);

    private Task AlterarSituacao(int id, bool ativo) =>
        ExecutarAsync(c => c.ExecuteAsync(
            "UPDATE dbo.Disciplina SET Ativo_Disciplina = @Ativo WHERE Id_Disciplina = @Id;",
            new { Id = id, Ativo = ativo }));

    public Task<int> ContarQuestoesAtivas(int idDisciplina) =>
        ExecutarAsync(c => c.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM dbo.Questao WHERE Id_Disciplina = @Id AND Ativo_Questao = 1;",
            new { Id = idDisciplina }));

    public Task<int> ContarAtivos() =>
        ExecutarAsync(c => c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.Disciplina WHERE Ativo_Disciplina = 1;"));
}
