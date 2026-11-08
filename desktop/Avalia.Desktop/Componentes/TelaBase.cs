namespace Avalia.Desktop.Componentes;

/// <summary>
/// Base das telas exibidas dentro do Form_Administrador. Implementa as mensagens
/// dos diagramas de sequência (exibirMensagemSucesso, exibirAviso e exibirErro),
/// mostradas na barra de mensagens da própria janela, sem pop-ups.
/// </summary>
public class TelaBase : Form
{
    /// <summary>Definida no construtor de cada tela (a classe base precisa de construtor vazio para o Designer).</summary>
    protected INavegacao? Navegacao { get; set; }

    protected void ExibirMensagemSucesso(string mensagem) => Navegacao?.ExibirMensagem(TipoMensagem.Sucesso, mensagem);

    protected void ExibirAviso(string mensagem) => Navegacao?.ExibirMensagem(TipoMensagem.Aviso, mensagem);

    protected void ExibirErro(string mensagem) => Navegacao?.ExibirMensagem(TipoMensagem.Erro, mensagem);

    /// <summary>Executa uma chamada ao controle com cursor de espera; o erro aparece em exibirErro.</summary>
    protected Task<bool> Executar(Func<Task> acao, Control? bloquear = null) =>
        Operacao.ExecutarAsync(bloquear ?? this, ExibirErro, acao);
}

/// <summary>Modos da tela de cadastro, exibidos em label_modo.</summary>
public enum ModoCadastro
{
    Listagem,
    Inclusao,
    Edicao,
    Consulta
}

/// <summary>
/// Base das telas de cadastro (Form_Usuario, Form_Turma, Form_Disciplina,
/// Form_Questao e Form_Medalha): uma única tela com a lista e o cadastro,
/// alternados conforme o modo (label_modo), como no diagrama de classes.
/// </summary>
public class TelaCadastro : TelaBase
{
    private Panel? _lista;
    private Panel? _cadastro;
    private Panel? _cartao;
    private Label? _modo;
    private Button? _incluir;
    private Button? _editar;

    public ModoCadastro Modo { get; private set; } = ModoCadastro.Listagem;

    protected void ConfigurarModos(Panel lista, Panel cadastro, Panel cartao, Label modo, Button incluir, Button editar)
    {
        (_lista, _cadastro, _cartao, _modo, _incluir, _editar) = (lista, cadastro, cartao, modo, incluir, editar);
        KeyPreview = true;
        Estilo.BotaoPrimario(incluir, editar);
    }

    /// <summary>Mostra a lista ou o cadastro e libera os campos só na inclusão e na edição.</summary>
    protected void DefinirModo(ModoCadastro modo)
    {
        Modo = modo;
        if (_lista is null || _cadastro is null || _cartao is null || _modo is null || _incluir is null || _editar is null)
            return;

        _lista.Visible = modo == ModoCadastro.Listagem;
        _cadastro.Visible = modo != ModoCadastro.Listagem;
        _modo.Text = modo switch
        {
            ModoCadastro.Inclusao => "Modo: Inclusão",
            ModoCadastro.Edicao => "Modo: Edição",
            ModoCadastro.Consulta => "Modo: Consulta (inativo)",
            _ => "Modo: Listagem"
        };
        _modo.AccessibleDescription = _modo.Text;
        _incluir.Visible = modo == ModoCadastro.Inclusao;
        _editar.Visible = modo == ModoCadastro.Edicao;
        AcceptButton = modo switch
        {
            ModoCadastro.Inclusao => _incluir,
            ModoCadastro.Edicao => _editar,
            _ => null
        };
        HabilitarEdicao(_cartao, modo is ModoCadastro.Inclusao or ModoCadastro.Edicao);
        _cadastro.AutoScrollPosition = Point.Empty;
    }

    private void HabilitarEdicao(Control pai, bool editavel)
    {
        foreach (Control c in pai.Controls)
        {
            if (c == _incluir || c == _editar || c.Name is "btn_voltar" or "button_ativo")
                continue;

            switch (c)
            {
                case TextBoxBase texto:
                    texto.ReadOnly = !editavel;
                    break;
                case ComboBox or NumericUpDown or DateTimePicker or CheckedListBox or Button:
                    c.Enabled = editavel;
                    break;
                default:
                    if (c.HasChildren)
                        HabilitarEdicao(c, editavel);
                    break;
            }
        }
    }

    /// <summary>Atalhos: F5 lista de novo, Ctrl+N inclui e Esc volta para a lista.</summary>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode == Keys.F5 && Modo == ModoCadastro.Listagem)
        {
            TeclaListar();
            e.Handled = true;
        }
        else if (e.Control && e.KeyCode == Keys.N && Modo == ModoCadastro.Listagem)
        {
            TeclaNovo();
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.Escape && Modo != ModoCadastro.Listagem)
        {
            TeclaVoltar();
            e.Handled = true;
        }
    }

    protected virtual void TeclaListar() { }
    protected virtual void TeclaNovo() { }
    protected virtual void TeclaVoltar() { }
}
