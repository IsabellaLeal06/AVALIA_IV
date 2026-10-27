namespace Avalia.Desktop.Core.Interfaces;

/// <summary>
/// Guarda as imagens das questões. Hoje copia o arquivo para uma pasta
/// configurável; com a API, passará a enviar o arquivo por HTTP.
/// </summary>
public interface IArmazenamentoImagem
{
    /// <summary>Copia o arquivo e devolve o caminho a gravar no banco.</summary>
    Task<string> SalvarAsync(string caminhoArquivoOrigem);

    /// <summary>Remove uma imagem salva (usado quando a gravação falha).</summary>
    Task RemoverAsync(string imagemUrl);

    /// <summary>Caminho local para exibir a imagem, ou null se o arquivo não estiver disponível.</summary>
    string? ObterCaminhoLocal(string? imagemUrl);
}

/// <summary>Armazenamento das imagens das medalhas (pasta separada da das questões).</summary>
public interface IArmazenamentoImagemMedalha : IArmazenamentoImagem;
