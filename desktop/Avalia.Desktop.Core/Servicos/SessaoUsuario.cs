using Avalia.Desktop.Core.Modelos;

namespace Avalia.Desktop.Core.Servicos;

/// <summary>Usuário autenticado na aplicação. Registrada como singleton.</summary>
public class SessaoUsuario
{
    public UsuarioLogado? Usuario { get; private set; }

    public bool Autenticado => Usuario is not null;

    /// <summary>Id do usuário logado; usado, por exemplo, na autoria das questões (RN08).</summary>
    public int IdUsuario => Usuario?.Id ?? throw new InvalidOperationException("Nenhum usuário autenticado.");

    /// <summary>RN02: a senha usada no login ainda é o CPF. A troca é recomendada, não obrigatória.</summary>
    public bool RecomendarTrocaSenha { get; set; }

    /// <summary>Token da API (prazo de 7 dias), enviado em toda requisição. Fica só na memória.</summary>
    public string? Token { get; private set; }

    public void Iniciar(UsuarioLogado usuario, bool recomendarTrocaSenha = false, string? token = null)
    {
        Usuario = usuario;
        RecomendarTrocaSenha = recomendarTrocaSenha;
        Token = token;
    }

    public void Encerrar()
    {
        Usuario = null;
        RecomendarTrocaSenha = false;
        Token = null;
    }
}
