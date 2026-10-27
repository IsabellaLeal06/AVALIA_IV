using Avalia.Desktop.Componentes;
using Avalia.Desktop.Core.Controles;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Core.Servicos;

namespace Avalia.Desktop.Telas;

/// <summary>
/// Form_Administrador: janela principal do Administrador, com o menu lateral e o
/// painel com a quantidade de cadastros ativos (caso de uso Visualizar Painel:
/// visualizarPainel() → ControlePainel.contar()). As demais telas abrem dentro
/// dela, sem janelas pop-up (RN26).
/// </summary>
public partial class Form_Administrador : Form
{
    private readonly Navegador _navegador;
    private readonly ControlePainel _controlePainel;
    private readonly Visualizar_perfil _controlePerfil;
    private readonly SessaoUsuario _sessao;
    private readonly Dictionary<Type, Button> _menu;

    public Form_Administrador(Navegador navegador, ControlePainel controlePainel, Visualizar_perfil controlePerfil,
        SessaoUsuario sessao, InformacoesAmbiente ambiente)
    {
        _navegador = navegador;
        _controlePainel = controlePainel;
        _controlePerfil = controlePerfil;
        _sessao = sessao;
        InitializeComponent();

        Icon = Imagens.Icone;
        picLogo.Image = Imagens.LogoBranco;
        Estilo.BotaoMenu(btn_visualizarPainel, btn_gerenciarUsuario, btn_gerenciarTurma, btn_gerenciarDisciplina,
            btn_gerenciarQuestoes, btn_gerenciarMedalha, btn_visualizarPerfil, btn_sairConta);
        Estilo.BotaoSecundario(btnAtualizarPainel);
        Estilo.BotaoPrimario(btnAlterarSenhaAgora);
        _menu = new Dictionary<Type, Button>
        {
            [typeof(Form_Usuario)] = btn_gerenciarUsuario,
            [typeof(Form_Turma)] = btn_gerenciarTurma,
            [typeof(Form_Disciplina)] = btn_gerenciarDisciplina,
            [typeof(Form_Questao)] = btn_gerenciarQuestoes,
            [typeof(Form_Medalha)] = btn_gerenciarMedalha,
            [typeof(Form_Visualizar_perfil)] = btn_visualizarPerfil
        };
        foreach (var botao in _menu.Values.Append(btn_visualizarPainel))
            botao.Font = new Font(botao.Font.FontFamily, 11F);

        var usuario = sessao.Usuario!;
        label_usuarioLogado.Text = $"Olá, {usuario.Nome}\n{usuario.Perfil.Descricao()}";
        lblStatusBanco.Text = $"Banco: {ambiente.DestinoBanco}";
        lblStatusVersao.Text = $"Avalia+ Desktop {ambiente.Versao}";
        lblDetalheTurmas.Text = "Turmas disponíveis para vincular alunos";
        lblDetalheDisciplinas.Text = "Disciplinas disponíveis para questões e simulados";
        lblDetalheQuestoes.Text = "Questões disponíveis para o sorteio";

        _navegador.Configurar(pnlConteudo, barraMensagem);
        _navegador.TelaAlterada += Navegador_TelaAlterada;
        _navegador.SaidaSolicitada += Navegador_SaidaSolicitada;
    }

