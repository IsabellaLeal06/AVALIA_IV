namespace Avalia.Desktop.Core.Modelos;

/// <summary>Tabela Usuario. Usado na edição e na autenticação.</summary>
public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    /// <summary>Hash BCrypt da senha (RNF01). Nunca contém a senha em texto.</summary>
    public string SenhaHash { get; set; } = string.Empty;

    public string? FotoUrl { get; set; }
    public DateTime DataIngresso { get; set; } = DateTime.Today;
    public bool Ativo { get; set; } = true;
    public int PontuacaoTotal { get; set; }
    public PerfilUsuario Perfil { get; set; } = PerfilUsuario.Aluno;

    /// <summary>Preenchido somente para Aluno (RN05).</summary>
    public int? IdTurma { get; set; }

    public string? NomeTurma { get; set; }
}

/// <summary>
/// Linha da listagem de usuários. Não possui CPF nem senha: o CPF só
/// aparece na tela de edição.
/// </summary>
public class UsuarioResumo
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }
    public string? NomeTurma { get; set; }
    public bool Ativo { get; set; }

    public string PerfilDescricao => Perfil.Descricao();
    public string TurmaDescricao => NomeTurma ?? "—";
    public string Situacao => Ativo ? "Ativo" : "Inativo";
}

/// <summary>Usuário autenticado na sessão da Desktop.</summary>
public sealed record UsuarioLogado(int Id, string Nome, string Email, PerfilUsuario Perfil);
