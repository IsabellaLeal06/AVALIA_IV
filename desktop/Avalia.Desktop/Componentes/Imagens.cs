using System.Reflection;

namespace Avalia.Desktop.Componentes;

/// <summary>Logo e ícone embutidos no executável (pasta Recursos).</summary>
public static class Imagens
{
    private static readonly Assembly Assembly = typeof(Imagens).Assembly;

    public static Image Logo => Carregar("logo.png");
    public static Image LogoBranco => Carregar("logo_branco.png");

    public static Icon Icone
    {
        get
        {
            using var fluxo = Abrir("avalia.ico");
            return new Icon(fluxo);
        }
    }

    private static Image Carregar(string arquivo)
    {
        using var fluxo = Abrir(arquivo);
        // Copia para memória para que o stream possa ser fechado
        return new Bitmap(Image.FromStream(fluxo));
    }

    private static Stream Abrir(string arquivo) =>
        Assembly.GetManifestResourceStream($"Avalia.Desktop.Recursos.{arquivo}")
        ?? throw new InvalidOperationException($"Recurso {arquivo} não encontrado.");
}
