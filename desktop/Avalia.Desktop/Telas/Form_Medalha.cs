using Avalia.Desktop.Componentes;
using Avalia.Desktop.Core.Controles;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Core.Servicos;
using Avalia.Desktop.Core.Validacoes;

namespace Avalia.Desktop.Telas;

/// <summary>
/// Form_Medalha: o Administrador cria e configura as faixas de medalha
/// (nome, pontuação mínima e imagem) sem alterar o código das aplicações (RN22).
/// A Web e o Mobile mostram ao aluno a pontuação e a medalha da faixa em que ele está (RN23).
/// Uma única tela com a lista e o cadastro (label_modo), como as demais.
/// </summary>
public partial class Form_Medalha : TelaCadastro
{
    private readonly ControleMedalha _controle;
    private readonly ControladorListagem<Medalha> _lista;
    private readonly ValidacaoVisual _validacao;
    private int _idMedalha;
    private string? _arquivoNovaImagem;

    public Form_Medalha(ControleMedalha controle, INavegacao navegacao)
    {
        _controle = controle;
        Navegacao = navegacao;
        InitializeComponent();
        ConfigurarModos(pnlLista, pnlCadastro, pnlCartao, label_modo, btn_incluir, btn_editar);

        Estilo.BotaoPrimario(btn_novo);
        Estilo.BotaoSecundario(btn_consultarPorId, btn_listar, btn_voltar, btn_selecionarImagem);
        Estilo.BotaoPerigo(btn_excluir);
        colPontos.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        colPontos.DefaultCellStyle.Format = "N0";
        colAlunos.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        button_pontosMinimos.Maximum = ControleMedalha.PontosMaximos;

        // Sem janela de seleção de arquivo: a imagem é arrastada para o quadro ou o caminho é colado
        ImagemPrevia.AceitarArrastar(picImagem, UsarImagem);
        ImagemPrevia.AceitarArrastar(button_imagemURL, UsarImagem);

        _lista = new ControladorListagem<Medalha>(this, ExibirErro, dgvMedalhas, label_totalRegistros, btn_excluir, null)
        {
            NomePlural = "medalhas",
            Carregar = () => _controle.Listar(),
            Id = m => m.Id
        };

        _validacao = new ValidacaoVisual(errorProvider)
            .Campo(nameof(Medalha.Nome), button_nome)
            .Campo(nameof(Medalha.PontosMinimos), button_pontosMinimos)
            .Campo(ControleMedalha.CampoImagem, button_imagemURL);
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        await AbrirGerenciamento();
    }

    /// <summary>abrirGerenciamento(): mostra a lista (listarMedalhas) e os alunos sem medalha.</summary>
    public Task AbrirGerenciamento() => AbrirListagem();

    /// <summary>abrirListagem(): volta para a lista.</summary>
    public async Task AbrirListagem(int? selecionarId = null)
    {
        DefinirModo(ModoCadastro.Listagem);
        label_titulo.Text = "Medalhas";
        await ListarMedalhas(selecionarId);
        dgvMedalhas.Focus();
    }

    /// <summary>listarMedalhas() → ControleMedalha.listar() e qtdAlunosSemMedalha()</summary>
    public async Task ListarMedalhas(int? selecionarId = null)
    {
        await _lista.CarregarAsync(selecionarId);

        int semMedalha = 0;
        if (!await Executar(async () => semMedalha = await _controle.QtdAlunosSemMedalha(), pnlResumo))
            return;

        var primeira = (dgvMedalhas.DataSource as List<Medalha>)?.FirstOrDefault();
        label_qtdAlunos.Text = primeira is null
            ? "Nenhuma faixa cadastrada: os alunos não recebem medalha. Cadastre ao menos uma (ex.: Bronze, a partir de 250 pontos)."
            : $"{semMedalha} aluno(s) ativo(s) ainda sem medalha (abaixo de {primeira.PontosMinimos:N0} pontos, " +
              $"a faixa {primeira.Nome}). Eles veem a medalha {primeira.Nome} em versão acinzentada (RN23).";
    }

    /// <summary>abrirInclusao(): formulário vazio para uma nova faixa.</summary>
    public void AbrirInclusao()
    {
        _idMedalha = 0;
        _validacao.Limpar();
        ExibirDados(new Medalha());
        DefinirModo(ModoCadastro.Inclusao);
        button_nome.Focus();
    }

