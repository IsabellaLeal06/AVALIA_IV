// Acompanhamento de desempenho (RF06). O Administrador não tem nenhuma rota aqui (RN24).
const { desempenhoRepository } = require('../repositories/desempenhoRepository');
const { ErroNaoEncontrado } = require('../utils/erros');
const { dataHora } = require('../utils/datas');

const numero = (valor) => (valor === null || valor === undefined ? null : Number(valor));

/** Turma do escopo do professor; fora do escopo responde como inexistente (vazamento de dado pessoal, RN24). */
async function turmaDoProfessor(idProfessor, idTurma) {
  const turma = await desempenhoRepository.turmaNoEscopo(idProfessor, idTurma);
  if (!turma) throw new ErroNaoEncontrado('Turma não encontrada entre as suas turmas.');
  return turma;
}

const desempenhoService = {
  /** Aluno: o próprio desempenho, histórico completo (RN25 não se aplica aqui). */
  async doAluno(usuarioToken) {
    const resumo = await desempenhoRepository.resumoAluno(usuarioToken.id);
    const porDisciplina = await desempenhoRepository.porDisciplinaAluno(usuarioToken.id);
    return {
      qtdSimulados: resumo.qtdSimulados,
      mediaAcertos: numero(resumo.mediaAcertos),
      ultimoSimulado: dataHora(resumo.ultimoSimulado),
      porDisciplina: porDisciplina.map((d) => ({
        idDisciplina: d.idDisciplina,
        disciplina: d.nomeDisciplina,
        totalRespostas: d.totalRespostas,
        totalAcertos: d.totalAcertos,
        percentual: numero(d.percentual),
        ultimaAtualizacao: dataHora(d.ultimaAtualizacao)
      }))
    };
  },

  /** Professor: média de acertos por turma (só alunos ativos, RN25). */
  async turmasDoProfessor(usuarioToken) {
    const turmas = await desempenhoRepository.turmasProfessor(usuarioToken.id);
    return turmas.map((t) => ({
      id: t.id,
      nome: t.nome,
      anoLetivo: t.anoLetivo,
      ativo: t.ativo,
      qtdAlunos: t.qtdAlunos,
      mediaAcertos: numero(t.mediaAcertos),
      qtdSimulados: t.qtdSimulados,
      ultimoSimulado: dataHora(t.ultimoSimulado)
    }));
  },

  /** Professor: desempenho por disciplina (as suas) na turma. */
  async turma(usuarioToken, idTurma) {
    const turma = await turmaDoProfessor(usuarioToken.id, idTurma);
    const disciplinas = await desempenhoRepository.disciplinasDaTurma(usuarioToken.id, idTurma);
    return {
      ...turma,
      porDisciplina: disciplinas.map((d) => ({
        idDisciplina: d.idDisciplina,
        disciplina: d.nomeDisciplina,
        mediaAcertos: numero(d.mediaAcertos),
        totalRespostas: d.totalRespostas
      }))
    };
  },

  /** Professor: alunos matriculados (ativos) na turma, com dados consolidados. */
  async alunosDaTurma(usuarioToken, idTurma) {
    await turmaDoProfessor(usuarioToken.id, idTurma);
    const alunos = await desempenhoRepository.alunosDaTurma(usuarioToken.id, idTurma);
    return alunos.map((a) => ({
      id: a.id,
      nome: a.nome,
      mediaAcertos: numero(a.mediaAcertos),
      qtdSimulados: a.qtdSimulados,
      ultimoSimulado: dataHora(a.ultimoSimulado)
    }));
  },

  /**
   * Professor: painel CONSOLIDADO do aluno (média por disciplina e quantidade de simulados).
   * Nunca as respostas questão a questão (RN24): a operação é separada da revisão do aluno.
   */
  async alunoParaProfessor(usuarioToken, idTurma, idAluno) {
    const turma = await turmaDoProfessor(usuarioToken.id, idTurma);
    const aluno = await desempenhoRepository.painelAlunoParaProfessor(usuarioToken.id, idTurma, idAluno);
    if (!aluno) throw new ErroNaoEncontrado('Aluno não encontrado nesta turma.');
    return {
      id: aluno.id,
      nome: aluno.nome,
      turma: { id: turma.id, nome: turma.nome, anoLetivo: turma.anoLetivo },
      mediaAcertos: numero(aluno.mediaAcertos),
      qtdSimulados: aluno.qtdSimulados,
      ultimoSimulado: dataHora(aluno.ultimoSimulado),
      porDisciplina: aluno.porDisciplina.map((d) => ({
        idDisciplina: d.idDisciplina,
        disciplina: d.nomeDisciplina,
        mediaAcertos: numero(d.mediaAcertos),
        totalRespostas: d.totalRespostas
      }))
    };
  },

  /** Professor: questões das suas disciplinas com maior índice de erro (RF06). */
  async questoesComMaisErros(usuarioToken, filtros) {
    let idTurma = null;
    if (filtros.turma) {
      idTurma = Number(filtros.turma);
      if (!Number.isInteger(idTurma) || idTurma <= 0) throw new ErroNaoEncontrado('Turma não encontrada entre as suas turmas.');
      await turmaDoProfessor(usuarioToken.id, idTurma);
    }
    const limite = Math.min(50, Math.max(1, Number.parseInt(filtros.limite, 10) || 10));
    const questoes = await desempenhoRepository.questoesComMaisErros(usuarioToken.id, idTurma, limite);
    return questoes.map((q) => ({
      idQuestao: q.idQuestao,
      enunciado: String(q.enunciado).replace(/\s+/g, ' ').slice(0, 180),
      disciplina: q.nomeDisciplina,
      totalRespostas: q.totalRespostas,
      totalErros: q.totalErros,
      percentualErro: numero(q.percentualErro)
    }));
  }
};

module.exports = desempenhoService;
