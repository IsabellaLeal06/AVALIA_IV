// Entrada e saída HTTP da autenticação. Sem SQL e sem regra: tudo fica no service.
const authService = require('../services/authService');

const authController = {
  async login(req, res) {
    res.json(await authService.login(req.body || {}));
  },

  async alterarSenha(req, res) {
    res.json(await authService.alterarSenha(req.usuario, req.body || {}));
  },

  async recuperarSenha(req, res) {
    res.json(await authService.recuperarSenha(req.body || {}));
  },

  async validarCodigo(req, res) {
    res.json(await authService.validarCodigo({ email: req.query.email, codigo: req.query.codigo }));
  },

  async novaSenha(req, res) {
    res.json(await authService.novaSenha(req.body || {}));
  }
};

module.exports = authController;
