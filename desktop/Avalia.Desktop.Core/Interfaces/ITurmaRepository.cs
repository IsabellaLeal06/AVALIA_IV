using Avalia.Desktop.Core.Modelos;

namespace Avalia.Desktop.Core.Interfaces;

/// <summary>Persistência da classe Turma do diagrama de classes.</summary>
public interface ITurmaRepository
{
    Task<Turma?> Consultar(int id);
    Task<IReadOnlyList<Turma>> Listar(FiltroCadastro filtro);

    /// <summary>Turmas para seleção (ComboBox de usuários e filtros).</summary>
    Task<IReadOnlyList<OpcaoSelecao>> ListarOpcoes(bool somenteAtivas);

    /// <summary>RN06: nome + ano letivo é único.</summary>
    Task<bool> Existe(string nome, int anoLetivo, int? ignorarId = null);

    /// <summary>Insere a turma e os vínculos com disciplinas na mesma transação.</summary>
    Task<int> Incluir(Turma turma, IEnumerable<int> disciplinas);

    /// <summary>Atualiza a turma e substitui os vínculos com disciplinas na mesma transação.</summary>
    Task Editar(int id, Turma turma, IEnumerable<int> disciplinas);

    Task Desativar(int id);
    Task Reativar(int id);
    Task<int> ContarAlunosAtivos(int idTurma);
    Task<IReadOnlyList<int>> ListarIdsDisciplinasVinculadas(int idTurma);
    Task<int> ContarAtivos();
}
