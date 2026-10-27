using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Core.Servicos;
using Avalia.Desktop.Core.Validacoes;

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
/// </summary>
public class ControleAutenticarUsuario(IUsuarioRepository usuarios, IHashSenha hashSenha, SessaoUsuario sessao)
{
    /// <summary>Mensagem única para qualquer falha: não revela se o erro foi no e-mail ou na senha.</summary>
    public const string MensagemFalhaLogin = "E-mail ou senha inválidos.";

    public const string MensagemPerfilSemTelas =
        "A aplicação Desktop contém apenas as telas do perfil Administrador.\n\n" +
        "As telas do seu perfil estão disponíveis na versão Web" +
        " (e, para alunos, também no aplicativo Mobile).";

    // Hash usado quando o e-mail não existe, para que o tempo de resposta
    // seja parecido com o de uma senha errada (não revela e-mails cadastrados).
    private static readonly Lazy<string> HashFicticio =
        new(() => BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString(), BCryptHashSenha.Custo));

    /// <summary>autenticarUsuario(email, senha) → Usuario.consultarPorEmail(email)</summary>
    public async Task<ResultadoAutenticacao> AutenticarUsuario(string? email, string? senha)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(senha))
            return new ResultadoAutenticacao { Mensagem = "Informe o e-mail e a senha." };

        var usuario = await usuarios.ConsultarPorEmail(ValidadorEmail.Normalizar(email));

        if (usuario is null)
        {
            hashSenha.Verificar(senha, HashFicticio.Value);
            return new ResultadoAutenticacao { Mensagem = MensagemFalhaLogin };
        }

        var senhaConfere = hashSenha.Verificar(senha, usuario.SenhaHash);

        // Conta desativada recebe a mesma mensagem genérica
        if (!senhaConfere || !usuario.Ativo)
            return new ResultadoAutenticacao { Mensagem = MensagemFalhaLogin };

        var logado = new UsuarioLogado(usuario.Id, usuario.Nome, usuario.Email, usuario.Perfil);
        var possuiTelas = usuario.Perfil.PossuiTelasNaDesktop();
        var recomendarTroca = senha == usuario.Cpf;

        if (possuiTelas)
            sessao.Iniciar(logado, recomendarTroca);

        return new ResultadoAutenticacao
        {
            Sucesso = true,
            Usuario = logado,
            PossuiTelasNaDesktop = possuiTelas,
            RecomendarTrocaSenha = recomendarTroca,
            Mensagem = possuiTelas ? $"Bem-vindo(a), {usuario.Nome}!" : MensagemPerfilSemTelas
        };
    }

    /// <summary>sairSistema(): encerra a sessão, se houver, ao fechar a aplicação.</summary>
    public void SairSistema() => sessao.Encerrar();
}
