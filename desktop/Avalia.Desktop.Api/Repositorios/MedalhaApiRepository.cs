using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Modelos;

namespace Avalia.Desktop.Api.Repositorios;

/// <summary>Faixas de medalha (RN22), pela API (/medalhas). A medalha do aluno é apurada pela API, não gravada.</summary>
public class MedalhaApiRepository(ClienteApi api) : RepositorioApiBase(api), IMedalhaRepository
{
    private static readonly Dictionary<string, string> Campos = new()
    {
        ["nome"] = nameof(Medalha.Nome),
        ["pontosMinimos"] = nameof(Medalha.PontosMinimos),
        ["imagemUrl"] = nameof(Medalha.ImagemUrl)
    };

    private static Medalha Mapear(MedalhaDto m) => new()
    {
        Id = m.Id,
        Nome = m.Nome,
        PontosMinimos = m.PontosMinimos,
        ImagemUrl = m.ImagemUrl,
        PontosProximaFaixa = m.PontosProximaFaixa,
        QtdAlunos = m.QtdAlunos ?? 0
    };

    private async Task<ListaMedalhasDto> Todas() =>
        await Ler(() => Api.Get<ListaMedalhasDto>("medalhas")) ?? new ListaMedalhasDto([], 0);

    public async Task<IReadOnlyList<Medalha>> Listar() => (await Todas()).Faixas.Select(Mapear).ToList();

    public async Task<Medalha?> Consultar(int id)
    {
        var m = await Ler(() => Api.Get<MedalhaDto>($"medalhas/{id}"));
        return m is null ? null : Mapear(m);
    }

    public async Task<bool> ExisteNome(string nome, int? ignorarId = null) =>
        (await Todas()).Faixas.Any(m => m.Id != ignorarId && string.Equals(m.Nome, nome, StringComparison.OrdinalIgnoreCase));

    public async Task<bool> ExistePontosMinimos(int pontosMinimos, int? ignorarId = null) =>
        (await Todas()).Faixas.Any(m => m.Id != ignorarId && m.PontosMinimos == pontosMinimos);

    private static object Corpo(Medalha m) => new { nome = m.Nome, pontosMinimos = m.PontosMinimos, imagemUrl = m.ImagemUrl };

    public async Task<int> Incluir(Medalha medalha)
    {
        var resposta = await Escrever(() => Api.Post<IncluidoDto>("medalhas", Corpo(medalha)), Campos);
        return resposta!.Id;
    }

    public async Task Editar(int id, Medalha medalha) =>
        await Escrever(() => Api.Put<IncluidoDto>($"medalhas/{id}", Corpo(medalha)), Campos);

    /// <summary>A faixa não tem vínculo com nenhum aluno, por isso a exclusão é física (como sempre foi).</summary>
    public Task Excluir(int id) => Acao(() => Api.Delete<object>($"medalhas/{id}"));

    public async Task<int> QtdAlunosSemMedalha() => (await Todas()).QtdAlunosSemMedalha;
}
