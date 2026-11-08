namespace Avalia.Desktop.Api;

/// <summary>
/// Endereço da API. Ordem de busca (a primeira que existir vale):
/// 1º variável de ambiente AVALIA_API_URL;
/// 2º appsettings.Local.json, chave "Api:Url" (arquivo fora do Git);
/// 3º http://localhost:3000/ (a API rodando no mesmo computador).
/// Não há senha nem string de conexão: só o endereço.
/// </summary>
public static class ConfiguracaoApi
{
    public const string VariavelAmbiente = "AVALIA_API_URL";
    public const string UrlPadrao = "http://localhost:3000/";

    public static string ObterUrl(string? configurado)
    {
        var url = Environment.GetEnvironmentVariable(VariavelAmbiente);
        if (string.IsNullOrWhiteSpace(url))
            url = configurado;
        if (string.IsNullOrWhiteSpace(url))
            url = UrlPadrao;
        return Normalizar(url);
    }

    /// <summary>Garante o esquema (http://) e a barra final.</summary>
    public static string Normalizar(string url)
    {
        url = url.Trim();
        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            url = "http://" + url;
        return url.EndsWith('/') ? url : url + "/";
    }

    /// <summary>Texto curto para a barra de status: só o servidor e a porta.</summary>
    public static string DescreverDestino(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) ? uri.Authority : url;
}
