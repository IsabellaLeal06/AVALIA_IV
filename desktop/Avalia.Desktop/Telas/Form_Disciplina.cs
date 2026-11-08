using Avalia.Desktop.Componentes;
using Avalia.Desktop.Core.Controles;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Core.Servicos;

namespace Avalia.Desktop.Telas;

/// <summary>
/// Form_Disciplina: gerenciamento das disciplinas e do professor responsável
/// (RF02, RN07, RN12). Uma única tela com a lista e o cadastro (label_modo).
/// Os métodos seguem as mensagens dos diagramas de sequência Gerenciar, Listar,
/// Incluir, Consultar, Editar e Desativar Disciplina.
/// </summary>
public partial class Form_Disciplina : TelaCadastro
{
    private readonly ControleDisciplina _controle;
    private readonly ControleUsuario _controleUsuario;
    private readonly ControladorListagem<Disciplina> _lista;
    private readonly ValidacaoVisual _validacao;
    private int _idDisciplina;

    public Form_Disciplina(ControleDisciplina controle, ControleUsuario controleUsuario, INavegacao navegacao)
    {
        _controle = controle;
        _controleUsuario = controleUsuario;
        Navegacao = navegacao;
        InitializeComponent();
        ConfigurarModos(pnlLista, pnlCadastro, pnlCartao, label_modo, btn_incluir, btn_editar);

        Estilo.BotaoPrimario(btn_novo);
        Estilo.BotaoSecundario(btn_consultar, btn_reativar, btn_listar, btnLimparFiltros, btn_voltar);
        Estilo.BotaoPerigo(btn_excluir);
        colQuestoes.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        _lista = new ControladorListagem<Disciplina>(this, ExibirErro, dgvDisciplinas, label_totalRegistros, btn_excluir, btn_reativar)
        {
            NomePlural = "disciplinas",
            Carregar = () => _controle.Listar(new FiltroCadastro
            {
                Situacao = cboSituacaoFiltro.Valor<FiltroSituacao>(),
                Busca = txtBusca.Text
            }),
            Id = d => d.Id,
            Ativo = d => d.Ativo
        };
        _lista.DestacarInativos();

        _validacao = new ValidacaoVisual(errorProvider)
            .Campo(nameof(Disciplina.Nome), button_nome)
            .Campo(nameof(Disciplina.Descricao), button_descricao)
            .Campo(nameof(Disciplina.IdProfessorResponsavel), button_usuarioId);
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        await AbrirGerenciamento();
    }

    /// <summary>abrirGerenciamento(): carrega os filtros e mostra a lista (listarDisciplinas).</summary>
    public async Task AbrirGerenciamento()
    {
        _lista.IgnorarFiltros = true;
        cboSituacaoFiltro.Preencher(OpcoesFiltro.Situacoes(feminino: true));
        _lista.IgnorarFiltros = false;
        await AbrirListagem();
    }

    /// <summary>abrirListagem(): volta para a lista e lista as disciplinas.</summary>
    public async Task AbrirListagem(int? selecionarId = null)
    {
        DefinirModo(ModoCadastro.Listagem);
        label_titulo.Text = "Disciplinas";
        await ListarDisciplinas(selecionarId);
        dgvDisciplinas.Focus();
    }

    /// <summary>listarDisciplinas() → ControleDisciplina.listar()</summary>
    public Task ListarDisciplinas(int? selecionarId = null) => _lista.CarregarAsync(selecionarId);

    /// <summary>abrirInclusao(): formulário vazio para uma nova disciplina.</summary>
    public async Task AbrirInclusao()
    {
        _idDisciplina = 0;
        _validacao.Limpar();
        if (!await CarregarProfessores(null))
            return;

        ExibirDados(new Disciplina());
        DefinirModo(ModoCadastro.Inclusao);
        button_nome.Focus();
    }

    /// <summary>incluirDisciplina(dados) → ControleDisciplina.incluir(disciplina)</summary>
    public async Task IncluirDisciplina(Disciplina dados)
    {
        ResultadoOperacao<int>? resultado = null;
        if (!await Executar(async () => resultado = await _controle.Incluir(dados)))
            return;

        if (!resultado!.Sucesso)
        {
            _validacao.Mostrar(resultado, ExibirAviso);
            return;
        }

        await AbrirListagem(resultado.Valor);
        ExibirMensagemSucesso(resultado.Mensagem);
    }

