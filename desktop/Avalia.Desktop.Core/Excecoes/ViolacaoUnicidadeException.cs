namespace Avalia.Desktop.Core.Excecoes;

/// <summary>
/// Lançada pela camada de dados quando o banco recusa um valor duplicado
/// (restrições UNIQUE do script 02, RN27). O serviço transforma em uma
/// mensagem amigável.
/// </summary>
public class ViolacaoUnicidadeException(string? restricao, Exception? interna = null)
    : Exception($"Valor duplicado ({restricao ?? "restrição não identificada"}).", interna)
{
    /// <summary>Nome da restrição, por exemplo UQ_Usuario_Email.</summary>
    public string? Restricao { get; } = restricao;
}
