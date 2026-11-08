// Ciclo do simulado (RF04, RF05): geração, realização, correção, revisão e histórico.
// Sorteio, correção e pontuação acontecem SOMENTE aqui no servidor (RNF07).
const { emTransacao } = require('../config/banco');
const simuladoRepository = require('../repositories/simuladoRepository');
const disciplinaRepository = require('../repositories/disciplinaRepository');
const { ErroValidacao, ErroNaoEncontrado, ErroConflito } = require('../utils/erros');
const { dataHora, hojeBrasiliaFormatado } = require('../utils/datas');
const { urlCompleta } = require('../utils/urls');

const QUANTIDADES = [10, 15, 20]; // RN14
const MINIMO_DISCIPLINAS = 2; // RN13
const PONTOS_POR_ACERTO = 10; // RN21
const LETRAS = ['A', 'B', 'C', 'D', 'E']; // RN18
const TAMANHO_PAGINA_PADRAO = 10;
const TAMANHO_PAGINA_MAXIMO = 50;

/** Simulado do próprio aluno; de outro aluno responde como inexistente (RN24). */
async function obterDoAluno(id, usuarioToken) {
  const simulado = await simuladoRepository.consultar(id);
  if (!simulado || simulado.idUsuario !== usuarioToken.id) throw new ErroNaoEncontrado('Simulado não encontrado.');
  return simulado;
}

function arredondar(valor) {
  return Math.round(valor * 100) / 100;
}

async function montarResultado(idSimulado, jaFinalizado = false) {
  const resultado = await simuladoRepository.consultarResultado(idSimulado);
  const porDisciplina = await simuladoRepository.resultadoPorDisciplina(idSimulado);
  return {
    idSimulado: resultado.idSimulado,
    titulo: resultado.titulo,
    totalQuestoes: resultado.totalQuestoes,
    totalAcertos: resultado.totalAcertos,
    emBranco: resultado.emBranco,
    totalErros: resultado.totalQuestoes - resultado.totalAcertos - resultado.emBranco,
    percentual: Number(resultado.percentual),
    pontosObtidos: resultado.pontosObtidos,
    pontuacaoTotal: resultado.pontuacaoTotal,
    dataRealizacao: dataHora(resultado.dataRealizacao),
    porDisciplina: porDisciplina.map((d) => ({
      idDisciplina: d.idDisciplina,
      disciplina: d.nomeDisciplina,
      totalQuestoes: d.totalQuestoes,
      totalAcertos: d.totalAcertos,
      percentual: arredondar((100 * d.totalAcertos) / d.totalQuestoes)
    })),
    // Reenvio da finalização (ex.: falha de rede no app): devolve o resultado já apurado (RN16)
    jaFinalizado
  };
}

/** Correção pura (sem banco): usada pela finalização e pelos testes. Em branco conta como erro (RN18). */
function corrigir(gabarito, respostasRecebidas) {
  const marcadas = new Map();
  for (const r of respostasRecebidas) {
    const letra = r.alternativa === null || r.alternativa === undefined || r.alternativa === '' ? null : String(r.alternativa).toUpperCase();
    marcadas.set(Number(r.idQuestao), letra);
  }
  const respostas = gabarito.map((g) => {
    const marcada = marcadas.get(g.idQuestao) ?? null;
    return { idQuestao: g.idQuestao, idDisciplina: g.idDisciplina, marcada, acertou: marcada !== null && marcada === g.correta };
  });
  const totalAcertos = respostas.filter((r) => r.acertou).length;
  const porDisciplina = new Map();
  for (const r of respostas) {
    const atual = porDisciplina.get(r.idDisciplina) || { respostas: 0, acertos: 0 };
    atual.respostas += 1;
    atual.acertos += r.acertou ? 1 : 0;
    porDisciplina.set(r.idDisciplina, atual);
  }
  return {
    respostas,
    totalQuestoes: respostas.length,
    totalAcertos,
    percentual: respostas.length ? arredondar((100 * totalAcertos) / respostas.length) : 0,
    pontosObtidos: totalAcertos * PONTOS_POR_ACERTO,
    porDisciplina
  };
}

function erroDeUnicidade(erro) {
  const numero = erro?.number ?? erro?.originalError?.info?.number;
  return numero === 2627 || numero === 2601;
}