    /// <summary>selecionarDisciplina(id): a disciplina escolhida na lista é consultada.</summary>
    public Task SelecionarDisciplina(int id) => ConsultarDisciplina(id);

    /// <summary>consultarDisciplina(id) → ControleDisciplina.consultar(id) → exibirDados()</summary>
    public async Task ConsultarDisciplina(int id)
    {
        Disciplina? disciplina = null;
        if (!await Executar(async () => disciplina = await _controle.Consultar(id)))
            return;

        if (disciplina is null)
        {
            ExibirErro($"Disciplina nº {id} não encontrada.");
            return;
        }

        _validacao.Limpar();
        if (!await CarregarProfessores(disciplina.IdProfessorResponsavel))
            return;

        _idDisciplina = disciplina.Id;
        ExibirDados(disciplina);
        DefinirModo(disciplina.Ativo ? ModoCadastro.Edicao : ModoCadastro.Consulta);
        if (!disciplina.Ativo)
        {
            label_titulo.Text = "Consultar disciplina";
            ExibirAviso("Disciplina inativa: os dados estão só para consulta. Para editar, reative-a na lista.");
        }
        button_nome.Focus();
    }

    /// <summary>exibirDados(): preenche o formulário com os dados da disciplina.</summary>
    public void ExibirDados(Disciplina disciplina)
    {
        var nova = disciplina.Id == 0;
        label_titulo.Text = nova ? "Nova disciplina" : "Editar disciplina";
        lblRegistro.Text = nova ? "Nova disciplina" : $"Disciplina nº {disciplina.Id}";
        button_nome.Text = disciplina.Nome;
        button_descricao.Text = disciplina.Descricao;
        button_ativo.Checked = disciplina.Ativo;
        button_ativo.Text = disciplina.Ativo ? "Ativa" : "Inativa";
        button_usuarioId.SelectedIndex = 0;
        foreach (ItemCombo<int?> item in button_usuarioId.Items)
            if (item.Valor == disciplina.IdProfessorResponsavel)
                button_usuarioId.SelectedItem = item;
    }

    /// <summary>editar() → ControleDisciplina.editar(id, disciplina)</summary>
    public async Task Editar()
    {
        ResultadoOperacao<int>? resultado = null;
        if (!await Executar(async () => resultado = await _controle.Editar(_idDisciplina, LerDados())))
            return;

        if (!resultado!.Sucesso)
        {
            _validacao.Mostrar(resultado, ExibirAviso);
            return;
        }

        await AbrirListagem(resultado.Valor);
        ExibirMensagemSucesso(resultado.Mensagem);
    }

    /// <summary>desativar() → Form_Conf_Desativação.solicitarDesativacao(mensagem)</summary>
    public async Task Desativar()
    {
        if (_lista.IdSelecionado is not int id)
            return;

        string? mensagem = null;
        if (!await Executar(async () => mensagem = await _controle.MensagemDesativacao(id)))
            return;

        Navegacao!.Abrir<Form_Conf_Desativação>(f => f.SolicitarDesativacao(mensagem!, () => ConcluirDesativacao(id)));
    }

    /// <summary>Depois de confirmarDesativação(): ControleDisciplina.desativar(id) e exibirMensagemSucesso().</summary>
    private async Task ConcluirDesativacao(int id)
    {
        ResultadoOperacao? resultado = null;
        if (!await Executar(async () => resultado = await _controle.Desativar(id)))
            return;

        await ListarDisciplinas(id);
        dgvDisciplinas.Focus();
        if (resultado!.Sucesso)
            ExibirMensagemSucesso(resultado.Mensagem);
        else
            ExibirAviso(resultado.MensagemErros);
    }

    public void Reativar()
    {
        if (_lista.Selecionado is not { Ativo: false } disciplina)
            return;

        Navegacao!.Abrir<Form_Conf_Desativação>(f => f.SolicitarReativacao(
            $"Deseja reativar a disciplina \"{disciplina.Nome}\"?", () => ConcluirReativacao(disciplina.Id)));
    }

