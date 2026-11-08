// TST03 e TST06: sorteio, correção, tentativa única, desativação em cascata (RN12) e RN11.
const apoio = require('./apoio');
const { test, describe, before, after } = require('node:test');
const assert = require('node:assert/strict');

const { chamar, entrar, sql, banco } = apoio;
const { sql: tipos } = banco;

async function gabarito(idSimulado) {
  return sql(
    `SELECT sq.Id_Questao AS idQuestao, a.Letra_Alternativa AS correta, q.Id_Disciplina AS idDisciplina
     FROM dbo.SimuladoQuestao sq
     JOIN dbo.Questao q ON q.Id_Questao = sq.Id_Questao
     JOIN dbo.Alternativa a ON a.Id_Questao = sq.Id_Questao AND a.Correta_Alternativa = 1
     WHERE sq.Id_Simulado = @Id ORDER BY sq.Ordem`,
    { Id: [tipos.Int, idSimulado] }
  );
}

async function pontuacao(idUsuario) {
  const [linha] = await sql('SELECT PontuacaoTotal_Usuario AS p FROM dbo.Usuario WHERE Id_Usuario = @Id', { Id: [tipos.Int, idUsuario] });
  return linha.p;
}

const errada = (letra) => ({ A: 'B', B: 'C', C: 'D', D: 'E', E: 'A' })[letra];

describe('sorteio (RN12, RN13, RN14, RN15)', { skip: apoio.motivoPulo }, () => {
  before(apoio.iniciar);
  after(apoio.encerrar);

  test('exige duas ou mais disciplinas, no servidor', async () => {
    const token = await entrar('camila');
    const r = await chamar('POST', '/simulados', { token, corpo: { disciplinas: [1], quantidade: 10 } });
    assert.equal(r.status, 400);
    assert.ok(r.corpo.erro.campos.disciplinas);
  });

  test('aceita só 10, 15 ou 20 questões', async () => {
    const token = await entrar('camila');
    for (const quantidade of [3, 5, 7, 12, 25]) {
      const r = await chamar('POST', '/simulados', { token, corpo: { disciplinas: [1, 2], quantidade } });
      assert.equal(r.status, 400, String(quantidade));
    }
    for (const quantidade of [10, 15, 20]) {
      const r = await chamar('POST', '/simulados', { token, corpo: { disciplinas: [1, 2], quantidade } });
      assert.equal(r.status, 201, String(quantidade));
      const simulado = await chamar('GET', `/simulados/${r.corpo.id}`, { token });
      const ids = simulado.corpo.questoes.map((q) => q.id);
      assert.equal(ids.length, quantidade);
      assert.equal(new Set(ids).size, quantidade, 'sem repetição');
      assert.deepEqual(simulado.corpo.questoes.map((q) => q.ordem), ids.map((_, i) => i + 1), 'ordem gravada');
      await chamar('POST', `/simulados/${r.corpo.id}/finalizar`, { token, corpo: { respostas: [] } });
    }
  });

  test('só questões ativas: com uma questão desativada, faltam questões para 20', async () => {
    const admin = await entrar('admin');
    const token = await entrar('camila');
    // Disciplinas 1 e 2 têm exatamente 20 questões ativas
    assert.equal((await chamar('PATCH', '/questoes/2/situacao', { token: admin, corpo: { ativo: false } })).status, 200);
    try {
      const r = await chamar('POST', '/simulados', { token, corpo: { disciplinas: [1, 2], quantidade: 20 } });
      assert.equal(r.status, 409);
      const r15 = await chamar('POST', '/simulados', { token, corpo: { disciplinas: [1, 2], quantidade: 15 } });
      const simulado = await chamar('GET', `/simulados/${r15.corpo.id}`, { token });
      assert.ok(!simulado.corpo.questoes.some((q) => q.id === 2), 'questão desativada não é sorteada');
      await chamar('POST', `/simulados/${r15.corpo.id}/finalizar`, { token, corpo: { respostas: [] } });
    } finally {
      await chamar('PATCH', '/questoes/2/situacao', { token: admin, corpo: { ativo: true } });
    }
  });

  test('disciplina desativada retira as questões do sorteio mesmo ativas', async () => {
    const admin = await entrar('admin');
    const token = await entrar('camila');
    assert.equal((await chamar('PATCH', '/disciplinas/7/situacao', { token: admin, corpo: { ativo: false } })).status, 200);
    try {
      const ativas = await chamar('GET', '/disciplinas/ativas', { token });
      assert.ok(!ativas.corpo.some((d) => d.id === 7));
      const r = await chamar('POST', '/simulados', { token, corpo: { disciplinas: [6, 7], quantidade: 10 } });
      assert.equal(r.status, 400);
      const [{ total }] = await sql('SELECT COUNT(*) AS total FROM dbo.Questao WHERE Id_Disciplina = 7 AND Ativo_Questao = 1');
      assert.equal(total, 10, 'as questões continuam ativas');
    } finally {
      await chamar('PATCH', '/disciplinas/7/situacao', { token: admin, corpo: { ativo: true } });
    }
  });

  test('turma desativada não desativa os alunos (RN12)', async () => {
    const admin = await entrar('admin');
    const r = await chamar('PATCH', '/turmas/2/situacao', { token: admin, corpo: { ativo: false } });
    try {
      assert.equal(r.status, 200);
      assert.ok(r.corpo.alunosAtivos > 0, 'informa os alunos ativos para a tela alertar');
      const login = await chamar('POST', '/auth/login', {
        corpo: { email: apoio.USUARIOS.henrique.email, senha: apoio.USUARIOS.henrique.cpf }
      });
      assert.equal(login.status, 200);
    } finally {
      await chamar('PATCH', '/turmas/2/situacao', { token: admin, corpo: { ativo: true } });
    }
  });
});

