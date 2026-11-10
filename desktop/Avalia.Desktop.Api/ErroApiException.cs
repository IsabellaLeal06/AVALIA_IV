namespace Avalia.Desktop.Api;

/// <summary>Resposta de erro da API no formato padrão: { "erro": { "mensagem", "campos" } }.</summary>
public class ErroApiException(int status, string mensagem, IReadOnlyDictionary<string, string>? campos = null)
    : Exception(mensagem)
{
    public int Status { get; } = status;
    public IReadOnlyDictionary<string, string> Campos { get; } = campos ?? new Dictionary<string, string>();

    /// <summary>400 (dados inválidos) ou 409 (conflito com o estado atual): o usuário pode corrigir.</summary>
    public bool EhValidacao => Status is 400 or 409;
}
