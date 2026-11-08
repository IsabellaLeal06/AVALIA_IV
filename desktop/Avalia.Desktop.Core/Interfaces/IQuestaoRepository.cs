using Avalia.Desktop.Core.Modelos;

namespace Avalia.Desktop.Core.Interfaces;

/// <summary>Persistência das classes Questao e Alternativa do diagrama de classes.</summary>
public interface IQuestaoRepository
{
    /// <summary>Questão com as cinco alternativas.</summary>
    Task<Questao?> Consultar(int id);

    Task<IReadOnlyList<QuestaoResumo>> Listar(FiltroQuestoes filtro);

    /// <summary>Insere a questão e as cinco alternativas na mesma transação.</summary>
    Task<int> Incluir(Questao questao, IReadOnlyList<Alternativa> alternativas);

    /// <summary>Atualiza a questão e as cinco alternativas na mesma transação.</summary>
    Task Editar(int id, Questao questao, IReadOnlyList<Alternativa> alternativas);

    Task Desativar(int id);
    Task Reativar(int id);

    /// <summary>RN11: a questão já aparece em algum simulado?</summary>
    Task<bool> FoiUtilizadaEmSimulado(int idQuestao);

    Task<int> ContarAtivos();
}
