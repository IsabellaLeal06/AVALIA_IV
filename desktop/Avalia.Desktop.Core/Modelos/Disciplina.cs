namespace Avalia.Desktop.Core.Modelos;

/// <summary>Tabela Disciplina.</summary>
public class Disciplina
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; } = true;

    /// <summary>Professor responsável (Id_Usuario). Opcional: RN07.</summary>
    public int? IdProfessorResponsavel { get; set; }

    // Preenchidos nas consultas
    public string? NomeProfessorResponsavel { get; set; }
    public int QuantidadeQuestoesAtivas { get; set; }

    public string Responsavel => NomeProfessorResponsavel ?? "Sem responsável";
    public string Situacao => Ativo ? "Ativa" : "Inativa";
}
