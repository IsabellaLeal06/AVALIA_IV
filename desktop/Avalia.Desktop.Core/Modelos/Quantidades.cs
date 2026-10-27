namespace Avalia.Desktop.Core.Modelos;

/// <summary>
/// Retorno de ControlePainel.contar(): quantidade de cadastros ativos exibida
/// no painel do Administrador. Sem indicadores de desempenho (RF06, RN24).
/// </summary>
public class Quantidades
{
    public int Usuarios { get; set; }
    public int Alunos { get; set; }
    public int Professores { get; set; }
    public int Administradores { get; set; }
    public int Turmas { get; set; }
    public int Disciplinas { get; set; }
    public int Questoes { get; set; }
}
