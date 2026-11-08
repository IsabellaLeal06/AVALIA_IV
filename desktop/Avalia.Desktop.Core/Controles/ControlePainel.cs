using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Modelos;

namespace Avalia.Desktop.Core.Controles;

/// <summary>
/// Controle ControlePainel (caso de uso Visualizar Painel): contar() devolve
/// a quantidade de cadastros ativos, pedindo contarAtivos() a Usuario, Turma,
/// Disciplina e Questao. Sem indicadores de desempenho (RF06, RN24).
/// </summary>
public class ControlePainel(
    IUsuarioRepository usuarios,
    ITurmaRepository turmas,
    IDisciplinaRepository disciplinas,
    IQuestaoRepository questoes)
{
    public async Task<Quantidades> Contar() => new()
    {
        Usuarios = await usuarios.ContarAtivos(),
        Alunos = await usuarios.ContarAtivos(PerfilUsuario.Aluno),
        Professores = await usuarios.ContarAtivos(PerfilUsuario.Professor),
        Administradores = await usuarios.ContarAtivos(PerfilUsuario.Administrador),
        Turmas = await turmas.ContarAtivos(),
        Disciplinas = await disciplinas.ContarAtivos(),
        Questoes = await questoes.ContarAtivos()
    };
}
