// As colunas DATETIME guardam o horário de Brasília sem fuso. O driver as entrega como Date em UTC
// com os mesmos números; aqui elas viram texto sem fuso ("2026-10-14T19:30:00"), lido como horário local.

function dataHora(valor) {
  if (!valor) return null;
  return (valor instanceof Date ? valor : new Date(valor)).toISOString().slice(0, 19);
}

function data(valor) {
  if (!valor) return null;
  return (valor instanceof Date ? valor : new Date(valor)).toISOString().slice(0, 10);
}

/** Hoje no horário de Brasília, como "AAAA-MM-DD". */
function hojeBrasilia() {
  return new Intl.DateTimeFormat('en-CA', { timeZone: 'America/Sao_Paulo' }).format(new Date());
}

/** "DD/MM/AAAA" no horário de Brasília. */
function hojeBrasiliaFormatado() {
  return new Intl.DateTimeFormat('pt-BR', { timeZone: 'America/Sao_Paulo' }).format(new Date());
}

module.exports = { dataHora, data, hojeBrasilia, hojeBrasiliaFormatado };
