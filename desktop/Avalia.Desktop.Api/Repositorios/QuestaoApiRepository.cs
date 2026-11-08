using Avalia.Desktop.Core.Controles;
using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Modelos;

namespace Avalia.Desktop.Api.Repositorios;

/// <summary>Classes Questao e Alternativa do diagrama, pela API (/questoes), com o escopo do Administrador (RN08).</summary>
public class QuestaoApiRepository(ClienteApi api) : RepositorioApiBase(api), IQuestaoRepository
{
    private static readonly Dictionary<string, string> Campos = MontarCampos();

    private static Dictionary<string, string> MontarCampos()
    {
        var campos = new Dictionary<string, string>
        {
            ["enunciado"] = nameof(Questao.Enunciado),
            ["explicacao"] = nameof(Questao.Explicacao),
            ["idDisciplina"] = nameof(Questao.IdDisciplina),
            ["imagemUrl"] = ControleQuestao.CampoImagem,
            ["correta"] = ControleQuestao.CampoCorreta,
            ["alternativas"] = ControleQuestao.CampoAlternativas
        };
        foreach (var letra in Questao.Letras)
            campos[$"alternativa{letra}"] = ControleQuestao.CampoAlternativa(letra);
        return campos;
    }

    public async Task<Questao?> Consultar(int id)
    {
        var q = await Ler(() => Api.Get<QuestaoDto>($"questoes/{id}"));
        return q is null ? null : new Questao
        {
            Id = q.Id,
            Enunciado = q.Enunciado,
            ImagemUrl = q.ImagemUrl,
            Explicacao = q.Explicacao,
            Ativo = q.Ativo,
            IdDisciplina = q.IdDisciplina,
            IdUsuarioCadastro = q.IdAutor,
            NomeDisciplina = q.Disciplina,
            NomeUsuarioCadastro = q.Autor,
            Alternativas = q.Alternativas
                .Select(a => new Alternativa { Letra = a.Letra, Texto = a.Texto, Correta = a.Correta, IdQuestao = q.Id })
                .OrderBy(a => a.Letra, StringComparer.Ordinal)
                .ToList()
        };
    }

    public async Task<IReadOnlyList<QuestaoResumo>> Listar(FiltroQuestoes filtro)
    {
        var lista = await Ler(() => Api.Get<List<QuestaoResumoDto>>("questoes",
            ("disciplina", filtro.IdDisciplina?.ToString()),
            ("situacao", Situacao(filtro.Situacao)),
            ("busca", filtro.Busca))) ?? [];
        return lista.Select(q => new QuestaoResumo
        {
            Id = q.Id,
            Enunciado = q.Enunciado,
            NomeDisciplina = q.Disciplina,
            DisciplinaAtiva = q.DisciplinaAtiva,
            NomeUsuarioCadastro = q.Autor,
            Ativo = q.Ativo,
            Utilizada = q.Utilizada
        }).ToList();
    }

    private static object Corpo(Questao questao, IReadOnlyList<Alternativa> alternativas) => new
    {
        idDisciplina = questao.IdDisciplina,
        enunciado = questao.Enunciado,
        explicacao = questao.Explicacao,
        imagemUrl = questao.ImagemUrl,
        // Questão e as cinco alternativas viajam juntas e são gravadas numa única transação pela API
        correta = alternativas.FirstOrDefault(a => a.Correta)?.Letra,
        alternativas = alternativas.Select(a => new { letra = a.Letra, texto = a.Texto }).ToArray()
    };

    public async Task<int> Incluir(Questao questao, IReadOnlyList<Alternativa> alternativas)
    {
        var resposta = await Escrever(() => Api.Post<IncluidoDto>("questoes", Corpo(questao, alternativas)), Campos);
        return resposta!.Id;
    }

    public async Task Editar(int id, Questao questao, IReadOnlyList<Alternativa> alternativas) =>
        await Escrever(() => Api.Put<IncluidoDto>($"questoes/{id}", Corpo(questao, alternativas)), Campos);

    public Task Desativar(int id) => Acao(() => Api.Patch<object>($"questoes/{id}/situacao", new { ativo = false }));

    public Task Reativar(int id) => Acao(() => Api.Patch<object>($"questoes/{id}/situacao", new { ativo = true }));

    /// <summary>RN11: a questão já entrou em algum simulado (a API informa em "utilizada").</summary>
    public async Task<bool> FoiUtilizadaEmSimulado(int idQuestao) =>
        (await Ler(() => Api.Get<QuestaoDto>($"questoes/{idQuestao}")))?.Utilizada ?? false;

    public async Task<int> ContarAtivos() => (await Painel()).Questoes;
}
