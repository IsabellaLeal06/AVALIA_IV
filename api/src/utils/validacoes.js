// Validações compartilhadas. Seguem as mesmas regras da aplicação Desktop
// (Avalia.Desktop.Core/Validacoes), para que as duas aceitem e recusem os mesmos dados.
const { ErroNaoEncontrado } = require('./erros');

/** Mantém só os dígitos do CPF (aceita 000.000.000-00). */
function normalizarCpf(cpf) {
  return String(cpf ?? '').replace(/\D/g, '');
}

function calcularDigito(digitos, quantidade) {
  let soma = 0;
  for (let i = 0; i < quantidade; i++) soma += Number(digitos[i]) * (quantidade + 1 - i);
  const resto = (soma * 10) % 11;
  return resto === 10 ? 0 : resto;
}

/** CPF válido pelos dígitos verificadores (módulo 11). */
function cpfValido(cpf) {
  const texto = String(cpf ?? '');
  if (/[^\d.\-\s]/.test(texto)) return false;
  const digitos = normalizarCpf(texto);
  if (digitos.length !== 11) return false;
  // 000.000.000-00, 111.111.111-11... passam no cálculo, mas não são válidos
  if (/^(\d)\1{10}$/.test(digitos)) return false;
  return Number(digitos[9]) === calcularDigito(digitos, 9) && Number(digitos[10]) === calcularDigito(digitos, 10);
}

const TAMANHO_MAXIMO_EMAIL = 150;
const FORMATO_EMAIL =
  /^[a-z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?(?:\.[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?)+$/;

function normalizarEmail(email) {
  return String(email ?? '').trim().toLowerCase();
}

function emailValido(email) {
  const valor = normalizarEmail(email);
  return valor.length > 0 && valor.length <= TAMANHO_MAXIMO_EMAIL && FORMATO_EMAIL.test(valor);
}

const SENHA_TAMANHO_MINIMO = 8;
const SENHA_REGRAS = 'Mínimo de 8 caracteres, com pelo menos uma letra e um número.';

/** Regras da nova senha (RN04). Devolve a lista de problemas encontrados. */
function validarSenha(senha) {
  const valor = String(senha ?? '');
  const erros = [];
  if (valor.length < SENHA_TAMANHO_MINIMO) erros.push(`A senha deve ter pelo menos ${SENHA_TAMANHO_MINIMO} caracteres.`);
  // O BCrypt considera no máximo 72 bytes
  if (Buffer.byteLength(valor, 'utf8') > 72) erros.push('A senha é longa demais.');
  if (!/\p{L}/u.test(valor)) erros.push('A senha deve conter pelo menos uma letra.');
  if (!/\d/.test(valor)) erros.push('A senha deve conter pelo menos um número.');
  if (valor.length > 0 && valor.trim().length !== valor.length) erros.push('A senha não pode começar nem terminar com espaço.');
  return erros;
}

/** Remove espaços repetidos e das pontas. */
function normalizarTexto(texto) {
  return String(texto ?? '')
    .split(/\s+/)
    .filter(Boolean)
    .join(' ');
}

/** Texto longo (enunciado, explicação): mantém quebras de linha, só tira espaços das pontas. */
function textoLongo(texto) {
  return String(texto ?? '').trim();
}

/** Inteiro positivo ou null. */
function idOuNulo(valor) {
  if (valor === null || valor === undefined || valor === '') return null;
  const numero = Number(valor);
  return Number.isInteger(numero) && numero > 0 ? numero : NaN;
}

/** Converte o :id da rota em inteiro positivo ou lança 404. */
function idDaRota(valor) {
  const numero = Number(valor);
  if (!Number.isInteger(numero) || numero <= 0 || numero > 2147483647) throw new ErroNaoEncontrado();
  return numero;
}

/** Filtro de situação das listagens: ativo (padrão), inativo ou todos. */
function filtroSituacao(valor, padrao = 'ativo') {
  const texto = String(valor ?? padrao).toLowerCase();
  if (texto === 'ativo' || texto === 'ativos') return true;
  if (texto === 'inativo' || texto === 'inativos') return false;
  return null;
}

/** Converte "ativo" do corpo (true/false) em booleano, ou lança erro. */
function booleano(valor) {
  if (valor === true || valor === 'true' || valor === 1 || valor === '1') return true;
  if (valor === false || valor === 'false' || valor === 0 || valor === '0') return false;
  return null;
}

module.exports = {
  normalizarCpf,
  cpfValido,
  normalizarEmail,
  emailValido,
  validarSenha,
  SENHA_REGRAS,
  normalizarTexto,
  textoLongo,
  idOuNulo,
  idDaRota,
  filtroSituacao,
  booleano
};