    /// <summary>Indica que o usuário escolheu "Sair da conta" (volta para a autenticação).</summary>
    public bool SaiuDaConta { get; private set; }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        AtualizarMenu();
        await VisualizarPainel();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _navegador.TelaAlterada -= Navegador_TelaAlterada;
        _navegador.SaidaSolicitada -= Navegador_SaidaSolicitada;
        _navegador.Encerrar();
        base.OnFormClosed(e);
    }

    protected override async void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode == Keys.F5 && pnlPainel.Visible)
        {
            e.Handled = true;
            await VisualizarPainel();
        }
    }

    /// <summary>visualizarPainel() → ControlePainel.contar() : Quantidades</summary>
    public async Task VisualizarPainel()
    {
        if (_navegador.TelaInicial is not null)
        {
            _navegador.FecharTodas(); // TelaAlterada chama VisualizarPainel de novo, já com o painel à mostra
            return;
        }

        // RN02: recomendação de troca da senha inicial, sem obrigar
        pnlRecomendacao.Visible = _sessao.RecomendarTrocaSenha;

        Quantidades? dados = null;
        if (!await Operacao.ExecutarAsync(pnlPainel, erro => _navegador.ExibirMensagem(TipoMensagem.Erro, erro),
                async () => dados = await _controlePainel.Contar()))
            return;

        Exibir(label_qtdTurmas, pnlCardTurmas, dados!.Turmas, "turmas ativas");
        Exibir(label_qtdUsuarios, pnlCardUsuarios, dados.Usuarios, "usuários ativos");
        Exibir(label_qtdDisciplinas, pnlCardDisciplinas, dados.Disciplinas, "disciplinas ativas");
        Exibir(label_qtdQuestoes, pnlCardQuestoes, dados.Questoes, "questões ativas");
        lblDetalheUsuarios.Text =
            $"{dados.Alunos} aluno(s) · {dados.Professores} professor(es) · {dados.Administradores} administrador(es)";
        lblAtualizadoEm.Text = $"Atualizado em {DateTime.Now:dd/MM/yyyy HH:mm:ss}.";
    }

    private static void Exibir(Label valor, Panel cartao, int quantidade, string descricao)
    {
        valor.Text = quantidade.ToString("N0");
        cartao.AccessibleDescription = $"{quantidade} {descricao}";
    }

    // Cada item do menu abre a sua tela, que executa abrirGerenciamento()
    public void GerenciarUsuario() => _navegador.AbrirInicio<Form_Usuario>();
    public void GerenciarTurma() => _navegador.AbrirInicio<Form_Turma>();
    public void GerenciarDisciplina() => _navegador.AbrirInicio<Form_Disciplina>();
    public void GerenciarQuestoes() => _navegador.AbrirInicio<Form_Questao>();
    public void GerenciarMedalha() => _navegador.AbrirInicio<Form_Medalha>();
    public void VisualizarPerfil(bool abrirAlteracaoSenha = false) =>
        _navegador.AbrirInicio<Form_Visualizar_perfil>(f => f.AbrirAlteracaoSenha = abrirAlteracaoSenha);

    /// <summary>sairConta() → Visualizar_perfil.sairConta(): volta para a autenticação.</summary>
    public void SairConta()
    {
        _controlePerfil.SairConta();
        SaiuDaConta = true;
        Close();
    }

    /// <summary>Destaca no menu a tela aberta e mostra o painel quando nenhuma tela está aberta.</summary>
    private async void Navegador_TelaAlterada(object? sender, EventArgs e)
    {
        var painel = _navegador.TelaInicial is null;
        var estavaVisivel = pnlPainel.Visible;
        pnlPainel.Visible = painel;
        AtualizarMenu();
        if (painel && !estavaVisivel)
            await VisualizarPainel();
    }

    private void AtualizarMenu()
    {
        var inicial = _navegador.TelaInicial;
        foreach (var (botao, selecionado) in _menu.Select(m => (m.Value, m.Key == inicial))
                     .Append((btn_visualizarPainel, inicial is null)))
        {
            botao.BackColor = selecionado ? Estilo.MenuSelecionado : Estilo.Menu;
            botao.Font = new Font(botao.Font, selecionado ? FontStyle.Bold : FontStyle.Regular);
            botao.AccessibleDescription = selecionado ? "Tela aberta" : null;
        }

        Text = inicial is not null && _menu.TryGetValue(inicial, out var atual)
            ? $"Avalia+ | {atual.AccessibleName ?? atual.Text.Replace("&", string.Empty)}"
            : "Avalia+ | Painel";
    }

    private void Navegador_SaidaSolicitada(object? sender, EventArgs e)
    {
        SaiuDaConta = true;
        Close();
    }

    private async void btn_visualizarPainel_Click(object? sender, EventArgs e) => await VisualizarPainel();
    private void btn_gerenciarUsuario_Click(object? sender, EventArgs e) => GerenciarUsuario();
    private void btn_gerenciarTurma_Click(object? sender, EventArgs e) => GerenciarTurma();
    private void btn_gerenciarDisciplina_Click(object? sender, EventArgs e) => GerenciarDisciplina();
    private void btn_gerenciarQuestoes_Click(object? sender, EventArgs e) => GerenciarQuestoes();
    private void btn_gerenciarMedalha_Click(object? sender, EventArgs e) => GerenciarMedalha();
    private void btn_visualizarPerfil_Click(object? sender, EventArgs e) => VisualizarPerfil();
    private void btn_sairConta_Click(object? sender, EventArgs e) => SairConta();
    private async void btnAtualizarPainel_Click(object? sender, EventArgs e) => await VisualizarPainel();
    private void btnAlterarSenhaAgora_Click(object? sender, EventArgs e) => VisualizarPerfil(abrirAlteracaoSenha: true);
}

/// <summary>Informações exibidas na barra de status.</summary>
public sealed record InformacoesAmbiente(string DestinoBanco, string Versao);
