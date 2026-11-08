// Turmas e grade de disciplinas (RF02, RN06, RN12, RN27).
const { emTransacao } = require('../config/banco');
const turmaRepository = require('../repositories/turmaRepository');
const disciplinaRepository = require('../repositories/disciplinaRepository');
const { ErroValidacao, ErroNaoEncontrado, ErroConflito } = require('../utils/erros');
const v = require('../utils/validacoes');

const TAMANHO_MAXIMO_NOME = 100;
const ANO_MINIMO = 2000;
const ANO_MAXIMO = 2100;

function nomeCompleto(turma) {
  return `${turma.nome} (${turma.anoLetivo})`;
}

async function obter(id) {
  const turma = await turmaRepository.consultar(id);
  if (!turma) throw new ErroNaoEncontrado('Turma não encontrada.');
  return turma;
}

/** Ids de disciplinas enviados (opcional). Só disciplinas ativas podem ser vinculadas. */
async function validarDisciplinas(lista, campos, idsJaVinculados = []) {
  if (lista === undefined || lista === null) return null;
  if (!Array.isArray(lista)) {
    campos.disciplinas = 'Informe a lista de disciplinas.';
    return null;
  }
  const ids = [...new Set(lista.map(Number))];
  if (ids.some((id) => !Number.isInteger(id) || id <= 0)) {
    campos.disciplinas = 'Lista de disciplinas inválida.';
    return null;
  }
  const encontradas = await disciplinaRepository.listarPorIds(ids);
  const invalidas = ids.filter((id) => {
    const disciplina = encontradas.find((d) => d.id === id);
    // vínculo já existente com disciplina desativada é mantido
    return !disciplina || (!disciplina.ativo && !idsJaVinculados.includes(id));
  });
  if (invalidas.length) campos.disciplinas = 'Somente disciplinas ativas podem ser vinculadas à turma.';
  return ids;
}

async function validar(turma, id, campos) {
  if (!turma.nome) campos.nome = 'Informe o nome da turma (o período faz parte do nome, ex.: ADS 3º Semestre A).';
  else if (turma.nome.length > TAMANHO_MAXIMO_NOME) campos.nome = `O nome deve ter no máximo ${TAMANHO_MAXIMO_NOME} caracteres.`;

  if (!Number.isInteger(turma.anoLetivo) || turma.anoLetivo < ANO_MINIMO || turma.anoLetivo > ANO_MAXIMO) {
    campos.anoLetivo = `Informe um ano letivo entre ${ANO_MINIMO} e ${ANO_MAXIMO}.`;
  }
  // RN06/RN27: nome + ano letivo não se repetem
  if (!campos.nome && !campos.anoLetivo && (await turmaRepository.existe(turma.nome, turma.anoLetivo, id))) {
    campos.nome = `Já existe a turma "${turma.nome}" no ano letivo ${turma.anoLetivo}.`;
  }
}

function normalizar(dados) {
  return { nome: v.normalizarTexto(dados.nome), anoLetivo: Number(dados.anoLetivo) };
}

const turmaService = {
  async listar(filtros) {
    return turmaRepository.listar({ ativo: v.filtroSituacao(filtros.situacao, 'todos'), busca: filtros.busca });
  },

  async consultar(id) {
    const turma = await obter(id);
    turma.disciplinas = await turmaRepository.listarDisciplinas(id);
    return turma;
  },

  async incluir(dados) {
    const turma = normalizar(dados);
    const campos = {};
    await validar(turma, null, campos);
    const disciplinas = await validarDisciplinas(dados.disciplinas, campos);
    ErroValidacao.verificar(campos);

    const id = await emTransacao(async (transacao) => {
      const novoId = await turmaRepository.incluir(turma, transacao);
      if (disciplinas) await turmaRepository.substituirDisciplinas(novoId, disciplinas, transacao);
      return novoId;
    });
    return { id, mensagem: 'Turma cadastrada com sucesso.' };
  },

  async editar(id, dados) {
    const atual = await obter(id);
    if (!atual.ativo) throw new ErroConflito('A turma está desativada e não pode ser editada. Reative-a antes.');

    const turma = normalizar(dados);
    const campos = {};
    await validar(turma, id, campos);
    const vinculadas = (await turmaRepository.listarDisciplinas(id)).map((d) => d.id);
    const disciplinas = await validarDisciplinas(dados.disciplinas, campos, vinculadas);
    ErroValidacao.verificar(campos);

    await emTransacao(async (transacao) => {
      await turmaRepository.editar(id, turma, transacao);
      if (disciplinas) await turmaRepository.substituirDisciplinas(id, disciplinas, transacao);
    });
    return { id, mensagem: 'Turma atualizada com sucesso.' };
  },

  /**
   * PATCH /turmas/:id/situacao. Devolve quantos alunos ativos estão vinculados, para a tela alertar,
   * SEM impedir: a turma é desativada e os alunos continuam ativos (RN12).
   */
  async alterarSituacao(id, ativo) {
    if (ativo === null) throw new ErroValidacao('Informe a situação: ativo true ou false.', { ativo: 'Valor inválido.' });
    const turma = await obter(id);
    if (turma.ativo === ativo) throw new ErroConflito(ativo ? 'A turma já está ativa.' : 'A turma já está desativada.');

    await turmaRepository.alterarSituacao(id, ativo);
    const alunosAtivos = turma.qtdAlunosAtivos;
    return {
      id,
      ativo,
      alunosAtivos,
      mensagem: ativo
        ? `Turma ${nomeCompleto(turma)} reativada.`
        : alunosAtivos > 0
          ? `Turma ${nomeCompleto(turma)} desativada. Os ${alunosAtivos} aluno(s) vinculado(s) continuam ativos.`
          : `Turma ${nomeCompleto(turma)} desativada.`
    };
  },

  /** POST /turmas/:id/disciplinas: inclui uma disciplina ativa na grade. */
  async vincularDisciplina(idTurma, idDisciplina) {
    const turma = await obter(idTurma);
    if (!turma.ativo) throw new ErroConflito('A turma está desativada. Reative-a antes de alterar a grade.');
    const id = Number(idDisciplina);
    const [disciplina] = Number.isInteger(id) && id > 0 ? await disciplinaRepository.listarPorIds([id]) : [];
    if (!disciplina) throw new ErroValidacao('Disciplina não encontrada.', { idDisciplina: 'Disciplina não encontrada.' });
    if (!disciplina.ativo) throw new ErroConflito('Somente disciplinas ativas podem ser vinculadas à turma.');
    if (await turmaRepository.existeVinculo(idTurma, id)) throw new ErroConflito('A disciplina já está na grade desta turma.');
    await turmaRepository.vincularDisciplina(idTurma, id);
    return { mensagem: `Disciplina "${disciplina.nome}" incluída na grade.` };
  },

  /** DELETE /turmas/:id/disciplinas/:idDisciplina: remove o vínculo (não exclui a disciplina). */
  async desvincularDisciplina(idTurma, idDisciplina) {
    await obter(idTurma);
    if (!(await turmaRepository.existeVinculo(idTurma, idDisciplina))) {
      throw new ErroNaoEncontrado('A disciplina não está na grade desta turma.');
    }
    await turmaRepository.desvincularDisciplina(idTurma, idDisciplina);
    return { mensagem: 'Disciplina retirada da grade da turma.' };
  }
};

module.exports = turmaService;
