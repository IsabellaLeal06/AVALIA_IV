namespace Avalia.Desktop.Core.Modelos;

public enum FiltroSituacao
{
    Todos,
    Ativos,
    Inativos
}

public class FiltroUsuarios
{
    public PerfilUsuario? Perfil { get; set; }
    public int? IdTurma { get; set; }
    public FiltroSituacao Situacao { get; set; } = FiltroSituacao.Todos;

    /// <summary>Busca por parte do nome ou do e-mail.</summary>
    public string? Busca { get; set; }
}

public class FiltroCadastro
{
    public FiltroSituacao Situacao { get; set; } = FiltroSituacao.Todos;
    public string? Busca { get; set; }
}

public class FiltroQuestoes
{
    public int? IdDisciplina { get; set; }
    public FiltroSituacao Situacao { get; set; } = FiltroSituacao.Todos;

    /// <summary>Busca por parte do enunciado.</summary>
    public string? Busca { get; set; }
}
