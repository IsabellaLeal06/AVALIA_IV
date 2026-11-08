// Tratamento de erros num ponto só (RNF02). O detalhe técnico vai para o log;
// o cliente recebe só uma mensagem compreensível, no formato padrão:
// { "erro": { "mensagem": "...", "campos": { ... } } }
// Pilha de chamadas, nome de tabela e erro do banco nunca chegam ao cliente.
const crypto = require('crypto');
const multer = require('multer');
const { ErroAplicacao } = require('../utils/erros');

function responder(res, status, mensagem, campos = null) {
  res.status(status).json({ erro: campos ? { mensagem, campos } : { mensagem } });
}

function numeroErroBanco(erro) {
  return erro?.number ?? erro?.originalError?.info?.number ?? null;
}

// eslint-disable-next-line no-unused-vars
function tratarErros(erro, req, res, next) {
  if (erro instanceof ErroAplicacao) return responder(res, erro.status, erro.message, erro.campos);

  if (erro instanceof multer.MulterError) {
    const mensagem = erro.code === 'LIMIT_FILE_SIZE' ? 'A imagem deve ter no máximo 2 MB.' : 'Envio de arquivo inválido.';
    return responder(res, 400, mensagem, { imagem: mensagem });
  }

  // Arquivo de /uploads inexistente (express.static)
  if (erro?.statusCode === 404 || erro?.status === 404) return responder(res, 404, 'Arquivo não encontrado.');
  if (erro?.statusCode === 403 || erro?.status === 403) return responder(res, 403, 'Acesso negado.');

  if (erro?.type === 'entity.parse.failed') return responder(res, 400, 'O corpo da requisição não é um JSON válido.');
  if (erro?.type === 'entity.too.large') return responder(res, 413, 'O corpo da requisição é grande demais.');

  const numero = numeroErroBanco(erro);
  if (numero === 2627 || numero === 2601) return responder(res, 409, 'Já existe um registro com estes dados.');
  if (numero === 547) return responder(res, 409, 'A operação foi recusada por uma regra de integridade do banco.');

  const id = crypto.randomUUID().slice(0, 8);
  console.error(`[erro ${id}] ${req.method} ${req.originalUrl}`, erro);

  const indisponivel =
    ['ConnectionError', 'TimeoutError'].includes(erro?.name) ||
    ['ESOCKET', 'ETIMEOUT', 'ELOGIN', 'ECONNCLOSED', 'EINSTLOOKUP'].includes(erro?.code) ||
    [40613, 40197, 40501, 40615, 49918, 49919, 49920].includes(numero);
  if (indisponivel) {
    return responder(
      res,
      503,
      'O banco de dados está indisponível no momento (ele pode estar saindo da pausa automática). Tente novamente em instantes.'
    );
  }
  return responder(res, 500, `Não foi possível concluir a operação. Tente novamente. (código ${id})`);
}

function rotaNaoEncontrada(req, res) {
  responder(res, 404, 'Rota não encontrada.');
}

module.exports = { tratarErros, rotaNaoEncontrada };
