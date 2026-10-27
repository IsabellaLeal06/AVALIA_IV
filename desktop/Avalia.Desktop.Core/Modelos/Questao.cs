namespace Avalia.Desktop.Core.Modelos;

/// <summary>Tabela Questao com as suas cinco alternativas.</summary>
public class Questao
{
    public static readonly string[] Letras = ["A", "B", "C", "D", "E"];

    public int Id { get; set; }
    public string Enunciado { get; set; } = string.Empty;
    public string? ImagemUrl { get; set; }

    /// <summary>Opcional (RN09).</summary>
    public string? Explicacao { get; set; }

    public bool Ativo { get; set; } = true;
    public int IdDisciplina { get; set; }

    /// <summary>Usuário que cadastrou a questão (RN08). Registra autoria, não propriedade (RN10).</summary>
    public int IdUsuarioCadastro { get; set; }

    public string? NomeDisciplina { get; set; }
    public string? NomeUsuarioCadastro { get; set; }

    public List<Alternativa> Alternativas { get; set; } = [];

    public string? LetraCorreta => Alternativas.Count(a => a.Correta) == 1
        ? Alternativas.Single(a => a.Correta).Letra
        : null;
}

/// <summary>Tabela Alternativa.</summary>
public class Alternativa
{
    public int Id { get; set; }
    public string Letra { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public bool Correta { get; set; }
    public int IdQuestao { get; set; }
}

/// <summary>Linha da listagem de questões.</summary>
public class QuestaoResumo
{
    private const int TamanhoResumo = 120;

    public int Id { get; set; }
    public string Enunciado { get; set; } = string.Empty;
    public string NomeDisciplina { get; set; } = string.Empty;
    public bool DisciplinaAtiva { get; set; }
    public string NomeUsuarioCadastro { get; set; } = string.Empty;
    public bool Ativo { get; set; }

    /// <summary>Já aparece em algum simulado (RN11).</summary>
    public bool Utilizada { get; set; }

    public string EnunciadoResumido
    {
        get
        {
            var texto = string.Join(' ', Enunciado.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
            return texto.Length <= TamanhoResumo ? texto : texto[..TamanhoResumo].TrimEnd() + "…";
        }
    }

    public string DisciplinaDescricao => DisciplinaAtiva ? NomeDisciplina : $"{NomeDisciplina} (inativa)";
    public string UtilizadaDescricao => Utilizada ? "Sim" : "Não";
    public string Situacao => Ativo ? "Ativa" : "Inativa";
}
