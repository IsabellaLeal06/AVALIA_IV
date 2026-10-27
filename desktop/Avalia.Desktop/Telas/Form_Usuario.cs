using Avalia.Desktop.Componentes;
using Avalia.Desktop.Core.Controles;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Core.Servicos;

namespace Avalia.Desktop.Telas;

/// <summary>
/// Form_Usuario: gerenciamento de usuários (RF02, RN01, RN02, RN05, RN12).
/// Uma única tela com a lista e o cadastro (label_modo). Os métodos seguem as
/// mensagens dos diagramas de sequência Gerenciar, Listar, Incluir, Consultar,
/// Editar e Desativar Usuário. O CPF não aparece na lista (RNF02).
/// </summary>
public partial class Form_Usuario : TelaCadastro
{
    private readonly ControleUsuario _controle;
    private readonly ControleTurma _controleTurma;
    private readonly ControladorListagem<UsuarioResumo> _lista;
    private readonly ValidacaoVisual _validacao;
    private int _idUsuario;

    public Form_Usuario(ControleUsuario controle, ControleTurma controleTurma, INavegacao navegacao)
    {
        _controle = controle;
        _controleTurma = controleTurma;
        Navegacao = navegacao;
        InitializeComponent();
        ConfigurarModos(pnlLista, pnlCadastro, pnlCartao, label_modo, btn_incluir, btn_editar);

        Estilo.BotaoPrimario(btn_novo);
        Estilo.BotaoSecundario(btn_consultarPorId, btn_reativar, btn_listar, btnLimparFiltros, btn_voltar);
        Estilo.BotaoPerigo(btn_excluir);
        button_dataIngresso.MinDate = new DateTime(2000, 1, 1);
        button_dataIngresso.MaxDate = DateTime.Today.AddYears(1);

        _lista = new ControladorListagem<UsuarioResumo>(this, ExibirErro, dgvUsuarios, label_totalRegistros, btn_excluir, btn_reativar)
        {
            NomePlural = "usuários",
            Carregar = () => _controle.Listar(new FiltroUsuarios
            {
                Perfil = cboPerfilFiltro.Valor<PerfilUsuario?>(),
                IdTurma = cboTurmaFiltro.Valor<int?>(),
                Situacao = cboSituacaoFiltro.Valor<FiltroSituacao>(),
                Busca = txtBusca.Text
            }),
            Id = u => u.Id,
            Ativo = u => u.Ativo
        };
        _lista.DestacarInativos();

        _validacao = new ValidacaoVisual(errorProvider)
            .Campo(nameof(Usuario.Nome), button_nome)
            .Campo(nameof(Usuario.Cpf), button_CPF)
            .Campo(nameof(Usuario.Email), button_email)
            .Campo(nameof(Usuario.DataIngresso), button_dataIngresso)
            .Campo(nameof(Usuario.Perfil), button_perfilId)
            .Campo(nameof(Usuario.IdTurma), button_turmaId);
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        await AbrirGerenciamento();
    }

    /// <summary>abrirGerenciamento(): carrega os filtros e mostra a lista (listarUsuarios).</summary>
    public async Task AbrirGerenciamento()
    {
        _lista.IgnorarFiltros = true;
        cboPerfilFiltro.Preencher<PerfilUsuario?>(
        [
            new(null, "Todos"),
            new(PerfilUsuario.Aluno, "Aluno"),
            new(PerfilUsuario.Professor, "Professor"),
            new(PerfilUsuario.Administrador, "Administrador")
        ]);
        cboSituacaoFiltro.Preencher(OpcoesFiltro.Situacoes());
        button_perfilId.Preencher<PerfilUsuario>(
        [
            new(PerfilUsuario.Aluno, "Aluno"),
            new(PerfilUsuario.Professor, "Professor"),
            new(PerfilUsuario.Administrador, "Administrador")
        ]);
        await Executar(async () =>
        {
            var turmas = await _controleTurma.ListarTodas();
            cboTurmaFiltro.Preencher(turmas.Select(t => new ItemCombo<int?>(t.Id, t.Descricao)).Prepend(new(null, "Todas")));
        });
        _lista.IgnorarFiltros = false;
        await AbrirListagem();
    }

    /// <summary>abrirListagem(): volta para a lista e lista os usuários.</summary>
    public async Task AbrirListagem(int? selecionarId = null)
    {
        DefinirModo(ModoCadastro.Listagem);
        label_titulo.Text = "Usuários";
        await ListarUsuarios(selecionarId);
        dgvUsuarios.Focus();
    }

    /// <summary>listarUsuarios() → ControleUsuario.listar()</summary>
    public Task ListarUsuarios(int? selecionarId = null) => _lista.CarregarAsync(selecionarId);

