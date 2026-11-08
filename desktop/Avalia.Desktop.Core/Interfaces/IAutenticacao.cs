using Avalia.Desktop.Core.Modelos;

namespace Avalia.Desktop.Core.Interfaces;

/// <summary>Resultado de um login aceito pela API: o token (prazo de 7 dias) e os dados do usuário.</summary>
public sealed record SessaoApi(string Token, UsuarioLogado Usuario, bool SenhaInicial);

/// <summary>
/// Autenticação e senha pela API (RF01, RN04). O Desktop não calcula nem confere hash:
/// a senha vai à API por HTTPS/HTTP e a API compara com o hash BCrypt do banco.
/// </summary>
public interface IAutenticacao
{
    /// <summary>POST /auth/login. Devolve null se o e-mail/senha estiverem errados ou a conta estiver desativada.</summary>
    Task<SessaoApi?> Entrar(string email, string senha);

    /// <summary>POST /auth/alterar-senha: sempre do usuário do token (RN04).</summary>
    Task AlterarSenha(string senhaAtual, string novaSenha, string confirmacao);
}
