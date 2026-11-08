namespace Avalia.Desktop.Componentes;

public enum TipoMensagem
{
    Sucesso,
    Informacao,
    Aviso,
    Erro
}

/// <summary>
/// Faixa de mensagens exibida no topo da área de conteúdo. Substitui as
/// caixas de mensagem: a tela informa o resultado de cada operação sem
/// abrir janelas (exibirMensagemSucesso, exibirAviso e exibirErro do
/// diagrama de sequência).
/// </summary>
public class BarraMensagem : Panel
{
    private readonly Label _texto;
    private readonly Button _fechar;

    public BarraMensagem()
    {
        Visible = false;
        Dock = DockStyle.Top;
        Padding = new Padding(16, 8, 8, 8);
        Margin = new Padding(0);

        _fechar = new Button
        {
            Dock = DockStyle.Right,
            Width = 36,
            Text = "×",
            Font = new Font("Segoe UI", 14F),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            AccessibleName = "Fechar mensagem",
            TabStop = false
        };
        _fechar.FlatAppearance.BorderSize = 0;
        _fechar.Click += (_, _) => Ocultar();

        _texto = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            AccessibleRole = AccessibleRole.Alert,
            AccessibleName = "Mensagem"
        };

        Controls.Add(_texto);
        Controls.Add(_fechar);
    }

    public string Texto => _texto.Text;

    public void Exibir(TipoMensagem tipo, string mensagem)
    {
        var (fundo, cor) = tipo switch
        {
            TipoMensagem.Sucesso => (Color.FromArgb(220, 252, 231), Color.FromArgb(22, 101, 52)),
            TipoMensagem.Aviso => (Estilo.AvisoFundo, Estilo.AvisoTexto),
            TipoMensagem.Erro => (Color.FromArgb(254, 226, 226), Estilo.Perigo),
            _ => (Color.FromArgb(239, 246, 255), Estilo.Menu)
        };

        BackColor = fundo;
        _texto.ForeColor = cor;
        _fechar.ForeColor = cor;
        _fechar.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(fundo, 0.05f);
        _texto.Text = mensagem;
        _texto.Font = new Font(Font, tipo == TipoMensagem.Informacao ? FontStyle.Regular : FontStyle.Bold);
        _texto.AccessibleDescription = mensagem;

        var linhas = Math.Clamp(mensagem.Split('\n').Length, 1, 6);
        Height = 22 * linhas + Padding.Vertical + 12;
        Visible = true;
    }

    public void Ocultar()
    {
        _texto.Text = string.Empty;
        Visible = false;
    }
}
