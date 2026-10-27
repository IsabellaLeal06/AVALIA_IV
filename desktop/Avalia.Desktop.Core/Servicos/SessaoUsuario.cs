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

    public void Iniciar(UsuarioLogado usuario, bool recomendarTrocaSenha = false)
    {
        Usuario = usuario;
        RecomendarTrocaSenha = recomendarTrocaSenha;
    }

    public void Encerrar()
    {
        Usuario = null;
        RecomendarTrocaSenha = false;
    }
}
