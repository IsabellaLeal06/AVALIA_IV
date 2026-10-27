namespace Avalia.Desktop.Core.Validacoes;

/// <summary>Confere os arquivos de imagem das questões e das medalhas.</summary>
public static class ValidadorImagem
{
    public static readonly string[] Extensoes = [".png", ".jpg", ".jpeg", ".gif", ".bmp"];
    public const long TamanhoMaximoBytes = 2 * 1024 * 1024;
    public const int TamanhoMaximoUrl = 500;

    /// <summary>Tira espaços e aspas do caminho colado pelo usuário.</summary>
    public static string LimparCaminho(string? caminho) => (caminho ?? string.Empty).Trim().Trim('"');

    /// <summary>Devolve o erro, ou null se o arquivo existe e tem tipo e tamanho aceitos.</summary>
    public static string? Validar(string? caminho)
    {
        caminho = LimparCaminho(caminho);
        if (caminho.Length == 0)
            return "Informe o arquivo de imagem.";

        FileInfo arquivo;
        try
        {
            arquivo = new FileInfo(caminho);
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return "Caminho de arquivo inválido.";
        }

        if (!arquivo.Exists)
            return "Arquivo de imagem não encontrado.";
        if (!Extensoes.Contains(arquivo.Extension.ToLowerInvariant()))
            return "Formato de imagem não suportado. Use PNG, JPG, GIF ou BMP.";
        if (arquivo.Length > TamanhoMaximoBytes)
            return "A imagem deve ter no máximo 2 MB.";
        return null;
    }

    /// <summary>Verdadeiro quando o texto é o caminho de um arquivo que existe neste computador.</summary>
    public static bool EhArquivoLocal(string? caminho)
    {
        caminho = LimparCaminho(caminho);
        try
        {
            return Path.IsPathRooted(caminho) && File.Exists(caminho);
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException)
        {
            return false;
        }
    }
}
