// Banco de questões (RF03, RN07, RN08, RN09, RN10, RN11, RN12).
const { emTransacao } = require('../config/banco');
const questaoRepository = require('../repositories/questaoRepository');
const disciplinaRepository = require('../repositories/disciplinaRepository');
const { ErroValidacao, ErroNaoEncontrado, ErroConflito, ErroProibido } = require('../utils/erros');
const v = require('../utils/validacoes');
const { PERFIL } = require('../utils/perfis');
const { urlCompleta } = require('../utils/urls');
const { caminhoDeImagem } = require('./uploadService');

const LETRAS = ['A', 'B', 'C', 'D', 'E'];
const TAMANHO_MAXIMO_TEXTO = 8000;

const MENSAGEM_RN11 =
  'Esta questão já foi utilizada em simulado, por isso a alternativa correta não pode ser alterada (RN11). ' +
  'Se a resposta estiver errada, desative esta questão e cadastre uma nova.';

/**
 * Escopo do Professor (RN08): as disciplinas vêm SEMPRE do usuário do token,
 * nunca de uma indicação do cliente. null = sem restrição (Administrador).
 */
async function disciplinasDoEscopo(usuarioToken) {
  if (usuarioToken.idPerfil !== PERFIL.PROFESSOR) return null;
  const disciplinas = await disciplinaRepository.listar({ idResponsavel: usuarioToken.id });
  return disciplinas.map((d) => d.id);
}

/** Busca a questão respeitando o escopo. Fora do escopo responde como inexistente. */
async function obterNoEscopo(id, usuarioToken) {
  const questao = await questaoRepository.consultar(id);
  const escopo = await disciplinasDoEscopo(usuarioToken);
  if (!questao || (escopo && !escopo.includes(questao.idDisciplina))) throw new ErroNaoEncontrado('Questão não encontrada.');
  return questao;
}

function resumir(texto, tamanho = 180) {
  const limpo = String(texto ?? '').replace(/\s+/g, ' ').trim();
  return limpo.length > tamanho ? `${limpo.slice(0, tamanho - 1)}…` : limpo;
}

/** Aceita "correta": "C" no corpo ou "correta": true em uma das alternativas. */
function normalizar(dados) {
  const corretaInformada = typeof dados.correta === 'string' ? dados.correta.trim().toUpperCase() : null;
  const alternativas = (Array.isArray(dados.alternativas) ? dados.alternativas : [])
    .map((a) => {
      const letra = String(a?.letra ?? '').trim().toUpperCase();
      return {
        letra,
        texto: v.textoLongo(a?.texto),
        correta: corretaInformada ? letra === corretaInformada : v.booleano(a?.correta) === true
      };
    })
    .sort((a, b) => a.letra.localeCompare(b.letra));

  return {
    enunciado: v.textoLongo(dados.enunciado),
    explicacao: v.textoLongo(dados.explicacao) || null,
    imagemUrl: caminhoDeImagem(dados.imagemUrl ?? dados.imagem, 'questoes'),
    imagemInvalida: Boolean(dados.imagemUrl ?? dados.imagem) && !caminhoDeImagem(dados.imagemUrl ?? dados.imagem, 'questoes'),
    idDisciplina: v.idOuNulo(dados.idDisciplina),
    alternativas
  };
}

/** RN09: exatamente cinco alternativas, de A a E, sem letras repetidas e com apenas uma correta. */
function validarAlternativas(alternativas, campos) {
  const letras = alternativas.map((a) => a.letra);
  if (alternativas.length !== 5 || letras.join('') !== LETRAS.join('')) {
    campos.alternativas = 'A questão deve ter exatamente cinco alternativas, de A a E, sem letras repetidas.';
    return;
  }
  for (const a of alternativas) {
    if (!a.texto) campos[`alternativa${a.letra}`] = `Informe o texto da alternativa ${a.letra}.`;
    else if (a.texto.length > TAMANHO_MAXIMO_TEXTO) campos[`alternativa${a.letra}`] = 'Texto longo demais.';
  }
  const vistos = new Map();
  for (const a of alternativas.filter((x) => x.texto)) {
    const chave = a.texto.toLowerCase();
    if (vistos.has(chave)) campos[`alternativa${a.letra}`] = `As alternativas ${vistos.get(chave)} e ${a.letra} têm o mesmo texto.`;
    else vistos.set(chave, a.letra);
  }
  const corretas = alternativas.filter((a) => a.correta).length;
  if (corretas === 0) campos.correta = 'Selecione a alternativa correta.';
  else if (corretas > 1) campos.correta = 'Apenas uma alternativa pode ser a correta.';
}

async function validar(questao, atual, usuarioToken, campos) {
  if (!questao.enunciado) campos.enunciado = 'Informe o enunciado da questão.';
  else if (questao.enunciado.length > TAMANHO_MAXIMO_TEXTO) campos.enunciado = 'O enunciado é longo demais.';
  if (questao.explicacao && questao.explicacao.length > TAMANHO_MAXIMO_TEXTO) campos.explicacao = 'A explicação é longa demais.';
  if (questao.imagemInvalida) campos.imagemUrl = 'Envie a imagem pelo upload antes de gravar a questão.';

  if (!questao.idDisciplina || Number.isNaN(questao.idDisciplina)) {
    campos.idDisciplina = 'Selecione a disciplina.';
  } else {
    const escopo = await disciplinasDoEscopo(usuarioToken);
    const [disciplina] = await disciplinaRepository.listarPorIds([questao.idDisciplina]);
    const mesmaDisciplina = atual?.idDisciplina === questao.idDisciplina;
    if (!disciplina || (escopo && !escopo.includes(disciplina.id))) {
      campos.idDisciplina = 'Disciplina não encontrada entre as suas disciplinas.';
    } else if (!disciplina.ativo && !mesmaDisciplina) {
      campos.idDisciplina = 'A disciplina selecionada está desativada.';
    } else if (!atual && !disciplina.idResponsavel) {
      // RN07: o cadastro de novas questões fica suspenso em disciplina sem responsável
      campos.idDisciplina = 'Esta disciplina está sem professor responsável. O cadastro de novas questões fica suspenso até a designação de um responsável.';
    }
  }
  validarAlternativas(questao.alternativas, campos);
}