describe('correção e tentativa única (RN16, RN18, RN19, RN21)', { skip: apoio.motivoPulo }, () => {
  before(apoio.iniciar);
  after(apoio.encerrar);

  test('em branco conta como erro, pontos = acertos x 10 e a pontuação acumula', async () => {
    const token = await entrar('camila');
    const antes = await pontuacao(7);
    const gerado = await chamar('POST', '/simulados', { token, corpo: { disciplinas: [3, 4, 5], quantidade: 10 } });
    assert.equal(gerado.status, 201);
    const g = await gabarito(gerado.corpo.id);

    // 6 certas, 2 erradas, 2 em branco (uma enviada como null e outra não enviada)
    const respostas = g.slice(0, 6).map((q) => ({ idQuestao: q.idQuestao, alternativa: q.correta }));
    respostas.push(...g.slice(6, 8).map((q) => ({ idQuestao: q.idQuestao, alternativa: errada(q.correta) })));
    respostas.push({ idQuestao: g[8].idQuestao, alternativa: null });
    // o cliente não informa acertos nem pontos; se informar, é ignorado
    const r = await chamar('POST', `/simulados/${gerado.corpo.id}/finalizar`, {
      token,
      corpo: { respostas, totalAcertos: 10, pontosObtidos: 999, tempo: '00:10:00' }
    });
    assert.equal(r.status, 200);
    assert.equal(r.corpo.totalAcertos, 6);
    assert.equal(r.corpo.emBranco, 2);
    assert.equal(r.corpo.totalErros, 2);
    assert.equal(r.corpo.pontosObtidos, 60);
    assert.equal(r.corpo.percentual, 60);
    assert.equal(r.corpo.jaFinalizado, false);
    assert.equal(await pontuacao(7), antes + 60);
    assert.ok(r.corpo.porDisciplina.length >= 1);

    const [soma] = await sql(
      `SELECT u.PontuacaoTotal_Usuario AS total, (SELECT SUM(PontosObtidos_ResultadoSimulado) FROM dbo.ResultadoSimulado
         WHERE Id_Usuario = u.Id_Usuario) AS soma FROM dbo.Usuario u WHERE u.Id_Usuario = 7`
    );
    assert.equal(soma.total, soma.soma, 'pontuação acumulada = soma dos PontosObtidos');

    const brancos = await sql(
      'SELECT COUNT(*) AS n FROM dbo.RespostaAluno WHERE Id_Simulado = @Id AND AlternativaMarcada_RespostaAluno IS NULL',
      { Id: [tipos.Int, gerado.corpo.id] }
    );
    assert.equal(brancos[0].n, 2);

    const revisao = await chamar('GET', `/simulados/${gerado.corpo.id}/revisao`, { token });
    assert.equal(revisao.status, 200);
    assert.equal(revisao.corpo.questoes.filter((q) => q.emBranco).length, 2);
    assert.ok(revisao.corpo.questoes.every((q) => q.correta));
  });

  test('segunda finalização devolve o resultado já apurado, sem pontuar de novo', async () => {
    const token = await entrar('camila');
    const gerado = await chamar('POST', '/simulados', { token, corpo: { disciplinas: [1, 2], quantidade: 10 } });
    const g = await gabarito(gerado.corpo.id);
    const respostas = g.map((q) => ({ idQuestao: q.idQuestao, alternativa: q.correta }));

    const primeira = await chamar('POST', `/simulados/${gerado.corpo.id}/finalizar`, { token, corpo: { respostas } });
    const depois = await pontuacao(7);
    const segunda = await chamar('POST', `/simulados/${gerado.corpo.id}/finalizar`, { token, corpo: { respostas: [] } });

    assert.equal(primeira.corpo.totalAcertos, 10);
    assert.equal(segunda.status, 200);
    assert.equal(segunda.corpo.jaFinalizado, true);
    assert.equal(segunda.corpo.totalAcertos, 10);
    assert.equal(await pontuacao(7), depois);
  });

  test('finalizações simultâneas gravam um único resultado', async () => {
    const token = await entrar('camila');
    const gerado = await chamar('POST', '/simulados', { token, corpo: { disciplinas: [1, 2], quantidade: 10 } });
    const antes = await pontuacao(7);
    const g = await gabarito(gerado.corpo.id);
    const respostas = g.map((q) => ({ idQuestao: q.idQuestao, alternativa: q.correta }));
    const resultados = await Promise.all(
      [1, 2, 3].map(() => chamar('POST', `/simulados/${gerado.corpo.id}/finalizar`, { token, corpo: { respostas } }))
    );
    assert.ok(resultados.every((r) => r.status === 200 && r.corpo.totalAcertos === 10));
    const [{ n }] = await sql('SELECT COUNT(*) AS n FROM dbo.ResultadoSimulado WHERE Id_Simulado = @Id', {
      Id: [tipos.Int, gerado.corpo.id]
    });
    assert.equal(n, 1);
    assert.equal(await pontuacao(7), antes + 100);
  });

  test('letra fora de A a E é recusada', async () => {
    const token = await entrar('camila');
    const gerado = await chamar('POST', '/simulados', { token, corpo: { disciplinas: [1, 2], quantidade: 10 } });
    const r = await chamar('POST', `/simulados/${gerado.corpo.id}/finalizar`, {
      token,
      corpo: { respostas: [{ idQuestao: 1, alternativa: 'F' }] }
    });
    assert.equal(r.status, 400);
    await chamar('POST', `/simulados/${gerado.corpo.id}/finalizar`, { token, corpo: { respostas: [] } });
  });

  test('não existe rota para alterar ou excluir resposta e resultado (RN19)', async () => {
    const token = await entrar('ana');
    for (const metodo of ['PUT', 'PATCH', 'DELETE']) {
      const r = await chamar(metodo, '/simulados/1', { token, corpo: {} });
      assert.equal(r.status, 404, metodo);
    }
  });

  test('histórico paginado só com simulados finalizados', async () => {
    const token = await entrar('ana');
    const r = await chamar('GET', '/simulados?pagina=1&tamanho=2', { token });
    assert.equal(r.status, 200);
    assert.ok(r.corpo.itens.length <= 2);
    assert.ok(r.corpo.total >= 1);
    assert.ok(r.corpo.itens.every((s) => typeof s.percentual === 'number'));
  });
});

