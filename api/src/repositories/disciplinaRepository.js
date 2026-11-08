// Acesso à tabela Disciplina (Id_Usuario = professor responsável, RN07).
const { sql, consultar, consultarUm, executar } = require('../config/banco');
const { padraoBusca, listaDeIds } = require('./auxiliares');

const SELECAO = `
  SELECT d.Id_Disciplina AS id, d.Nome_Disciplina AS nome, d.Descricao_Disciplina AS descricao,
         d.Ativo_Disciplina AS ativo, d.Id_Usuario AS idResponsavel, u.Nome_Usuario AS nomeResponsavel,
         (SELECT COUNT(*) FROM dbo.Questao q
           WHERE q.Id_Disciplina = d.Id_Disciplina AND q.Ativo_Questao = 1) AS qtdQuestoesAtivas,
         (SELECT COUNT(*) FROM dbo.TurmaDisciplina td WHERE td.Id_Disciplina = d.Id_Disciplina) AS qtdTurmas
  FROM dbo.Disciplina d
  LEFT JOIN dbo.Usuario u ON u.Id_Usuario = d.Id_Usuario`;

const disciplinaRepository = {
  /** idResponsavel restringe às disciplinas de um professor (escopo do Professor, RN08). */
  listar({ ativo = null, busca = null, idResponsavel = null } = {}) {
    return consultar(
      `${SELECAO}
       WHERE (@Ativo IS NULL OR d.Ativo_Disciplina = @Ativo)
         AND (@Busca IS NULL OR d.Nome_Disciplina LIKE @Busca ESCAPE '\\')
         AND (@Responsavel IS NULL OR d.Id_Usuario = @Responsavel)
       ORDER BY d.Nome_Disciplina`,
      {
        Ativo: [sql.Bit, ativo],
        Busca: [sql.NVarChar(110), padraoBusca(busca)],
        Responsavel: [sql.Int, idResponsavel]
      }
    );
  },

  consultar(id) {
    return consultarUm(`${SELECAO} WHERE d.Id_Disciplina = @Id`, { Id: [sql.Int, id] });
  },

  /** Disciplinas ativas para o aluno montar o simulado, com as questões disponíveis (só ativas). */
  listarAtivasParaSimulado() {
    return consultar(
      `SELECT d.Id_Disciplina AS id, d.Nome_Disciplina AS nome, d.Descricao_Disciplina AS descricao,
              (SELECT COUNT(*) FROM dbo.Questao q
                WHERE q.Id_Disciplina = d.Id_Disciplina AND q.Ativo_Questao = 1) AS qtdQuestoes
       FROM dbo.Disciplina d
       WHERE d.Ativo_Disciplina = 1
       ORDER BY d.Nome_Disciplina`
    );
  },

  /** Disciplinas pelos ids, para validar o que o cliente enviou. */
  listarPorIds(ids) {
    if (!ids.length) return Promise.resolve([]);
    const { lista, parametros } = listaDeIds(ids, 'd');
    return consultar(
      `SELECT Id_Disciplina AS id, Nome_Disciplina AS nome, Ativo_Disciplina AS ativo, Id_Usuario AS idResponsavel
       FROM dbo.Disciplina WHERE Id_Disciplina IN (${lista})`,
      parametros
    );
  },

  listarTurmas(idDisciplina) {
    return consultar(
      `SELECT t.Id_Turma AS id, t.Nome_Turma AS nome, t.AnoLetivo_Turma AS anoLetivo, t.Ativo_Turma AS ativo
       FROM dbo.TurmaDisciplina td
       JOIN dbo.Turma t ON t.Id_Turma = td.Id_Turma
       WHERE td.Id_Disciplina = @Id
       ORDER BY t.AnoLetivo_Turma DESC, t.Nome_Turma`,
      { Id: [sql.Int, idDisciplina] }
    );
  },

  async existeNome(nome, ignorarId = null) {
    const linha = await consultarUm(
      `SELECT 1 AS existe FROM dbo.Disciplina
       WHERE Nome_Disciplina = @Nome AND (@IgnorarId IS NULL OR Id_Disciplina <> @IgnorarId)`,
      { Nome: [sql.NVarChar(100), nome], IgnorarId: [sql.Int, ignorarId] }
    );
    return Boolean(linha);
  },

  async incluir(disciplina, transacao) {
    const linha = await consultarUm(
      `INSERT INTO dbo.Disciplina (Nome_Disciplina, Descricao_Disciplina, Ativo_Disciplina, Id_Usuario)
       OUTPUT INSERTED.Id_Disciplina AS id
       VALUES (@Nome, @Descricao, 1, @Responsavel)`,
      {
        Nome: [sql.NVarChar(100), disciplina.nome],
        Descricao: [sql.NVarChar(500), disciplina.descricao],
        Responsavel: [sql.Int, disciplina.idResponsavel]
      },
      transacao
    );
    return linha.id;
  },

  async editar(id, disciplina, transacao) {
    await executar(
      `UPDATE dbo.Disciplina
          SET Nome_Disciplina = @Nome, Descricao_Disciplina = @Descricao, Id_Usuario = @Responsavel
        WHERE Id_Disciplina = @Id`,
      {
        Id: [sql.Int, id],
        Nome: [sql.NVarChar(100), disciplina.nome],
        Descricao: [sql.NVarChar(500), disciplina.descricao],
        Responsavel: [sql.Int, disciplina.idResponsavel]
      },
      transacao
    );
  },

  /** Exclusão lógica (RN12). As questões saem do sorteio pela condição Ativo_Disciplina. */
  async alterarSituacao(id, ativo) {
    await executar('UPDATE dbo.Disciplina SET Ativo_Disciplina = @Ativo WHERE Id_Disciplina = @Id', {
      Id: [sql.Int, id],
      Ativo: [sql.Bit, ativo]
    });
  },

  /** Deixa a disciplina vinculada exatamente às turmas informadas. */
  async substituirTurmas(idDisciplina, idsTurmas, transacao) {
    // Lista vazia = nenhuma turma. (NOT IN (NULL) nunca é verdadeiro no SQL e não apagaria nada.)
    if (idsTurmas.length === 0) {
      await executar('DELETE FROM dbo.TurmaDisciplina WHERE Id_Disciplina = @Disciplina;', { Disciplina: [sql.Int, idDisciplina] }, transacao);
      return;
    }
    const { lista, parametros } = listaDeIds(idsTurmas, 't');
    await executar(
      `DELETE FROM dbo.TurmaDisciplina WHERE Id_Disciplina = @Disciplina AND Id_Turma NOT IN (${lista});`,
      { Disciplina: [sql.Int, idDisciplina], ...parametros },
      transacao
    );
    for (const idTurma of idsTurmas) {
      await executar(
        `IF NOT EXISTS (SELECT 1 FROM dbo.TurmaDisciplina WHERE Id_Turma = @Turma AND Id_Disciplina = @Disciplina)
           INSERT INTO dbo.TurmaDisciplina (Id_Turma, Id_Disciplina) VALUES (@Turma, @Disciplina)`,
        { Turma: [sql.Int, idTurma], Disciplina: [sql.Int, idDisciplina] },
        transacao
      );
    }
  }
};

module.exports = disciplinaRepository;
