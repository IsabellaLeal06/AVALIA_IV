// Acesso às tabelas Turma e TurmaDisciplina (grade da turma).
const { sql, consultar, consultarUm, executar } = require('../config/banco');
const { padraoBusca, listaDeIds } = require('./auxiliares');

const ALUNO = 1;

const turmaRepository = {
  listar({ ativo = null, busca = null } = {}) {
    return consultar(
      `SELECT t.Id_Turma AS id, t.Nome_Turma AS nome, t.AnoLetivo_Turma AS anoLetivo, t.Ativo_Turma AS ativo,
              (SELECT COUNT(*) FROM dbo.Usuario u
                WHERE u.Id_Turma = t.Id_Turma AND u.Id_Perfil = @Aluno AND u.Ativo_Usuario = 1) AS qtdAlunosAtivos,
              (SELECT COUNT(*) FROM dbo.TurmaDisciplina td WHERE td.Id_Turma = t.Id_Turma) AS qtdDisciplinas
       FROM dbo.Turma t
       WHERE (@Ativo IS NULL OR t.Ativo_Turma = @Ativo)
         AND (@Busca IS NULL OR t.Nome_Turma LIKE @Busca ESCAPE '\\')
       ORDER BY t.AnoLetivo_Turma DESC, t.Nome_Turma`,
      { Ativo: [sql.Bit, ativo], Busca: [sql.NVarChar(110), padraoBusca(busca)], Aluno: [sql.Int, ALUNO] }
    );
  },

  consultar(id, transacao) {
    return consultarUm(
      `SELECT t.Id_Turma AS id, t.Nome_Turma AS nome, t.AnoLetivo_Turma AS anoLetivo, t.Ativo_Turma AS ativo,
              (SELECT COUNT(*) FROM dbo.Usuario u
                WHERE u.Id_Turma = t.Id_Turma AND u.Id_Perfil = @Aluno AND u.Ativo_Usuario = 1) AS qtdAlunosAtivos
       FROM dbo.Turma t WHERE t.Id_Turma = @Id`,
      { Id: [sql.Int, id], Aluno: [sql.Int, ALUNO] },
      transacao
    );
  },

  /** Turmas pelos ids, para validar o que o cliente enviou. */
  listarPorIds(ids) {
    if (!ids.length) return Promise.resolve([]);
    const { lista, parametros } = listaDeIds(ids, 't');
    return consultar(
      `SELECT Id_Turma AS id, Nome_Turma AS nome, AnoLetivo_Turma AS anoLetivo, Ativo_Turma AS ativo
       FROM dbo.Turma WHERE Id_Turma IN (${lista})`,
      parametros
    );
  },

  listarDisciplinas(idTurma) {
    return consultar(
      `SELECT d.Id_Disciplina AS id, d.Nome_Disciplina AS nome, d.Ativo_Disciplina AS ativo
       FROM dbo.TurmaDisciplina td
       JOIN dbo.Disciplina d ON d.Id_Disciplina = td.Id_Disciplina
       WHERE td.Id_Turma = @Id
       ORDER BY d.Nome_Disciplina`,
      { Id: [sql.Int, idTurma] }
    );
  },

  async existe(nome, anoLetivo, ignorarId = null) {
    const linha = await consultarUm(
      `SELECT 1 AS existe FROM dbo.Turma
       WHERE Nome_Turma = @Nome AND AnoLetivo_Turma = @Ano AND (@IgnorarId IS NULL OR Id_Turma <> @IgnorarId)`,
      { Nome: [sql.NVarChar(100), nome], Ano: [sql.Int, anoLetivo], IgnorarId: [sql.Int, ignorarId] }
    );
    return Boolean(linha);
  },

  async incluir(turma, transacao) {
    const linha = await consultarUm(
      `INSERT INTO dbo.Turma (Nome_Turma, AnoLetivo_Turma, Ativo_Turma)
       OUTPUT INSERTED.Id_Turma AS id
       VALUES (@Nome, @Ano, 1)`,
      { Nome: [sql.NVarChar(100), turma.nome], Ano: [sql.Int, turma.anoLetivo] },
      transacao
    );
    return linha.id;
  },

  async editar(id, turma, transacao) {
    await executar(
      'UPDATE dbo.Turma SET Nome_Turma = @Nome, AnoLetivo_Turma = @Ano WHERE Id_Turma = @Id',
      { Id: [sql.Int, id], Nome: [sql.NVarChar(100), turma.nome], Ano: [sql.Int, turma.anoLetivo] },
      transacao
    );
  },

  /** Exclusão lógica (RN12). Os alunos vinculados NÃO são desativados. */
  async alterarSituacao(id, ativo) {
    await executar('UPDATE dbo.Turma SET Ativo_Turma = @Ativo WHERE Id_Turma = @Id', {
      Id: [sql.Int, id],
      Ativo: [sql.Bit, ativo]
    });
  },

  async existeVinculo(idTurma, idDisciplina) {
    const linha = await consultarUm(
      'SELECT 1 AS existe FROM dbo.TurmaDisciplina WHERE Id_Turma = @Turma AND Id_Disciplina = @Disciplina',
      { Turma: [sql.Int, idTurma], Disciplina: [sql.Int, idDisciplina] }
    );
    return Boolean(linha);
  },

  async vincularDisciplina(idTurma, idDisciplina, transacao) {
    await executar(
      `IF NOT EXISTS (SELECT 1 FROM dbo.TurmaDisciplina WHERE Id_Turma = @Turma AND Id_Disciplina = @Disciplina)
         INSERT INTO dbo.TurmaDisciplina (Id_Turma, Id_Disciplina) VALUES (@Turma, @Disciplina)`,
      { Turma: [sql.Int, idTurma], Disciplina: [sql.Int, idDisciplina] },
      transacao
    );
  },

  async desvincularDisciplina(idTurma, idDisciplina, transacao) {
    await executar(
      'DELETE FROM dbo.TurmaDisciplina WHERE Id_Turma = @Turma AND Id_Disciplina = @Disciplina',
      { Turma: [sql.Int, idTurma], Disciplina: [sql.Int, idDisciplina] },
      transacao
    );
  },

  /** Deixa a grade da turma exatamente com as disciplinas informadas. */
  async substituirDisciplinas(idTurma, idsDisciplinas, transacao) {
    // Lista vazia = grade vazia. (NOT IN (NULL) nunca é verdadeiro no SQL e não apagaria nada.)
    if (idsDisciplinas.length === 0) {
      await executar('DELETE FROM dbo.TurmaDisciplina WHERE Id_Turma = @Turma;', { Turma: [sql.Int, idTurma] }, transacao);
      return;
    }
    const { lista, parametros } = listaDeIds(idsDisciplinas, 'd');
    await executar(
      `DELETE FROM dbo.TurmaDisciplina WHERE Id_Turma = @Turma AND Id_Disciplina NOT IN (${lista});`,
      { Turma: [sql.Int, idTurma], ...parametros },
      transacao
    );
    for (const idDisciplina of idsDisciplinas) {
      await turmaRepository.vincularDisciplina(idTurma, idDisciplina, transacao);
    }
  }
};

module.exports = turmaRepository;
