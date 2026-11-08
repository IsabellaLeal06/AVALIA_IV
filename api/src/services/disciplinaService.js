// Disciplinas e professor responsável (RF02, RN07, RN12, RN27).
const { emTransacao } = require('../config/banco');
const disciplinaRepository = require('../repositories/disciplinaRepository');
const turmaRepository = require('../repositories/turmaRepository');
const usuarioRepository = require('../repositories/usuarioRepository');
const { ErroValidacao, ErroNaoEncontrado, ErroConflito } = require('../utils/erros');
const v = require('../utils/validacoes');
const { PERFIL } = require('../utils/perfis');

const TAMANHO_MAXIMO_NOME = 100;
const TAMANHO_MAXIMO_DESCRICAO = 500;

async function obter(id) {
  const disciplina = await disciplinaRepository.consultar(id);
  if (!disciplina) throw new ErroNaoEncontrado('Disciplina não encontrada.');
  return disciplina;
}

function normalizar(dados) {
  const descricao = v.textoLongo(dados.descricao);
  return {
    nome: v.normalizarTexto(dados.nome),
    descricao: descricao || null,
    // "Sem responsável" = vazio (RN07)
    idResponsavel: v.idOuNulo(dados.idResponsavel)
  };
}

async function validar(disciplina, atual, campos) {
  if (!disciplina.nome) campos.nome = 'Informe o nome da disciplina.';
  else if (disciplina.nome.length > TAMANHO_MAXIMO_NOME) campos.nome = `O nome deve ter no máximo ${TAMANHO_MAXIMO_NOME} caracteres.`;

  if (disciplina.descricao && disciplina.descricao.length > TAMANHO_MAXIMO_DESCRICAO) {
    campos.descricao = `A descrição deve ter no máximo ${TAMANHO_MAXIMO_DESCRICAO} caracteres.`;
  }

  // RN07: no máximo um responsável, que precisa ter mesmo o perfil Professor
  if (Number.isNaN(disciplina.idResponsavel)) {
    campos.idResponsavel = 'Responsável inválido.';
  } else if (disciplina.idResponsavel) {
    const professor = await usuarioRepository.consultar(disciplina.idResponsavel);
    if (!professor || professor.idPerfil !== PERFIL.PROFESSOR) {
      campos.idResponsavel = 'O responsável deve ser um usuário com perfil Professor.';
    } else if (!professor.ativo && atual?.idResponsavel !== professor.id) {
      campos.idResponsavel = 'O professor selecionado está desativado.';
    }
  }

  if (!campos.nome && (await disciplinaRepository.existeNome(disciplina.nome, atual?.id))) {
    campos.nome = 'Já existe uma disciplina com este nome.';
  }
}

/** Turmas vinculadas (opcional). Só turmas ativas podem receber a disciplina; vínculos antigos são mantidos. */
async function validarTurmas(lista, campos, idsJaVinculados = []) {
  if (lista === undefined || lista === null) return null;
  if (!Array.isArray(lista)) {
    campos.turmas = 'Informe a lista de turmas.';
    return null;
  }
  const ids = [...new Set(lista.map(Number))];
  if (ids.some((id) => !Number.isInteger(id) || id <= 0)) {
    campos.turmas = 'Lista de turmas inválida.';
    return null;
  }
  const encontradas = await turmaRepository.listarPorIds(ids);
  const invalidas = ids.filter((id) => {
    const turma = encontradas.find((t) => t.id === id);
    return !turma || (!turma.ativo && !idsJaVinculados.includes(id));
  });
  if (invalidas.length) campos.turmas = 'Somente turmas ativas podem receber a disciplina.';
  return ids;
}

const disciplinaService = {
  /** Administrador: todas. Professor: somente as disciplinas sob a sua responsabilidade (RN08). */
  async listar(filtros, usuarioToken) {
    const idResponsavel = usuarioToken.idPerfil === PERFIL.PROFESSOR ? usuarioToken.id : null;
    return disciplinaRepository.listar({
      ativo: v.filtroSituacao(filtros.situacao, 'todos'),
      busca: filtros.busca,
      idResponsavel
    });
  },

  async consultar(id) {
    const disciplina = await obter(id);
    disciplina.turmas = await disciplinaRepository.listarTurmas(id);
    return disciplina;
  },

  /** GET /disciplinas/ativas: para o aluno montar o simulado, com as questões disponíveis. */
  listarAtivas() {
    return disciplinaRepository.listarAtivasParaSimulado();
  },

  async incluir(dados) {
    const disciplina = normalizar(dados);
    const campos = {};
    await validar(disciplina, null, campos);
    const turmas = await validarTurmas(dados.turmas, campos);
    ErroValidacao.verificar(campos);

    const id = await emTransacao(async (transacao) => {
      const novoId = await disciplinaRepository.incluir(disciplina, transacao);
      if (turmas) await disciplinaRepository.substituirTurmas(novoId, turmas, transacao);
      return novoId;
    });
    return { id, mensagem: 'Disciplina cadastrada com sucesso.' };
  },

  async editar(id, dados) {
    const atual = await obter(id);
    if (!atual.ativo) throw new ErroConflito('A disciplina está desativada e não pode ser editada. Reative-a antes.');

    const disciplina = normalizar(dados);
    const campos = {};
    await validar(disciplina, atual, campos);
    const vinculadas = (await disciplinaRepository.listarTurmas(id)).map((t) => t.id);
    const turmas = await validarTurmas(dados.turmas, campos, vinculadas);
    ErroValidacao.verificar(campos);

    await emTransacao(async (transacao) => {
      await disciplinaRepository.editar(id, disciplina, transacao);
      if (turmas) await disciplinaRepository.substituirTurmas(id, turmas, transacao);
    });
    // RN10: a troca do responsável não remove as questões existentes
    return { id, mensagem: 'Disciplina atualizada com sucesso.' };
  },

  /** PATCH /disciplinas/:id/situacao. As questões da disciplina saem do sorteio enquanto ela estiver desativada. */
  async alterarSituacao(id, ativo) {
    if (ativo === null) throw new ErroValidacao('Informe a situação: ativo true ou false.', { ativo: 'Valor inválido.' });
    const disciplina = await obter(id);
    if (disciplina.ativo === ativo) {
      throw new ErroConflito(ativo ? 'A disciplina já está ativa.' : 'A disciplina já está desativada.');
    }
    await disciplinaRepository.alterarSituacao(id, ativo);
    return {
      id,
      ativo,
      questoesAtivas: disciplina.qtdQuestoesAtivas,
      mensagem: ativo
        ? `Disciplina "${disciplina.nome}" reativada.`
        : `Disciplina "${disciplina.nome}" desativada. As questões dela saíram do sorteio de simulados.`
    };
  }
};

module.exports = disciplinaService;
