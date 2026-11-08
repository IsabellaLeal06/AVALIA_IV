// Consultas de desempenho (RF06). Visibilidade por perfil (RN24) e alunos desativados (RN25).
const { sql, consultar, consultarUm } = require('../config/banco');

const ALUNO = 1;

/**
 * ESCOPO DO PROFESSOR — escrito uma única vez e usado em TODAS as consultas do professor (RN24).
 *
 * O caminho não é direto: Disciplina (professor responsável) -> TurmaDisciplina -> Turma.
 *  - EscopoTurmaDisciplina: pares (turma, disciplina) em que o professor é o responsável pela disciplina.
 *    Disciplina sem responsável não dá visibilidade a nenhum professor (RN07).
 *  - AlunosEscopo: alunos ATIVOS dessas turmas. Alunos desativados ficam fora das médias (RN25).
 *
 * Devolve o texto das CTEs (só nomes de parâmetros, nunca valores) e os parâmetros.
 */
function escopoProfessor(idProfessor) {
  return {
    cte: `
      EscopoTurmaDisciplina AS (
        SELECT DISTINCT td.Id_Turma, td.Id_Disciplina
        FROM dbo.Disciplina d
        JOIN dbo.TurmaDisciplina td ON td.Id_Disciplina = d.Id_Disciplina
        WHERE d.Id_Usuario = @IdProfessor
      ),
      AlunosEscopo AS (
        SELECT u.Id_Usuario, u.Id_Turma, u.Nome_Usuario
        FROM dbo.Usuario u
        WHERE u.Id_Perfil = @PerfilAluno
          AND u.Ativo_Usuario = 1
          AND u.Id_Turma IN (SELECT Id_Turma FROM EscopoTurmaDisciplina)
      ),
      DesempenhoEscopo AS (
        -- desempenho dos alunos do escopo, só nas disciplinas do professor que estão na grade da turma do aluno
        SELECT a.Id_Turma, a.Id_Usuario, de.Id_Disciplina, de.TotalRespostas_Desempenho, de.TotalAcertos_Desempenho
        FROM dbo.Desempenho de
        JOIN AlunosEscopo a ON a.Id_Usuario = de.Id_Usuario
        JOIN EscopoTurmaDisciplina e ON e.Id_Turma = a.Id_Turma AND e.Id_Disciplina = de.Id_Disciplina
      )`,
    parametros: { IdProfessor: [sql.Int, idProfessor], PerfilAluno: [sql.Int, ALUNO] }
  };
}

const MEDIA = (alias) =>
  `CAST(100.0 * SUM(${alias}.TotalAcertos_Desempenho) / NULLIF(SUM(${alias}.TotalRespostas_Desempenho), 0) AS DECIMAL(5, 2))`;

