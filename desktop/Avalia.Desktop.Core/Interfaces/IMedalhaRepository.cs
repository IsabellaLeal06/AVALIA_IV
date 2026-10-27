using Avalia.Desktop.Core.Modelos;

namespace Avalia.Desktop.Core.Interfaces;

/// <summary>Persistência da classe Medalha do diagrama de classes (faixas de pontuação, RN22).</summary>
public interface IMedalhaRepository
{
    /// <summary>Faixas em ordem de pontuação, com a quantidade de alunos ativos em cada uma.</summary>
    Task<IReadOnlyList<Medalha>> Listar();

    Task<Medalha?> Consultar(int id);
    Task<bool> ExisteNome(string nome, int? ignorarId = null);
    Task<bool> ExistePontosMinimos(int pontosMinimos, int? ignorarId = null);
    Task<int> Incluir(Medalha medalha);
    Task Editar(int id, Medalha medalha);

    /// <summary>Exclusão física: a medalha não é gravada no aluno, é apurada na consulta (RN22).</summary>
    Task Excluir(int id);

    /// <summary>Alunos ativos com pontuação abaixo da primeira faixa.</summary>
    Task<int> QtdAlunosSemMedalha();
}
