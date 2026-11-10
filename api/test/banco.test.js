// INF05 e INF06: procedures (script 05) e triggers (script 06) do banco.
// Rodam no banco de TESTE com os scripts 01 a 06. Se o 05 e o 06 não estiverem instalados, os testes
// são pulados com um aviso. Tudo que muda dados roda dentro de uma transação que é SEMPRE desfeita.
const apoio = require('./apoio');
const { test, describe, before, after } = require('node:test');
const assert = require('node:assert/strict');

const { sql, banco } = apoio;
const { sql: tipos } = banco;

let instalado = false;
const AVISO = 'rode os scripts 05_procedures.sql e 06_triggers.sql no banco de teste';

// Só pula quando NENHUM objeto dos scripts 05 e 06 existe. Instalação pela metade reprova de propósito.
before(async () => {
  if (!apoio.temBanco) return;
  const [linha] = await sql(
    `SELECT (SELECT COUNT(*) FROM sys.procedures WHERE name LIKE 'sp[_]%')
          + (SELECT COUNT(*) FROM sys.triggers WHERE name LIKE 'trg[_]%') AS total`
  );
  instalado = linha.total > 0;
});

after(() => banco.fechar());

/** Executa o comando numa transação desfeita no fim. Devolve { erro, mensagem } (erro 0 = nada barrou). */
async function tentar(comando) {
  const [r] = await sql(
    `SET NOCOUNT ON;
     BEGIN TRY
       BEGIN TRAN;
       ${comando};
       ROLLBACK;
       SELECT 0 AS erro, N'' AS mensagem;
     END TRY
     BEGIN CATCH
       IF @@TRANCOUNT > 0 ROLLBACK;
       SELECT ERROR_NUMBER() AS erro, ERROR_MESSAGE() AS mensagem;
     END CATCH`
  );
  return r;
}

/** Aluno novo (só dentro da transação desfeita), para não mexer na pontuação de ninguém. */
const NOVO_ALUNO = `
  DECLARE @aluno INT, @sim1 INT, @sim2 INT;
  INSERT INTO dbo.Usuario (Nome_Usuario, CPF_Usuario, Email_Usuario, Senha_Usuario, DataIngresso_Usuario, Id_Perfil, Id_Turma)
  VALUES (N'Aluno do teste de banco', RIGHT('00000000000' + CAST(ABS(CHECKSUM(NEWID())) AS VARCHAR(11)), 11),
          'banco.' + CAST(NEWID() AS VARCHAR(36)) + '@aluno.avaliamais.test', 'x', GETDATE(), 1, 1);
  SET @aluno = SCOPE_IDENTITY();`;

function resultado(simulado, pontos) {
  return `
  INSERT INTO dbo.Simulado (Titulo_Simulado, QuantidadeQuestoes_Simulado, DataCriacao_Simulado, Id_Usuario)
  VALUES (N'teste de banco', 10, GETDATE(), @aluno);
  SET ${simulado} = SCOPE_IDENTITY();
  INSERT INTO dbo.ResultadoSimulado (TotalQuestoes_ResultadoSimulado, TotalAcertos_ResultadoSimulado, Percentual_ResultadoSimulado,
                                     PontosObtidos_ResultadoSimulado, DataRealizacao_ResultadoSimulado, Id_Usuario, Id_Simulado)
  VALUES (10, ${pontos / 10}, ${pontos}, ${pontos}, GETDATE(), @aluno, ${simulado});`;
}

describe('procedures (INF05)', { skip: apoio.motivoPulo }, () => {
  test('sp_PontuacaoAluno: medalha atual e pontos faltantes seguem as faixas da tabela Medalha (RN22, RN23)', async (t) => {
    if (!instalado) return t.skip(AVISO);
    const [aluno] = await sql('EXEC dbo.sp_PontuacaoAluno @IdUsuario = 5');
    const faixas = await sql('SELECT Nome_Medalha AS nome, PontosMinimos_Medalha AS pontos FROM dbo.Medalha ORDER BY PontosMinimos_Medalha');

    const alcancadas = faixas.filter((f) => f.pontos <= aluno.PontuacaoTotal);
    const proxima = faixas.find((f) => f.pontos > aluno.PontuacaoTotal);
    assert.equal(aluno.MedalhaAtual, alcancadas.at(-1)?.nome ?? null);
    assert.equal(aluno.ProximaMedalha, proxima?.nome ?? null);
    assert.equal(aluno.PontosFaltantes, proxima ? proxima.pontos - aluno.PontuacaoTotal : 0);
  });

  test('sp_PontuacaoAluno: recusa quem não é aluno ou não existe', async (t) => {
    if (!instalado) return t.skip(AVISO);
    await assert.rejects(sql('EXEC dbo.sp_PontuacaoAluno @IdUsuario = 1'), /Aluno não encontrado/); // administrador
    await assert.rejects(sql('EXEC dbo.sp_PontuacaoAluno @IdUsuario = 999999'), /Aluno não encontrado/);
  });

  test('sp_DesempenhoAluno: percentual = acertos / respostas', async (t) => {
    if (!instalado) return t.skip(AVISO);
    const linhas = await sql('EXEC dbo.sp_DesempenhoAluno @IdUsuario = 5');
    assert.ok(linhas.length > 0);
    for (const l of linhas) {
      assert.equal(Number(l.Percentual), Math.round((10000 * l.TotalAcertos) / l.TotalRespostas) / 100);
    }
  });

  test('sp_HistoricoSimulados: mais recentes primeiro e paginação validada', async (t) => {
    if (!instalado) return t.skip(AVISO);
    const linhas = await sql('EXEC dbo.sp_HistoricoSimulados @IdUsuario = 5, @Pagina = 1, @Tamanho = 10');
    assert.ok(linhas.length > 0);
    for (let i = 1; i < linhas.length; i++) assert.ok(linhas[i - 1].DataRealizacao >= linhas[i].DataRealizacao);
    await assert.rejects(sql('EXEC dbo.sp_HistoricoSimulados @IdUsuario = 5, @Pagina = 0'), /Página deve ser 1 ou mais/);
    await assert.rejects(sql('EXEC dbo.sp_HistoricoSimulados @IdUsuario = 5, @Tamanho = 51'), /tamanho de 1 a 50/);
  });

  test('sp_ConferirPontuacao: nenhum aluno com pontuação diferente da soma dos resultados (RN21)', async (t) => {
    if (!instalado) return t.skip(AVISO);
    assert.deepEqual(await sql('EXEC dbo.sp_ConferirPontuacao'), []);
  });
});

