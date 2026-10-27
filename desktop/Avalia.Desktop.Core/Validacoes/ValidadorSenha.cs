using System.Text;

namespace Avalia.Desktop.Core.Validacoes;

/// <summary>Regras para a nova senha (RN04: o usuário altera a própria senha).</summary>
public static class ValidadorSenha
{
    public const int TamanhoMinimo = 8;

    /// <summary>O BCrypt considera no máximo 72 bytes da senha.</summary>
    public const int TamanhoMaximoBytes = 72;

    public const string Regras = "Mínimo de 8 caracteres, com pelo menos uma letra e um número.";

    public static IReadOnlyList<string> Validar(string? senha)
    {
        var erros = new List<string>();
        senha ??= string.Empty;

        if (senha.Length < TamanhoMinimo)
            erros.Add($"A senha deve ter pelo menos {TamanhoMinimo} caracteres.");
        if (Encoding.UTF8.GetByteCount(senha) > TamanhoMaximoBytes)
            erros.Add("A senha é longa demais.");
        if (!senha.Any(char.IsLetter))
            erros.Add("A senha deve conter pelo menos uma letra.");
        if (!senha.Any(char.IsDigit))
            erros.Add("A senha deve conter pelo menos um número.");
        if (senha.Length > 0 && senha.Trim().Length != senha.Length)
            erros.Add("A senha não pode começar nem terminar com espaço.");

        return erros;
    }
}