describe('banco de questões (RN07, RN09, RN11)', { skip: apoio.motivoPulo }, () => {
  before(apoio.iniciar);
  after(apoio.encerrar);

  const alternativas = (correta) =>
    ['A', 'B', 'C', 'D', 'E'].map((letra) => ({ letra, texto: `Alternativa ${letra} ${Date.now()}`, correta: letra === correta }));

  test('RN11: não troca a alternativa correta de questão já utilizada; o resto continua editável', async () => {
    const token = await entrar('admin');
    const atual = (await chamar('GET', '/questoes/1', { token })).corpo;
    assert.equal(atual.utilizada, true);
    const lista = (await chamar('GET', '/questoes?disciplina=1', { token })).corpo;
    assert.equal(lista.find((q) => q.id === 1).utilizada, true, 'a listagem também informa a RN11');

    // Uma questão recém-criada ainda não entrou em nenhum simulado
    const nova = await chamar('POST', '/questoes', {
      token,
      corpo: {
        idDisciplina: 1,
        enunciado: `Questão da listagem ${Date.now()}`,
        correta: 'A',
        alternativas: ['A', 'B', 'C', 'D', 'E'].map((letra) => ({ letra, texto: `Texto ${letra} ${Date.now()}` }))
      }
    });
    assert.equal(nova.status, 201);
    const depois = (await chamar('GET', '/questoes?disciplina=1', { token })).corpo;
    assert.equal(depois.find((q) => q.id === nova.corpo.id).utilizada, false);
    await chamar('PATCH', `/questoes/${nova.corpo.id}/situacao`, { token, corpo: { ativo: false } });

    const trocar = await chamar('PUT', '/questoes/1', { token, corpo: { ...atual, correta: errada(atual.correta) } });
    assert.equal(trocar.status, 409);
    assert.ok(trocar.corpo.erro.campos.correta);

    const explicacao = `Explicação revisada em teste ${Date.now()}`;
    const editar = await chamar('PUT', '/questoes/1', { token, corpo: { ...atual, explicacao } });
    assert.equal(editar.status, 200);
    assert.equal((await chamar('GET', '/questoes/1', { token })).corpo.explicacao, explicacao);
    await chamar('PUT', '/questoes/1', { token, corpo: atual });
  });

  test('RN09: exatamente 5 alternativas, A a E, uma correta', async () => {
    const token = await entrar('ricardo');
    const base = { enunciado: 'Questão de teste RN09', idDisciplina: 1 };
    const quatro = await chamar('POST', '/questoes', { token, corpo: { ...base, alternativas: alternativas('A').slice(0, 4) } });
    assert.equal(quatro.status, 400);
    const duasCorretas = alternativas('A').map((a, i) => ({ ...a, correta: i < 2 }));
    assert.equal((await chamar('POST', '/questoes', { token, corpo: { ...base, alternativas: duasCorretas } })).status, 400);
    const repetida = alternativas('A').map((a, i) => (i === 4 ? { ...a, letra: 'A' } : a));
    assert.equal((await chamar('POST', '/questoes', { token, corpo: { ...base, alternativas: repetida } })).status, 400);

    const ok = await chamar('POST', '/questoes', { token, corpo: { ...base, alternativas: alternativas('C') } });
    assert.equal(ok.status, 201);
    const criada = (await chamar('GET', `/questoes/${ok.corpo.id}`, { token })).corpo;
    assert.equal(criada.autor, 'Ricardo Tavares Monteiro');
    assert.equal(criada.utilizada, false);
    assert.equal(criada.explicacao, null, 'explicação é opcional');

    // Ainda não usada: a correta pode mudar
    const mudar = await chamar('PUT', `/questoes/${ok.corpo.id}`, { token, corpo: { ...criada, correta: 'D' } });
    assert.equal(mudar.status, 200);
    // Professor desativa, mas só o Administrador reativa (RN12)
    assert.equal((await chamar('PATCH', `/questoes/${ok.corpo.id}/situacao`, { token, corpo: { ativo: false } })).status, 200);
    assert.equal((await chamar('PATCH', `/questoes/${ok.corpo.id}/situacao`, { token, corpo: { ativo: true } })).status, 403);
  });

  test('RN07: disciplina sem responsável não recebe questões novas', async () => {
    const token = await entrar('admin');
    const r = await chamar('POST', '/questoes', {
      token,
      corpo: { enunciado: 'Questão RN07', idDisciplina: 8, alternativas: alternativas('A') }
    });
    assert.equal(r.status, 400);
    assert.match(r.corpo.erro.campos.idDisciplina, /sem professor responsável/);
  });

  test('não existe exclusão física de usuário, turma, disciplina ou questão (RN12)', async () => {
    const token = await entrar('admin');
    for (const rota of ['/usuarios/6', '/turmas/1', '/disciplinas/1', '/questoes/1']) {
      assert.equal((await chamar('DELETE', rota, { token })).status, 404, rota);
    }
  });
});

