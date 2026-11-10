using System.Security.Cryptography;
using System.Text;
using Avalia.Desktop.Core.Excecoes;
using Avalia.Desktop.Core.Interfaces;

namespace Avalia.Desktop.Api.Arquivos;

/// <summary>
/// Imagens das questões e das medalhas pela API.
///  - <see cref="SalvarAsync"/>: envia o arquivo para POST /upload/imagem (a API confere o tipo pelo conteúdo,
///    limita a 2 MB e gera o nome) e devolve o caminho a gravar no cadastro, antes de gravar o registro.
///  - <see cref="ObterCaminhoLocal"/>: para a prévia na tela, baixa a imagem (pública) para uma pasta temporária.
/// </summary>
public class ArmazenamentoImagemApi(ClienteApi api, string tipo) : IArmazenamentoImagem
{
    private static readonly string PastaCache = Path.Combine(Path.GetTempPath(), "AvaliaMais", "imagens");

    public async Task<string> SalvarAsync(string caminhoArquivoOrigem)
    {
        try
        {
            var enviado = await api.EnviarArquivo<UploadDto>($"upload/imagem?tipo={tipo}", "imagem", caminhoArquivoOrigem,
                TipoConteudo(caminhoArquivoOrigem));
            return enviado?.Caminho ?? throw new FalhaAcessoDadosException("A API não devolveu o caminho da imagem.");
        }
        catch (ErroApiException ex)
        {
            throw new FalhaAcessoDadosException(ex.Message, ex);
        }
    }

    /// <summary>A API não tem rota para apagar imagens: se a gravação falhar, o arquivo enviado fica órfão no servidor.</summary>
    public Task RemoverAsync(string imagemUrl) => Task.CompletedTask;

    public string? ObterCaminhoLocal(string? imagemUrl)
    {
        if (string.IsNullOrWhiteSpace(imagemUrl))
            return null;

        // Arquivo escolhido neste computador (prévia antes de enviar)
        if (Path.IsPathRooted(imagemUrl))
            return File.Exists(imagemUrl) ? Path.GetFullPath(imagemUrl) : null;

        // Caminho da API (uploads/questoes/x.png, assets/medalhas/bronze.png): baixa uma vez para a pasta temporária
        var destino = Path.Combine(PastaCache, Hash(api.UrlBase + imagemUrl) + Path.GetExtension(imagemUrl).ToLowerInvariant());
        if (File.Exists(destino))
            return destino;

        // Chamada síncrona de propósito: a tela pede um caminho de arquivo. O limite curto evita travar a tela
        var bytes = Task.Run(() => api.BaixarArquivo(imagemUrl, TimeSpan.FromSeconds(8))).GetAwaiter().GetResult();
        if (bytes is null)
            return null;
        Directory.CreateDirectory(PastaCache);
        File.WriteAllBytes(destino, bytes);
        return destino;
    }

    private static string TipoConteudo(string caminho) => Path.GetExtension(caminho).ToLowerInvariant() switch
    {
        ".png" => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".gif" => "image/gif",
        ".webp" => "image/webp",
        _ => "application/octet-stream"
    };

    private static string Hash(string texto) =>
        Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(texto)))[..16];
}

public class ArmazenamentoImagemMedalhaApi(ClienteApi api) : ArmazenamentoImagemApi(api, "medalhas"), IArmazenamentoImagemMedalha;
