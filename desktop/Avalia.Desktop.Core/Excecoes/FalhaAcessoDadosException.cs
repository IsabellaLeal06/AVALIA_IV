namespace Avalia.Desktop.Core.Excecoes;

/// <summary>
/// Falha ao acessar os dados (banco fora do ar, firewall, login inválido...).
/// A mensagem já vem pronta para ser exibida ao usuário. A camada que hoje
/// acessa o banco e a futura camada que chamará a API lançam esta mesma exceção.
/// </summary>
public class FalhaAcessoDadosException(string mensagemAmigavel, Exception? interna = null)
    : Exception(mensagemAmigavel, interna);
