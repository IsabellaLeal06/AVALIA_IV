// Faixas de medalha mantidas pelo Administrador (RN22). Mesmas regras da tela Gerenciar Medalha da Desktop.
const medalhaRepository = require('../repositories/medalhaRepository');
const { ErroValidacao, ErroNaoEncontrado } = require('../utils/erros');
const v = require('../utils/validacoes');
const { urlCompleta } = require('../utils/urls');
const { caminhoDeImagem } = require('./uploadService');

const TAMANHO_MAXIMO_NOME = 50;
const PONTOS_MAXIMOS = 1_000_000;

function paraResposta(m) {
  return {
    id: m.id,
    nome: m.nome,
    pontosMinimos: m.pontosMinimos,
    pontosProximaFaixa: m.pontosProximaFaixa ?? null,
    imagemUrl: m.imagemUrl,
    imagemUrlCompleta: urlCompleta(m.imagemUrl),
    qtdAlunos: m.qtdAlunos ?? undefined
  };
}

async function validar(dados, id) {
  const medalha = {
    nome: v.normalizarTexto(dados.nome),
    pontosMinimos: Number(dados.pontosMinimos),
    imagemUrl: caminhoDeImagem(dados.imagemUrl, 'medalhas')
  };
  const campos = {};
  if (!medalha.nome) campos.nome = 'Informe o nome da medalha (ex.: Bronze).';
  else if (medalha.nome.length > TAMANHO_MAXIMO_NOME) campos.nome = `O nome deve ter no máximo ${TAMANHO_MAXIMO_NOME} caracteres.`;

  if (!Number.isInteger(medalha.pontosMinimos) || medalha.pontosMinimos < 1 || medalha.pontosMinimos > PONTOS_MAXIMOS) {
    campos.pontosMinimos = `Informe a pontuação mínima entre 1 e ${PONTOS_MAXIMOS.toLocaleString('pt-BR')}.`;
  }
  if (!medalha.imagemUrl) {
    campos.imagemUrl = dados.imagemUrl
      ? 'Imagem inválida. Envie a imagem pelo upload ou use uma de assets/medalhas/.'
      : 'Informe a imagem da medalha.';
  }

  // RN27: nome e pontuação mínima não se repetem
  if (!campos.nome && (await medalhaRepository.existeNome(medalha.nome, id))) campos.nome = 'Já existe uma medalha com este nome.';
  if (!campos.pontosMinimos && (await medalhaRepository.existePontosMinimos(medalha.pontosMinimos, id))) {
    campos.pontosMinimos = 'Já existe uma medalha com esta pontuação mínima.';
  }
  ErroValidacao.verificar(campos);
  return medalha;
}

const medalhaService = {
  async listar() {
    const faixas = await medalhaRepository.listar();
    return {
      faixas: faixas.map(paraResposta),
      qtdAlunosSemMedalha: await medalhaRepository.qtdAlunosSemMedalha()
    };
  },

  async consultar(id) {
    const medalha = await medalhaRepository.consultar(id);
    if (!medalha) throw new ErroNaoEncontrado('Medalha não encontrada.');
    return paraResposta(medalha);
  },

  async incluir(dados) {
    const medalha = await validar(dados, null);
    const id = await medalhaRepository.incluir(medalha);
    return { id, mensagem: `Medalha "${medalha.nome}" cadastrada a partir de ${medalha.pontosMinimos.toLocaleString('pt-BR')} pontos.` };
  },

  async editar(id, dados) {
    if (!(await medalhaRepository.consultar(id))) throw new ErroNaoEncontrado('Medalha não encontrada.');
    const medalha = await validar(dados, id);
    await medalhaRepository.editar(id, medalha);
    return { id, mensagem: `Medalha "${medalha.nome}" atualizada.` };
  },

  /**
   * Exclusão física: a faixa não guarda vínculo com nenhum aluno. A pontuação dos alunos não muda;
   * cada um passa a ver a medalha da faixa anterior (ou nenhuma).
   */
  async excluir(id) {
    const faixas = await medalhaRepository.listar();
    const medalha = faixas.find((m) => m.id === id);
    if (!medalha) throw new ErroNaoEncontrado('Medalha não encontrada.');
    await medalhaRepository.excluir(id);
    return {
      mensagem: `Medalha "${medalha.nome}" excluída.`,
      alunosNaFaixa: medalha.qtdAlunos
    };
  }
};

module.exports = medalhaService;
