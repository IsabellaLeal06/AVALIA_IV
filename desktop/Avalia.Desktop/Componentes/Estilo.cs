namespace Avalia.Desktop.Componentes;

/// <summary>
/// Identidade visual do Avalia+ (mesmas cores da versão Web) aplicada aos
/// controles em tempo de execução. Contrastes conferidos para leitura (RNF06).
/// </summary>
public static class Estilo
{
    public static readonly Color Primaria = Color.FromArgb(37, 99, 235);       // #2563EB
    public static readonly Color PrimariaEscura = Color.FromArgb(29, 78, 216); // #1D4ED8
    public static readonly Color Roxo = Color.FromArgb(124, 58, 237);          // #7C3AED
    public static readonly Color Menu = Color.FromArgb(30, 58, 138);           // #1E3A8A
    public static readonly Color MenuSelecionado = Color.FromArgb(37, 99, 235);
    public static readonly Color Fundo = Color.FromArgb(245, 246, 250);        // #F5F6FA
    public static readonly Color Superficie = Color.White;
    public static readonly Color Borda = Color.FromArgb(229, 231, 235);        // #E5E7EB
    public static readonly Color Texto = Color.FromArgb(17, 24, 39);           // #111827
    public static readonly Color TextoSecundario = Color.FromArgb(75, 85, 99); // #4B5563
    public static readonly Color Perigo = Color.FromArgb(185, 28, 28);         // #B91C1C
    public static readonly Color Sucesso = Color.FromArgb(21, 128, 61);        // #15803D
    public static readonly Color AvisoFundo = Color.FromArgb(254, 243, 199);   // #FEF3C7
    public static readonly Color AvisoTexto = Color.FromArgb(146, 64, 14);     // #92400E
    public static readonly Color LinhaAlternada = Color.FromArgb(248, 250, 252);
    public static readonly Color Inativo = Color.FromArgb(107, 114, 128);

    public static void BotaoPrimario(params Button[] botoes) =>
        Aplicar(botoes, Primaria, Color.White, PrimariaEscura, negrito: true);

    public static void BotaoSecundario(params Button[] botoes) =>
        Aplicar(botoes, Superficie, Primaria, Color.FromArgb(239, 246, 255), negrito: false, borda: Primaria);

    public static void BotaoPerigo(params Button[] botoes) =>
        Aplicar(botoes, Superficie, Perigo, Color.FromArgb(254, 242, 242), negrito: false, borda: Perigo);

    public static void BotaoMenu(params Button[] botoes)
    {
        foreach (var botao in botoes)
        {
            botao.FlatStyle = FlatStyle.Flat;
            botao.FlatAppearance.BorderSize = 0;
            botao.FlatAppearance.MouseOverBackColor = PrimariaEscura;
            botao.FlatAppearance.MouseDownBackColor = Primaria;
            botao.BackColor = Menu;
            botao.ForeColor = Color.White;
            botao.TextAlign = ContentAlignment.MiddleLeft;
            botao.Padding = new Padding(16, 0, 0, 0);
            botao.Cursor = Cursors.Hand;
            botao.UseVisualStyleBackColor = false;
        }
    }

    /// <summary>Padroniza as listagens: seleção de linha inteira, somente leitura, cabeçalho em destaque.</summary>
    public static void Grade(DataGridView grade)
    {
        grade.AutoGenerateColumns = false;
        grade.ReadOnly = true;
        grade.AllowUserToAddRows = false;
        grade.AllowUserToDeleteRows = false;
        grade.AllowUserToResizeRows = false;
        grade.MultiSelect = false;
        grade.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grade.RowHeadersVisible = false;
        grade.BackgroundColor = Superficie;
        grade.BorderStyle = BorderStyle.None;
        grade.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grade.GridColor = Borda;
        grade.EnableHeadersVisualStyles = false;
        grade.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        grade.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        grade.ColumnHeadersHeight = 40;
        grade.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(239, 246, 255);
        grade.ColumnHeadersDefaultCellStyle.ForeColor = Texto;
        grade.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 246, 255);
        grade.ColumnHeadersDefaultCellStyle.Font = new Font(grade.Font, FontStyle.Bold);
        grade.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 0, 0);
        grade.DefaultCellStyle.ForeColor = Texto;
        grade.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
        grade.DefaultCellStyle.SelectionForeColor = Texto;
        grade.DefaultCellStyle.Padding = new Padding(6, 0, 0, 0);
        grade.AlternatingRowsDefaultCellStyle.BackColor = LinhaAlternada;
        grade.RowTemplate.Height = 34;
        grade.StandardTab = true; // Tab sai da grade em vez de andar pelas células (RNF06)
    }

    /// <summary>Linhas de registros inativos ficam em cinza e itálico.</summary>
    public static void DestacarInativos<T>(DataGridView grade, Func<T, bool> ativo)
    {
        grade.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || grade.Rows[e.RowIndex].DataBoundItem is not T item || ativo(item) || e.CellStyle is null)
                return;
            e.CellStyle.ForeColor = Inativo;
            e.CellStyle.Font = new Font(grade.Font, FontStyle.Italic);
        };
    }

    private static void Aplicar(Button[] botoes, Color fundo, Color texto, Color hover, bool negrito, Color? borda = null)
    {
        foreach (var botao in botoes)
        {
            botao.FlatStyle = FlatStyle.Flat;
            botao.BackColor = fundo;
            botao.ForeColor = texto;
            botao.FlatAppearance.BorderSize = borda is null ? 0 : 1;
            botao.FlatAppearance.BorderColor = borda ?? fundo;
            botao.FlatAppearance.MouseOverBackColor = hover;
            botao.Font = new Font(botao.Font, negrito ? FontStyle.Bold : FontStyle.Regular);
            botao.Cursor = Cursors.Hand;
            botao.UseVisualStyleBackColor = false;
        }
    }
}