    private async Task ConcluirReativacao(int id)
    {
        ResultadoOperacao? resultado = null;
        if (!await Executar(async () => resultado = await _controle.Reativar(id)))
            return;

        await ListarDisciplinas(id);
        dgvDisciplinas.Focus();
        if (resultado!.Sucesso)
            ExibirMensagemSucesso(resultado.Mensagem);
        else
            ExibirAviso(resultado.MensagemErros);
    }

    /// <summary>voltar(): do cadastro para a lista.</summary>
    public Task Voltar()
    {
        Navegacao!.OcultarMensagem();
        return AbrirListagem(_idDisciplina == 0 ? null : _idDisciplina);
    }

    /// <summary>
    /// RN07: responsável escolhido entre os professores ativos (ControleUsuario.listarProfessoresAtivos),
    /// com a opção "Sem responsável".
    /// </summary>
    private Task<bool> CarregarProfessores(int? idResponsavelAtual) => Executar(async () =>
    {
        var professores = await _controleUsuario.ListarProfessoresAtivos(idResponsavelAtual);
        button_usuarioId.Preencher(
            professores.Select(p => new ItemCombo<int?>(p.Id, p.Descricao)).Prepend(new(null, "Sem responsável")),
            idResponsavelAtual);
    });

    private Disciplina LerDados() => new()
    {
        Id = _idDisciplina,
        Nome = button_nome.Text,
        Descricao = button_descricao.Text,
        IdProfessorResponsavel = button_usuarioId.Valor<int?>()
    };

    // ---------- eventos da tela ----------
    protected override void TeclaListar() => _ = ListarDisciplinas();
    protected override void TeclaNovo() => _ = AbrirInclusao();
    protected override void TeclaVoltar() => _ = Voltar();

    private async void btn_novo_Click(object? sender, EventArgs e) => await AbrirInclusao();
    private async void btn_incluir_Click(object? sender, EventArgs e) => await IncluirDisciplina(LerDados());
    private async void btn_editar_Click(object? sender, EventArgs e) => await Editar();
    private async void btn_excluir_Click(object? sender, EventArgs e) => await Desativar();
    private void btn_reativar_Click(object? sender, EventArgs e) => Reativar();
    private async void btn_listar_Click(object? sender, EventArgs e) => await AbrirListagem();
    private async void btn_voltar_Click(object? sender, EventArgs e) => await Voltar();

    private async void btn_consultar_Click(object? sender, EventArgs e)
    {
        switch (_lista.IdParaConsulta(button_id))
        {
            case null:
                ExibirAviso("Informe o ID em \"ID\" ou selecione uma disciplina na lista.");
                break;
            case < 0:
                ExibirAviso("O ID deve ser um número inteiro positivo.");
                button_id.Focus();
                break;
            case int id:
                button_id.Clear();
                await ConsultarDisciplina(id);
                break;
        }
    }

    private async void dgvDisciplinas_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && _lista.IdSelecionado is int id)
            await SelecionarDisciplina(id);
    }

    private void dgvDisciplinas_SelectionChanged(object? sender, EventArgs e) => _lista.AtualizarBotoes();

    private void dgvDisciplinas_KeyDown(object? sender, KeyEventArgs e) =>
        _lista.TratarTeclaGrade(e, () => _ = SelecionarDisciplina(_lista.IdSelecionado!.Value));

    private async void Filtro_Alterado(object? sender, EventArgs e) => await ListarDisciplinas();

    private void txtBusca_TextChanged(object? sender, EventArgs e)
    {
        tmrBusca.Stop();
        tmrBusca.Start();
    }

    private async void tmrBusca_Tick(object? sender, EventArgs e)
    {
        tmrBusca.Stop();
        await ListarDisciplinas();
    }

    private async void btnLimparFiltros_Click(object? sender, EventArgs e)
    {
        _lista.IgnorarFiltros = true;
        txtBusca.Clear();
        tmrBusca.Stop();
        cboSituacaoFiltro.SelectedIndex = 0;
        _lista.IgnorarFiltros = false;
        await ListarDisciplinas();
    }
}