const desempenhoRepository = {
  // ------------------------------------------------------------------ Aluno (próprios dados, histórico completo)
  resumoAluno(idAluno) {
    return consultarUm(
      `SELECT COUNT(*) AS qtdSimulados,
              CAST(100.0 * SUM(TotalAcertos_ResultadoSimulado) / NULLIF(SUM(TotalQuestoes_ResultadoSimulado), 0)
                   AS DECIMAL(5, 2)) AS mediaAcertos,
              MAX(DataRealizacao_ResultadoSimulado) AS ultimoSimulado
       FROM dbo.ResultadoSimulado WHERE Id_Usuario = @Aluno`,
      { Aluno: [sql.Int, idAluno] }
    );
  },

  porDisciplinaAluno(idAluno) {
    return consultar(
      `SELECT d.Id_Disciplina AS idDisciplina, d.Nome_Disciplina AS nomeDisciplina,
              de.TotalRespostas_Desempenho AS totalRespostas, de.TotalAcertos_Desempenho AS totalAcertos,
              de.Percentual_Desempenho AS percentual, de.UltimaAtualizacao_Desempenho AS ultimaAtualizacao
       FROM dbo.Desempenho de
       JOIN dbo.Disciplina d ON d.Id_Disciplina = de.Id_Disciplina
       WHERE de.Id_Usuario = @Aluno
       ORDER BY d.Nome_Disciplina`,
      { Aluno: [sql.Int, idAluno] }
    );
  },

  // ------------------------------------------------------------------ Professor (sempre pelo escopo)
  turmasProfessor(idProfessor) {
    const { cte, parametros } = escopoProfessor(idProfessor);
    return consultar(
      `WITH ${cte}
       SELECT t.Id_Turma AS id, t.Nome_Turma AS nome, t.AnoLetivo_Turma AS anoLetivo, t.Ativo_Turma AS ativo,
              (SELECT COUNT(*) FROM AlunosEscopo a WHERE a.Id_Turma = t.Id_Turma) AS qtdAlunos,
              (SELECT ${MEDIA('de')} FROM DesempenhoEscopo de WHERE de.Id_Turma = t.Id_Turma) AS mediaAcertos,
              (SELECT COUNT(*) FROM dbo.ResultadoSimulado r
                 JOIN AlunosEscopo a ON a.Id_Usuario = r.Id_Usuario WHERE a.Id_Turma = t.Id_Turma) AS qtdSimulados,
              (SELECT MAX(r.DataRealizacao_ResultadoSimulado) FROM dbo.ResultadoSimulado r
                 JOIN AlunosEscopo a ON a.Id_Usuario = r.Id_Usuario WHERE a.Id_Turma = t.Id_Turma) AS ultimoSimulado
       FROM dbo.Turma t
       WHERE t.Id_Turma IN (SELECT Id_Turma FROM EscopoTurmaDisciplina)
       ORDER BY t.Ativo_Turma DESC, t.AnoLetivo_Turma DESC, t.Nome_Turma`,
      parametros
    );
  },

  async turmaNoEscopo(idProfessor, idTurma) {
    const { cte, parametros } = escopoProfessor(idProfessor);
    return consultarUm(
      `WITH ${cte}
       SELECT t.Id_Turma AS id, t.Nome_Turma AS nome, t.AnoLetivo_Turma AS anoLetivo, t.Ativo_Turma AS ativo
       FROM dbo.Turma t
       WHERE t.Id_Turma = @Turma AND t.Id_Turma IN (SELECT Id_Turma FROM EscopoTurmaDisciplina)`,
      { ...parametros, Turma: [sql.Int, idTurma] }
    );
  },

  disciplinasDaTurma(idProfessor, idTurma) {
    const { cte, parametros } = escopoProfessor(idProfessor);
    return consultar(
      `WITH ${cte}
       SELECT d.Id_Disciplina AS idDisciplina, d.Nome_Disciplina AS nomeDisciplina,
              (SELECT ${MEDIA('de')} FROM DesempenhoEscopo de
                WHERE de.Id_Turma = e.Id_Turma AND de.Id_Disciplina = e.Id_Disciplina) AS mediaAcertos,
              (SELECT COALESCE(SUM(de.TotalRespostas_Desempenho), 0) FROM DesempenhoEscopo de
                WHERE de.Id_Turma = e.Id_Turma AND de.Id_Disciplina = e.Id_Disciplina) AS totalRespostas
       FROM EscopoTurmaDisciplina e
       JOIN dbo.Disciplina d ON d.Id_Disciplina = e.Id_Disciplina
       WHERE e.Id_Turma = @Turma
       ORDER BY d.Nome_Disciplina`,
      { ...parametros, Turma: [sql.Int, idTurma] }
    );
  },

  alunosDaTurma(idProfessor, idTurma) {
    const { cte, parametros } = escopoProfessor(idProfessor);
    return consultar(
      `WITH ${cte}
       SELECT a.Id_Usuario AS id, a.Nome_Usuario AS nome,
              (SELECT ${MEDIA('de')} FROM DesempenhoEscopo de WHERE de.Id_Usuario = a.Id_Usuario) AS mediaAcertos,
              (SELECT COUNT(*) FROM dbo.ResultadoSimulado r WHERE r.Id_Usuario = a.Id_Usuario) AS qtdSimulados,
              (SELECT MAX(r.DataRealizacao_ResultadoSimulado) FROM dbo.ResultadoSimulado r
                WHERE r.Id_Usuario = a.Id_Usuario) AS ultimoSimulado
       FROM AlunosEscopo a
       WHERE a.Id_Turma = @Turma
       ORDER BY a.Nome_Usuario`,
      { ...parametros, Turma: [sql.Int, idTurma] }
    );
  },

  /**
   * Painel CONSOLIDADO do aluno para o professor: média por disciplina e quantidade de simulados.
   * Operação própria, sem reaproveitar a revisão: nenhuma resposta questão a questão sai daqui (RN24).
   */
  async painelAlunoParaProfessor(idProfessor, idTurma, idAluno) {
    const { cte, parametros } = escopoProfessor(idProfessor);
    const p = { ...parametros, Turma: [sql.Int, idTurma], Aluno: [sql.Int, idAluno] };
    const aluno = await consultarUm(
      `WITH ${cte}
       SELECT a.Id_Usuario AS id, a.Nome_Usuario AS nome,
              (SELECT ${MEDIA('de')} FROM DesempenhoEscopo de WHERE de.Id_Usuario = a.Id_Usuario) AS mediaAcertos,
              (SELECT COUNT(*) FROM dbo.ResultadoSimulado r WHERE r.Id_Usuario = a.Id_Usuario) AS qtdSimulados,
              (SELECT MAX(r.DataRealizacao_ResultadoSimulado) FROM dbo.ResultadoSimulado r
                WHERE r.Id_Usuario = a.Id_Usuario) AS ultimoSimulado
       FROM AlunosEscopo a
       WHERE a.Id_Turma = @Turma AND a.Id_Usuario = @Aluno`,
      p
    );
    if (!aluno) return null;
    aluno.porDisciplina = await consultar(
      `WITH ${cte}
       SELECT d.Id_Disciplina AS idDisciplina, d.Nome_Disciplina AS nomeDisciplina,
              ${MEDIA('de')} AS mediaAcertos,
              COALESCE(SUM(de.TotalRespostas_Desempenho), 0) AS totalRespostas
       FROM EscopoTurmaDisciplina e
       JOIN dbo.Disciplina d ON d.Id_Disciplina = e.Id_Disciplina
       LEFT JOIN DesempenhoEscopo de ON de.Id_Disciplina = e.Id_Disciplina AND de.Id_Usuario = @Aluno
       WHERE e.Id_Turma = @Turma
       GROUP BY d.Id_Disciplina, d.Nome_Disciplina
       ORDER BY d.Nome_Disciplina`,
      p
    );
    return aluno;
  },

  /** Questões das disciplinas do professor com maior índice de erro, entre os alunos do escopo (RF06). */
  questoesComMaisErros(idProfessor, idTurma = null, limite = 10) {
    const { cte, parametros } = escopoProfessor(idProfessor);
    return consultar(
      `WITH ${cte}
       SELECT TOP (@Limite) q.Id_Questao AS idQuestao, q.Enunciado_Questao AS enunciado,
              d.Nome_Disciplina AS nomeDisciplina,
              COUNT(*) AS totalRespostas,
              SUM(CASE WHEN ra.Acertou_RespostaAluno = 0 THEN 1 ELSE 0 END) AS totalErros,
              CAST(100.0 * SUM(CASE WHEN ra.Acertou_RespostaAluno = 0 THEN 1 ELSE 0 END) / COUNT(*) AS DECIMAL(5, 2))
                AS percentualErro
       FROM dbo.RespostaAluno ra
       JOIN dbo.Questao q ON q.Id_Questao = ra.Id_Questao
       JOIN dbo.Disciplina d ON d.Id_Disciplina = q.Id_Disciplina
       JOIN AlunosEscopo a ON a.Id_Usuario = ra.Id_Usuario
       JOIN EscopoTurmaDisciplina e ON e.Id_Turma = a.Id_Turma AND e.Id_Disciplina = q.Id_Disciplina
       WHERE (@Turma IS NULL OR a.Id_Turma = @Turma)
       GROUP BY q.Id_Questao, q.Enunciado_Questao, d.Nome_Disciplina
       ORDER BY percentualErro DESC, totalRespostas DESC, q.Id_Questao`,
      { ...parametros, Turma: [sql.Int, idTurma], Limite: [sql.Int, limite] }
    );
  }
};

module.exports = { desempenhoRepository, escopoProfessor };