describe('gamificação (RN22, RN23)', { skip: apoio.motivoPulo }, () => {
  before(apoio.iniciar);
  after(apoio.encerrar);

  test('aluno sem medalha vê a primeira faixa e quantos pontos faltam', async () => {
    const token = await entrar('henrique');
    const r = await chamar('GET', '/perfil', { token });
    assert.equal(r.status, 200);
    const { gamificacao, pontuacaoTotal } = r.corpo;
    const [primeira] = await sql('SELECT TOP (1) Nome_Medalha AS nome, PontosMinimos_Medalha AS p FROM dbo.Medalha ORDER BY PontosMinimos_Medalha');
    if (pontuacaoTotal < primeira.p) {
      assert.equal(gamificacao.semMedalha, true);
      assert.equal(gamificacao.medalha, null);
      assert.equal(gamificacao.proxima.nome, primeira.nome);
      assert.equal(gamificacao.proxima.faltam, primeira.p - pontuacaoTotal);
      assert.match(gamificacao.proxima.imagemUrl, /^https?:\/\//, 'endereço completo da imagem');
    }
  });

  test('as faixas vêm da tabela Medalha, não do código', async () => {
    const admin = await entrar('admin');
    const nova = await chamar('POST', '/medalhas', {
      token: admin,
      corpo: { nome: `Iniciante ${Date.now()}`, pontosMinimos: 50, imagemUrl: 'assets/medalhas/bronze.png' }
    });
    assert.equal(nova.status, 201);
    try {
      const token = await entrar('henrique'); // 110 pontos nos dados de teste
      const r = await chamar('GET', '/perfil', { token });
      if (r.corpo.pontuacaoTotal >= 50 && r.corpo.pontuacaoTotal < 250) {
        assert.match(r.corpo.gamificacao.medalha.nome, /^Iniciante/);
      }
      const repetida = await chamar('POST', '/medalhas', {
        token: admin,
        corpo: { nome: 'Outra', pontosMinimos: 50, imagemUrl: 'assets/medalhas/bronze.png' }
      });
      assert.equal(repetida.status, 400, 'pontuação mínima não se repete (RN27)');
    } finally {
      await chamar('DELETE', `/medalhas/${nova.corpo.id}`, { token: admin });
    }
  });

  test('professor e administrador não têm pontuação nem medalha', async () => {
    for (const nome of ['ricardo', 'admin']) {
      const r = await chamar('GET', '/perfil', { token: await entrar(nome) });
      assert.equal(r.corpo.gamificacao, undefined);
    }
  });
});