describe('triggers (INF06)', { skip: apoio.motivoPulo }, () => {
  test('resultado novo soma os pontos no aluno, acumulando (RN21)', async (t) => {
    if (!instalado) return t.skip(AVISO);
    const [r] = await sql(`SET NOCOUNT ON; SET XACT_ABORT ON;
      BEGIN TRAN;
      ${NOVO_ALUNO}
      DECLARE @p0 INT = (SELECT PontuacaoTotal_Usuario FROM dbo.Usuario WHERE Id_Usuario = @aluno);
      ${resultado('@sim1', 70)}
      DECLARE @p1 INT = (SELECT PontuacaoTotal_Usuario FROM dbo.Usuario WHERE Id_Usuario = @aluno);
      ${resultado('@sim2', 30)}
      DECLARE @p2 INT = (SELECT PontuacaoTotal_Usuario FROM dbo.Usuario WHERE Id_Usuario = @aluno);
      ROLLBACK;
      SELECT @p0 AS p0, @p1 AS p1, @p2 AS p2;`);
    assert.deepEqual({ p0: r.p0, p1: r.p1, p2: r.p2 }, { p0: 0, p1: 70, p2: 100 });
  });

  test('a pontuação não pode diminuir, mas pode aumentar (RN21)', async (t) => {
    if (!instalado) return t.skip(AVISO);
    const reducao = await tentar('UPDATE dbo.Usuario SET PontuacaoTotal_Usuario = PontuacaoTotal_Usuario - 1 WHERE Id_Usuario = 5');
    assert.equal(reducao.erro, 50021);
    assert.match(reducao.mensagem, /nunca é reduzida/);
    assert.equal((await tentar('UPDATE dbo.Usuario SET PontuacaoTotal_Usuario = PontuacaoTotal_Usuario + 1 WHERE Id_Usuario = 5')).erro, 0);
    assert.equal((await tentar('UPDATE dbo.Usuario SET Nome_Usuario = Nome_Usuario WHERE Id_Usuario = 5')).erro, 0);
  });

  test('respostas e resultados não podem ser alterados nem excluídos (RN19)', async (t) => {
    if (!instalado) return t.skip(AVISO);
    const umaResposta = '(SELECT MIN(Id_RespostaAluno) FROM dbo.RespostaAluno)';
    const umResultado = '(SELECT MIN(Id_ResultadoSimulado) FROM dbo.ResultadoSimulado)';
    assert.equal((await tentar(`UPDATE dbo.RespostaAluno SET Acertou_RespostaAluno = 1 - Acertou_RespostaAluno WHERE Id_RespostaAluno = ${umaResposta}`)).erro, 50022);
    assert.equal((await tentar(`DELETE FROM dbo.RespostaAluno WHERE Id_RespostaAluno = ${umaResposta}`)).erro, 50022);
    assert.equal((await tentar(`UPDATE dbo.ResultadoSimulado SET PontosObtidos_ResultadoSimulado = 999 WHERE Id_ResultadoSimulado = ${umResultado}`)).erro, 50023);
    assert.equal((await tentar(`DELETE FROM dbo.ResultadoSimulado WHERE Id_ResultadoSimulado = ${umResultado}`)).erro, 50023);
    // comando que não atinge nenhuma linha não é barrado
    assert.equal((await tentar('DELETE FROM dbo.RespostaAluno WHERE 1 = 0')).erro, 0);
  });

  test('questão já utilizada em simulado mantém a alternativa correta, mas o texto pode mudar (RN11)', async (t) => {
    if (!instalado) return t.skip(AVISO);
    const usada = '(SELECT TOP 1 Id_Questao FROM dbo.SimuladoQuestao ORDER BY Id_Questao)';
    const livre = `(SELECT TOP 1 q.Id_Questao FROM dbo.Questao q WHERE NOT EXISTS (SELECT 1 FROM dbo.SimuladoQuestao sq WHERE sq.Id_Questao = q.Id_Questao) ORDER BY q.Id_Questao)`;
    const trocar = (questao) => `UPDATE dbo.Alternativa SET Correta_Alternativa = 1 - Correta_Alternativa
                                  WHERE Id_Questao = ${questao} AND Letra_Alternativa IN ('A', 'B')`;

    const bloqueada = await tentar(trocar(usada));
    assert.equal(bloqueada.erro, 50024);
    assert.match(bloqueada.mensagem, /RN11/);
    assert.equal((await tentar(`UPDATE dbo.Alternativa SET Texto_Alternativa = Texto_Alternativa + N' ' WHERE Id_Questao = ${usada}`)).erro, 0);
    assert.equal((await tentar(trocar(livre))).erro, 0);
  });
});
