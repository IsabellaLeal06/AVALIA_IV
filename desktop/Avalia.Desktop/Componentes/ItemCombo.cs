namespace Avalia.Desktop.Componentes;

/// <summary>Item de ComboBox com valor opcional, para opções como "Todos" ou "Sem responsável".</summary>
public sealed record ItemCombo<T>(T Valor, string Texto)
{
    public override string ToString() => Texto;
}

public static class ComboBoxExtensoes
{
    public static void Preencher<T>(this ComboBox combo, IEnumerable<ItemCombo<T>> itens, T? selecionado = default)
    {
        var lista = itens.ToList();
        combo.DataSource = null;
        combo.DisplayMember = nameof(ItemCombo<T>.Texto);
        combo.DataSource = lista;
        var indice = lista.FindIndex(i => EqualityComparer<T>.Default.Equals(i.Valor, selecionado));
        combo.SelectedIndex = indice >= 0 ? indice : (lista.Count > 0 ? 0 : -1);
    }

    public static T? Valor<T>(this ComboBox combo) => combo.SelectedItem is ItemCombo<T> item ? item.Valor : default;
}
