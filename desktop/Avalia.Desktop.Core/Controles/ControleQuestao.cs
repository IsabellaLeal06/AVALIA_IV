using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Core.Servicos;
using Avalia.Desktop.Core.Validacoes;

namespace Avalia.Desktop.Core.Controles;

/// <summary>
/// Controle ControleQuestao: listar, consultar, incluir, editar, desativar e
/// reativar as questões e as suas alternativas (RF03, RN08, RN09, RN10, RN11, RN12).
/// </summary>
public class ControleQuestao(
    IQuestaoRepository questoes,
    IDisciplinaRepository disciplinas,
    IArmazenamentoImagem armazenamentoImagem,
    SessaoUsuario sessao)
{
    public const string MensagemRN11 =
        "Esta questão já foi utilizada em simulado, por isso a alternativa correta não pode ser alterada (RN11). " +
        "Se a resposta estiver errada, desative esta questão e cadastre uma nova.";

    public const string CampoAlternativas = "Alternativas";
    public const string CampoCorreta = "Correta";
    public const string CampoImagem = "Imagem";

    public static string CampoAlternativa(string letra) => $"Alternativa{letra}";

    public Task<IReadOnlyList<QuestaoResumo>> Listar(FiltroQuestoes filtro) => questoes.Listar(filtro);

    /// <summary>consultar(id) → Questao.consultar(id). Devolve a questão com as cinco alternativas.</summary>
    public Task<Questao?> Consultar(int id) => questoes.Consultar(id);

    /// <summary>RN11: questão já usada em simulado não pode trocar a alternativa correta.</summary>
    public Task<bool> FoiUtilizada(int id) => questoes.FoiUtilizadaEmSimulado(id);

    public string? ObterCaminhoLocalImagem(string? imagemUrl) => armazenamentoImagem.ObterCaminhoLocal(imagemUrl);

    /// <summary>
    /// incluir(questao, alternativas) → Questao.incluir(questao, alternativas).
    /// <paramref name="arquivoNovaImagem"/>: arquivo escolhido pelo usuário, copiado só ao gravar.
    /// </summary>
    public async Task<ResultadoOperacao<int>> Incluir(Questao dados, IReadOnlyList<Alternativa> alternativas,
        string? arquivoNovaImagem = null)
    {
        var questao = Normalizar(dados, alternativas);
        var erros = new ListaErros();
        await ValidarAsync(questao, null, false, arquivoNovaImagem, erros);
        if (erros.PossuiErros)
            return ResultadoOperacao<int>.Falha(erros);

        // RN08: registra quem cadastrou
        questao.IdUsuarioCadastro = sessao.IdUsuario;
        questao.Ativo = true;
        return await GravarComImagemAsync(questao, arquivoNovaImagem, async () =>
        {
            var id = await questoes.Incluir(questao, questao.Alternativas);
            return ResultadoOperacao<int>.Ok(id, "Questão cadastrada com sucesso.");
        });
    }

    /// <summary>editar(id, questao, alternativas) → Questao.editar(id, questao, alternativas).</summary>
    public async Task<ResultadoOperacao<int>> Editar(int id, Questao dados, IReadOnlyList<Alternativa> alternativas,
        string? arquivoNovaImagem = null)
    {
        var atual = await questoes.Consultar(id);
        if (atual is null)
            return ResultadoOperacao<int>.Falha("Questão não encontrada.");
        if (!atual.Ativo)
            return ResultadoOperacao<int>.Falha("A questão está desativada e não pode ser editada. Reative-a antes.");

        var questao = Normalizar(dados, alternativas);
        questao.Id = id;
        var utilizada = await questoes.FoiUtilizadaEmSimulado(id);
        var erros = new ListaErros();
        await ValidarAsync(questao, atual, utilizada, arquivoNovaImagem, erros);
        if (erros.PossuiErros)
            return ResultadoOperacao<int>.Falha(erros);

        // RN10: a autoria permanece com quem cadastrou originalmente
        questao.IdUsuarioCadastro = atual.IdUsuarioCadastro;
        questao.Ativo = atual.Ativo;
        return await GravarComImagemAsync(questao, arquivoNovaImagem, async () =>
        {
            await questoes.Editar(id, questao, questao.Alternativas);
            return ResultadoOperacao<int>.Ok(id, "Questão atualizada com sucesso.");
        });
    }

    /// <summary>Texto de solicitarDesativacao(mensagem).</summary>
    public static string MensagemDesativacao(int id) =>
        $"Deseja desativar a questão nº {id}?\n\n" +
        "Ela deixará de entrar no sorteio de novos simulados. Os simulados já realizados continuam com ela no histórico. " +
        "É possível reativá-la depois.";

    /// <summary>desativar(id) → Questao.desativar(id)</summary>
    public async Task<ResultadoOperacao> Desativar(int id)
    {
        var questao = await questoes.Consultar(id);
        if (questao is null)
            return ResultadoOperacao.Falha("Questão não encontrada.");
        if (!questao.Ativo)
            return ResultadoOperacao.Falha("A questão já está desativada.");

        await questoes.Desativar(id);
        return ResultadoOperacao.Ok($"Questão nº {id} desativada. Ela não entrará em novos simulados.");
    }

    public async Task<ResultadoOperacao> Reativar(int id)
    {
        var questao = await questoes.Consultar(id);
        if (questao is null)
            return ResultadoOperacao.Falha("Questão não encontrada.");
        if (questao.Ativo)
            return ResultadoOperacao.Falha("A questão já está ativa.");

        await questoes.Reativar(id);

        var disciplina = await disciplinas.Consultar(questao.IdDisciplina);
        return ResultadoOperacao.Ok(disciplina is { Ativo: false }
            ? $"Questão nº {id} reativada. Ela só entrará nos simulados quando a disciplina \"{disciplina.Nome}\" for reativada."
            : $"Questão nº {id} reativada.");
    }

    /// <summary>Copia a imagem nova, grava e, se a gravação falhar, apaga a cópia.</summary>
    private async Task<ResultadoOperacao<int>> GravarComImagemAsync(Questao questao, string? arquivoNovaImagem,
        Func<Task<ResultadoOperacao<int>>> gravar)
    {
        string? imagemCopiada = null;
        if (arquivoNovaImagem is not null)
        {
            imagemCopiada = await armazenamentoImagem.SalvarAsync(ValidadorImagem.LimparCaminho(arquivoNovaImagem));
            if (imagemCopiada.Length > ValidadorImagem.TamanhoMaximoUrl)
            {
                await armazenamentoImagem.RemoverAsync(imagemCopiada);
                return ResultadoOperacao<int>.Falha("O caminho da imagem ficou longo demais.", CampoImagem);
            }
            questao.ImagemUrl = imagemCopiada;
        }

        try
        {
            return await gravar();
        }
        catch
        {
            if (imagemCopiada is not null)
                await armazenamentoImagem.RemoverAsync(imagemCopiada);
            throw;
        }
    }

    private static Questao Normalizar(Questao dados, IReadOnlyList<Alternativa> alternativas) => new()
    {
        Id = dados.Id,
        Enunciado = (dados.Enunciado ?? string.Empty).Trim(),
        Explicacao = string.IsNullOrWhiteSpace(dados.Explicacao) ? null : dados.Explicacao.Trim(),
        ImagemUrl = string.IsNullOrWhiteSpace(dados.ImagemUrl) ? null : dados.ImagemUrl.Trim(),
        IdDisciplina = dados.IdDisciplina,
        Alternativas = (alternativas ?? [])
            .Select(a => new Alternativa
            {
                Letra = (a.Letra ?? string.Empty).Trim().ToUpperInvariant(),
                Texto = (a.Texto ?? string.Empty).Trim(),
                Correta = a.Correta
            })
            .OrderBy(a => a.Letra, StringComparer.Ordinal)
            .ToList()
    };

    private async Task ValidarAsync(Questao questao, Questao? atual, bool utilizada, string? arquivoNovaImagem, ListaErros erros)
    {
        if (questao.Enunciado.Length == 0)
            erros.Adicionar(nameof(Questao.Enunciado), "Informe o enunciado da questão.");

        await ValidarDisciplinaAsync(questao, atual, erros);
        ValidarAlternativas(questao, erros);

        if (utilizada && atual!.LetraCorreta is { } corretaAtual
            && questao.LetraCorreta is { } corretaNova && corretaAtual != corretaNova)
            erros.Adicionar(CampoCorreta, MensagemRN11);

        if (arquivoNovaImagem is not null && ValidadorImagem.Validar(arquivoNovaImagem) is { } erroImagem)
            erros.Adicionar(CampoImagem, erroImagem);
    }

    private async Task ValidarDisciplinaAsync(Questao questao, Questao? atual, ListaErros erros)
    {
        if (questao.IdDisciplina <= 0)
        {
            erros.Adicionar(nameof(Questao.IdDisciplina), "Selecione a disciplina.");
            return;
        }

        var disciplina = await disciplinas.Consultar(questao.IdDisciplina);
        if (disciplina is null)
            erros.Adicionar(nameof(Questao.IdDisciplina), "Disciplina não encontrada.");
        else if (!disciplina.Ativo && atual?.IdDisciplina != disciplina.Id)
            erros.Adicionar(nameof(Questao.IdDisciplina), "A disciplina selecionada está desativada.");
    }

    /// <summary>RN09: exatamente cinco alternativas, de A a E, com apenas uma correta.</summary>
    internal static void ValidarAlternativas(Questao questao, ListaErros erros)
    {
        var alternativas = questao.Alternativas;
        var letras = alternativas.Select(a => a.Letra).ToList();

        if (alternativas.Count != 5 || !letras.SequenceEqual(Questao.Letras))
        {
            erros.Adicionar(CampoAlternativas, "A questão deve ter exatamente cinco alternativas, de A a E.");
            return;
        }

        foreach (var alternativa in alternativas.Where(a => a.Texto.Length == 0))
            erros.Adicionar(CampoAlternativa(alternativa.Letra), $"Informe o texto da alternativa {alternativa.Letra}.");

        var repetidas = alternativas
            .Where(a => a.Texto.Length > 0)
            .GroupBy(a => a.Texto, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1);
        foreach (var grupo in repetidas)
            erros.Adicionar(CampoAlternativa(grupo.Last().Letra),
                $"As alternativas {string.Join(" e ", grupo.Select(a => a.Letra))} têm o mesmo texto.");

        var corretas = alternativas.Count(a => a.Correta);
        if (corretas == 0)
            erros.Adicionar(CampoCorreta, "Selecione a alternativa correta.");
        else if (corretas > 1)
            erros.Adicionar(CampoCorreta, "Apenas uma alternativa pode ser a correta.");
    }
}
