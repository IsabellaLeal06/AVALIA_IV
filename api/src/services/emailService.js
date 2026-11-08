// Envio de e-mail (RN03) atrás de uma interface única: enviar({ para, assunto, texto }).
//  - modo "log" (padrão): registra a mensagem no console, sem enviar. Suficiente para fins acadêmicos.
//  - modo "smtp": envia com nodemailer, usando as credenciais do .env.
const nodemailer = require('nodemailer');
const ambiente = require('../config/ambiente');

function criarEnviadorLog() {
  return async ({ para, assunto, texto }) => {
    console.log(
      ['', '=== E-MAIL (modo log: não enviado) ===', `Para: ${para}`, `Assunto: ${assunto}`, '', texto, '======================================', ''].join(
        '\n'
      )
    );
  };
}

function criarEnviadorSmtp() {
  const transporte = nodemailer.createTransport({
    host: ambiente.email.host,
    port: ambiente.email.porta,
    secure: ambiente.email.seguro,
    auth: ambiente.email.usuario ? { user: ambiente.email.usuario, pass: ambiente.email.senha } : undefined
  });
  return async ({ para, assunto, texto }) => {
    await transporte.sendMail({ from: ambiente.email.remetente, to: para, subject: assunto, text: texto });
  };
}

let enviador = ambiente.email.modo === 'smtp' ? criarEnviadorSmtp() : criarEnviadorLog();

const emailService = {
  enviar(mensagem) {
    return enviador(mensagem);
  },

  /** Usado pelos testes para capturar as mensagens em vez de enviá-las. */
  definirEnviador(novoEnviador) {
    enviador = novoEnviador;
  }
};

module.exports = emailService;
