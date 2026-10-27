using Avalia.Desktop.Componentes;
using Avalia.Desktop.Core.Controles;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Core.Servicos;

namespace Avalia.Desktop.Telas;

/// <summary>
/// Form_Turma: gerenciamento das turmas e da grade de disciplinas (RF02, RN06, RN12).
/// Uma única tela com a lista e o cadastro (label_modo). Os métodos seguem as
/// mensagens dos diagramas de sequência Gerenciar, Listar, Incluir, Consultar,
/// Editar e Desativar Turma.
/// </summary>
public partial class Form_Turma : TelaCadastro
{
    private readonly ControleTurma _controle;
    private readonly ControladorListagem<Turma> _lista;
    private readonly ValidacaoVisual _validacao;
    private int _idTurma;

    public Form_Turma(ControleTurma controle, INavegacao navegacao)
    {
        _controle = controle;
        Navegacao = navegacao;
        InitializeComponent();
        ConfigurarModos(pnlLista, pnlCadastro, pnlCartao, label_modo, btn_incluir, btn_editar);

        Estilo.BotaoPrimario(btn_novo);
        Estilo.BotaoSecundario(btn_consultarPorId, btn_reativar, btn_listar, btnLimparFiltros, btn_voltar);
        Estilo.BotaoPerigo(btn_excluir);
        colAno.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        colAlunos.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        colDisciplinas.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        button_anoLetivo.Minimum = ControleTurma.AnoMinimo;
        button_anoLetivo.Maximum = ControleTurma.AnoMaximo;

        _lista = new ControladorListagem<Turma>(this, ExibirErro, dgvTurmas, label_totalRegistros, btn_excluir, btn_reativar)
        {
            NomePlural = "turmas",
            Carregar = () => _controle.Listar(new FiltroCadastro
            {
                Situacao = cboSituacaoFiltro.Valor<FiltroSituacao>(),
                Busca = txtBusca.Text
            }),
            Id = t => t.Id,
            Ativo = t => t.Ativo
        };
        _lista.DestacarInativos();

        _validacao = new ValidacaoVisual(errorProvider)
            .Campo(nameof(Turma.Nome), button_nome)
            .Campo(nameof(Turma.AnoLetivo), button_anoLetivo)
            .Campo("Disciplinas", button_disciplinas);
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        await AbrirGerenciamento();
    }

    /// <summary>abrirGerenciamento(): carrega os filtros e mostra a lista (listarTurmas).</summary>
    public async Task AbrirGerenciamento()
    {
        _lista.IgnorarFiltros = true;
        cboSituacaoFiltro.Preencher(OpcoesFiltro.Situacoes(feminino: true));
        _lista.IgnorarFiltros = false;
        await AbrirListagem();
    }

    /// <summary>abrirListagem(): volta para a lista e lista as turmas.</summary>
    public async Task AbrirListagem(int? selecionarId = null)
    {
        DefinirModo(ModoCadastro.Listagem);
        label_titulo.Text = "Turmas";
        await ListarTurmas(selecionarId);
        dgvTurmas.Focus();
    }

    /// <summary>listarTurmas() → ControleTurma.listar()</summary>
    public Task ListarTurmas(int? selecionarId = null) => _lista.CarregarAsync(selecionarId);

    /// <summary>abrirInclusao(): formulário vazio para uma nova turma.</summary>
    public async Task AbrirInclusao()
    {
        _idTurma = 0;
        _validacao.Limpar();
        VinculoDisciplinasTurma? vinculo = null;
        if (!await Executar(async () => vinculo = await _controle.ConsultarDisciplinas(null)))
            return;

        ExibirDados(new Turma { Ativo = true }, vinculo!);
        DefinirModo(ModoCadastro.Inclusao);
        button_nome.Focus();
    }

