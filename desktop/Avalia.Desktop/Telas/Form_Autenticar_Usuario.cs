using System.Drawing.Drawing2D;
using Avalia.Desktop.Componentes;
using Avalia.Desktop.Core.Controles;

namespace Avalia.Desktop.Telas;

/// <summary>
/// Form_Autenticar_Usuario: login por e-mail e senha (RF01, RN02, RN26).
/// Caso de uso Autenticar Usuário: digitarCredenciais(email, senha) →
/// ControleAutenticarUsuario.autenticarUsuario(email, senha) → loginSucesso() ou exibirErro(mensagem).
/// Todas as mensagens aparecem na própria tela (label_erro), sem janelas pop-up.
/// </summary>
public partial class Form_Autenticar_Usuario : Form
{
    private readonly ControleAutenticarUsuario _controle;

    public Form_Autenticar_Usuario(ControleAutenticarUsuario controle)
    {
        _controle = controle;
        InitializeComponent();

        Icon = Imagens.Icone;
        picLogo.Image = Imagens.LogoBranco;
        Estilo.BotaoPrimario(btn_autenticarUsuario);
        Estilo.BotaoSecundario(btn_sairSistema);
    }

    /// <summary>Verdadeiro quando um Administrador entrou; o Form_Administrador é aberto em seguida.</summary>
    public bool LoginRealizado { get; private set; }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        button_email.Focus();
    }

    /// <summary>Banco não configurado: avisa na tela e impede o login.</summary>
    public void ExibirErroConfiguracao(string mensagem)
    {
        ExibirErro(mensagem);
        btn_autenticarUsuario.Enabled = false;
    }

    /// <summary>digitarCredenciais(email, senha) → autenticarUsuario(email, senha)</summary>
    public async Task DigitarCredenciais(string email, string senha)
    {
        ExibirInformacao("Conectando ao banco de dados...");

        ResultadoAutenticacao? resultado = null;
        var concluiu = await Operacao.ExecutarAsync(this, ExibirErro,
            async () => resultado = await _controle.AutenticarUsuario(email, senha));

        if (!concluiu || resultado is null)
            return;

        if (!resultado.Sucesso)
        {
            // Mensagem genérica: não informa se o erro está no e-mail ou na senha
            ExibirErro(resultado.Mensagem);
            button_senha.Clear();
            (button_email.Text.Length == 0 ? button_email : button_senha).Focus();
            return;
        }

        if (!resultado.PossuiTelasNaDesktop)
        {
            // RN26: limitação de escopo de telas, não bloqueio de segurança
            ExibirInformacao($"Olá, {resultado.Usuario!.Nome}! A aplicação Desktop tem apenas as telas do Administrador. " +
                             "As telas do seu perfil estão na versão Web (e, para alunos, também no aplicativo Mobile).");
            button_senha.Clear();
            button_email.Focus();
            return;
        }

        LoginSucesso();
    }

    /// <summary>loginSucesso(): fecha a autenticação e abre o Form_Administrador.</summary>
    public void LoginSucesso()
    {
        LoginRealizado = true;
        Close();
    }

    /// <summary>exibirErro(mensagem)</summary>
    public void ExibirErro(string mensagem) => ExibirTexto(mensagem, Estilo.Perigo);

    private void ExibirInformacao(string mensagem) => ExibirTexto(mensagem, Estilo.Menu);

    private void ExibirTexto(string texto, Color cor)
    {
        label_erro.ForeColor = cor;
        label_erro.Text = texto;
        label_erro.AccessibleDescription = texto;
    }

    /// <summary>sairSistema(): encerra a aplicação.</summary>
    public void SairSistema()
    {
        _controle.SairSistema();
        Close();
    }

    private async void btn_autenticarUsuario_Click(object? sender, EventArgs e) =>
        await DigitarCredenciais(button_email.Text, button_senha.Text);

    private void btn_sairSistema_Click(object? sender, EventArgs e) => SairSistema();

    private void chkMostrarSenha_CheckedChanged(object? sender, EventArgs e) =>
        button_senha.UseSystemPasswordChar = !chkMostrarSenha.Checked;

    private void lnkEsqueciSenha_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e) =>
        ExibirInformacao("Esqueci minha senha: disponível na versão Web. A recuperação por e-mail (RN03) " +
                         "depende da API e ainda não está na aplicação Desktop.");

    private void pnlLateral_Paint(object? sender, PaintEventArgs e)
    {
        // Mesmo degradê azul → roxo da versão Web
        using var pincel = new LinearGradientBrush(pnlLateral.ClientRectangle, Estilo.Primaria, Estilo.Roxo, 135f);
        e.Graphics.FillRectangle(pincel, pnlLateral.ClientRectangle);
    }
}
