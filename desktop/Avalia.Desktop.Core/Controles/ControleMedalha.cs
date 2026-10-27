using Avalia.Desktop.Core.Excecoes;
using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Core.Servicos;
using Avalia.Desktop.Core.Validacoes;

namespace Avalia.Desktop.Core.Controles;

/// <summary>
/// Controle ControleMedalha: listar, consultarPorId, incluir, editar, excluir e
/// qtdAlunosSemMedalha. O Administrador mantém as faixas de medalha sem alterar
/// o código das aplicações (RN22). A medalha de cada aluno é apurada na consulta,
/// comparando a pontuação com estas faixas, por isso a exclusão é física.
/// </summary>
public class ControleMedalha(IMedalhaRepository medalhas, IArmazenamentoImagemMedalha armazenamentoImagem)
{
    public const int TamanhoMaximoNome = 50;
    public const int PontosMaximos = 1_000_000;
    public const string CampoImagem = nameof(Medalha.ImagemUrl);

    /// <summary>listar() → Medalha.listar(): faixas em ordem de pontuação, com os alunos em cada uma.</summary>
    public Task<IReadOnlyList<Medalha>> Listar() => medalhas.Listar();

    /// <summary>consultarPorId(id) → Medalha.consultar(id)</summary>
    public Task<Medalha?> ConsultarPorId(int id) => medalhas.Consultar(id);

    /// <summary>qtdAlunosSemMedalha(): alunos ativos que ainda não alcançaram a primeira faixa.</summary>
    public Task<int> QtdAlunosSemMedalha() => medalhas.QtdAlunosSemMedalha();

    public string? ObterCaminhoLocalImagem(string? imagemUrl) => armazenamentoImagem.ObterCaminhoLocal(imagemUrl);

    /// <summary>
    /// incluir(medalha) → Medalha.incluir(medalha). <paramref name="arquivoNovaImagem"/>:
    /// arquivo escolhido no computador, copiado só ao gravar. Sem ele, vale o caminho digitado
    /// (por exemplo, o usado pela versão Web: assets/medalhas/bronze.png).
    /// </summary>
    public async Task<ResultadoOperacao<int>> Incluir(Medalha dados, string? arquivoNovaImagem = null)
    {
        var medalha = Normalizar(dados);
        var erros = await ValidarAsync(medalha, null, arquivoNovaImagem);
        if (erros.PossuiErros)
            return ResultadoOperacao<int>.Falha(erros);

        return await GravarComImagemAsync(medalha, arquivoNovaImagem, async () =>
        {
            var id = await medalhas.Incluir(medalha);
            return ResultadoOperacao<int>.Ok(id, $"Medalha \"{medalha.Nome}\" cadastrada a partir de {medalha.PontosMinimos:N0} pontos.");
        });
    }

    /// <summary>editar(id, medalha) → Medalha.editar(id, medalha)</summary>
    public async Task<ResultadoOperacao<int>> Editar(int id, Medalha dados, string? arquivoNovaImagem = null)
    {
        if (await medalhas.Consultar(id) is null)
            return ResultadoOperacao<int>.Falha("Medalha não encontrada.");

        var medalha = Normalizar(dados);
        medalha.Id = id;
        var erros = await ValidarAsync(medalha, id, arquivoNovaImagem);
        if (erros.PossuiErros)
            return ResultadoOperacao<int>.Falha(erros);

        return await GravarComImagemAsync(medalha, arquivoNovaImagem, async () =>
        {
            await medalhas.Editar(id, medalha);
            return ResultadoOperacao<int>.Ok(id, $"Medalha \"{medalha.Nome}\" atualizada.");
        });
    }

    /// <summary>Texto da confirmação de exclusão, com a quantidade de alunos afetados.</summary>
    public async Task<string> MensagemExclusao(int id)
    {
        var lista = await medalhas.Listar();
        var medalha = lista.FirstOrDefault(m => m.Id == id);
        if (medalha is null)
            return "Deseja excluir esta medalha?";

        var texto = $"Deseja excluir a medalha \"{medalha.Nome}\" ({medalha.Faixa})?\n\n" +
                    "A faixa deixa de existir. A pontuação dos alunos não muda: cada aluno passa a ver a medalha " +
                    "da faixa anterior (ou nenhuma, se não houver faixa abaixo).";
        if (medalha.QtdAlunos > 0)
            texto += $"\n\nAtenção: {medalha.QtdAlunos} aluno(s) ativo(s) estão nesta faixa hoje.";
        if (lista.Count == 1)
            texto += "\n\nAtenção: esta é a única medalha cadastrada. Sem faixas, nenhum aluno terá medalha.";
        return texto;
    }

