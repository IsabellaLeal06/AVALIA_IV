namespace Avalia.Desktop.Core.Servicos;

/// <summary>Erro de validação. Campo nulo indica um erro geral, não ligado a um campo da tela.</summary>
public sealed record ErroValidacao(string? Campo, string Mensagem);

/// <summary>Resultado das operações dos serviços: sucesso com mensagem ou lista de erros.</summary>
public class ResultadoOperacao
{
    protected ResultadoOperacao(string mensagem, IReadOnlyList<ErroValidacao> erros)
    {
        Mensagem = mensagem;
        Erros = erros;
    }

    public bool Sucesso => Erros.Count == 0;
    public string Mensagem { get; }
    public IReadOnlyList<ErroValidacao> Erros { get; }

    /// <summary>Todas as mensagens de erro, uma por linha.</summary>
    public string MensagemErros => string.Join(Environment.NewLine, Erros.Select(e => e.Mensagem).Distinct());

    public static ResultadoOperacao Ok(string mensagem) => new(mensagem, []);

    public static ResultadoOperacao Falha(string mensagem, string? campo = null) =>
        new(mensagem, [new ErroValidacao(campo, mensagem)]);

    public static ResultadoOperacao Falha(IEnumerable<ErroValidacao> erros)
    {
        var lista = erros.ToList();
        return new(lista.FirstOrDefault()?.Mensagem ?? "Operação não concluída.", lista);
    }
}

public sealed class ResultadoOperacao<T> : ResultadoOperacao
{
    private ResultadoOperacao(T? valor, string mensagem, IReadOnlyList<ErroValidacao> erros)
        : base(mensagem, erros) => Valor = valor;

    public T? Valor { get; }

    public static ResultadoOperacao<T> Ok(T valor, string mensagem) => new(valor, mensagem, []);

    public static new ResultadoOperacao<T> Falha(string mensagem, string? campo = null) =>
        new(default, mensagem, [new ErroValidacao(campo, mensagem)]);

    public static new ResultadoOperacao<T> Falha(IEnumerable<ErroValidacao> erros)
    {
        var lista = erros.ToList();
        return new(default, lista.FirstOrDefault()?.Mensagem ?? "Operação não concluída.", lista);
    }
}

/// <summary>Acumula os erros de validação antes de salvar.</summary>
public sealed class ListaErros : List<ErroValidacao>
{
    public void Adicionar(string? campo, string mensagem) => Add(new ErroValidacao(campo, mensagem));
    public bool PossuiErros => Count > 0;
}
