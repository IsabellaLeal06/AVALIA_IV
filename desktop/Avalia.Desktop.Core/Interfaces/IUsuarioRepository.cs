using Avalia.Desktop.Core.Modelos;

namespace Avalia.Desktop.Core.Interfaces;

/// <summary>
/// Acesso aos dados da classe Usuario do diagrama de classes (consultar, listar, incluir,
/// editar, desativar, reativar e contarAtivos), feito pela API. O login e a troca de senha
/// ficam em <see cref="IAutenticacao"/>.
/// </summary>
public interface IUsuarioRepository
{
    /// <summary>Dados completos do usuário, incluindo o CPF (a senha nunca é devolvida).</summary>
    Task<Usuario?> Consultar(int id);

    /// <summary>Listagem sem CPF e sem senha.</summary>
    Task<IReadOnlyList<UsuarioResumo>> Listar(FiltroUsuarios filtro);

    Task<bool> ExisteEmail(string email, int? ignorarId = null);
    Task<bool> ExisteCpf(string cpf, int? ignorarId = null);

    /// <summary>Insere e devolve o Id gerado.</summary>
    Task<int> Incluir(Usuario usuario);

    /// <summary>Atualiza nome, CPF, e-mail, perfil, turma e data de ingresso. Não altera senha, pontuação nem situação.</summary>
    Task Editar(int id, Usuario usuario);

    Task Desativar(int id);
    Task Reativar(int id);

    /// <summary>Professores para o ComboBox de responsável pela disciplina.</summary>
    Task<IReadOnlyList<OpcaoSelecao>> ListarProfessores(bool somenteAtivos);

    /// <summary>Quantidade de disciplinas em que o usuário é o professor responsável.</summary>
    Task<int> ContarDisciplinasSobResponsabilidade(int idUsuario);

    /// <summary>Usuários ativos (de um perfil, ou de todos quando nulo).</summary>
    Task<int> ContarAtivos(PerfilUsuario? perfil = null);
}
