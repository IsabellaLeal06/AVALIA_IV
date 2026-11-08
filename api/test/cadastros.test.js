// Cadastros do Administrador: grade da turma e vínculo da disciplina (RF02, RN12).
const apoio = require('./apoio');
const { test, describe, before, after } = require('node:test');
const assert = require('node:assert/strict');

const { chamar, entrar } = apoio;

describe('grade de disciplinas e turmas', { skip: apoio.motivoPulo }, () => {
  before(apoio.iniciar);
  after(apoio.encerrar);

  test('uma lista vazia esvazia a grade da turma (e não é ignorada)', async () => {
    const token = await entrar('admin');
    const nome = `Turma grade ${Date.now()}`;
    const criada = await chamar('POST', '/turmas', { token, corpo: { nome, anoLetivo: 2040, disciplinas: [1, 2] } });
    assert.equal(criada.status, 201);
    const id = criada.corpo.id;
    try {
      assert.equal((await chamar('GET', `/turmas/${id}`, { token })).corpo.disciplinas.length, 2);

      const editada = await chamar('PUT', `/turmas/${id}`, { token, corpo: { nome, anoLetivo: 2040, disciplinas: [] } });
      assert.equal(editada.status, 200);
      assert.equal((await chamar('GET', `/turmas/${id}`, { token })).corpo.disciplinas.length, 0);

      // Sem o campo "disciplinas", a grade é mantida
      await chamar('PUT', `/turmas/${id}`, { token, corpo: { nome, anoLetivo: 2040, disciplinas: [3] } });
      await chamar('PUT', `/turmas/${id}`, { token, corpo: { nome: `${nome} B`, anoLetivo: 2040 } });
      assert.deepEqual((await chamar('GET', `/turmas/${id}`, { token })).corpo.disciplinas.map((d) => d.id), [3]);
    } finally {
      await chamar('PUT', `/turmas/${id}`, { token, corpo: { nome, anoLetivo: 2040, disciplinas: [] } });
      await chamar('PATCH', `/turmas/${id}/situacao`, { token, corpo: { ativo: false } });
    }
  });

  test('uma lista vazia tira a disciplina de todas as turmas', async () => {
    const token = await entrar('admin');
    const turma = await chamar('POST', '/turmas', { token, corpo: { nome: `Turma vínculo ${Date.now()}`, anoLetivo: 2041 } });
    const disciplina = await chamar('POST', '/disciplinas', {
      token,
      corpo: { nome: `Disciplina vínculo ${Date.now()}`, turmas: [turma.corpo.id] }
    });
    assert.equal(disciplina.status, 201);
    try {
      assert.equal((await chamar('GET', `/disciplinas/${disciplina.corpo.id}`, { token })).corpo.turmas.length, 1);
      await chamar('PUT', `/disciplinas/${disciplina.corpo.id}`, { token, corpo: { nome: `Disciplina vínculo ${Date.now()}`, turmas: [] } });
      assert.equal((await chamar('GET', `/disciplinas/${disciplina.corpo.id}`, { token })).corpo.turmas.length, 0);
    } finally {
      await chamar('PATCH', `/disciplinas/${disciplina.corpo.id}/situacao`, { token, corpo: { ativo: false } });
      await chamar('PATCH', `/turmas/${turma.corpo.id}/situacao`, { token, corpo: { ativo: false } });
    }
  });
});