const simuladoService = {
  QUANTIDADES,
  corrigir,

  /** POST /simulados: valida no servidor e sorteia (RN12, RN13, RN14, RN15). */
  async gerar(dados, usuarioToken) {
    const campos = {};
    const lista = Array.isArray(dados.disciplinas) ? dados.disciplinas : [];
    const idsDisciplinas = [...new Set(lista.map(Number))].filter((id) => Number.isInteger(id) && id > 0);
    if (idsDisciplinas.length !== lista.length) campos.disciplinas = 'Lista de disciplinas inválida.';
    else if (idsDisciplinas.length < MINIMO_DISCIPLINAS) campos.disciplinas = 'Selecione duas ou mais disciplinas.';

    const quantidade = Number(dados.quantidade);
    if (!QUANTIDADES.includes(quantidade)) campos.quantidade = 'A quantidade de questões deve ser 10, 15 ou 20.';
    ErroValidacao.verificar(campos);

    const disciplinas = await disciplinaRepository.listarPorIds(idsDisciplinas);
    const inativas = idsDisciplinas.filter((id) => !disciplinas.find((d) => d.id === id && d.ativo));
    if (inativas.length) {
      throw new ErroValidacao('Selecione somente disciplinas ativas.', { disciplinas: 'Há disciplina inexistente ou desativada na seleção.' });
    }

    const disponiveis = await simuladoRepository.contarDisponiveis(idsDisciplinas);
    const total = disponiveis.reduce((soma, d) => soma + d.total, 0);
    if (total < quantidade) {
      const detalhe = disciplinas
        .map((d) => `${d.nome}: ${disponiveis.find((x) => x.idDisciplina === d.id)?.total ?? 0}`)
        .join('; ');
      throw new ErroConflito(
        `As disciplinas escolhidas têm só ${total} questão(ões) disponível(is), menos que as ${quantidade} pedidas. ` +
          `Escolha mais disciplinas ou uma quantidade menor. (${detalhe})`,
        { quantidade: `Disponíveis: ${total}.` }
      );
    }

    const idsQuestoes = await simuladoRepository.sortear(idsDisciplinas, quantidade);
    // RN13: as disciplinas não são gravadas no simulado; saem de Simulado -> SimuladoQuestao -> Questao -> Disciplina
    const titulo = `Simulado de ${quantidade} questões - ${hojeBrasiliaFormatado()}`;
    const criado = await emTransacao((transacao) =>
      simuladoRepository.criar({ titulo, idUsuario: usuarioToken.id, idsQuestoes }, transacao)
    );
    return { id: criado.id, titulo, quantidade, dataCriacao: dataHora(criado.dataCriacao) };
  },

  /** GET /simulados/:id: questões SEM gabarito e SEM explicação. */
  async consultarParaRealizacao(id, usuarioToken) {
    const simulado = await obterDoAluno(id, usuarioToken);
    const resposta = {
      id: simulado.id,
      titulo: simulado.titulo,
      quantidade: simulado.quantidade,
      dataCriacao: dataHora(simulado.dataCriacao),
      finalizado: simulado.finalizado
    };
    // Tentativa única (RN16): depois de finalizado, as questões só aparecem na revisão
    if (simulado.finalizado) return resposta;
    const questoes = await simuladoRepository.questoesParaRealizacao(id);
    resposta.questoes = questoes.map((q) => ({
      ordem: q.ordem,
      id: q.id,
      enunciado: q.enunciado,
      imagemUrl: urlCompleta(q.imagemUrl),
      disciplina: q.nomeDisciplina,
      alternativas: q.alternativas
    }));
    return resposta;
  },

  /**
   * POST /simulados/:id/finalizar. Correção inteiramente no servidor e gravação numa única transação (RNF03):
   * resultado, respostas, desempenho por disciplina e pontuação. O tempo NÃO é recebido nem gravado (RN17).
   */
  async finalizar(id, dados, usuarioToken) {
    const simulado = await obterDoAluno(id, usuarioToken);
    if (simulado.finalizado) return montarResultado(id, true);

    const recebidas = Array.isArray(dados.respostas) ? dados.respostas : [];
    const invalidas = recebidas.filter(
      (r) => r.alternativa !== null && r.alternativa !== undefined && r.alternativa !== '' && !LETRAS.includes(String(r.alternativa).toUpperCase())
    );
    if (invalidas.length) {
      throw new ErroValidacao('As respostas devem usar as letras A, B, C, D ou E, ou ficar em branco.', { respostas: 'Letra inválida.' });
    }

    try {
      await emTransacao(async (transacao) => {
        const gabarito = await simuladoRepository.gabarito(id, transacao);
        const correcao = corrigir(gabarito, recebidas);
        // Primeiro o resultado: o UNIQUE de Id_Simulado barra uma finalização simultânea (RN16)
        await simuladoRepository.inserirResultado(
          { ...correcao, idUsuario: usuarioToken.id, idSimulado: id },
          transacao
        );
        for (const r of correcao.respostas) {
          await simuladoRepository.inserirResposta({ ...r, idUsuario: usuarioToken.id, idSimulado: id }, transacao);
        }
        for (const [idDisciplina, totais] of correcao.porDisciplina) {
          await simuladoRepository.acumularDesempenho({ idUsuario: usuarioToken.id, idDisciplina, ...totais }, transacao);
        }
        if (correcao.pontosObtidos > 0) await simuladoRepository.somarPontuacao(usuarioToken.id, correcao.pontosObtidos, transacao);
      });
    } catch (erro) {
      // Outra requisição finalizou o mesmo simulado ao mesmo tempo: vale o resultado já gravado
      if (erroDeUnicidade(erro)) return montarResultado(id, true);
      throw erro;
    }
    return montarResultado(id, false);
  },

  /** GET /simulados/:id/resultado */
  async resultado(id, usuarioToken) {
    const simulado = await obterDoAluno(id, usuarioToken);
    if (!simulado.finalizado) throw new ErroConflito('Este simulado ainda não foi finalizado.');
    return montarResultado(id, false);
  },

  /** GET /simulados/:id/revisao: gabarito e explicação liberados, só depois da finalização (RF05). */
  async revisao(id, usuarioToken) {
    const simulado = await obterDoAluno(id, usuarioToken);
    if (!simulado.finalizado) throw new ErroConflito('A revisão fica disponível depois que o simulado é finalizado.');
    const questoes = await simuladoRepository.revisao(id);
    const resultado = await simuladoRepository.consultarResultado(id);
    return {
      id: simulado.id,
      titulo: simulado.titulo,
      dataRealizacao: dataHora(resultado.dataRealizacao),
      totalQuestoes: resultado.totalQuestoes,
      totalAcertos: resultado.totalAcertos,
      percentual: Number(resultado.percentual),
      questoes: questoes.map((q) => ({
        ordem: q.ordem,
        id: q.id,
        enunciado: q.enunciado,
        imagemUrl: urlCompleta(q.imagemUrl),
        disciplina: q.nomeDisciplina,
        alternativas: q.alternativas.map(({ letra, texto }) => ({ letra, texto })),
        marcada: q.marcada,
        emBranco: q.marcada === null,
        correta: q.alternativas.find((a) => a.correta)?.letra ?? null,
        acertou: Boolean(q.acertou),
        explicacao: q.explicacao || null
      }))
    };
  },

  /** GET /simulados: histórico paginado do próprio aluno (RN20). */
  async historico(filtros, usuarioToken) {
    const pagina = Math.max(1, Number.parseInt(filtros.pagina, 10) || 1);
    const tamanho = Math.min(TAMANHO_PAGINA_MAXIMO, Math.max(1, Number.parseInt(filtros.tamanho, 10) || TAMANHO_PAGINA_PADRAO));
    const { total, itens } = await simuladoRepository.historico(usuarioToken.id, pagina, tamanho);
    return {
      pagina,
      tamanho,
      total,
      totalPaginas: Math.max(1, Math.ceil(total / tamanho)),
      itens: itens.map((s) => ({
        id: s.id,
        titulo: s.titulo,
        totalQuestoes: s.totalQuestoes,
        totalAcertos: s.totalAcertos,
        percentual: Number(s.percentual),
        pontosObtidos: s.pontosObtidos,
        dataRealizacao: dataHora(s.dataRealizacao)
      }))
    };
  }
};

module.exports = simuladoService;
