namespace Avalia.Desktop.Componentes;

/// <summary>
/// Navegação entre as telas dentro da janela principal (Form_Administrador).
/// Nenhuma tela abre em janela separada: as telas do menu e a confirmação
/// (Form_Conf_Desativação) aparecem na área de conteúdo e "Voltar" retorna à anterior.
/// </summary>
public interface INavegacao
{
    /// <summary>Abre uma tela do menu, descartando as telas abertas antes dela.</summary>
    void AbrirInicio<T>(Action<T>? configurar = null) where T : Form;

    /// <summary>Abre uma tela sobre a atual (por exemplo, a confirmação de desativação).</summary>
    void Abrir<T>(Action<T>? configurar = null) where T : Form;

    /// <summary>voltar(): fecha a tela atual e mostra a anterior.</summary>
    void Voltar();

    /// <summary>Fecha todas as telas e volta ao painel do Form_Administrador.</summary>
    void FecharTodas();

    /// <summary>Pede ao Form_Administrador que encerre a sessão e volte à autenticação.</summary>
    void SairDaConta();

    void ExibirMensagem(TipoMensagem tipo, string mensagem);
    void OcultarMensagem();

    /// <summary>Tela da base da pilha (a escolhida no menu); nula quando o painel está à mostra.</summary>
    Type? TelaInicial { get; }

    event EventHandler? TelaAlterada;
    event EventHandler? SaidaSolicitada;
}

public sealed class Navegador(IFabricaFormularios fabrica) : INavegacao
{
    private readonly Stack<Form> _pilha = new();
    private Panel? _area;
    private BarraMensagem? _barra;

    public event EventHandler? TelaAlterada;
    public event EventHandler? SaidaSolicitada;

    public Type? TelaInicial => _pilha.Count > 0 ? _pilha.Last().GetType() : null;

    /// <summary>Chamado pela janela principal ao abrir.</summary>
    public void Configurar(Panel area, BarraMensagem barra)
    {
        Encerrar();
        _area = area;
        _barra = barra;
    }

    /// <summary>Descarta as telas abertas (ao sair da conta).</summary>
    public void Encerrar()
    {
        while (_pilha.Count > 0)
            Descartar(_pilha.Pop());
        _area = null;
        _barra = null;
    }

    public void AbrirInicio<T>(Action<T>? configurar = null) where T : Form
    {
        OcultarMensagem();
        while (_pilha.Count > 0)
            Descartar(_pilha.Pop());
        Hospedar(configurar);
    }

    public void Abrir<T>(Action<T>? configurar = null) where T : Form
    {
        OcultarMensagem();
        if (_pilha.TryPeek(out var atual))
            atual.Hide();
        Hospedar(configurar);
    }

    public void Voltar()
    {
        if (_pilha.Count <= 1)
        {
            FecharTodas();
            return;
        }

        Descartar(_pilha.Pop());
        var anterior = _pilha.Peek();
        anterior.Show();
        Focar(anterior);
        TelaAlterada?.Invoke(this, EventArgs.Empty);
    }

    public void FecharTodas()
    {
        while (_pilha.Count > 0)
            Descartar(_pilha.Pop());
        TelaAlterada?.Invoke(this, EventArgs.Empty);
    }

    public void SairDaConta() => SaidaSolicitada?.Invoke(this, EventArgs.Empty);

    public void ExibirMensagem(TipoMensagem tipo, string mensagem) => _barra?.Exibir(tipo, mensagem);

    public void OcultarMensagem() => _barra?.Ocultar();

    private void Hospedar<T>(Action<T>? configurar) where T : Form
    {
        if (_area is null)
            throw new InvalidOperationException("A janela principal ainda não foi aberta.");

        var tela = fabrica.Criar<T>();
        configurar?.Invoke(tela);

        // A confirmação (tamanho fixo) fica como um cartão no canto; as demais ocupam a área toda
        var cartao = tela.FormBorderStyle == FormBorderStyle.FixedDialog;
        tela.TopLevel = false;
        tela.FormBorderStyle = FormBorderStyle.None;
        if (cartao)
        {
            tela.Dock = DockStyle.None;
            tela.Location = new Point(_area.Padding.Left, _area.Padding.Top);
        }
        else
        {
            tela.Dock = DockStyle.Fill;
        }

        _pilha.Push(tela);
        _area.Controls.Add(tela);
        tela.Show();
        tela.BringToFront();
        Focar(tela);
        TelaAlterada?.Invoke(this, EventArgs.Empty);
    }

    private static void Focar(Form tela) => tela.SelectNextControl(tela, true, true, true, true);

    private void Descartar(Form tela)
    {
        _area?.Controls.Remove(tela);
        tela.Close();
        tela.Dispose();
    }
}
