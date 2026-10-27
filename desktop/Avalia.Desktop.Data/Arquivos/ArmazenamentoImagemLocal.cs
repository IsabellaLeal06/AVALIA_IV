using Avalia.Desktop.Core.Interfaces;

namespace Avalia.Desktop.Data.Arquivos;

/// <summary>
/// Copia as imagens para uma pasta configurável
/// (Armazenamento:PastaImagensQuestoes) e devolve um caminho relativo,
/// como "uploads/questoes/arquivo.png", que é gravado no banco.
/// Quando a API existir, ela poderá servir esse mesmo caminho para Web e Mobile.
/// </summary>
public class ArmazenamentoImagemLocal(string pastaDestino, string prefixoUrl) : IArmazenamentoImagem
{
    private readonly string _prefixo = prefixoUrl.EndsWith('/') ? prefixoUrl : prefixoUrl + "/";

    public async Task<string> SalvarAsync(string caminhoArquivoOrigem)
    {
        Directory.CreateDirectory(pastaDestino);

        var nomeArquivo = $"{Guid.NewGuid():N}{Path.GetExtension(caminhoArquivoOrigem).ToLowerInvariant()}";
        var destino = Path.Combine(pastaDestino, nomeArquivo);

        await using (var origem = File.OpenRead(caminhoArquivoOrigem))
        await using (var copia = File.Create(destino))
            await origem.CopyToAsync(copia);

        return _prefixo + nomeArquivo;
    }

    public Task RemoverAsync(string imagemUrl)
    {
        var caminho = ObterCaminhoLocal(imagemUrl);
        if (caminho is not null && caminho.StartsWith(Path.GetFullPath(pastaDestino), StringComparison.OrdinalIgnoreCase))
            File.Delete(caminho);
        return Task.CompletedTask;
    }

    public string? ObterCaminhoLocal(string? imagemUrl)
    {
        if (string.IsNullOrWhiteSpace(imagemUrl))
            return null;

        string caminho;
        if (imagemUrl.StartsWith(_prefixo, StringComparison.OrdinalIgnoreCase))
        {
            // Path.GetFileName impede caminhos como "../../arquivo"
            caminho = Path.Combine(pastaDestino, Path.GetFileName(imagemUrl[_prefixo.Length..]));
        }
        else if (Path.IsPathRooted(imagemUrl))
        {
            caminho = imagemUrl;
        }
        else
        {
            return null;
        }

        caminho = Path.GetFullPath(caminho);
        return File.Exists(caminho) ? caminho : null;
    }
}

/// <summary>Imagens das medalhas, em pasta própria (Armazenamento:PastaImagensMedalhas).</summary>
public class ArmazenamentoImagemMedalhaLocal(string pastaDestino, string prefixoUrl)
    : ArmazenamentoImagemLocal(pastaDestino, prefixoUrl), IArmazenamentoImagemMedalha;
