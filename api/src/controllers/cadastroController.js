// Entrada e saída HTTP dos cadastros institucionais (Administrador): usuários, turmas, disciplinas,
// medalhas e painel. Sem SQL e sem regra: cada método só repassa ao service.
const usuarioService = require('../services/usuarioService');
const turmaService = require('../services/turmaService');
const disciplinaService = require('../services/disciplinaService');
const medalhaService = require('../services/medalhaService');
const painelService = require('../services/painelService');
const { idDaRota, booleano } = require('../utils/validacoes');

const corpo = (req) => req.body || {};

const cadastroController = {
  // Painel (RF02)
  async painel(req, res) {
    res.json(await painelService.contar());
  },

  // Usuários
  async listarUsuarios(req, res) {
    res.json(await usuarioService.listar(req.query));
  },
  async consultarUsuario(req, res) {
    res.json(await usuarioService.consultar(idDaRota(req.params.id)));
  },
  async incluirUsuario(req, res) {
    res.status(201).json(await usuarioService.incluir(corpo(req), req.usuario));
  },
  async editarUsuario(req, res) {
    res.json(await usuarioService.editar(idDaRota(req.params.id), corpo(req), req.usuario));
  },
  async situacaoUsuario(req, res) {
    res.json(await usuarioService.alterarSituacao(idDaRota(req.params.id), booleano(corpo(req).ativo), req.usuario));
  },

  // Turmas
  async listarTurmas(req, res) {
    res.json(await turmaService.listar(req.query));
  },
  async consultarTurma(req, res) {
    res.json(await turmaService.consultar(idDaRota(req.params.id)));
  },
  async incluirTurma(req, res) {
    res.status(201).json(await turmaService.incluir(corpo(req)));
  },
  async editarTurma(req, res) {
    res.json(await turmaService.editar(idDaRota(req.params.id), corpo(req)));
  },
  async situacaoTurma(req, res) {
    res.json(await turmaService.alterarSituacao(idDaRota(req.params.id), booleano(corpo(req).ativo)));
  },
  async vincularDisciplina(req, res) {
    res.status(201).json(await turmaService.vincularDisciplina(idDaRota(req.params.id), corpo(req).idDisciplina));
  },
  async desvincularDisciplina(req, res) {
    res.json(await turmaService.desvincularDisciplina(idDaRota(req.params.id), idDaRota(req.params.idDisciplina)));
  },

  // Disciplinas
  async listarDisciplinas(req, res) {
    res.json(await disciplinaService.listar(req.query, req.usuario));
  },
  async listarDisciplinasAtivas(req, res) {
    res.json(await disciplinaService.listarAtivas());
  },
  async consultarDisciplina(req, res) {
    res.json(await disciplinaService.consultar(idDaRota(req.params.id)));
  },
  async incluirDisciplina(req, res) {
    res.status(201).json(await disciplinaService.incluir(corpo(req)));
  },
  async editarDisciplina(req, res) {
    res.json(await disciplinaService.editar(idDaRota(req.params.id), corpo(req)));
  },
  async situacaoDisciplina(req, res) {
    res.json(await disciplinaService.alterarSituacao(idDaRota(req.params.id), booleano(corpo(req).ativo)));
  },

  // Medalhas (RN22)
  async listarMedalhas(req, res) {
    res.json(await medalhaService.listar());
  },
  async consultarMedalha(req, res) {
    res.json(await medalhaService.consultar(idDaRota(req.params.id)));
  },
  async incluirMedalha(req, res) {
    res.status(201).json(await medalhaService.incluir(corpo(req)));
  },
  async editarMedalha(req, res) {
    res.json(await medalhaService.editar(idDaRota(req.params.id), corpo(req)));
  },
  async excluirMedalha(req, res) {
    res.json(await medalhaService.excluir(idDaRota(req.params.id)));
  }
};

module.exports = cadastroController;
