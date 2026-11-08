// Autenticação e autorização (RF01). A verificação acontece no servidor a cada requisição.
//
// Proteção por padrão: toda ação passa por `acao()`, que recusa a requisição se nenhuma regra
// de acesso (`permitir` ou `publico`) tiver sido aplicada antes. Uma rota nova esquecida sem
// regra nasce bloqueada, nunca aberta.
const authService = require('../services/authService');
const usuarioRepository = require('../repositories/usuarioRepository');
const { ErroNaoAutenticado, ErroProibido } = require('../utils/erros');

// A situação da conta é conferida no banco, com um cache curto: conta desativada ou com perfil
// alterado perde o acesso em até 30 segundos, sem esperar o token expirar
const CACHE_MS = 30_000;
const cacheSituacao = new Map();

async function situacaoAtual(idUsuario) {
  const emCache = cacheSituacao.get(idUsuario);
  if (emCache && emCache.ate > Date.now()) return emCache.situacao;
  const situacao = await usuarioRepository.consultarSituacao(idUsuario);
  cacheSituacao.set(idUsuario, { situacao, ate: Date.now() + CACHE_MS });
  return situacao;
}

/** Exige token válido. O usuário e o perfil vêm SEMPRE do token, nunca do cliente. */
async function autenticar(req, res, next) {
  try {
    const [tipo, token] = String(req.headers.authorization || '').split(' ');
    if (tipo !== 'Bearer' || !token) throw new ErroNaoAutenticado('Faça login para continuar.');

    const usuario = authService.verificarToken(token);
    const situacao = await situacaoAtual(usuario.id);
    if (!situacao || !situacao.ativo || situacao.idPerfil !== usuario.idPerfil) {
      throw new ErroNaoAutenticado('Sua sessão não é mais válida. Entre novamente.');
    }
    req.usuario = usuario;
    next();
  } catch (erro) {
    next(erro);
  }
}

/** Libera a rota só para os perfis informados. */
function permitir(...perfis) {
  return (req, res, next) => {
    if (!req.usuario) return next(new ErroNaoAutenticado('Faça login para continuar.'));
    if (!perfis.includes(req.usuario.idPerfil)) return next(new ErroProibido());
    req.autorizado = true;
    next();
  };
}

/** Rota aberta (login e recuperação de senha). */
function publico(req, res, next) {
  req.autorizado = true;
  next();
}

/** Envolve o controller: repassa erros ao tratador e barra rotas sem regra de acesso. */
function acao(controller) {
  return async (req, res, next) => {
    try {
      if (!req.autorizado) throw new ErroProibido();
      await controller(req, res);
    } catch (erro) {
      next(erro);
    }
  };
}

/** Usado pelos testes e após alterações de situação feitas na própria API. */
function limparCacheSituacao(idUsuario) {
  if (idUsuario === undefined) cacheSituacao.clear();
  else cacheSituacao.delete(idUsuario);
}

module.exports = { autenticar, permitir, publico, acao, limparCacheSituacao };
