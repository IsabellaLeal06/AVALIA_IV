namespace Avalia.Desktop.Core.Validacoes;

/// <summary>Validação de CPF pelos dígitos verificadores (módulo 11).</summary>
public static class ValidadorCpf
{
    /// <summary>Remove pontos, traço e espaços, mantendo apenas os dígitos.</summary>
    public static string Normalizar(string? cpf) =>
        new((cpf ?? string.Empty).Where(char.IsAsciiDigit).ToArray());

    public static bool EhValido(string? cpf)
    {
        var digitos = Normalizar(cpf);

        // A entrada não pode ter letras ou outros símbolos além de . - e espaço
        if (cpf is not null && cpf.Any(c => !char.IsAsciiDigit(c) && c is not '.' and not '-' and not ' '))
            return false;

        if (digitos.Length != 11)
            return false;

        // 000.000.000-00, 111.111.111-11... passam no cálculo, mas não são válidos
        if (digitos.All(c => c == digitos[0]))
            return false;

        return digitos[9] - '0' == CalcularDigito(digitos, 9)
            && digitos[10] - '0' == CalcularDigito(digitos, 10);
    }

    /// <summary>Formata 11 dígitos como 000.000.000-00.</summary>
    public static string Formatar(string? cpf)
    {
        var d = Normalizar(cpf);
        return d.Length == 11 ? $"{d[..3]}.{d[3..6]}.{d[6..9]}-{d[9..]}" : cpf ?? string.Empty;
    }

    private static int CalcularDigito(string digitos, int quantidade)
    {
        var soma = 0;
        for (var i = 0; i < quantidade; i++)
            soma += (digitos[i] - '0') * (quantidade + 1 - i);

        var resto = soma * 10 % 11;
        return resto == 10 ? 0 : resto;
    }
}
