// Entrada e saída HTTP do ciclo do simulado, do desempenho e do perfil.
const simuladoService = require('../services/simuladoService');
const desempenhoService = require('../services/desempenhoService');
const perfilService = require('../services/perfilService');
const { idDaRota } = require('../utils/validacoes');

const alunoController = {
  // Simulados (Aluno)
  async gerarSimulado(req, res) {
    res.status(201).json(await simuladoService.gerar(req.body || {}, req.usuario));
  },
  async consultarSimulado(req, res) {
    res.json(await simuladoService.consultarParaRealizacao(idDaRota(req.params.id), req.usuario));
  },
  async finalizarSimulado(req, res) {
    res.json(await simuladoService.finalizar(idDaRota(req.params.id), req.body || {}, req.usuario));
  },
  async resultadoSimulado(req, res) {
    res.json(await simuladoService.resultado(idDaRota(req.params.id), req.usuario));
  },
  async revisaoSimulado(req, res) {
    res.json(await simuladoService.revisao(idDaRota(req.params.id), req.usuario));
  },
  async historico(req, res) {
    res.json(await simuladoService.historico(req.query, req.usuario));
  },

  // Desempenho
  async desempenhoAluno(req, res) {
    res.json(await desempenhoService.doAluno(req.usuario));
  },
  async turmasProfessor(req, res) {
    res.json(await desempenhoService.turmasDoProfessor(req.usuario));
  },
  async turmaProfessor(req, res) {
    res.json(await desempenhoService.turma(req.usuario, idDaRota(req.params.id)));
  },
  async alunosDaTurma(req, res) {
    res.json(await desempenhoService.alunosDaTurma(req.usuario, idDaRota(req.params.id)));
  },
  async alunoParaProfessor(req, res) {
    res.json(await desempenhoService.alunoParaProfessor(req.usuario, idDaRota(req.params.id), idDaRota(req.params.idAluno)));
  },
  async questoesComMaisErros(req, res) {
    res.json(await desempenhoService.questoesComMaisErros(req.usuario, req.query));
  },

  // Perfil e gamificação
  async perfil(req, res) {
    res.json(await perfilService.consultar(req.usuario));
  }
};

module.exports = alunoController;
