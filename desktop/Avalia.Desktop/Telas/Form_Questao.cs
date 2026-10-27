using Avalia.Desktop.Componentes;
using Avalia.Desktop.Core.Controles;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Core.Servicos;
using Avalia.Desktop.Core.Validacoes;

namespace Avalia.Desktop.Telas;

/// <summary>
/// Form_Questao: banco de questões de todas as disciplinas (RF03, RN08, RN09, RN10, RN11).
/// Uma única tela com a lista e o cadastro (label_modo). As cinco alternativas
/// (A a E) ficam na própria tela e a correta é escolhida em button_alternativaCorreta.
/// Os métodos seguem os diagramas de sequência Gerenciar, Listar, Incluir,
/// Consultar, Editar e Desativar Questão.
/// </summary>
public partial class Form_Questao : TelaCadastro
{
    private const string TextoOrientacao =
        "Toda questão tem exatamente cinco alternativas (A a E) e apenas uma correta (RN09). " +
        "Escreva as cinco e escolha a correta abaixo delas.";

    private readonly ControleQuestao _controle;
    private readonly ControleDisciplina _controleDisciplina;
    private readonly SessaoUsuario _sessao;
    private readonly ControladorListagem<QuestaoResumo> _lista;
    private readonly ValidacaoVisual _validacao;
    private int _idQuestao;
    private string? _imagemUrl;
    private string? _arquivoNovaImagem;

    public Form_Questao(ControleQuestao controle, ControleDisciplina controleDisciplina, SessaoUsuario sessao, INavegacao navegacao)
    {
        _controle = controle;
        _controleDisciplina = controleDisciplina;
        _sessao = sessao;
        Navegacao = navegacao;
        InitializeComponent();
        ConfigurarModos(pnlLista, pnlCadastro, pnlCartao, label_modo, btn_incluir, btn_editar);

        Estilo.BotaoPrimario(btn_novo);
        Estilo.BotaoSecundario(btn_consultarPorId, btn_reativar, btn_listar, btnLimparFiltros, btn_voltar,
            btn_selecionarImagem, btnRemoverImagem);
        Estilo.BotaoPerigo(btn_excluir);
        colId.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        colUtilizada.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        // Sem janela de seleção de arquivo: a imagem é arrastada para o quadro ou o caminho é colado
        ImagemPrevia.AceitarArrastar(picImagem, UsarImagem);
        ImagemPrevia.AceitarArrastar(button_imagemURL, UsarImagem);

        _lista = new ControladorListagem<QuestaoResumo>(this, ExibirErro, dgvQuestoes, label_totalRegistros, btn_excluir, btn_reativar)
        {
            NomePlural = "questões",
            Carregar = () => _controle.Listar(new FiltroQuestoes
            {
                IdDisciplina = cboDisciplinaFiltro.Valor<int?>(),
                Situacao = cboSituacaoFiltro.Valor<FiltroSituacao>(),
                Busca = txtBusca.Text
            }),
            Id = q => q.Id,
            Ativo = q => q.Ativo
        };
        _lista.DestacarInativos();

        _validacao = new ValidacaoVisual(errorProvider)
            .Campo(nameof(Questao.Enunciado), button_enunciado)
            .Campo(nameof(Questao.IdDisciplina), button_disciplina)
            .Campo(ControleQuestao.CampoAlternativas, grpAlternativas)
            .Campo(ControleQuestao.CampoCorreta, button_alternativaCorreta)
            .Campo(ControleQuestao.CampoImagem, picImagem);
        for (var i = 0; i < Questao.Letras.Length; i++)
            _validacao.Campo(ControleQuestao.CampoAlternativa(Questao.Letras[i]), Alternativas[i]);
    }