    /// <summary>incluirMedalha(dados) → ControleMedalha.incluir(medalha)</summary>
    public async Task IncluirMedalha(Medalha dados)
    {
        ResultadoOperacao<int>? resultado = null;
        if (!await Executar(async () => resultado = await _controle.Incluir(dados, _arquivoNovaImagem)))
            return;

        if (!resultado!.Sucesso)
        {
            _validacao.Mostrar(resultado, ExibirAviso);
            return;
        }

        await AbrirListagem(resultado.Valor);
        ExibirMensagemSucesso(resultado.Mensagem);
    }

    /// <summary>selecionarMedalha(id): a faixa escolhida na lista é consultada.</summary>
    public Task SelecionarMedalha(int id) => ConsultarMedalha(id);

    /// <summary>consultarMedalha(id) → ControleMedalha.consultarPorId(id) → exibirDados()</summary>
    public async Task ConsultarMedalha(int id)
    {
        Medalha? medalha = null;
        if (!await Executar(async () => medalha = await _controle.ConsultarPorId(id)))
            return;

        if (medalha is null)
        {
            ExibirErro($"Medalha nº {id} não encontrada.");
            return;
        }

        _validacao.Limpar();
        _idMedalha = medalha.Id;
        ExibirDados(medalha);
        DefinirModo(ModoCadastro.Edicao);
        button_nome.Focus();
    }

    /// <summary>exibirDados(): preenche o formulário com os dados da faixa.</summary>
    public void ExibirDados(Medalha medalha)
    {
        var nova = medalha.Id == 0;
        label_titulo.Text = nova ? "Nova medalha" : "Editar medalha";
        lblRegistro.Text = nova ? "Nova medalha" : $"Medalha nº {medalha.Id}";
        button_nome.Text = medalha.Nome;
        button_pontosMinimos.Value = Math.Clamp(medalha.PontosMinimos, 0, ControleMedalha.PontosMaximos);
        button_imagemURL.Text = medalha.ImagemUrl;
        _arquivoNovaImagem = null;

        var listada = (dgvMedalhas.DataSource as List<Medalha>)?.FirstOrDefault(m => m.Id == medalha.Id);
        lblFaixa.Text = listada is null
            ? "A faixa vai da pontuação mínima informada até a pontuação mínima da próxima medalha."
            : $"Faixa atual: {listada.Faixa}. Alunos ativos nesta faixa: {listada.QtdAlunos}.";

        var caminhoLocal = _controle.ObterCaminhoLocalImagem(medalha.ImagemUrl);
        ImagemPrevia.Exibir(picImagem, caminhoLocal);
        picImagem.AccessibleDescription = caminhoLocal is null && medalha.ImagemUrl.Length > 0
            ? "Imagem da versão Web: a prévia não está disponível neste computador."
            : null;
    }

    /// <summary>editar() → ControleMedalha.editar(id, medalha)</summary>
    public async Task Editar()
    {
        ResultadoOperacao<int>? resultado = null;
        if (!await Executar(async () => resultado = await _controle.Editar(_idMedalha, LerDados(), _arquivoNovaImagem)))
            return;

        if (!resultado!.Sucesso)
        {
            _validacao.Mostrar(resultado, ExibirAviso);
            return;
        }

        await AbrirListagem(resultado.Valor);
        ExibirMensagemSucesso(resultado.Mensagem);
    }

    /// <summary>excluir(): confirmação na Form_Conf_Desativação e ControleMedalha.excluir(id).</summary>
    public async Task Excluir()
    {
        if (_lista.IdSelecionado is not int id)
            return;

        string? mensagem = null;
        if (!await Executar(async () => mensagem = await _controle.MensagemExclusao(id)))
            return;

        Navegacao!.Abrir<Form_Conf_Desativação>(f => f.SolicitarExclusao(mensagem!, () => ConcluirExclusao(id)));
    }

    private async Task ConcluirExclusao(int id)
    {
        ResultadoOperacao? resultado = null;
        if (!await Executar(async () => resultado = await _controle.Excluir(id)))
            return;

        await ListarMedalhas();
        dgvMedalhas.Focus();
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
        return AbrirListagem(_idMedalha == 0 ? null : _idMedalha);
    }

