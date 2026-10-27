namespace Avalia.Desktop.Core.Modelos;

/// <summary>
/// Perfis de acesso. Os valores correspondem aos Ids fixos da tabela Perfil
/// (script database/03_carga_inicial.sql).
/// </summary>
public enum PerfilUsuario
{
    Aluno = 1,
    Professor = 2,
    Administrador = 3
}

public static class PerfilUsuarioExtensoes
{
    public static string Descricao(this PerfilUsuario perfil) => perfil switch
    {
        PerfilUsuario.Aluno => "Aluno",
        PerfilUsuario.Professor => "Professor",
        PerfilUsuario.Administrador => "Administrador",
        _ => "Desconhecido"
    };

    /// <summary>RN26: a Desktop contém apenas as telas do Administrador.</summary>
    public static bool PossuiTelasNaDesktop(this PerfilUsuario perfil) => perfil == PerfilUsuario.Administrador;
}