    /// <summary>excluir(id) → Medalha.excluir(id)</summary>
    public async Task<ResultadoOperacao> Excluir(int id)
    {
        var medalha = await medalhas.Consultar(id);
        if (medalha is null)
            return ResultadoOperacao.Falha("Medalha não encontrada.");

        await medalhas.Excluir(id);
        return ResultadoOperacao.Ok($"Medalha \"{medalha.Nome}\" excluída.");
    }

    private static Medalha Normalizar(Medalha dados) => new()
    {
        Id = dados.Id,
        Nome = string.Join(' ', (dados.Nome ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries)),
        PontosMinimos = dados.PontosMinimos,
        ImagemUrl = ValidadorImagem.LimparCaminho(dados.ImagemUrl).Replace('\\', '/')
    };

    private async Task<ListaErros> ValidarAsync(Medalha medalha, int? id, string? arquivoNovaImagem)
    {
        var erros = new ListaErros();
        if (medalha.Nome.Length == 0)
            erros.Adicionar(nameof(Medalha.Nome), "Informe o nome da medalha (ex.: Bronze).");
        else if (medalha.Nome.Length > TamanhoMaximoNome)
            erros.Adicionar(nameof(Medalha.Nome), $"O nome deve ter no máximo {TamanhoMaximoNome} caracteres.");

        if (medalha.PontosMinimos is < 1 or > PontosMaximos)
            erros.Adicionar(nameof(Medalha.PontosMinimos), $"Informe a pontuação mínima entre 1 e {PontosMaximos:N0}.");

        if (arquivoNovaImagem is not null)
        {
            if (ValidadorImagem.Validar(arquivoNovaImagem) is { } erroImagem)
                erros.Adicionar(CampoImagem, erroImagem);
        }
        else if (medalha.ImagemUrl.Length == 0)
            erros.Adicionar(CampoImagem, "Informe a imagem da medalha: arraste o arquivo ou digite o caminho usado pela versão Web.");
        else if (medalha.ImagemUrl.Length > ValidadorImagem.TamanhoMaximoUrl)
            erros.Adicionar(CampoImagem, $"O caminho da imagem deve ter no máximo {ValidadorImagem.TamanhoMaximoUrl} caracteres.");

        if (!erros.Any(e => e.Campo == nameof(Medalha.Nome)) && await medalhas.ExisteNome(medalha.Nome, id))
            erros.Adicionar(nameof(Medalha.Nome), "Já existe uma medalha com este nome.");

        if (!erros.Any(e => e.Campo == nameof(Medalha.PontosMinimos)) && await medalhas.ExistePontosMinimos(medalha.PontosMinimos, id))
            erros.Adicionar(nameof(Medalha.PontosMinimos), "Já existe uma medalha com esta pontuação mínima.");

        return erros;
    }

    private async Task<ResultadoOperacao<int>> GravarComImagemAsync(Medalha medalha, string? arquivoNovaImagem,
        Func<Task<ResultadoOperacao<int>>> gravar)
    {
        string? imagemCopiada = null;
        if (arquivoNovaImagem is not null)
        {
            imagemCopiada = await armazenamentoImagem.SalvarAsync(ValidadorImagem.LimparCaminho(arquivoNovaImagem));
            medalha.ImagemUrl = imagemCopiada;
        }

        try
        {
            return await gravar();
        }
        catch (ViolacaoUnicidadeException ex)
        {
            if (imagemCopiada is not null)
                await armazenamentoImagem.RemoverAsync(imagemCopiada);
            return ex.Restricao?.Contains("Pontos", StringComparison.OrdinalIgnoreCase) == true
                ? ResultadoOperacao<int>.Falha("Já existe uma medalha com esta pontuação mínima.", nameof(Medalha.PontosMinimos))
                : ResultadoOperacao<int>.Falha("Já existe uma medalha com este nome.", nameof(Medalha.Nome));
        }
        catch
        {
            if (imagemCopiada is not null)
                await armazenamentoImagem.RemoverAsync(imagemCopiada);
            throw;
        }
    }
}
