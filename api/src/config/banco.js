// Conexão com o SQL Server (Azure). Toda consulta passa por aqui, sempre parametrizada (RNF01).
const sql = require('mssql');
const ambiente = require('./ambiente');

let poolPromessa = null;

function obterPool() {
  if (!poolPromessa) {
    // Aceita a mesma string de conexão do portal do Azure (formato ADO.NET) usada pela Desktop
    const config = sql.ConnectionPool.parseConnectionString(ambiente.conexaoBanco);
    poolPromessa = new sql.ConnectionPool({
      ...config,
      options: { ...config.options, enableArithAbort: true },
      pool: { max: 10, min: 0, idleTimeoutMillis: 30000 }
    })
      .connect()
      .catch((erro) => {
        poolPromessa = null; // permite nova tentativa na próxima requisição
        throw erro;
      });
  }
  return poolPromessa;
}

/**
 * Executa um comando parametrizado.
 * @param {string} comando SQL com parâmetros @nome
 * @param {Record<string, [any, any]>} parametros { nome: [tipo, valor] }
 * @param {sql.Transaction} [transacao] executa dentro da transação informada
 */
async function executar(comando, parametros = {}, transacao = null) {
  const requisicao = transacao ? new sql.Request(transacao) : (await obterPool()).request();
  for (const [nome, [tipo, valor]] of Object.entries(parametros)) {
    requisicao.input(nome, tipo, valor === undefined ? null : valor);
  }
  return requisicao.query(comando);
}

/** Linhas do primeiro conjunto de resultados. */
async function consultar(comando, parametros, transacao) {
  return (await executar(comando, parametros, transacao)).recordset || [];
}

/** Primeira linha ou null. */
async function consultarUm(comando, parametros, transacao) {
  return (await consultar(comando, parametros, transacao))[0] || null;
}

/** Executa a função dentro de uma transação (RNF03): confirma no fim ou desfaz tudo em caso de erro. */
async function emTransacao(funcao) {
  const transacao = new sql.Transaction(await obterPool());
  await transacao.begin();
  try {
    const resultado = await funcao(transacao);
    await transacao.commit();
    return resultado;
  } catch (erro) {
    try {
      await transacao.rollback();
    } catch {
      // a transação já pode ter sido desfeita pelo servidor
    }
    throw erro;
  }
}

async function fechar() {
  if (poolPromessa) {
    const pool = await poolPromessa.catch(() => null);
    poolPromessa = null;
    if (pool) await pool.close();
  }
}

module.exports = { sql, executar, consultar, consultarUm, emTransacao, fechar };