    /// <summary>incluirTurma(dados) → ControleTurma.incluir(turma, disciplinas)</summary>
    public async Task IncluirTurma(Turma dados, IReadOnlyList<int> disciplinas)
    {
        ResultadoOperacao<int>? resultado = null;
        if (!await Executar(async () => resultado = await _controle.Incluir(dados, disciplinas)))
            return;

        if (!resultado!.Sucesso)
        {
            _validacao.Mostrar(resultado, ExibirAviso);
            return;
        }

        await AbrirListagem(resultado.Valor);
        ExibirMensagemSucesso(resultado.Mensagem);
    }

    /// <summary>selecionarTurma(id): a turma escolhida na lista é consultada.</summary>
    public Task SelecionarTurma(int id) => ConsultarTurma(id);

    /// <summary>consultarTurma(id) → ControleTurma.consultarPorId(id) → exibirDados()</summary>
    public async Task ConsultarTurma(int id)
    {
        Turma? turma = null;
        VinculoDisciplinasTurma? vinculo = null;
        if (!await Executar(async () =>
            {
                turma = await _controle.ConsultarPorId(id);
                if (turma is not null)
                    vinculo = await _controle.ConsultarDisciplinas(id);
            }))
            return;

        if (turma is null)
        {
            ExibirErro($"Turma nº {id} não encontrada.");
            return;
        }

        _validacao.Limpar();
        _idTurma = turma.Id;
        ExibirDados(turma, vinculo!);
        DefinirModo(turma.Ativo ? ModoCadastro.Edicao : ModoCadastro.Consulta);
        if (!turma.Ativo)
        {
            label_titulo.Text = "Consultar turma";
            ExibirAviso("Turma inativa: os dados estão só para consulta. Para editar, reative-a na lista.");
        }
        button_nome.Focus();
    }

    /// <summary>exibirDados(): preenche o formulário com a turma e as disciplinas vinculadas.</summary>
    public void ExibirDados(Turma turma, VinculoDisciplinasTurma vinculo)
    {
        var nova = turma.Id == 0;
        label_titulo.Text = nova ? "Nova turma" : "Editar turma";
        lblRegistro.Text = nova ? "Nova turma" : $"Turma nº {turma.Id}";
        button_nome.Text = turma.Nome;
        button_anoLetivo.Value = Math.Clamp(nova ? DateTime.Today.Year : turma.AnoLetivo, ControleTurma.AnoMinimo, ControleTurma.AnoMaximo);
        button_ativo.Checked = turma.Ativo;
        button_ativo.Text = turma.Ativo ? "Ativa" : "Inativa";

        // CheckedListBox apenas com disciplinas ativas
        button_disciplinas.Items.Clear();
        foreach (var disciplina in vinculo.DisciplinasAtivas)
            button_disciplinas.Items.Add(disciplina, vinculo.IdsVinculados.Contains(disciplina.Id));

        lblDisciplinasInativas.Text = vinculo.VinculadasInativas.Count == 0
            ? string.Empty
            : "Vínculos mantidos com disciplinas desativadas: " +
              string.Join(", ", vinculo.VinculadasInativas.Select(d => d.Nome)) + ".";
    }

