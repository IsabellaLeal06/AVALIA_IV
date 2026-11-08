// Erros de negócio. O middleware de erro (middleware/erros.js) transforma cada um na resposta padrão:
// { "erro": { "mensagem": "...", "campos": { "campo": "mensagem" } } }

class ErroAplicacao extends Error {
  constructor(status, mensagem, campos = null) {
    super(mensagem);
    this.status = status;
    this.campos = campos;
  }
}

/** 400: dados inválidos. `campos` indica qual campo corrigir. */
class ErroValidacao extends ErroAplicacao {
  constructor(mensagem, campos = null) {
    super(400, mensagem, campos);
  }

  /** Lança um ErroValidacao se a lista de erros tiver algum campo. */
  static verificar(campos, mensagem = 'Confira os campos destacados.') {
    if (Object.keys(campos).length > 0) throw new ErroValidacao(mensagem, campos);
  }
}

class ErroNaoAutenticado extends ErroAplicacao {
  constructor(mensagem = 'Sessão expirada ou inválida. Entre novamente.') {
    super(401, mensagem);
  }
}

class ErroProibido extends ErroAplicacao {
  constructor(mensagem = 'Você não tem permissão para esta operação.') {
    super(403, mensagem);
  }
}

class ErroNaoEncontrado extends ErroAplicacao {
  constructor(mensagem = 'Registro não encontrado.') {
    super(404, mensagem);
  }
}

/** 409: a operação conflita com o estado atual (duplicidade, registro inativo, regra de negócio). */
class ErroConflito extends ErroAplicacao {
  constructor(mensagem, campos = null) {
    super(409, mensagem, campos);
  }
}

module.exports = {
  ErroAplicacao,
  ErroValidacao,
  ErroNaoAutenticado,
  ErroProibido,
  ErroNaoEncontrado,
  ErroConflito
};
