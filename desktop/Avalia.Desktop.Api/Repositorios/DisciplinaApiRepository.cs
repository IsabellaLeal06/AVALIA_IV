using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Modelos;

namespace Avalia.Desktop.Api.Repositorios;

/// <summary>Classe Disciplina do diagrama, pela API (/disciplinas), com o professor responsável (RN07).</summary>
public class DisciplinaApiRepository(ClienteApi api) : RepositorioApiBase(api), IDisciplinaRepository
{
    private static readonly Dictionary<string, string> Campos = new()
    {
        ["nome"] = nameof(Disciplina.Nome),
        ["descricao"] = nameof(Disciplina.Descricao),
        ["idResponsavel"] = nameof(Disciplina.IdProfessorResponsavel),
        ["turmas"] = "Turmas"
    };

    private static Disciplina Mapear(DisciplinaDto d) => new()
    {
        Id = d.Id,
        Nome = d.Nome,
        Descricao = d.Descricao,
        Ativo = d.Ativo,
        IdProfessorResponsavel = d.IdResponsavel,
        NomeProfessorResponsavel = d.NomeResponsavel,
        QuantidadeQuestoesAtivas = d.QtdQuestoesAtivas
    };

    private async Task<List<DisciplinaDto>> Buscar(string situacao, string? busca = null) =>
        await Ler(() => Api.Get<List<DisciplinaDto>>("disciplinas", ("situacao", situacao), ("busca", busca))) ?? [];

    public async Task<Disciplina?> Consultar(int id)
    {
        var d = await Ler(() => Api.Get<DisciplinaDto>($"disciplinas/{id}"));
        return d is null ? null : Mapear(d);
    }

    public async Task<IReadOnlyList<Disciplina>> Listar(FiltroCadastro filtro) =>
        (await Buscar(Situacao(filtro.Situacao), filtro.Busca)).Select(Mapear).ToList();

    public async Task<IReadOnlyList<OpcaoSelecao>> ListarOpcoes(bool somenteAtivas) =>
        (await Buscar(somenteAtivas ? "ativo" : "todos"))
        .Select(d => new OpcaoSelecao(d.Id, d.Nome, d.Ativo, "(inativa)"))
        .ToList();

    public async Task<bool> ExisteNome(string nome, int? ignorarId = null) =>
        (await Buscar("todos", nome)).Any(d => d.Id != ignorarId && string.Equals(d.Nome, nome, StringComparison.OrdinalIgnoreCase));

    // Sem o campo "turmas": a grade de cada turma é mantida na tela de turmas
    private static object Corpo(Disciplina d) => new { nome = d.Nome, descricao = d.Descricao, idResponsavel = d.IdProfessorResponsavel };

    public async Task<int> Incluir(Disciplina disciplina)
    {
        var resposta = await Escrever(() => Api.Post<IncluidoDto>("disciplinas", Corpo(disciplina)), Campos);
        return resposta!.Id;
    }

    public async Task Editar(int id, Disciplina disciplina) =>
        await Escrever(() => Api.Put<IncluidoDto>($"disciplinas/{id}", Corpo(disciplina)), Campos);

    /// <summary>Exclusão lógica (RN12): as questões saem do sorteio enquanto a disciplina estiver desativada.</summary>
    public Task Desativar(int id) => Acao(() => Api.Patch<object>($"disciplinas/{id}/situacao", new { ativo = false }));

    public Task Reativar(int id) => Acao(() => Api.Patch<object>($"disciplinas/{id}/situacao", new { ativo = true }));

    public async Task<int> ContarQuestoesAtivas(int idDisciplina) =>
        (await Ler(() => Api.Get<DisciplinaDto>($"disciplinas/{idDisciplina}")))?.QtdQuestoesAtivas ?? 0;

    public async Task<int> ContarAtivos() => (await Painel()).Disciplinas;
}