    private TextBox[] Alternativas => [button_alternativaA, button_alternativaB, button_alternativaC, button_alternativaD, button_alternativaE];

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        await AbrirGerenciamento();
    }

    /// <summary>abrirGerenciamento(): carrega os filtros e mostra a lista (listarQuestoes).</summary>
    public async Task AbrirGerenciamento()
    {
        _lista.IgnorarFiltros = true;
        cboSituacaoFiltro.Preencher(OpcoesFiltro.Situacoes(feminino: true));
        button_alternativaCorreta.Preencher<string?>(
            Questao.Letras.Select(l => new ItemCombo<string?>(l, $"Alternativa {l}")).Prepend(new(null, "Selecione")));
        await Executar(async () =>
        {
            var disciplinas = await _controleDisciplina.ListarTodas();
            cboDisciplinaFiltro.Preencher(disciplinas.Select(d => new ItemCombo<int?>(d.Id, d.Descricao)).Prepend(new(null, "Todas")));
        });
        _lista.IgnorarFiltros = false;
        await AbrirListagem();
    }

    /// <summary>abrirListagem(): volta para a lista e lista as questões.</summary>
    public async Task AbrirListagem(int? selecionarId = null)
    {
        DefinirModo(ModoCadastro.Listagem);
        label_titulo.Text = "Questões";
        await ListarQuestoes(selecionarId);
        dgvQuestoes.Focus();
    }

    /// <summary>listarQuestoes() → ControleQuestao.listar()</summary>
    public Task ListarQuestoes(int? selecionarId = null) => _lista.CarregarAsync(selecionarId);

    /// <summary>abrirInclusao(): formulário vazio para uma nova questão.</summary>
    public async Task AbrirInclusao()
    {
        _idQuestao = 0;
        _validacao.Limpar();
        if (!await CarregarDisciplinas(null))
            return;

        ExibirDados(new Questao { Alternativas = Questao.Letras.Select(l => new Alternativa { Letra = l }).ToList() }, false);
        DefinirModo(ModoCadastro.Inclusao);
        button_disciplina.Focus();
    }

    /// <summary>incluirQuestao(dados) → ControleQuestao.incluir(questao, alternativas)</summary>
    public async Task IncluirQuestao(Questao dados)
    {
        ResultadoOperacao<int>? resultado = null;
        if (!await Executar(async () => resultado = await _controle.Incluir(dados, dados.Alternativas, _arquivoNovaImagem)))
            return;

        if (!resultado!.Sucesso)
        {
            _validacao.Mostrar(resultado, ExibirAviso);
            return;
        }

        await AbrirListagem(resultado.Valor);
        ExibirMensagemSucesso(resultado.Mensagem);
    }

    /// <summary>selecionarQuestao(id): a questão escolhida na lista é consultada.</summary>
    public Task SelecionarQuestao(int id) => ConsultarQuestao(id);

    /// <summary>consultarQuestao(id) → ControleQuestao.consultar(id) → exibirDados()</summary>
    public async Task ConsultarQuestao(int id)
    {
        Questao? questao = null;
        var utilizada = false;
        if (!await Executar(async () =>
            {
                questao = await _controle.Consultar(id);
                if (questao is not null)
                    utilizada = await _controle.FoiUtilizada(id);
            }))
            return;

        if (questao is null)
        {
            ExibirErro($"Questão nº {id} não encontrada.");
            return;
        }

        _validacao.Limpar();
        if (!await CarregarDisciplinas(questao.IdDisciplina))
            return;

        _idQuestao = questao.Id;
        ExibirDados(questao, utilizada);
        // Questão desativada não pode ser editada: abre só para consulta
        DefinirModo(questao.Ativo ? ModoCadastro.Edicao : ModoCadastro.Consulta);
        if (questao.Ativo)
            button_alternativaCorreta.Enabled = !utilizada; // RN11
        else
        {
            label_titulo.Text = "Consultar questão";
            ExibirAviso("Questão desativada: os dados estão só para consulta. Para editar, reative-a na lista.");
        }
        button_disciplina.Focus();
    }

    /// <summary>exibirDados(): preenche o formulário com a questão e as alternativas.</summary>
    public void ExibirDados(Questao questao, bool utilizada)
    {
        var nova = questao.Id == 0;
        label_titulo.Text = nova ? "Nova questão" : "Editar questão";
        lblRegistro.Text = nova ? "Nova questão" : $"Questão nº {questao.Id}";
        label_autor.Text = nova
            ? $"Será registrada como cadastrada por {_sessao.Usuario?.Nome}."
            : $"Cadastrada por: {questao.NomeUsuarioCadastro}";
        button_enunciado.Text = questao.Enunciado;
        button_explicacaoResposta.Text = questao.Explicacao;
        button_ativo.Checked = questao.Ativo;
        button_ativo.Text = questao.Ativo ? "Ativa" : "Inativa";
        button_disciplina.SelectedIndex = 0;
        foreach (ItemCombo<int?> item in button_disciplina.Items)
            if (item.Valor == questao.IdDisciplina)
                button_disciplina.SelectedItem = item;

        for (var i = 0; i < Questao.Letras.Length; i++)
            Alternativas[i].Text = questao.Alternativas.FirstOrDefault(a => a.Letra == Questao.Letras[i])?.Texto;
        button_alternativaCorreta.SelectedIndex = questao.LetraCorreta is { } letra ? Array.IndexOf(Questao.Letras, letra) + 1 : 0;

        // label_avisoGabarito: orientação (RN09) ou, se já usada em simulado, o aviso da RN11
        pnlAvisoGabarito.BackColor = utilizada ? Estilo.AvisoFundo : Color.FromArgb(239, 246, 255);
        label_avisoGabarito.ForeColor = utilizada ? Estilo.AvisoTexto : Estilo.Menu;
        label_avisoGabarito.Text = utilizada ? ControleQuestao.MensagemRN11 : TextoOrientacao;
        pnlAvisoGabarito.AccessibleRole = utilizada ? AccessibleRole.Alert : AccessibleRole.StaticText;
        button_alternativaCorreta.AccessibleDescription = utilizada ? "Bloqueado: questão já utilizada em simulado." : null;

        _arquivoNovaImagem = null;
        _imagemUrl = questao.ImagemUrl;
        button_imagemURL.Text = questao.ImagemUrl;
        var caminhoLocal = _controle.ObterCaminhoLocalImagem(_imagemUrl);
        ImagemPrevia.Exibir(picImagem, caminhoLocal);
        label_imagemURL.Text = _imagemUrl is not null && caminhoLocal is null
            ? "Imagem (arquivo não encontrado neste computador)"
            : "Imagem (opcional): arraste o arquivo";
    }

    /// <summary>editar() → ControleQuestao.editar(id, questao, alternativas)</summary>
    public async Task Editar()
    {
        var dados = LerDados();
        ResultadoOperacao<int>? resultado = null;
        if (!await Executar(async () => resultado = await _controle.Editar(_idQuestao, dados, dados.Alternativas, _arquivoNovaImagem)))
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
    public void Desativar()
    {
        if (_lista.IdSelecionado is not int id)
            return;

        Navegacao!.Abrir<Form_Conf_Desativação>(f =>
            f.SolicitarDesativacao(ControleQuestao.MensagemDesativacao(id), () => ConcluirDesativacao(id)));
    }

    /// <summary>Depois de confirmarDesativação(): ControleQuestao.desativar(id) e exibirMensagemSucesso().</summary>
    private async Task ConcluirDesativacao(int id)
    {
        ResultadoOperacao? resultado = null;
        if (!await Executar(async () => resultado = await _controle.Desativar(id)))
            return;

        await ListarQuestoes(id);
        dgvQuestoes.Focus();
        if (resultado!.Sucesso)
            ExibirMensagemSucesso(resultado.Mensagem);
        else
            ExibirAviso(resultado.MensagemErros);
    }

    public void Reativar()
    {
        if (_lista.Selecionado is not { Ativo: false } questao)
            return;

        Navegacao!.Abrir<Form_Conf_Desativação>(f => f.SolicitarReativacao(
            $"Deseja reativar a questão nº {questao.Id}?", () => ConcluirReativacao(questao.Id)));
    }

    private async Task ConcluirReativacao(int id)
    {
        ResultadoOperacao? resultado = null;
        if (!await Executar(async () => resultado = await _controle.Reativar(id)))
            return;

        await ListarQuestoes(id);
        dgvQuestoes.Focus();
        if (resultado!.Sucesso)
            ExibirMensagemSucesso(resultado.Mensagem);
        else
            ExibirAviso(resultado.MensagemErros);
    }

    /// <summary>voltar(): do cadastro para a lista.</summary>
    public Task Voltar()
    {
        Navegacao!.OcultarMensagem();
        ImagemPrevia.Exibir(picImagem, null);
        return AbrirListagem(_idQuestao == 0 ? null : _idQuestao);
    }

    /// <summary>Somente disciplinas ativas (e a atual da questão, se estiver desativada) — listarAtivas().</summary>
    private Task<bool> CarregarDisciplinas(int? idDisciplinaAtual) => Executar(async () =>
    {
        var disciplinas = await _controleDisciplina.ListarAtivas(idDisciplinaAtual);
        button_disciplina.Preencher(
            disciplinas.Select(d => new ItemCombo<int?>(d.Id, d.Descricao)).Prepend(new(null, "Selecione a disciplina")),
            idDisciplinaAtual);
    });

    private Questao LerDados()
    {
        var correta = button_alternativaCorreta.Valor<string?>();
        return new Questao
        {
            Id = _idQuestao,
            IdDisciplina = button_disciplina.Valor<int?>() ?? 0,
            Enunciado = button_enunciado.Text,
            Explicacao = button_explicacaoResposta.Text,
            ImagemUrl = _imagemUrl,
            Alternativas = Questao.Letras
                .Select((letra, i) => new Alternativa { Letra = letra, Texto = Alternativas[i].Text, Correta = letra == correta })
                .ToList()
        };
    }

    /// <summary>Confere o arquivo e mostra a prévia. Ele só é copiado para a pasta de imagens ao salvar.</summary>
    private void UsarImagem(string caminho)
    {
        if (Modo is not (ModoCadastro.Inclusao or ModoCadastro.Edicao))
            return;

        caminho = ValidadorImagem.LimparCaminho(caminho);
        button_imagemURL.Text = caminho;

        if (ValidadorImagem.Validar(caminho) is { } erro)
        {
            errorProvider.SetError(picImagem, erro);
            ExibirAviso(erro);
            return;
        }

        errorProvider.SetError(picImagem, string.Empty);
        Navegacao!.OcultarMensagem();
        _arquivoNovaImagem = caminho;
        label_imagemURL.Text = ImagemPrevia.Exibir(picImagem, caminho)
            ? "Imagem (nova, será salva)"
            : "Imagem (não foi possível abrir)";
    }

    // ---------- eventos da tela ----------
    protected override void TeclaListar() => _ = ListarQuestoes();
    protected override void TeclaNovo() => _ = AbrirInclusao();
    protected override void TeclaVoltar() => _ = Voltar();

    private async void btn_novo_Click(object? sender, EventArgs e) => await AbrirInclusao();
    private async void btn_incluir_Click(object? sender, EventArgs e) => await IncluirQuestao(LerDados());
    private async void btn_editar_Click(object? sender, EventArgs e) => await Editar();
    private void btn_excluir_Click(object? sender, EventArgs e) => Desativar();
    private void btn_reativar_Click(object? sender, EventArgs e) => Reativar();
    private async void btn_listar_Click(object? sender, EventArgs e) => await AbrirListagem();
    private async void btn_voltar_Click(object? sender, EventArgs e) => await Voltar();
    private void btn_selecionarImagem_Click(object? sender, EventArgs e) => UsarImagem(button_imagemURL.Text);

    private void btnRemoverImagem_Click(object? sender, EventArgs e)
    {
        _arquivoNovaImagem = null;
        _imagemUrl = null;
        button_imagemURL.Clear();
        ImagemPrevia.Exibir(picImagem, null);
        label_imagemURL.Text = "Imagem (opcional): arraste o arquivo";
    }

    private async void btn_consultarPorId_Click(object? sender, EventArgs e)
    {
        switch (_lista.IdParaConsulta(button_id))
        {
            case null:
                ExibirAviso("Informe o número em \"ID\" ou selecione uma questão na lista.");
                break;
            case < 0:
                ExibirAviso("O ID deve ser um número inteiro positivo.");
                button_id.Focus();
                break;
            case int id:
                button_id.Clear();
                await ConsultarQuestao(id);
                break;
        }
    }

    private async void dgvQuestoes_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && _lista.IdSelecionado is int id)
            await SelecionarQuestao(id);
    }

    private void dgvQuestoes_SelectionChanged(object? sender, EventArgs e) => _lista.AtualizarBotoes();

    private void dgvQuestoes_KeyDown(object? sender, KeyEventArgs e) =>
        _lista.TratarTeclaGrade(e, () => _ = SelecionarQuestao(_lista.IdSelecionado!.Value));

    private async void Filtro_Alterado(object? sender, EventArgs e) => await ListarQuestoes();

    private void txtBusca_TextChanged(object? sender, EventArgs e)
    {
        tmrBusca.Stop();
        tmrBusca.Start();
    }

    private async void tmrBusca_Tick(object? sender, EventArgs e)
    {
        tmrBusca.Stop();
        await ListarQuestoes();
    }

    private async void btnLimparFiltros_Click(object? sender, EventArgs e)
    {
        _lista.IgnorarFiltros = true;
        txtBusca.Clear();
        tmrBusca.Stop();
        cboDisciplinaFiltro.SelectedIndex = 0;
        cboSituacaoFiltro.SelectedIndex = 0;
        _lista.IgnorarFiltros = false;
        await ListarQuestoes();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        picImagem.Image?.Dispose();
        base.OnFormClosed(e);
    }
}
