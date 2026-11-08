// Testes de unidade: não precisam de banco.
require('./apoio');
const { test, describe } = require('node:test');
const assert = require('node:assert/strict');
const v = require('../src/utils/validacoes');
const { corrigir } = require('../src/services/simuladoService');
const { caminhoDeImagem } = require('../src/services/uploadService');
const { padraoBusca } = require('../src/repositories/auxiliares');

describe('validações (iguais às da Desktop)', () => {
  test('CPF válido pelos dígitos verificadores, com ou sem máscara', () => {
    assert.equal(v.cpfValido('529.982.247-25'), true);
    assert.equal(v.cpfValido('15881399803'), true);
    assert.equal(v.cpfValido('529.982.247-26'), false);
    assert.equal(v.cpfValido('111.111.111-11'), false);
    assert.equal(v.cpfValido('1234567890'), false);
    assert.equal(v.cpfValido('5299822472a'), false);
  });

  test('regras da nova senha (RN04)', () => {
    assert.deepEqual(v.validarSenha('Senha2026'), []);
    assert.ok(v.validarSenha('curta1').length > 0);
    assert.ok(v.validarSenha('semnumeros').length > 0);
    assert.ok(v.validarSenha('1234567890').length > 0);
    assert.ok(v.validarSenha(' Senha2026').length > 0);
  });

  test('e-mail', () => {
    assert.equal(v.emailValido('Fulano@Escola.test'), true);
    assert.equal(v.emailValido('sem-arroba.test'), false);
    assert.equal(v.normalizarEmail('  A@B.TEST '), 'a@b.test');
  });

  test('busca escapa os curingas do LIKE', () => {
    assert.equal(padraoBusca('50%_[a]'), '%50\\%\\_\\[a]%');
    assert.equal(padraoBusca('   '), null);
  });
});

describe('correção do simulado (RN18, RN21)', () => {
  const gabarito = [
    { idQuestao: 1, idDisciplina: 10, correta: 'A' },
    { idQuestao: 2, idDisciplina: 10, correta: 'B' },
    { idQuestao: 3, idDisciplina: 20, correta: 'C' },
    { idQuestao: 4, idDisciplina: 20, correta: 'D' }
  ];

  test('questão em branco conta como erro e pontos = acertos x 10', () => {
    const r = corrigir(gabarito, [
      { idQuestao: 1, alternativa: 'a' },
      { idQuestao: 2, alternativa: 'C' },
      { idQuestao: 3, alternativa: null }
      // questão 4 não enviada: em branco
    ]);
    assert.equal(r.totalQuestoes, 4);
    assert.equal(r.totalAcertos, 1);
    assert.equal(r.pontosObtidos, 10);
    assert.equal(r.percentual, 25);
    assert.equal(r.respostas.filter((x) => x.marcada === null).length, 2);
    assert.deepEqual(r.porDisciplina.get(10), { respostas: 2, acertos: 1 });
    assert.deepEqual(r.porDisciplina.get(20), { respostas: 2, acertos: 0 });
  });

  test('resposta de questão que não é do simulado é ignorada', () => {
    const r = corrigir(gabarito, [{ idQuestao: 99, alternativa: 'A' }]);
    assert.equal(r.totalAcertos, 0);
    assert.equal(r.totalQuestoes, 4);
  });
});

describe('caminho de imagem aceito pela API', () => {
  test('aceita só arquivos da pasta de upload do tipo', () => {
    assert.equal(caminhoDeImagem('uploads/questoes/a1.png', 'questoes'), 'uploads/questoes/a1.png');
    assert.equal(caminhoDeImagem('http://localhost:3000/uploads/questoes/a1.png', 'questoes'), 'uploads/questoes/a1.png');
    assert.equal(caminhoDeImagem('../segredo.png', 'questoes'), null);
    assert.equal(caminhoDeImagem('uploads/medalhas/a.png', 'questoes'), null);
    assert.equal(caminhoDeImagem('assets/medalhas/bronze.png', 'medalhas'), 'assets/medalhas/bronze.png');
    assert.equal(caminhoDeImagem('assets/medalhas/bronze.png', 'questoes'), null);
  });
});
