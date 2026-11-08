// Autenticação (RF01), troca de senha (RN04) e recuperação por código (RN03).
const crypto = require('crypto');
const bcrypt = require('bcryptjs');
const jwt = require('jsonwebtoken');
const ambiente = require('../config/ambiente');
const usuarioRepository = require('../repositories/usuarioRepository');
const emailService = require('./emailService');
const { ErroValidacao, ErroNaoAutenticado, ErroConflito } = require('../utils/erros');
const { normalizarEmail, validarSenha } = require('../utils/validacoes');
const { NOME_PERFIL } = require('../utils/perfis');

// Mesmo custo da Desktop (BCrypt.Net-Next): os hashes das duas aplicações são compatíveis
const CUSTO_BCRYPT = 10;
const MENSAGEM_FALHA_LOGIN = 'E-mail ou senha inválidos.';
const MENSAGEM_CODIGO_INVALIDO = 'Código inválido ou expirado. Solicite um novo código.';
const TENTATIVAS_CODIGO = 5;

// Hash de uma senha qualquer, comparado quando o e-mail não existe: a recusa leva o mesmo tempo
// e não revela se o e-mail está cadastrado
const HASH_FICTICIO = bcrypt.hashSync(crypto.randomUUID(), CUSTO_BCRYPT);

// Tentativas erradas de código por e-mail (em memória). Ao passar do limite, o código é invalidado.
const tentativasCodigo = new Map();

function gerarHashSenha(senha) {
  return bcrypt.hash(senha, CUSTO_BCRYPT);
}

function gerarToken(usuario) {
  return jwt.sign({ idUsuario: usuario.id, idPerfil: usuario.idPerfil }, ambiente.segredoJwt, {
    expiresIn: ambiente.prazoToken
  });
}

function verificarToken(token) {
  try {
    const conteudo = jwt.verify(token, ambiente.segredoJwt);
    return { id: conteudo.idUsuario, idPerfil: conteudo.idPerfil };
  } catch {
    throw new ErroNaoAutenticado();
  }
}

function hashCodigo(idUsuario, codigo) {
  return crypto.createHmac('sha256', ambiente.segredoJwt).update(`${idUsuario}:${codigo}`).digest('hex');
}

function codigosIguais(a, b) {
  const bufferA = Buffer.from(String(a ?? ''));
  const bufferB = Buffer.from(String(b ?? ''));
  return bufferA.length === bufferB.length && crypto.timingSafeEqual(bufferA, bufferB);
}

/** Confere o código. Devolve o id do usuário ou lança a mesma mensagem genérica para qualquer falha. */
async function conferirCodigo(email, codigo) {
  const emailNormalizado = normalizarEmail(email);
  const codigoLimpo = String(codigo ?? '').replace(/\D/g, '');
  const registro = emailNormalizado ? await usuarioRepository.consultarRecuperacao(emailNormalizado) : null;

  const valido =
    registro &&
    registro.ativo &&
    registro.codigoHash &&
    !registro.expirado &&
    codigoLimpo.length === 6 &&
    codigosIguais(registro.codigoHash, hashCodigo(registro.id, codigoLimpo));

  if (!valido) {
    if (registro?.codigoHash) {
      const tentativas = (tentativasCodigo.get(emailNormalizado) || 0) + 1;
      tentativasCodigo.set(emailNormalizado, tentativas);
      if (tentativas >= TENTATIVAS_CODIGO) {
        tentativasCodigo.delete(emailNormalizado);
        await usuarioRepository.limparCodigoRecuperacao(registro.id);
      }
    }
    throw new ErroValidacao(MENSAGEM_CODIGO_INVALIDO, { codigo: MENSAGEM_CODIGO_INVALIDO });
  }
  return registro.id;
}

function validarNovaSenha(novaSenha, confirmacao, campos) {
  const errosSenha = validarSenha(novaSenha);
  if (errosSenha.length) campos.novaSenha = errosSenha.join(' ');
  if (String(novaSenha ?? '') !== String(confirmacao ?? '')) campos.confirmacao = 'A confirmação não é igual à nova senha.';
}

