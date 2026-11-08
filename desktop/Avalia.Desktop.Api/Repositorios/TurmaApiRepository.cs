using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Modelos;

namespace Avalia.Desktop.Api.Repositorios;

/// <summary>Classe Turma do diagrama, pela API (/turmas), com a grade de disciplinas.</summary>
public class TurmaApiRepository(ClienteApi api) : RepositorioApiBase(api), ITurmaRepository
{
    private static readonly Dictionary<string, string> Campos = new()
    {
        ["nome"] = nameof(Turma.Nome),
        ["anoLetivo"] = nameof(Turma.AnoLetivo),
        ["disciplinas"] = "Disciplinas"
    };

    private static Turma Mapear(TurmaDto t) => new()
    {
        Id = t.Id,
        Nome = t.Nome,
        AnoLetivo = t.AnoLetivo,
        Ativo = t.Ativo,
        QuantidadeAlunosAtivos = t.QtdAlunosAtivos,
        QuantidadeDisciplinas = t.QtdDisciplinas ?? t.Disciplinas?.Count ?? 0
    };

    private async Task<List<TurmaDto>> Buscar(string situacao, string? busca = null) =>
        await Ler(() => Api.Get<List<TurmaDto>>("turmas", ("situacao", situacao), ("busca", busca))) ?? [];

    public async Task<Turma?> Consultar(int id)
    {
        var t = await Ler(() => Api.Get<TurmaDto>($"turmas/{id}"));
        return t is null ? null : Mapear(t);
    }

    public async Task<IReadOnlyList<Turma>> Listar(FiltroCadastro filtro) =>
        (await Buscar(Situacao(filtro.Situacao), filtro.Busca)).Select(Mapear).ToList();

    public async Task<IReadOnlyList<OpcaoSelecao>> ListarOpcoes(bool somenteAtivas) =>
        (await Buscar(somenteAtivas ? "ativo" : "todos"))
        .Select(t => new OpcaoSelecao(t.Id, $"{t.Nome} ({t.AnoLetivo})", t.Ativo, "(inativa)"))
        .ToList();

    public async Task<bool> Existe(string nome, int anoLetivo, int? ignorarId = null) =>
        (await Buscar("todos", nome)).Any(t =>
            t.Id != ignorarId && t.AnoLetivo == anoLetivo && string.Equals(t.Nome, nome, StringComparison.OrdinalIgnoreCase));

    public async Task<int> Incluir(Turma turma, IEnumerable<int> disciplinas)
    {
        var resposta = await Escrever(
            () => Api.Post<IncluidoDto>("turmas", new { nome = turma.Nome, anoLetivo = turma.AnoLetivo, disciplinas = disciplinas.ToArray() }),
            Campos);
        return resposta!.Id;
    }

    public async Task Editar(int id, Turma turma, IEnumerable<int> disciplinas) =>
        await Escrever(
            () => Api.Put<IncluidoDto>($"turmas/{id}", new { nome = turma.Nome, anoLetivo = turma.AnoLetivo, disciplinas = disciplinas.ToArray() }),
            Campos);

    /// <summary>Somente a turma muda de situação; os alunos continuam ativos (RN12).</summary>
    public Task Desativar(int id) => Acao(() => Api.Patch<object>($"turmas/{id}/situacao", new { ativo = false }));

    public Task Reativar(int id) => Acao(() => Api.Patch<object>($"turmas/{id}/situacao", new { ativo = true }));

    public async Task<int> ContarAlunosAtivos(int idTurma) =>
        (await Ler(() => Api.Get<TurmaDto>($"turmas/{idTurma}")))?.QtdAlunosAtivos ?? 0;

    public async Task<IReadOnlyList<int>> ListarIdsDisciplinasVinculadas(int idTurma) =>
        (await Ler(() => Api.Get<TurmaDto>($"turmas/{idTurma}")))?.Disciplinas?.Select(d => d.Id).ToList() ?? [];

    public async Task<int> ContarAtivos() => (await Painel()).Turmas;
}
