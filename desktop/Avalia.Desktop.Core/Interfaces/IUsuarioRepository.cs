using Avalia.Desktop.Core.Modelos;

namespace Avalia.Desktop.Core.Interfaces;

/// <summary>
/// Persistência da classe Usuario do diagrama de classes
/// (consultar, consultarPorEmail, listar, incluir, editar, desativar,
/// alterarSenha e contarAtivos).
/// </summary>
public interface IUsuarioRepository
{
    /// <summary>Dados completos, incluindo o hash da senha (uso interno dos controles).</summary>
    Task<Usuario?> Consultar(int id);

    /// <summary>Busca pelo e-mail para autenticação, incluindo o hash da senha.</summary>
    Task<Usuario?> ConsultarPorEmail(string email);

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
    Task AlterarSenha(int id, string hash);

    /// <summary>Professores para o ComboBox de responsável pela disciplina.</summary>
    Task<IReadOnlyList<OpcaoSelecao>> ListarProfessores(bool somenteAtivos);

    /// <summary>Quantidade de disciplinas em que o usuário é o professor responsável.</summary>
    Task<int> ContarDisciplinasSobResponsabilidade(int idUsuario);

    /// <summary>Usuários ativos (de um perfil, ou de todos quando nulo).</summary>
    Task<int> ContarAtivos(PerfilUsuario? perfil = null);
}
