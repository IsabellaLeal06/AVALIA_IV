using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Servicos;
using Avalia.Desktop.Core.Validacoes;
using Avalia.Desktop.Core.Modelos;

namespace Avalia.Desktop.Core.Controles;

public sealed class ResultadoAutenticacao
{
    public bool Sucesso { get; init; }
    public string Mensagem { get; init; } = string.Empty;
    public UsuarioLogado? Usuario { get; init; }

    /// <summary>RN26: falso para Aluno e Professor, cujas telas estão na Web/Mobile.</summary>
    public bool PossuiTelasNaDesktop { get; init; }

    /// <summary>RN02: a senha usada ainda é o CPF; a troca é recomendada, não obrigatória.</summary>
    public bool RecomendarTrocaSenha { get; init; }
}

/// <summary>
/// Controle ControleAutenticarUsuario (caso de uso Autenticar Usuário):
/// autenticarUsuario(email, senha) e sairSistema() (RF01, RN02, RN26, RNF01).
/// A conferência da senha é da API: o Desktop não guarda nem compara hash.
/// </summary>
public class ControleAutenticarUsuario(IAutenticacao autenticacao, SessaoUsuario sessao)
{
    /// <summary>Mensagem única para qualquer falha: não revela se o erro foi no e-mail ou na senha.</summary>
    public const string MensagemFalhaLogin = "E-mail ou senha inválidos.";

    public const string MensagemPerfilSemTelas =
        "A aplicação Desktop contém apenas as telas do perfil Administrador.\n\n" +
        "As telas do seu perfil estão disponíveis na versão Web" +
        " (e, para alunos, também no aplicativo Mobile).";

    /// <summary>autenticarUsuario(email, senha) → POST /auth/login da API.</summary>
    public async Task<ResultadoAutenticacao> AutenticarUsuario(string? email, string? senha)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(senha))
            return new ResultadoAutenticacao { Mensagem = "Informe o e-mail e a senha." };

        // E-mail inexistente, senha errada e conta desativada recebem a mesma resposta da API
        var sessaoApi = await autenticacao.Entrar(ValidadorEmail.Normalizar(email), senha);
        if (sessaoApi is null)
            return new ResultadoAutenticacao { Mensagem = MensagemFalhaLogin };

        var logado = sessaoApi.Usuario;
        var possuiTelas = logado.Perfil.PossuiTelasNaDesktop();

        // RN26 não é bloqueio de segurança: Aluno e Professor autenticam, mas só o Administrador usa esta aplicação
        if (possuiTelas)
            sessao.Iniciar(logado, sessaoApi.SenhaInicial, sessaoApi.Token);

        return new ResultadoAutenticacao
        {
            Sucesso = true,
            Usuario = logado,
            PossuiTelasNaDesktop = possuiTelas,
            RecomendarTrocaSenha = sessaoApi.SenhaInicial,
            Mensagem = possuiTelas ? $"Bem-vindo(a), {logado.Nome}!" : MensagemPerfilSemTelas
        };
    }

    /// <summary>sairSistema(): encerra a sessão (apaga o token da memória).</summary>
    public void SairSistema() => sessao.Encerrar();
}