    /// <summary>abrirInclusao(): formulário vazio para um novo usuário.</summary>
    public async Task AbrirInclusao()
    {
        _idUsuario = 0;
        _validacao.Limpar();
        if (!await CarregarTurmas(null))
            return;

        ExibirDados(new Usuario());
        DefinirModo(ModoCadastro.Inclusao);
        AtualizarCampoTurma();
        button_nome.Focus();
    }

    /// <summary>incluirUsuario(dados) → ControleUsuario.incluir(usuario)</summary>
    public async Task IncluirUsuario(Usuario dados)
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

    /// <summary>selecionarUsuario(id): o usuário escolhido na lista é consultado.</summary>
    public Task SelecionarUsuario(int id) => ConsultarUsuario(id);

    /// <summary>consultarUsuario(id) → ControleUsuario.consultarPorId(id) → exibirDados()</summary>
    public async Task ConsultarUsuario(int id)
    {
        Usuario? usuario = null;
        if (!await Executar(async () => usuario = await _controle.ConsultarPorId(id)))
            return;

        if (usuario is null)
        {
            ExibirErro($"Usuário nº {id} não encontrado.");
            return;
        }

        _validacao.Limpar();
        if (!await CarregarTurmas(usuario.IdTurma))
            return;

        _idUsuario = usuario.Id;
        ExibirDados(usuario);
        // Usuário inativo não pode ser editado: abre só para consulta
        DefinirModo(usuario.Ativo ? ModoCadastro.Edicao : ModoCadastro.Consulta);
        if (usuario.Ativo)
            AtualizarCampoTurma();
        else
        {
            label_titulo.Text = "Consultar usuário";
            ExibirAviso("Usuário inativo: os dados estão só para consulta. Para editar, reative-o na lista.");
        }
        button_nome.Focus();
    }

    /// <summary>exibirDados(): preenche o formulário com os dados do usuário.</summary>
    public void ExibirDados(Usuario usuario)
    {
        var novo = usuario.Id == 0;
        label_titulo.Text = novo ? "Novo usuário" : "Editar usuário";
        lblRegistro.Text = novo ? "Novo usuário" : $"Usuário nº {usuario.Id}";
        button_nome.Text = usuario.Nome;
        button_CPF.Text = usuario.Cpf;
        button_email.Text = usuario.Email;
        button_dataIngresso.Value = usuario.DataIngresso < button_dataIngresso.MinDate ? DateTime.Today : usuario.DataIngresso;
        button_perfilId.SelectedIndex = Math.Max(0, (int)usuario.Perfil - 1);
        button_turmaId.SelectedIndex = 0;
        foreach (ItemCombo<int?> item in button_turmaId.Items)
            if (item.Valor == usuario.IdTurma)
                button_turmaId.SelectedItem = item;
        button_ativo.Checked = usuario.Ativo;
        button_ativo.Text = usuario.Ativo ? "Ativo" : "Inativo";
        label_senha.Text = novo
            ? "A senha inicial será o CPF (somente números). O usuário poderá alterá-la depois do primeiro acesso."
            : "A senha não é alterada nesta tela. Cada usuário altera a própria senha em Meu perfil (RN04).";
    }

