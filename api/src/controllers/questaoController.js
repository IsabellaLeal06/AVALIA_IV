// Entrada e saída HTTP do banco de questões e do upload de imagens.
const questaoService = require('../services/questaoService');
const uploadService = require('../services/uploadService');
const { idDaRota, booleano } = require('../utils/validacoes');
const { ErroProibido } = require('../utils/erros');
const { PERFIL } = require('../utils/perfis');

const questaoController = {
  async listar(req, res) {
    res.json(await questaoService.listar(req.query, req.usuario));
  },
  async consultar(req, res) {
    res.json(await questaoService.consultar(idDaRota(req.params.id), req.usuario));
  },
  async incluir(req, res) {
    res.status(201).json(await questaoService.incluir(req.body || {}, req.usuario));
  },
  async editar(req, res) {
    res.json(await questaoService.editar(idDaRota(req.params.id), req.body || {}, req.usuario));
  },
  async situacao(req, res) {
    res.json(await questaoService.alterarSituacao(idDaRota(req.params.id), booleano((req.body || {}).ativo), req.usuario));
  },

  /** POST /upload/imagem?tipo=questoes|medalhas (multipart, campo "imagem"). */
  async uploadImagem(req, res) {
    const tipo = req.query.tipo === 'medalhas' ? 'medalhas' : 'questoes';
    if (tipo === 'medalhas' && req.usuario.idPerfil !== PERFIL.ADMINISTRADOR) {
      throw new ErroProibido('Somente o Administrador envia imagens de medalha.');
    }
    res.status(201).json(await uploadService.salvarImagem(req.file, tipo));
  }
};

module.exports = questaoController;