    private Medalha LerDados() => new()
    {
        Id = _idMedalha,
        Nome = button_nome.Text,
        PontosMinimos = (int)button_pontosMinimos.Value,
        ImagemUrl = button_imagemURL.Text
    };

    /// <summary>
    /// Arquivo do computador: confere e mostra a prévia (copiado só ao salvar).
    /// Outro texto (ex.: assets/medalhas/bronze.png) vale como caminho da versão Web.
    /// </summary>
    private void UsarImagem(string caminho)
    {
        if (Modo is not (ModoCadastro.Inclusao or ModoCadastro.Edicao))
            return;

        caminho = ValidadorImagem.LimparCaminho(caminho);
        button_imagemURL.Text = caminho;
        errorProvider.SetError(button_imagemURL, string.Empty);

        if (!ValidadorImagem.EhArquivoLocal(caminho))
        {
            _arquivoNovaImagem = null;
            ImagemPrevia.Exibir(picImagem, _controle.ObterCaminhoLocalImagem(caminho));
            ExibirAviso("Esse caminho não é um arquivo deste computador; ele será gravado como caminho da versão Web.");
            return;
        }

        if (ValidadorImagem.Validar(caminho) is { } erro)
        {
            errorProvider.SetError(button_imagemURL, erro);
            ExibirAviso(erro);
            return;
        }

        Navegacao!.OcultarMensagem();
        _arquivoNovaImagem = caminho;
        ImagemPrevia.Exibir(picImagem, caminho);
    }

    // ---------- eventos da tela ----------
    protected override void TeclaListar() => _ = ListarMedalhas();
    protected override void TeclaNovo() => AbrirInclusao();
    protected override void TeclaVoltar() => _ = Voltar();

    private void btn_novo_Click(object? sender, EventArgs e) => AbrirInclusao();

    private async void btn_incluir_Click(object? sender, EventArgs e)
    {
        PrepararImagemDigitada();
        await IncluirMedalha(LerDados());
    }

    private async void btn_editar_Click(object? sender, EventArgs e)
    {
        PrepararImagemDigitada();
        await Editar();
    }

    /// <summary>Se o usuário colou o caminho de um arquivo e não clicou em "Usar imagem", usa o arquivo.</summary>
    private void PrepararImagemDigitada()
    {
        if (_arquivoNovaImagem is null && ValidadorImagem.EhArquivoLocal(button_imagemURL.Text))
            _arquivoNovaImagem = ValidadorImagem.LimparCaminho(button_imagemURL.Text);
        else if (_arquivoNovaImagem is not null && ValidadorImagem.LimparCaminho(button_imagemURL.Text) != _arquivoNovaImagem)
            _arquivoNovaImagem = null; // o texto foi trocado depois de escolher o arquivo
    }

    private async void btn_excluir_Click(object? sender, EventArgs e) => await Excluir();
    private async void btn_listar_Click(object? sender, EventArgs e) => await AbrirListagem();
    private async void btn_voltar_Click(object? sender, EventArgs e) => await Voltar();
    private void btn_selecionarImagem_Click(object? sender, EventArgs e) => UsarImagem(button_imagemURL.Text);

    private async void btn_consultarPorId_Click(object? sender, EventArgs e)
    {
        switch (_lista.IdParaConsulta(button_id))
        {
            case null:
                ExibirAviso("Informe o ID em \"ID\" ou selecione uma medalha na lista.");
                break;
            case < 0:
                ExibirAviso("O ID deve ser um número inteiro positivo.");
                button_id.Focus();
                break;
            case int id:
                button_id.Clear();
                await ConsultarMedalha(id);
                break;
        }
    }

    private async void dgvMedalhas_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && _lista.IdSelecionado is int id)
            await SelecionarMedalha(id);
    }

    private void dgvMedalhas_SelectionChanged(object? sender, EventArgs e) => _lista.AtualizarBotoes();

    private void dgvMedalhas_KeyDown(object? sender, KeyEventArgs e) =>
        _lista.TratarTeclaGrade(e, () => _ = SelecionarMedalha(_lista.IdSelecionado!.Value));

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        picImagem.Image?.Dispose();
        base.OnFormClosed(e);
    }
}
