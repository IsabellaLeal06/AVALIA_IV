// TST02: visibilidade por perfil (RN08, RN24, RNF02). Cada tentativa fora do escopo é recusada no servidor.
const apoio = require('./apoio');
const { test, describe, before, after } = require('node:test');
const assert = require('node:assert/strict');

const { chamar, entrar } = apoio;

describe('visibilidade por perfil', { skip: apoio.motivoPulo }, () => {
  before(apoio.iniciar);
  after(apoio.encerrar);

  test('professor tentando acessar turma que não é sua', async () => {
    // Ricardo é responsável pelas disciplinas 1, 2 e 3; a turma 3 só tem as disciplinas 4 e 5 (Patrícia)
    const token = await entrar('ricardo');
    const turmas = await chamar('GET', '/desempenho/turmas', { token });
    assert.equal(turmas.status, 200);
    assert.deepEqual(turmas.corpo.map((t) => t.id).sort(), [1, 2]);

    for (const rota of ['/desempenho/turmas/3', '/desempenho/turmas/3/alunos', '/desempenho/turmas/3/alunos/18']) {
      assert.equal((await chamar('GET', rota, { token })).status, 404, rota);
    }
    // Aluno de outra turma pela turma do professor
    assert.equal((await chamar('GET', '/desempenho/turmas/1/alunos/12', { token })).status, 404);
  });

  test('o painel do aluno visto pelo professor é consolidado, sem respostas questão a questão', async () => {
    const token = await entrar('ricardo');
    const r = await chamar('GET', '/desempenho/turmas/1/alunos/5', { token });
    assert.equal(r.status, 200);
    assert.ok('mediaAcertos' in r.corpo && 'qtdSimulados' in r.corpo && Array.isArray(r.corpo.porDisciplina));
    const texto = JSON.stringify(r.corpo);
    assert.doesNotMatch(texto, /marcada|correta|alternativa|enunciado|cpf|email/i);
    // Só as disciplinas do Ricardo que estão na grade da turma 1
    assert.deepEqual(r.corpo.porDisciplina.map((d) => d.idDisciplina).sort(), [1, 2, 3]);
  });

  test('alunos desativados ficam fora das médias da turma (RN25)', async () => {
    const token = await entrar('ricardo');
    const alunos = await chamar('GET', '/desempenho/turmas/1/alunos', { token });
    assert.equal(alunos.status, 200);
    assert.ok(!alunos.corpo.some((a) => a.id === 11), 'Gabriela (desativada) não aparece');
  });

  test('aluno tentando acessar simulado de outro aluno', async () => {
    const token = await entrar('bruno');
    for (const rota of ['/simulados/1', '/simulados/1/revisao', '/simulados/1/resultado']) {
      assert.equal((await chamar('GET', rota, { token })).status, 404, rota);
    }
    assert.equal((await chamar('POST', '/simulados/1/finalizar', { token, corpo: { respostas: [] } })).status, 404);
  });

  test('administrador tentando acessar rota de desempenho', async () => {
    const token = await entrar('admin');
    for (const rota of ['/desempenho/aluno', '/desempenho/turmas', '/desempenho/turmas/1', '/desempenho/questoes-erro']) {
      assert.equal((await chamar('GET', rota, { token })).status, 403, rota);
    }
    const painel = await chamar('GET', '/painel', { token });
    assert.deepEqual(Object.keys(painel.corpo).sort(), ['disciplinas', 'questoes', 'turmas', 'usuarios']);
  });

  test('aluno e professor não acessam cadastros institucionais', async () => {
    for (const nome of ['ana', 'ricardo']) {
      const token = await entrar(nome);
      for (const [metodo, rota] of [['GET', '/usuarios'], ['GET', '/turmas'], ['POST', '/usuarios'], ['GET', '/painel'], ['GET', '/medalhas']]) {
        assert.equal((await chamar(metodo, rota, { token, corpo: metodo === 'POST' ? {} : undefined })).status, 403, `${nome} ${rota}`);
      }
    }
  });

  test('o CPF não aparece na listagem de usuários, só na consulta individual (RNF02)', async () => {
    const token = await entrar('admin');
    const lista = await chamar('GET', '/usuarios?situacao=todos', { token });
    assert.equal(lista.status, 200);
    assert.ok(lista.corpo.length > 10);
    assert.ok(lista.corpo.every((u) => !('cpf' in u)));
    assert.doesNotMatch(JSON.stringify(lista.corpo), /88447294072/);

    const um = await chamar('GET', '/usuarios/5', { token });
    assert.equal(um.corpo.cpf, '88447294072');
  });

  test('professor só vê e altera questões das suas disciplinas (RN08)', async () => {
    const token = await entrar('ricardo');
    const lista = await chamar('GET', '/questoes?disciplina=5', { token });
    assert.equal(lista.status, 200);
    assert.equal(lista.corpo.length, 0, 'filtro do cliente não amplia o escopo');

    const todas = await chamar('GET', '/questoes', { token });
    assert.ok(todas.corpo.every((q) => [1, 2, 3].includes(q.idDisciplina)));
    assert.ok(todas.corpo.every((q) => q.autor), 'devolve quem cadastrou');

    assert.equal((await chamar('GET', '/questoes/41', { token })).status, 404);
    assert.equal((await chamar('PATCH', '/questoes/41/situacao', { token, corpo: { ativo: false } })).status, 404);

    const disciplinas = await chamar('GET', '/disciplinas', { token });
    assert.deepEqual(disciplinas.corpo.map((d) => d.id).sort(), [1, 2, 3]);
  });

  test('a realização do simulado não entrega gabarito nem explicação', async () => {
    const token = await entrar('ana');
    const gerado = await chamar('POST', '/simulados', { token, corpo: { disciplinas: [1, 4], quantidade: 10 } });
    assert.equal(gerado.status, 201);
    const simulado = await chamar('GET', `/simulados/${gerado.corpo.id}`, { token });
    assert.equal(simulado.status, 200);
    const texto = JSON.stringify(simulado.corpo);
    assert.doesNotMatch(texto, /"correta"|"explicacao"|"acertou"/);
    // Revisão só depois de finalizar
    assert.equal((await chamar('GET', `/simulados/${gerado.corpo.id}/revisao`, { token })).status, 409);
    await chamar('POST', `/simulados/${gerado.corpo.id}/finalizar`, { token, corpo: { respostas: [] } });
  });
});