function paraResposta(questao, utilizada) {
  return {
    id: questao.id,
    enunciado: questao.enunciado,
    imagemUrl: questao.imagemUrl,
    imagemUrlCompleta: urlCompleta(questao.imagemUrl),
    explicacao: questao.explicacao,
    ativo: questao.ativo,
    idDisciplina: questao.idDisciplina,
    disciplina: questao.nomeDisciplina,
    disciplinaAtiva: questao.disciplinaAtiva,
    idAutor: questao.idAutor,
    autor: questao.nomeAutor,
    alternativas: questao.alternativas,
    correta: questao.alternativas.find((a) => a.correta)?.letra ?? null,
    // RN11: a tela bloqueia a troca da alternativa correta
    utilizada
  };
}

const questaoService = {
  MENSAGEM_RN11,

  /** Lista no escopo do perfil, com a disciplina e QUEM CADASTROU (RN08). */
  async listar(filtros, usuarioToken) {
    const idDisciplina = v.idOuNulo(filtros.disciplina);
    const lista = await questaoRepository.listar({
      idDisciplina: Number.isNaN(idDisciplina) ? null : idDisciplina,
      idsDisciplinas: await disciplinasDoEscopo(usuarioToken),
      ativo: v.filtroSituacao(filtros.situacao, 'todos'),
      busca: filtros.busca
    });
    return lista.map((q) => ({
      id: q.id,
      enunciado: resumir(q.enunciado),
      idDisciplina: q.idDisciplina,
      disciplina: q.nomeDisciplina,
      disciplinaAtiva: q.disciplinaAtiva,
      idAutor: q.idAutor,
      autor: q.nomeAutor,
      temImagem: Boolean(q.temImagem),
      // RN11: já entrou em algum simulado (a alternativa correta fica bloqueada)
      utilizada: Boolean(q.utilizada),
      ativo: q.ativo
    }));
  },

  async consultar(id, usuarioToken) {
    const questao = await obterNoEscopo(id, usuarioToken);
    return paraResposta(questao, await questaoRepository.foiUtilizada(id));
  },

  /** Questão + 5 alternativas na mesma transação. RN08: registra quem cadastrou. */
  async incluir(dados, usuarioToken) {
    const questao = normalizar(dados);
    const campos = {};
    await validar(questao, null, usuarioToken, campos);
    ErroValidacao.verificar(campos);

    questao.idAutor = usuarioToken.id;
    const id = await emTransacao((transacao) => questaoRepository.incluir(questao, transacao));
    return { id, mensagem: 'Questão cadastrada com sucesso.' };
  },

  async editar(id, dados, usuarioToken) {
    const atual = await obterNoEscopo(id, usuarioToken);
    if (!atual.ativo) throw new ErroConflito('A questão está desativada e não pode ser editada. Reative-a antes.');

    const questao = normalizar(dados);
    // A imagem que já estava gravada (inclusive a enviada pela Desktop) continua aceita
    const imagemEnviada = dados.imagemUrl ?? dados.imagem;
    if (atual.imagemUrl && (imagemEnviada === atual.imagemUrl || imagemEnviada === urlCompleta(atual.imagemUrl))) {
      questao.imagemUrl = atual.imagemUrl;
      questao.imagemInvalida = false;
    }
    const campos = {};
    await validar(questao, atual, usuarioToken, campos);
    ErroValidacao.verificar(campos);

    // RN11: enunciado, imagem e explicação continuam editáveis; a alternativa correta não
    const corretaAtual = atual.alternativas.find((a) => a.correta)?.letra;
    const corretaNova = questao.alternativas.find((a) => a.correta)?.letra;
    if (corretaAtual !== corretaNova && (await questaoRepository.foiUtilizada(id))) {
      throw new ErroConflito(MENSAGEM_RN11, { correta: MENSAGEM_RN11 });
    }

    await emTransacao((transacao) => questaoRepository.editar(id, questao, transacao));
    return { id, mensagem: 'Questão atualizada com sucesso.' };
  },

  /**
   * PATCH /questoes/:id/situacao. O Professor desativa questões das suas disciplinas;
   * a reativação é do Administrador (RN12).
   */
  async alterarSituacao(id, ativo, usuarioToken) {
    if (ativo === null) throw new ErroValidacao('Informe a situação: ativo true ou false.', { ativo: 'Valor inválido.' });
    const questao = await obterNoEscopo(id, usuarioToken);
    if (ativo && usuarioToken.idPerfil !== PERFIL.ADMINISTRADOR) {
      throw new ErroProibido('Somente o Administrador pode reativar questões.');
    }
    if (questao.ativo === ativo) throw new ErroConflito(ativo ? 'A questão já está ativa.' : 'A questão já está desativada.');

    await questaoRepository.alterarSituacao(id, ativo);
    let mensagem = ativo ? `Questão nº ${id} reativada.` : `Questão nº ${id} desativada. Ela não entrará em novos simulados.`;
    if (ativo && !questao.disciplinaAtiva) {
      mensagem = `Questão nº ${id} reativada. Ela só entrará nos simulados quando a disciplina "${questao.nomeDisciplina}" for reativada.`;
    }
    return { id, ativo, mensagem };
  }
};

module.exports = questaoService;
