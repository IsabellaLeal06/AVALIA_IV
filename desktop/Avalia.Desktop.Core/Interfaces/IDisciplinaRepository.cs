using Avalia.Desktop.Core.Modelos;

namespace Avalia.Desktop.Core.Interfaces;

/// <summary>Persistência da classe Disciplina do diagrama de classes.</summary>
public interface IDisciplinaRepository
{
    Task<Disciplina?> Consultar(int id);
    Task<IReadOnlyList<Disciplina>> Listar(FiltroCadastro filtro);

    /// <summary>Disciplinas para seleção (turmas, questões e filtros).</summary>
    Task<IReadOnlyList<OpcaoSelecao>> ListarOpcoes(bool somenteAtivas);

    Task<bool> ExisteNome(string nome, int? ignorarId = null);
    Task<int> Incluir(Disciplina disciplina);
    Task Editar(int id, Disciplina disciplina);
    Task Desativar(int id);
    Task Reativar(int id);
    Task<int> ContarQuestoesAtivas(int idDisciplina);
    Task<int> ContarAtivos();
}
