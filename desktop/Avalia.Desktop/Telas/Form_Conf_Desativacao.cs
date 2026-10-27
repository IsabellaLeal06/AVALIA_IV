using Avalia.Desktop.Componentes;
using Avalia.Desktop.Core.Controles;

namespace Avalia.Desktop.Telas;

/// <summary>
/// Form_Conf_Desativação: confirmação exibida antes de desativar (ou reativar/excluir)
/// um registro, dentro da janela principal (RN12). Caso de uso Confirmar Desativação:
/// solicitarDesativacao(mensagem) → confirmarDesativação() → ConfirmarDesativacao.confirmar().
/// Depois da confirmação, a tela que pediu executa desativar(id) e exibirMensagemSucesso().
/// </summary>
public partial class Form_Conf_Desativação : TelaBase
{
    private readonly ConfirmarDesativacao _controle;
    private Func<Task>? _aoConfirmar;

    public Form_Conf_Desativação(ConfirmarDesativacao controle, INavegacao navegacao)
    {
        _controle = controle;
        Navegacao = navegacao;
        InitializeComponent();
        Estilo.BotaoSecundario(button_cancelarDesativacao);
    }

    /// <summary>solicitarDesativacao(mensagem)</summary>
    public void SolicitarDesativacao(string mensagem, Func<Task> aoConfirmar) =>
        Configurar("Confirmar desativação", mensagem, "&Confirmar desativação", destrutiva: true, aoConfirmar);

    public void SolicitarReativacao(string mensagem, Func<Task> aoConfirmar) =>
        Configurar("Confirmar reativação", mensagem, "&Confirmar reativação", destrutiva: false, aoConfirmar);

    public void SolicitarExclusao(string mensagem, Func<Task> aoConfirmar) =>
        Configurar("Confirmar exclusão", mensagem, "&Confirmar exclusão", destrutiva: true, aoConfirmar);

    private void Configurar(string titulo, string mensagem, string textoBotao, bool destrutiva, Func<Task> aoConfirmar)
    {
        label_titulo.Text = titulo;
        label_mensagemConfirmacao.Text = mensagem;
        button_confirmarDesativacao.Text = textoBotao;
        button_confirmarDesativacao.AccessibleName = textoBotao.Replace("&", string.Empty);
        AccessibleName = titulo;
        AccessibleDescription = mensagem;
        _aoConfirmar = aoConfirmar;

        if (destrutiva)
        {
            Estilo.BotaoPerigo(button_confirmarDesativacao);
            button_confirmarDesativacao.BackColor = Estilo.Perigo;
            button_confirmarDesativacao.ForeColor = Color.White;
            button_confirmarDesativacao.FlatAppearance.MouseOverBackColor = Color.FromArgb(153, 27, 27);
        }
        else
        {
            Estilo.BotaoPrimario(button_confirmarDesativacao);
        }
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        button_cancelarDesativacao.Focus(); // o padrão é não confirmar, para evitar desativações acidentais
    }

    /// <summary>confirmarDesativação(): registra a confirmação, volta para a tela anterior e ela conclui a operação.</summary>
    public async Task ConfirmarDesativacao()
    {
        _controle.Confirmar();
        var aoConfirmar = _aoConfirmar;
        Navegacao!.Voltar();
        if (aoConfirmar is not null)
            await aoConfirmar();
    }

    /// <summary>Cancela: nada é alterado e a tela anterior volta.</summary>
    public void CancelarDesativacao()
    {
        _controle.Cancelar();
        Navegacao!.Voltar();
    }

    private async void button_confirmarDesativacao_Click(object? sender, EventArgs e) => await ConfirmarDesativacao();
    private void button_cancelarDesativacao_Click(object? sender, EventArgs e) => CancelarDesativacao();
}
