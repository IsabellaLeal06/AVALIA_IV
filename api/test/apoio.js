// Apoio aos testes. Os testes de integração rodam SOMENTE no banco de TESTE informado em
// DB_CONEXAO_TESTE (no .env ou no ambiente), carregado com os scripts 01 a 04 da pasta database.
// Eles criam simulados, trocam senhas e desativam registros: nunca aponte para o banco de produção.
const fs = require('fs');
const path = require('path');
const dotenv = require('dotenv');

const arquivoEnv = path.join(__dirname, '..', '.env');
const valores = fs.existsSync(arquivoEnv) ? dotenv.parse(fs.readFileSync(arquivoEnv)) : {};
const conexaoTeste = process.env.DB_CONEXAO_TESTE || valores.DB_CONEXAO_TESTE || '';

// Antes de carregar a API: aponta para o banco de teste e desliga o que não interessa aos testes
process.env.DB_CONEXAO = conexaoTeste || 'Server=nenhum;Database=nenhum;User ID=x;Password=x';
process.env.JWT_SEGREDO = process.env.JWT_SEGREDO || valores.JWT_SEGREDO || 'segredo-somente-para-os-testes-0123456789abcdef';
process.env.PASTA_WEB = '';
process.env.EMAIL_MODO = 'log';
process.env.PASTA_UPLOADS = path.join(require('os').tmpdir(), 'avalia-testes-uploads');

const temBanco = Boolean(conexaoTeste);
const motivoPulo = temBanco ? false : 'defina DB_CONEXAO_TESTE (banco de teste com os scripts 01 a 04) para rodar os testes de integração';

const { criarApp } = require('../src/app');
const banco = require('../src/config/banco');
const emailService = require('../src/services/emailService');

let servidor = null;
let base = '';

async function iniciar() {
  if (servidor) return;
  await new Promise((resolve) => {
    servidor = criarApp().listen(0, () => {
      base = `http://127.0.0.1:${servidor.address().port}`;
      resolve();
    });
  });
}

async function encerrar() {
  if (servidor) await new Promise((resolve) => servidor.close(resolve));
  servidor = null;
  await banco.fechar();
}

/** Chamada HTTP à API. Devolve { status, corpo }. */
async function chamar(metodo, rota, { token, corpo } = {}) {
  const resposta = await fetch(base + rota, {
    method: metodo,
    headers: {
      ...(corpo !== undefined ? { 'Content-Type': 'application/json' } : {}),
      ...(token ? { Authorization: `Bearer ${token}` } : {})
    },
    body: corpo !== undefined ? JSON.stringify(corpo) : undefined
  });
  const texto = await resposta.text();
  return { status: resposta.status, corpo: texto ? JSON.parse(texto) : null };
}

// Usuários do 04_dados_teste.sql (senha inicial = CPF)
const USUARIOS = {
  admin: { id: 1, email: 'admin@avaliamais.test', cpf: '15881399803' },
  ricardo: { id: 2, email: 'ricardo.monteiro@avaliamais.test', cpf: '69879730917' }, // disciplinas 1, 2 e 3
  patricia: { id: 3, email: 'patricia.vieira@avaliamais.test', cpf: '11417075350' }, // disciplinas 4 e 5
  marcos: { id: 4, email: 'marcos.pinto@avaliamais.test', cpf: '64556815177' }, // disciplinas 6 e 7
  ana: { id: 5, email: 'ana.souza@aluno.avaliamais.test', cpf: '88447294072' }, // turma 1, tem o simulado 1
  bruno: { id: 6, email: 'bruno.lima@aluno.avaliamais.test', cpf: '55761698757' },
  camila: { id: 7, email: 'camila.rocha@aluno.avaliamais.test', cpf: '16897684600' },
  daniel: { id: 8, email: 'daniel.pereira@aluno.avaliamais.test', cpf: '97880932054' },
  gabriela: { id: 11, email: 'gabriela.carvalho@aluno.avaliamais.test', cpf: '76634400210' }, // desativada
  henrique: { id: 12, email: 'henrique.barbosa@aluno.avaliamais.test', cpf: '97875347718' } // turma 2
};

const tokens = new Map();

/** Token do usuário (login com a senha inicial = CPF). */
async function entrar(nome) {
  if (tokens.has(nome)) return tokens.get(nome);
  const { email, cpf } = USUARIOS[nome];
  const { status, corpo } = await chamar('POST', '/auth/login', { corpo: { email, senha: cpf } });
  if (status !== 200) throw new Error(`Login de ${nome} falhou: ${JSON.stringify(corpo)}`);
  tokens.set(nome, corpo.token);
  return corpo.token;
}

/** Consulta direta ao banco de teste (para preparar e conferir dados). */
function sql(comando, parametros) {
  return banco.consultar(comando, parametros);
}

module.exports = { temBanco, motivoPulo, iniciar, encerrar, chamar, entrar, sql, USUARIOS, banco, emailService };
