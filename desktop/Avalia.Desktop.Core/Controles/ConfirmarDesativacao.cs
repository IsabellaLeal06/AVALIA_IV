namespace Avalia.Desktop.Core.Controles;

/// <summary>
/// Controle ConfirmarDesativacao (caso de uso Confirmar Desativação):
/// registra a confirmação ou o cancelamento pedido na tela Form_Conf_Desativação.
/// </summary>
public class ConfirmarDesativacao
{
    /// <summary>Nulo enquanto o usuário não respondeu.</summary>
    public bool? Confirmado { get; private set; }

    public void Confirmar() => Confirmado = true;

    public void Cancelar() => Confirmado = false;
}
