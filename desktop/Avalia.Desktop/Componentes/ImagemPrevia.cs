namespace Avalia.Desktop.Componentes;

/// <summary>Mostra a prévia de uma imagem sem manter o arquivo bloqueado.</summary>
public static class ImagemPrevia
{
    /// <returns>false se o arquivo não pôde ser aberto.</returns>
    public static bool Exibir(PictureBox quadro, string? caminho)
    {
        var anterior = quadro.Image;
        quadro.Image = null;
        anterior?.Dispose();

        if (caminho is null)
            return true;

        try
        {
            using var arquivo = File.OpenRead(caminho);
            using var imagem = Image.FromStream(arquivo);
            quadro.Image = new Bitmap(imagem);
            return true;
        }
        catch (Exception ex) when (ex is IOException or ArgumentException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    /// <summary>Aceita arquivos arrastados do Explorador de Arquivos.</summary>
    public static void AceitarArrastar(Control alvo, Action<string> aoSoltar)
    {
        alvo.AllowDrop = true;
        alvo.DragEnter += (_, e) =>
            e.Effect = e.Data?.GetDataPresent(DataFormats.FileDrop) == true ? DragDropEffects.Copy : DragDropEffects.None;
        alvo.DragDrop += (_, e) =>
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] { Length: > 0 } arquivos)
                aoSoltar(arquivos[0]);
        };
    }
}
