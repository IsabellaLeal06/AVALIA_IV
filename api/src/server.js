// Inicialização da API.
const ambiente = require('./config/ambiente');
const { criarApp } = require('./app');
const banco = require('./config/banco');

// Confere as variáveis obrigatórias antes de abrir a porta
try {
  void ambiente.conexaoBanco;
  void ambiente.segredoJwt;
} catch (erro) {
  console.error(`\n[configuração] ${erro.message}\n`);
  process.exit(1);
}

const servidor = criarApp().listen(ambiente.porta, () => {
  console.log(`Avalia+ API rodando em ${ambiente.urlPublica}`);
  if (ambiente.pastaWeb) console.log(`Aplicação web: ${ambiente.urlPublica}/`);
  console.log(`E-mail em modo "${ambiente.email.modo}". Prazo do token: ${ambiente.prazoToken}.`);
});

async function encerrar() {
  servidor.close();
  await banco.fechar();
  process.exit(0);
}
process.on('SIGINT', encerrar);
process.on('SIGTERM', encerrar);