    /// <summary>editar() → ControleUsuario.editar(id, usuario)</summary>
    public async Task Editar()
    {
        ResultadoOperacao<int>? resultado = null;
        if (!await Executar(async () => resultado = await _controle.Editar(_idUsuario, LerDados())))
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

    /// <summary>Depois de confirmarDesativação(): ControleUsuario.desativar(id) e exibirMensagemSucesso().</summary>
    private async Task ConcluirDesativacao(int id)
    {
        ResultadoOperacao? resultado = null;
        if (!await Executar(async () => resultado = await _controle.Desativar(id)))
            return;

        await ListarUsuarios(id);
        dgvUsuarios.Focus();
        if (resultado!.Sucesso)
            ExibirMensagemSucesso(resultado.Mensagem);
        else
            ExibirAviso(resultado.MensagemErros);
    }

    /// <summary>reativar(): confirmação e ControleUsuario.reativar(id).</summary>
    public void Reativar()
    {
        if (_lista.Selecionado is not { Ativo: false } usuario)
            return;

        Navegacao!.Abrir<Form_Conf_Desativação>(f => f.SolicitarReativacao(
            $"Deseja reativar o usuário \"{usuario.Nome}\"?", () => ConcluirReativacao(usuario.Id)));
    }

    private async Task ConcluirReativacao(int id)
    {
        ResultadoOperacao? resultado = null;
        if (!await Executar(async () => resultado = await _controle.Reativar(id)))
            return;

        await ListarUsuarios(id);
        dgvUsuarios.Focus();
        if (resultado!.Sucesso)
            ExibirMensagemSucesso(resultado.Mensagem);
        else
            ExibirAviso(resultado.MensagemErros);
    }

    /// <summary>voltar(): do cadastro para a lista.</summary>
    public Task Voltar()
    {
        Navegacao!.OcultarMensagem();
        return AbrirListagem(_idUsuario == 0 ? null : _idUsuario);
    }

    /// <summary>Somente turmas ativas (e a atual do aluno, se estiver desativada) — listarAtivas().</summary>
    private Task<bool> CarregarTurmas(int? idTurmaAtual) => Executar(async () =>
    {
        var turmas = await _controleTurma.ListarAtivas(idTurmaAtual);
        button_turmaId.Preencher(turmas.Select(t => new ItemCombo<int?>(t.Id, t.Descricao))
            .Prepend(new(null, "Selecione a turma")), idTurmaAtual);
    });

    private Usuario LerDados()
    {
        var perfil = button_perfilId.Valor<PerfilUsuario>();
        return new Usuario
        {
            Id = _idUsuario,
            Nome = button_nome.Text,
            Cpf = button_CPF.Text,
            Email = button_email.Text,
            DataIngresso = button_dataIngresso.Value.Date,
            Perfil = perfil,
            IdTurma = perfil == PerfilUsuario.Aluno ? button_turmaId.Valor<int?>() : null
        };
    }

    /// <summary>RN05: Professor e Administrador não possuem turma; o campo fica desabilitado e vazio.</summary>
    private void AtualizarCampoTurma()
    {
        if (Modo is not (ModoCadastro.Inclusao or ModoCadastro.Edicao))
            return;
        var aluno = button_perfilId.Valor<PerfilUsuario>() == PerfilUsuario.Aluno;
        button_turmaId.Enabled = aluno;
        if (!aluno && button_turmaId.Items.Count > 0)
            button_turmaId.SelectedIndex = 0;
        errorProvider.SetError(button_turmaId, string.Empty);
    }

    // ---------- eventos da tela ----------
    protected override void TeclaListar() => _ = ListarUsuarios();
    protected override void TeclaNovo() => _ = AbrirInclusao();
    protected override void TeclaVoltar() => _ = Voltar();

    private async void btn_novo_Click(object? sender, EventArgs e) => await AbrirInclusao();
    private async void btn_incluir_Click(object? sender, EventArgs e) => await IncluirUsuario(LerDados());
    private async void btn_editar_Click(object? sender, EventArgs e) => await Editar();
    private async void btn_excluir_Click(object? sender, EventArgs e) => await Desativar();
    private void btn_reativar_Click(object? sender, EventArgs e) => Reativar();
    private async void btn_listar_Click(object? sender, EventArgs e) => await AbrirListagem();
    private async void btn_voltar_Click(object? sender, EventArgs e) => await Voltar();
    private void button_perfilId_SelectedIndexChanged(object? sender, EventArgs e) => AtualizarCampoTurma();

    private async void btn_consultarPorId_Click(object? sender, EventArgs e)
    {
        switch (_lista.IdParaConsulta(button_id))
        {
            case null:
                ExibirAviso("Informe o ID em \"ID\" ou selecione um usuário na lista.");
                break;
            case < 0:
                ExibirAviso("O ID deve ser um número inteiro positivo.");
                button_id.Focus();
                break;
            case int id:
                button_id.Clear();
                await ConsultarUsuario(id);
                break;
        }
    }

    private async void dgvUsuarios_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && _lista.IdSelecionado is int id)
            await SelecionarUsuario(id);
    }

    private void dgvUsuarios_SelectionChanged(object? sender, EventArgs e) => _lista.AtualizarBotoes();

    private void dgvUsuarios_KeyDown(object? sender, KeyEventArgs e) =>
        _lista.TratarTeclaGrade(e, () => _ = SelecionarUsuario(_lista.IdSelecionado!.Value));

    private async void Filtro_Alterado(object? sender, EventArgs e) => await ListarUsuarios();

    private void txtBusca_TextChanged(object? sender, EventArgs e)
    {
        tmrBusca.Stop();
        tmrBusca.Start();
    }

    private async void tmrBusca_Tick(object? sender, EventArgs e)
    {
        tmrBusca.Stop();
        await ListarUsuarios();
    }

    private async void btnLimparFiltros_Click(object? sender, EventArgs e)
    {
        _lista.IgnorarFiltros = true;
        txtBusca.Clear();
        tmrBusca.Stop();
        cboPerfilFiltro.SelectedIndex = 0;
        cboTurmaFiltro.SelectedIndex = 0;
        cboSituacaoFiltro.SelectedIndex = 0;
        _lista.IgnorarFiltros = false;
        await ListarUsuarios();
    }
}
