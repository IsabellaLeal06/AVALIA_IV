using Avalia.Desktop.Core.Controles;
using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Modelos;

namespace Avalia.Desktop.Api.Repositorios;

/// <summary>Login (POST /auth/login) e troca da própria senha (POST /auth/alterar-senha).</summary>
public class AutenticacaoApi(ClienteApi api) : RepositorioApiBase(api), IAutenticacao
{
    private static readonly Dictionary<string, string> CamposSenha = new()
    {
        ["senhaAtual"] = Core.Controles.CamposSenha.SenhaAtual,
        ["novaSenha"] = Core.Controles.CamposSenha.NovaSenha,
        ["confirmacao"] = Core.Controles.CamposSenha.Confirmacao
    };

    public async Task<SessaoApi?> Entrar(string email, string senha)
    {
        try
        {
            var r = await Api.Post<LoginDto>("auth/login", new { email, senha });
            return r is null
                ? null
                : new SessaoApi(r.Token, new UsuarioLogado(r.Usuario.Id, r.Usuario.Nome, r.Usuario.Email, (PerfilUsuario)r.Usuario.IdPerfil), r.SenhaInicial);
        }
        catch (ErroApiException ex) when (ex.Status is 400 or 401)
        {
            // E-mail inexistente, senha errada e conta desativada: a API responde igual (mensagem genérica)
            return null;
        }
        catch (ErroApiException ex)
        {
            throw new Core.Excecoes.FalhaAcessoDadosException(ex.Message, ex);
        }
    }

    public async Task AlterarSenha(string senhaAtual, string novaSenha, string confirmacao) =>
        await Escrever(() => Api.Post<object>("auth/alterar-senha", new { senhaAtual, novaSenha, confirmacao }), CamposSenha);
}