const authService = {
  gerarToken,
  verificarToken,
  gerarHashSenha,
  MENSAGEM_FALHA_LOGIN,

  /** POST /auth/login. Mensagem genérica para e-mail inexistente, senha errada ou conta desativada. */
  async login({ email, senha }) {
    if (!String(email ?? '').trim() || !senha) throw new ErroValidacao('Informe o e-mail e a senha.');

    const usuario = await usuarioRepository.consultarParaLogin(normalizarEmail(email));
    if (!usuario) {
      await bcrypt.compare(String(senha), HASH_FICTICIO);
      throw new ErroNaoAutenticado(MENSAGEM_FALHA_LOGIN);
    }

    const senhaConfere = await bcrypt.compare(String(senha), usuario.senhaHash);
    if (!senhaConfere || !usuario.ativo) throw new ErroNaoAutenticado(MENSAGEM_FALHA_LOGIN);

    return {
      token: gerarToken(usuario),
      prazo: ambiente.prazoToken,
      usuario: {
        id: usuario.id,
        nome: usuario.nome,
        email: usuario.email,
        idPerfil: usuario.idPerfil,
        perfil: NOME_PERFIL[usuario.idPerfil]
      },
      // RN02: a troca da senha inicial é recomendada, não obrigatória
      senhaInicial: String(senha) === usuario.cpf
    };
  },

  /** POST /auth/alterar-senha: sempre sobre o usuário do token (RN04). */
  async alterarSenha(usuarioToken, { senhaAtual, novaSenha, confirmacao }) {
    const campos = {};
    if (!senhaAtual) campos.senhaAtual = 'Informe a senha atual.';
    validarNovaSenha(novaSenha, confirmacao, campos);
    ErroValidacao.verificar(campos);

    const registro = await usuarioRepository.obterHashSenha(usuarioToken.id);
    if (!registro || !(await bcrypt.compare(String(senhaAtual), registro.senhaHash))) {
      throw new ErroValidacao('A senha atual não confere.', { senhaAtual: 'A senha atual não confere.' });
    }
    // Como a nova senha precisa ter letra, ela nunca será o CPF
    if (String(novaSenha) === String(senhaAtual)) {
      throw new ErroValidacao('A nova senha deve ser diferente da senha atual.', {
        novaSenha: 'A nova senha deve ser diferente da senha atual.'
      });
    }

    await usuarioRepository.alterarSenha(usuarioToken.id, await gerarHashSenha(String(novaSenha)));
    return { mensagem: 'Senha alterada com sucesso.' };
  },

  /**
   * POST /auth/recuperar-senha. A resposta é a mesma exista ou não o e-mail.
   * Uma solicitação nova sobrescreve a anterior (o código antigo deixa de valer).
   */
  async recuperarSenha({ email }) {
    const emailNormalizado = normalizarEmail(email);
    const resposta = {
      mensagem: 'Se o e-mail estiver cadastrado, você receberá um código para criar uma nova senha.',
      validadeMinutos: ambiente.validadeCodigoMinutos
    };
    if (!emailNormalizado) throw new ErroValidacao('Informe o e-mail cadastrado.', { email: 'Informe o e-mail cadastrado.' });

    const registro = await usuarioRepository.consultarRecuperacao(emailNormalizado);
    if (!registro || !registro.ativo) return resposta;

    const codigo = String(crypto.randomInt(0, 1_000_000)).padStart(6, '0');
    await usuarioRepository.gravarCodigoRecuperacao(registro.id, hashCodigo(registro.id, codigo), ambiente.validadeCodigoMinutos);
    tentativasCodigo.delete(emailNormalizado);

    // O envio não atrasa a resposta: o tempo não revela se o e-mail existe
    emailService
      .enviar({
        para: emailNormalizado,
        assunto: 'Avalia+ | Código para criar uma nova senha',
        texto:
          `Seu código de recuperação de senha do Avalia+ é: ${codigo}\n\n` +
          `Ele vale por ${ambiente.validadeCodigoMinutos} minutos e só pode ser usado uma vez.\n` +
          'Se você não pediu a troca da senha, ignore este e-mail.'
      })
      .catch((erro) => console.error('[e-mail] Falha ao enviar o código de recuperação:', erro.message));

    return resposta;
  },

  /** GET /auth/validar-codigo: confirma o código antes de a tela mostrar os campos da nova senha. */
  async validarCodigo({ email, codigo }) {
    await conferirCodigo(email, codigo);
    return { valido: true };
  },

  /** POST /auth/nova-senha: troca a senha e limpa o código (uso único). */
  async novaSenha({ email, codigo, novaSenha, confirmacao }) {
    const idUsuario = await conferirCodigo(email, codigo);
    const campos = {};
    validarNovaSenha(novaSenha, confirmacao, campos);
    ErroValidacao.verificar(campos);

    const registro = await usuarioRepository.obterHashSenha(idUsuario);
    if (String(novaSenha) === registro.cpf) {
      throw new ErroConflito('A nova senha não pode ser o CPF.', { novaSenha: 'A nova senha não pode ser o CPF.' });
    }
    await usuarioRepository.alterarSenha(idUsuario, await gerarHashSenha(String(novaSenha)));
    tentativasCodigo.delete(normalizarEmail(email));
    return { mensagem: 'Senha alterada com sucesso. Entre com a nova senha.' };
  }
};

module.exports = authService;
