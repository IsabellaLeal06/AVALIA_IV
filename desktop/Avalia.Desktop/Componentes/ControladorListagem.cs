using Avalia.Desktop.Core.Modelos;

namespace Avalia.Desktop.Componentes;

/// <summary>
/// Comportamento comum das listas: carga da grade, seleção do registro,
/// habilitação dos botões e o ID para consultar (campo button_id ou a linha selecionada).
/// </summary>
public sealed class ControladorListagem<T> where T : class
{
    private readonly Control _tela;
    private readonly Action<string> _exibirErro;
    private readonly DataGridView _grade;
    private readonly Label _total;
    private readonly Button? _desativar;
    private readonly Button? _reativar;

    public ControladorListagem(Control tela, Action<string> exibirErro, DataGridView grade, Label total,
        Button? desativar, Button? reativar)
    {
        _tela = tela;
        _exibirErro = exibirErro;
        _grade = grade;
        _total = total;
        _desativar = desativar;
        _reativar = reativar;
        Estilo.Grade(grade);
        AtualizarBotoes();
    }

    public required Func<Task<IReadOnlyList<T>>> Carregar { get; init; }
    public required Func<T, int> Id { get; init; }

    /// <summary>Situação do registro; nula quando o cadastro não tem desativação (Medalha).</summary>
    public Func<T, bool>? Ativo { get; init; }

    public required string NomePlural { get; init; }

    /// <summary>Enquanto verdadeiro, mudanças nos filtros não recarregam a lista.</summary>
    public bool IgnorarFiltros { get; set; }

    public T? Selecionado => _grade.CurrentRow?.DataBoundItem as T;

    public int? IdSelecionado => Selecionado is { } item ? Id(item) : null;

    public async Task CarregarAsync(int? selecionarId = null)
    {
        if (IgnorarFiltros)
            return;

        selecionarId ??= IdSelecionado;

        IReadOnlyList<T>? itens = null;
        // Bloqueia só a grade: o foco continua no campo de busca enquanto o usuário digita
        if (!await Operacao.ExecutarAsync(_grade, _exibirErro, async () => itens = await Carregar()))
            return;

        _grade.DataSource = itens!.ToList();
        _total.Text = itens!.Count == 1 ? "1 registro" : $"{itens.Count} registros";
        _grade.AccessibleDescription = $"{itens.Count} {NomePlural} na lista";

        if (selecionarId is int id)
        {
            foreach (DataGridViewRow linha in _grade.Rows)
            {
                if (linha.DataBoundItem is T item && Id(item) == id)
                {
                    _grade.CurrentCell = linha.Cells[0];
                    break;
                }
            }
        }

        AtualizarBotoes();
    }

    public void AtualizarBotoes()
    {
        var item = Selecionado;
        if (_desativar is not null)
            _desativar.Enabled = item is not null && (Ativo is null || Ativo(item));
        if (_reativar is not null)
            _reativar.Enabled = item is not null && Ativo is not null && !Ativo(item);
    }

    /// <summary>ID digitado em button_id ou, se vazio, o do registro selecionado.</summary>
    public int? IdParaConsulta(TextBox campoId)
    {
        var texto = campoId.Text.Trim();
        if (texto.Length == 0)
            return IdSelecionado;
        return int.TryParse(texto, out var id) && id > 0 ? id : -1;
    }

    /// <summary>Enter na grade seleciona o registro (em vez de descer uma linha).</summary>
    public void TratarTeclaGrade(KeyEventArgs e, Action selecionar)
    {
        if (e.KeyCode != Keys.Enter)
            return;

        e.SuppressKeyPress = true;
        if (Selecionado is not null)
            selecionar();
    }

    /// <summary>Destaca em cinza e itálico as linhas de registros inativos.</summary>
    public void DestacarInativos()
    {
        if (Ativo is { } ativo)
            Estilo.DestacarInativos(_grade, ativo);
    }
}

/// <summary>Opções de filtro compartilhadas pelas listas.</summary>
public static class OpcoesFiltro
{
    public static IEnumerable<ItemCombo<FiltroSituacao>> Situacoes(bool feminino = false) =>
    [
        new(FiltroSituacao.Todos, feminino ? "Todas" : "Todos"),
        new(FiltroSituacao.Ativos, feminino ? "Ativas" : "Ativos"),
        new(FiltroSituacao.Inativos, feminino ? "Inativas" : "Inativos")
    ];
}
