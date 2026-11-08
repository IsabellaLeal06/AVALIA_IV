using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalia.Desktop.Core.Excecoes;
using Avalia.Desktop.Core.Servicos;

namespace Avalia.Desktop.Api;

/// <summary>
/// Cliente HTTP da API. Envia o token da sessão em toda requisição (o perfil nunca é informado
/// pelo Desktop: a API lê do token) e traduz as respostas de erro:
///  - 400/409 → <see cref="ErroApiException"/> (os repositórios decidem como mostrar);
///  - 401 com sessão → pede para entrar de novo; 5xx, rede fora do ar e tempo esgotado →
///    <see cref="FalhaAcessoDadosException"/> com mensagem compreensível (sem detalhe técnico).
/// </summary>
public class ClienteApi : IDisposable
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never
    };

    private readonly HttpClient _http;
    private readonly SessaoUsuario _sessao;

    public ClienteApi(string urlBase, SessaoUsuario sessao, HttpMessageHandler? handler = null)
    {
        _sessao = sessao;
        _http = handler is null ? new HttpClient() : new HttpClient(handler, disposeHandler: false);
        _http.BaseAddress = new Uri(ConfiguracaoApi.Normalizar(urlBase));
        // O Azure SQL gratuito pode levar cerca de um minuto para acordar da pausa automática
        _http.Timeout = TimeSpan.FromSeconds(90);
    }

    public Uri UrlBase => _http.BaseAddress!;

    public Task<T?> Get<T>(string rota, params (string Nome, string? Valor)[] consulta) =>
        Enviar<T>(HttpMethod.Get, Montar(rota, consulta), null);

    public Task<T?> Post<T>(string rota, object? corpo = null) => Enviar<T>(HttpMethod.Post, rota, corpo ?? new { });
    public Task<T?> Put<T>(string rota, object corpo) => Enviar<T>(HttpMethod.Put, rota, corpo);
    public Task<T?> Patch<T>(string rota, object corpo) => Enviar<T>(HttpMethod.Patch, rota, corpo);
    public Task<T?> Delete<T>(string rota) => Enviar<T>(HttpMethod.Delete, rota, null);

    /// <summary>Envia um arquivo (multipart, campo "imagem") para /upload/imagem.</summary>
    public async Task<T?> EnviarArquivo<T>(string rota, string campo, string caminhoArquivo, string tipoConteudo)
    {
        await using var arquivo = File.OpenRead(caminhoArquivo);
        using var conteudo = new MultipartFormDataContent();
        var parte = new StreamContent(arquivo);
        parte.Headers.ContentType = new MediaTypeHeaderValue(tipoConteudo);
        // O nome original não é usado pela API (ela gera o nome), mas o multipart exige um
        conteudo.Add(parte, campo, Path.GetFileName(caminhoArquivo));
        using var requisicao = Criar(HttpMethod.Post, rota);
        requisicao.Content = conteudo;
        return await Executar<T>(requisicao);
    }

    /// <summary>Baixa um arquivo público (imagens servidas pela API). Devolve null se não existir.</summary>
    public async Task<byte[]?> BaixarArquivo(string caminhoRelativo, TimeSpan limite)
    {
        using var cancelamento = new CancellationTokenSource(limite);
        try
        {
            using var resposta = await _http.GetAsync(caminhoRelativo.TrimStart('/'), cancelamento.Token);
            return resposta.IsSuccessStatusCode ? await resposta.Content.ReadAsByteArrayAsync(cancelamento.Token) : null;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
        {
            return null;
        }
    }

    private static string Montar(string rota, (string Nome, string? Valor)[] consulta)
    {
        var partes = consulta
            .Where(p => !string.IsNullOrWhiteSpace(p.Valor))
            .Select(p => $"{Uri.EscapeDataString(p.Nome)}={Uri.EscapeDataString(p.Valor!)}")
            .ToList();
        return partes.Count == 0 ? rota : $"{rota}?{string.Join('&', partes)}";
    }

    private HttpRequestMessage Criar(HttpMethod metodo, string rota)
    {
        var requisicao = new HttpRequestMessage(metodo, rota.TrimStart('/'));
        if (_sessao.Token is { Length: > 0 } token)
            requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return requisicao;
    }

    private async Task<T?> Enviar<T>(HttpMethod metodo, string rota, object? corpo)
    {
        using var requisicao = Criar(metodo, rota);
        if (corpo is not null)
            requisicao.Content = JsonContent.Create(corpo, corpo.GetType(), options: Json);
        return await Executar<T>(requisicao);
    }

    private async Task<T?> Executar<T>(HttpRequestMessage requisicao)
    {
        HttpResponseMessage resposta;
        try
        {
            resposta = await _http.SendAsync(requisicao);
        }
        catch (TaskCanceledException ex)
        {
            throw new FalhaAcessoDadosException(
                "A API demorou demais para responder. Se o banco estava em pausa, aguarde alguns segundos e tente novamente.", ex);
        }
        catch (HttpRequestException ex)
        {
            throw new FalhaAcessoDadosException(
                $"Não foi possível conectar à API em {UrlBase}. Confira se ela está rodando (npm start na pasta api), " +
                $"a internet e o endereço (appsettings.Local.json, chave Api:Url, ou a variável {ConfiguracaoApi.VariavelAmbiente}).", ex);
        }

        using (resposta)
        {
            if (resposta.IsSuccessStatusCode)
            {
                if (resposta.StatusCode == HttpStatusCode.NoContent || resposta.Content.Headers.ContentLength == 0)
                    return default;
                try
                {
                    return await resposta.Content.ReadFromJsonAsync<T>(Json);
                }
                catch (JsonException ex)
                {
                    throw new FalhaAcessoDadosException(
                        "A API respondeu num formato inesperado. Confira se o endereço aponta para a API do Avalia+.", ex);
                }
            }

            var (mensagem, campos) = await LerErro(resposta);
            var status = (int)resposta.StatusCode;

            if (status == 401 && _sessao.Autenticado)
                throw new FalhaAcessoDadosException(
                    "A sua sessão não é mais válida (prazo vencido ou conta desativada). Saia da conta e entre novamente.");
            if (status >= 500)
                throw new FalhaAcessoDadosException(mensagem);
            throw new ErroApiException(status, mensagem, campos);
        }
    }

    private static async Task<(string Mensagem, IReadOnlyDictionary<string, string>? Campos)> LerErro(HttpResponseMessage resposta)
    {
        try
        {
            var corpo = await resposta.Content.ReadFromJsonAsync<CorpoErro>(Json);
            if (corpo?.Erro is { Mensagem.Length: > 0 } erro)
                return (erro.Mensagem, erro.Campos);
        }
        catch (JsonException)
        {
            // corpo que não é JSON: usa a mensagem padrão abaixo
        }
        return ($"A API recusou a operação (erro {(int)resposta.StatusCode}).", null);
    }

    public void Dispose() => _http.Dispose();

    private sealed record CorpoErro(DetalheErro? Erro);
    private sealed record DetalheErro(string Mensagem, Dictionary<string, string>? Campos);
}
