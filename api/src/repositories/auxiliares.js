// Auxiliares dos repositórios: montam parâmetros sem nunca concatenar valores no SQL.
const { sql } = require('../config/banco');

/**
 * Lista de ids como parâmetros (@prefixo0, @prefixo1...). Só os NOMES dos parâmetros
 * entram no texto do SQL; os valores seguem como parâmetros tipados.
 */
function listaDeIds(ids, prefixo = 'id') {
  const parametros = {};
  const nomes = ids.map((id, i) => {
    parametros[`${prefixo}${i}`] = [sql.Int, id];
    return `@${prefixo}${i}`;
  });
  return { lista: nomes.length ? nomes.join(', ') : 'NULL', parametros };
}

/** Padrão para LIKE com os curingas do usuário escapados (\%, \_, \[). Usar com ESCAPE '\'. */
function padraoBusca(texto) {
  const valor = String(texto ?? '').trim();
  if (!valor) return null;
  return `%${valor.replace(/[\\%_[]/g, (c) => `\\${c}`)}%`;
}

/**
 * Data e hora atuais no horário de Brasília. As colunas DATETIME guardam o horário local
 * (como nos dados de teste); o Azure SQL roda em UTC, por isso a conversão.
 */
const AGORA = "CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'E. South America Standard Time' AS DATETIME)";

module.exports = { listaDeIds, padraoBusca, AGORA };