    /// <summary>editar() → ControleTurma.editar(id, turma, disciplinas)</summary>
    public async Task Editar()
    {
        ResultadoOperacao<int>? resultado = null;
        if (!await Executar(async () => resultado = await _controle.Editar(_idTurma, LerDados(), DisciplinasMarcadas())))
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

    /// <summary>Depois de confirmarDesativação(): ControleTurma.desativar(id) e exibirMensagemSucesso().</summary>
    private async Task ConcluirDesativacao(int id)
    {
        ResultadoOperacao? resultado = null;
        if (!await Executar(async () => resultado = await _controle.Desativar(id)))
            return;

        await ListarTurmas(id);
        dgvTurmas.Focus();
        if (resultado!.Sucesso)
            ExibirMensagemSucesso(resultado.Mensagem);
        else
            ExibirAviso(resultado.MensagemErros);
    }

    public void Reativar()
    {
        if (_lista.Selecionado is not { Ativo: false } turma)
            return;

        Navegacao!.Abrir<Form_Conf_Desativação>(f => f.SolicitarReativacao(
            $"Deseja reativar a turma {turma.NomeCompleto}?", () => ConcluirReativacao(turma.Id)));
    }

    private async Task ConcluirReativacao(int id)
    {
        ResultadoOperacao? resultado = null;
        if (!await Executar(async () => resultado = await _controle.Reativar(id)))
            return;

        await ListarTurmas(id);
        dgvTurmas.Focus();
        if (resultado!.Sucesso)
            ExibirMensagemSucesso(resultado.Mensagem);
        else
            ExibirAviso(resultado.MensagemErros);
    }

    /// <summary>voltar(): do cadastro para a lista.</summary>
    public Task Voltar()
    {
        Navegacao!.OcultarMensagem();
        return AbrirListagem(_idTurma == 0 ? null : _idTurma);
    }

    private Turma LerDados() => new() { Id = _idTurma, Nome = button_nome.Text, AnoLetivo = (int)button_anoLetivo.Value };

    private List<int> DisciplinasMarcadas() => button_disciplinas.CheckedItems.Cast<OpcaoSelecao>().Select(d => d.Id).ToList();

    // ---------- eventos da tela ----------
    protected override void TeclaListar() => _ = ListarTurmas();
    protected override void TeclaNovo() => _ = AbrirInclusao();
    protected override void TeclaVoltar() => _ = Voltar();

    private async void btn_novo_Click(object? sender, EventArgs e) => await AbrirInclusao();
    private async void btn_incluir_Click(object? sender, EventArgs e) => await IncluirTurma(LerDados(), DisciplinasMarcadas());
    private async void btn_editar_Click(object? sender, EventArgs e) => await Editar();
    private async void btn_excluir_Click(object? sender, EventArgs e) => await Desativar();
    private void btn_reativar_Click(object? sender, EventArgs e) => Reativar();
    private async void btn_listar_Click(object? sender, EventArgs e) => await AbrirListagem();
    private async void btn_voltar_Click(object? sender, EventArgs e) => await Voltar();

    private async void btn_consultarPorId_Click(object? sender, EventArgs e)
    {
        switch (_lista.IdParaConsulta(button_id))
        {
            case null:
                ExibirAviso("Informe o ID em \"ID\" ou selecione uma turma na lista.");
                break;
            case < 0:
                ExibirAviso("O ID deve ser um número inteiro positivo.");
                button_id.Focus();
                break;
            case int id:
                button_id.Clear();
                await ConsultarTurma(id);
                break;
        }
    }

    private async void dgvTurmas_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && _lista.IdSelecionado is int id)
            await SelecionarTurma(id);
    }

    private void dgvTurmas_SelectionChanged(object? sender, EventArgs e) => _lista.AtualizarBotoes();

    private void dgvTurmas_KeyDown(object? sender, KeyEventArgs e) =>
        _lista.TratarTeclaGrade(e, () => _ = SelecionarTurma(_lista.IdSelecionado!.Value));

    private async void Filtro_Alterado(object? sender, EventArgs e) => await ListarTurmas();

    private void txtBusca_TextChanged(object? sender, EventArgs e)
    {
        tmrBusca.Stop();
        tmrBusca.Start();
    }

    private async void tmrBusca_Tick(object? sender, EventArgs e)
    {
        tmrBusca.Stop();
        await ListarTurmas();
    }

    private async void btnLimparFiltros_Click(object? sender, EventArgs e)
    {
        _lista.IgnorarFiltros = true;
        txtBusca.Clear();
        tmrBusca.Stop();
        cboSituacaoFiltro.SelectedIndex = 0;
        _lista.IgnorarFiltros = false;
        await ListarTurmas();
    }
}
