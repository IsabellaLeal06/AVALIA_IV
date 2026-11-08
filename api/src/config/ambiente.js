// Lê as variáveis do arquivo .env (fora do Git). Nenhuma credencial fica no código.
const path = require('path');
require('dotenv').config({ path: path.join(__dirname, '..', '..', '.env') });

function obrigatoria(nome) {
  const valor = process.env[nome];
  if (!valor || !valor.trim()) {
    throw new Error(`A variável ${nome} não foi definida. Copie o .env.example para .env e preencha os valores.`);
  }
  return valor.trim();
}

function opcional(nome, padrao) {
  const valor = process.env[nome];
  return valor && valor.trim() ? valor.trim() : padrao;
}

const raizApi = path.join(__dirname, '..', '..');

const ambiente = {
  porta: Number(opcional('PORTA', '3000')),
  // Endereço público da API, usado para montar o endereço completo das imagens (o app móvel não resolve caminho relativo)
  urlPublica: opcional('URL_PUBLICA', `http://localhost:${opcional('PORTA', '3000')}`).replace(/\/+$/, ''),
  // Origens do navegador autorizadas a chamar a API (separadas por vírgula). Vazio = só a própria API.
  origensPermitidas: opcional('ORIGENS_PERMITIDAS', '')
    .split(',')
    .map((o) => o.trim())
    .filter(Boolean),

  get conexaoBanco() {
    const conexao = obrigatoria('DB_CONEXAO');
    if (!/password\s*=/i.test(conexao) || !/(initial catalog|database)\s*=/i.test(conexao)) {
      throw new Error('DB_CONEXAO incompleta. Coloque a string de conexão entre aspas no .env (uma senha com # corta o valor).');
    }
    return conexao;
  },

  get segredoJwt() {
    const segredo = obrigatoria('JWT_SEGREDO');
    if (segredo.length < 32) throw new Error('JWT_SEGREDO deve ter pelo menos 32 caracteres.');
    return segredo;
  },
  // Prazo único do token (RF01). 7 dias atende também o aplicativo móvel.
  prazoToken: opcional('JWT_PRAZO', '7d'),

  // Recuperação de senha (RN03)
  validadeCodigoMinutos: Number(opcional('CODIGO_VALIDADE_MINUTOS', '15')),

  // E-mail: "log" (padrão) registra no console; "smtp" envia de verdade
  email: {
    modo: opcional('EMAIL_MODO', 'log'),
    host: opcional('SMTP_HOST', ''),
    porta: Number(opcional('SMTP_PORTA', '587')),
    seguro: opcional('SMTP_SEGURO', 'false') === 'true',
    usuario: opcional('SMTP_USUARIO', ''),
    senha: opcional('SMTP_SENHA', ''),
    remetente: opcional('EMAIL_REMETENTE', 'Avalia+ <nao-responda@avaliamais.test>')
  },

  // Imagens enviadas (questões e medalhas)
  pastaUploads: path.resolve(raizApi, opcional('PASTA_UPLOADS', 'uploads')),

  // Pasta da aplicação web servida pela própria API (vazio = não servir)
  pastaWeb: opcional('PASTA_WEB', '../web') ? path.resolve(raizApi, opcional('PASTA_WEB', '../web')) : null
};

module.exports = ambiente;
