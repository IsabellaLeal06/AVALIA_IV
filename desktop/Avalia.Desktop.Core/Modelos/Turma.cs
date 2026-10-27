namespace Avalia.Desktop.Core.Modelos;

/// <summary>Tabela Turma. Identificada por nome + ano letivo (RN06).</summary>
public class Turma
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int AnoLetivo { get; set; } = DateTime.Today.Year;
    public bool Ativo { get; set; } = true;

    // Preenchidos na listagem
    public int QuantidadeAlunosAtivos { get; set; }
    public int QuantidadeDisciplinas { get; set; }

    public string Situacao => Ativo ? "Ativa" : "Inativa";
    public string NomeCompleto => $"{Nome} ({AnoLetivo})";
}

/// <summary>
/// Disciplinas que podem ser marcadas na edição da turma (somente ativas)
/// e vínculos já existentes com disciplinas desativadas, que são preservados.
/// </summary>
public class VinculoDisciplinasTurma
{
    public IReadOnlyList<OpcaoSelecao> DisciplinasAtivas { get; init; } = [];
    public IReadOnlySet<int> IdsVinculados { get; init; } = new HashSet<int>();
    public IReadOnlyList<OpcaoSelecao> VinculadasInativas { get; init; } = [];
}
