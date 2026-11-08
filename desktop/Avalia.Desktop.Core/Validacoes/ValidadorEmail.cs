using System.Text.RegularExpressions;

namespace Avalia.Desktop.Core.Validacoes;

public static partial class ValidadorEmail
{
    public const int TamanhoMaximo = 150;

    public static string Normalizar(string? email) => (email ?? string.Empty).Trim().ToLowerInvariant();

    public static bool EhValido(string? email)
    {
        var valor = Normalizar(email);
        return valor.Length is > 0 and <= TamanhoMaximo && Formato().IsMatch(valor);
    }

    // usuario@dominio.ext — sem espaços, com um único @ e ao menos um ponto no domínio
    [GeneratedRegex(@"^[a-z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?(?:\.[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?)+$")]
    private static partial Regex Formato();
}
