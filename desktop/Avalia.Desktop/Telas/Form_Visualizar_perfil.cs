using Avalia.Desktop.Componentes;
using Avalia.Desktop.Core.Controles;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Core.Servicos;
using Avalia.Desktop.Core.Validacoes;

namespace Avalia.Desktop.Telas;

/// <summary>
/// Form_Visualizar_perfil (Meu Perfil): dados do usuário conectado, alteração da
/// própria senha (RN04) e saída da conta. Caso de uso Visualizar perfil:
/// visualizarPerfil() → Visualizar_perfil.visualizarPerfil(id) → exibirDados();
/// alterarSenha(senhaAtual, novaSenha, confirmacao) → Visualizar_perfil.alterarSenha(...).
/// </summary>
public partial class Form_Visualizar_perfil : TelaBase
{
    private readonly Visualizar_perfil _controle;
    private readonly SessaoUsuario _sessao;
    private readonly ValidacaoVisual _validacao;

    public Form_Visualizar_perfil(Visualizar_perfil controle, SessaoUsuario sessao, INavegacao navegacao)
    {
        _controle = controle;
        _sessao = sessao;
        Navegacao = navegacao;
        InitializeComponent();

        lblRegras.Text = "Regras da nova senha: " + ValidadorSenha.Regras;
        Estilo.BotaoPrimario(botaoAlterarSenha, btn_salvarSenha);
        Estilo.BotaoSecundario(btn_voltar, btnCancelarSenha);
        Estilo.BotaoPerigo(botaoSair);

        _validacao = new ValidacaoVisual(errorProvider)
            .Campo(CamposSenha.SenhaAtual, txt_senhaAtual)
            .Campo(CamposSenha.NovaSenha, txt_novaSenha)
            .Campo(CamposSenha.Confirmacao, txt_confirmacao);
    }

    /// <summary>Abre já com a alteração de senha à mostra (recomendação do painel, RN02).</summary>
    public bool AbrirAlteracaoSenha { get; set; }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        await VisualizarPerfil();
        if (AbrirAlteracaoSenha)
            MostrarAlteracaoSenha();
    }

    /// <summary>visualizarPerfil() → Visualizar_perfil.visualizarPerfil(id)</summary>
    public async Task VisualizarPerfil()
    {
        Usuario? usuario = null;
        if (!await Executar(async () => usuario = await _controle.VisualizarPerfil(_sessao.IdUsuario)))
            return;

        if (usuario is null)
        {
            ExibirErro("Não foi possível carregar os dados do seu usuário.");
            return;
        }

        ExibirDados(usuario);
    }

    /// <summary>exibirDados(): nome, e-mail, tipo de usuário e data de ingresso.</summary>
    public void ExibirDados(Usuario usuario)
    {
        labelNome.Text = usuario.Nome;
        labelEmail.Text = usuario.Email;
        labelTipoUsuario.Text = usuario.Perfil.Descricao();
        labelDataIngresso.Text = usuario.DataIngresso.ToString("dd/MM/yyyy");
        pnlRecomendacao.Visible = _sessao.RecomendarTrocaSenha;
    }

    /// <summary>Mostra os campos de senha (botão Alterar senha).</summary>
    public void MostrarAlteracaoSenha()
    {
        _validacao.Limpar();
        pnlRecomendacao.Visible = _sessao.RecomendarTrocaSenha;
        pnlSenha.Visible = true;
        AcceptButton = btn_salvarSenha;
        txt_senhaAtual.Focus();
    }

    /// <summary>alterarSenha(senhaAtual, novaSenha, confirmacao) → Visualizar_perfil.alterarSenha(id, ...)</summary>
    public async Task AlterarSenha(string senhaAtual, string novaSenha, string confirmacao)
    {
        ResultadoOperacao? resultado = null;
        if (!await Executar(async () =>
                resultado = await _controle.AlterarSenha(_sessao.IdUsuario, senhaAtual, novaSenha, confirmacao)))
            return;

        if (!resultado!.Sucesso)
        {
            _validacao.Mostrar(resultado, ExibirAviso);
            return;
        }

        OcultarAlteracaoSenha();
        ExibirMensagemSucesso(resultado.Mensagem);
    }

    /// <summary>sairConta() → Visualizar_perfil.sairConta(): volta para a autenticação.</summary>
    public void SairConta()
    {
        _controle.SairConta();
        Navegacao!.SairDaConta();
    }

    /// <summary>voltar(): volta para o painel.</summary>
    public void Voltar() => Navegacao!.FecharTodas();

    private void OcultarAlteracaoSenha()
    {
        txt_senhaAtual.Clear();
        txt_novaSenha.Clear();
        txt_confirmacao.Clear();
        chkMostrarSenhas.Checked = false;
        _validacao.Limpar();
        pnlSenha.Visible = false;
        AcceptButton = null;
        botaoAlterarSenha.Focus();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode == Keys.Escape && pnlSenha.Visible)
        {
            OcultarAlteracaoSenha();
            e.Handled = true;
        }
    }

    private void botaoAlterarSenha_Click(object? sender, EventArgs e) => MostrarAlteracaoSenha();
    private void botaoSair_Click(object? sender, EventArgs e) => SairConta();
    private void btn_voltar_Click(object? sender, EventArgs e) => Voltar();
    private void btnCancelarSenha_Click(object? sender, EventArgs e) => OcultarAlteracaoSenha();

    private async void btn_salvarSenha_Click(object? sender, EventArgs e) =>
        await AlterarSenha(txt_senhaAtual.Text, txt_novaSenha.Text, txt_confirmacao.Text);

    private void chkMostrarSenhas_CheckedChanged(object? sender, EventArgs e)
    {
        var ocultar = !chkMostrarSenhas.Checked;
        txt_senhaAtual.UseSystemPasswordChar = ocultar;
        txt_novaSenha.UseSystemPasswordChar = ocultar;
        txt_confirmacao.UseSystemPasswordChar = ocultar;
    }
}
