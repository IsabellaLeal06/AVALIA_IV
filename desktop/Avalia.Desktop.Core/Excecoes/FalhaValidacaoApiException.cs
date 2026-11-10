using Avalia.Desktop.Core.Servicos;

namespace Avalia.Desktop.Core.Excecoes;

/// <summary>
/// A API recusou os dados (HTTP 400/409). Traz a mensagem e os campos a corrigir, já com os
/// nomes dos campos usados pelas telas do Desktop. Os controles transformam em <see cref="ResultadoOperacao"/>.
/// </summary>
public class FalhaValidacaoApiException(string mensagem, IReadOnlyList<ErroValidacao> erros)
    : Exception(mensagem)
{
    public IReadOnlyList<ErroValidacao> Erros { get; } = erros;
}
