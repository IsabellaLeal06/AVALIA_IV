// Montagem da aplicação Express (separada do server.js para os testes usarem sem abrir porta).
const fs = require('fs');
const express = require('express');
const cors = require('cors');
const ambiente = require('./config/ambiente');
const { rotasPublicas, rotasProtegidas } = require('./routes');
const { tratarErros, rotaNaoEncontrada } = require('./middleware/erros');

function criarApp() {
  const app = express();
  app.disable('x-powered-by');
  app.set('trust proxy', 1);

  // A web servida pela própria API dispensa CORS; outras origens (ex.: Live Server) vêm do .env
  app.use(cors({ origin: ambiente.origensPermitidas.length ? ambiente.origensPermitidas : false }));
  app.use(express.json({ limit: '1mb' }));

  // Imagens enviadas (questões e medalhas). Arquivos públicos, com nome gerado pelo servidor.
  app.use('/uploads', express.static(ambiente.pastaUploads, { fallthrough: false, index: false, dotfiles: 'deny' }));

  // Aplicação web (HTML, CSS e JS), quando a pasta existe
  if (ambiente.pastaWeb && fs.existsSync(ambiente.pastaWeb)) {
    app.use(express.static(ambiente.pastaWeb, { extensions: ['html'] }));
  }

  app.get('/saude', (req, res) => res.json({ situacao: 'ok' }));

  app.use(rotasPublicas());
  app.use(rotasProtegidas());

  app.use(rotaNaoEncontrada);
  app.use(tratarErros);
  return app;
}

module.exports = { criarApp };
