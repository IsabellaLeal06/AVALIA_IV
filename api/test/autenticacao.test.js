// TST01: autenticação, conta desativada, mensagem genérica e recuperação de senha (RF01, RN02, RN03, RN04).
const apoio = require('./apoio');
const { test, describe, before, after } = require('node:test');
const assert = require('node:assert/strict');
const bcrypt = require('bcryptjs');

const { chamar, USUARIOS, sql, banco } = apoio;
const GENERICA = 'E-mail ou senha inválidos.';

describe('autenticação', { skip: apoio.motivoPulo }, () => {
  before(apoio.iniciar);
  after(apoio.encerrar);

  test('login certo devolve token, perfil e senhaInicial quando a senha é o CPF', async () => {
    const { status, corpo } = await chamar('POST', '/auth/login', {
      corpo: { email: ' ADMIN@avaliamais.test ', senha: USUARIOS.admin.cpf }
    });
    assert.equal(status, 200);
    assert.ok(corpo.token);
    assert.equal(corpo.usuario.idPerfil, 3);
    assert.equal(corpo.usuario.perfil, 'Administrador');
    assert.equal(corpo.senhaInicial, true);
    assert.equal(corpo.usuario.cpf, undefined);
  });

  test('senha errada, e-mail inexistente e conta desativada recebem a mesma mensagem', async () => {
    const senhaErrada = await chamar('POST', '/auth/login', { corpo: { email: USUARIOS.admin.email, senha: 'errada123' } });
    const semEmail = await chamar('POST', '/auth/login', { corpo: { email: 'ninguem@avaliamais.test', senha: 'qualquer1' } });
    const desativada = await chamar('POST', '/auth/login', {
      corpo: { email: USUARIOS.gabriela.email, senha: USUARIOS.gabriela.cpf }
    });
    for (const r of [senhaErrada, semEmail, desativada]) {
      assert.equal(r.status, 401);
      assert.equal(r.corpo.erro.mensagem, GENERICA);
    }
  });

  test('não há bloqueio por plataforma (RN26): aluno, professor e admin entram', async () => {
    for (const nome of ['ana', 'ricardo', 'admin']) {
      const { status } = await chamar('POST', '/auth/login', { corpo: { email: USUARIOS[nome].email, senha: USUARIOS[nome].cpf } });
      assert.equal(status, 200, nome);
    }
  });

  test('rota protegida sem token ou com token inválido responde 401', async () => {
    assert.equal((await chamar('GET', '/perfil')).status, 401);
    assert.equal((await chamar('GET', '/perfil', { token: 'abc.def.ghi' })).status, 401);
  });

  test('erro não expõe detalhes internos', async () => {
    const r = await chamar('GET', '/usuarios/abc', { token: await apoio.entrar('admin') });
    assert.equal(r.status, 404);
    assert.deepEqual(Object.keys(r.corpo), ['erro']);
    assert.doesNotMatch(JSON.stringify(r.corpo), /dbo\.|stack|SELECT/i);
  });
});

describe('recuperação de senha (RN03)', { skip: apoio.motivoPulo }, () => {
  const usuario = USUARIOS.daniel;
  const mensagens = [];

  before(async () => {
    await apoio.iniciar();
    apoio.emailService.definirEnviador(async (m) => mensagens.push(m));
  });

  after(async () => {
    // Devolve a senha inicial (CPF) para os outros testes
    await banco.executar(
      'UPDATE dbo.Usuario SET Senha_Usuario = @Senha, TokenRecuperacao_Usuario = NULL, ValidadeToken_Usuario = NULL WHERE Id_Usuario = @Id',
      { Senha: [banco.sql.VarChar(200), bcrypt.hashSync(usuario.cpf, 10)], Id: [banco.sql.Int, usuario.id] }
    );
    await apoio.encerrar();
  });

  async function pedirCodigo() {
    const antes = mensagens.length;
    const r = await chamar('POST', '/auth/recuperar-senha', { corpo: { email: usuario.email } });
    assert.equal(r.status, 200);
    await new Promise((resolve) => setTimeout(resolve, 50));
    assert.equal(mensagens.length, antes + 1);
    return mensagens.at(-1).texto.match(/(\d{6})/)[1];
  }

  test('a resposta é a mesma para e-mail cadastrado ou não', async () => {
    const existe = await chamar('POST', '/auth/recuperar-senha', { corpo: { email: usuario.email } });
    const naoExiste = await chamar('POST', '/auth/recuperar-senha', { corpo: { email: 'nao.existe@avaliamais.test' } });
    assert.equal(existe.status, 200);
    assert.deepEqual(existe.corpo, naoExiste.corpo);
  });

  test('o código fica gravado como hash, nunca em texto', async () => {
    const codigo = await pedirCodigo();
    const [linha] = await sql('SELECT TokenRecuperacao_Usuario AS t FROM dbo.Usuario WHERE Id_Usuario = @Id', {
      Id: [banco.sql.Int, usuario.id]
    });
    assert.ok(linha.t);
    assert.notEqual(linha.t, codigo);
  });

  test('código válido: valida, troca a senha e não pode ser reutilizado', async () => {
    const codigo = await pedirCodigo();
    const validar = await chamar('GET', `/auth/validar-codigo?email=${encodeURIComponent(usuario.email)}&codigo=${codigo}`);
    assert.equal(validar.status, 200);

    const trocar = await chamar('POST', '/auth/nova-senha', {
      corpo: { email: usuario.email, codigo, novaSenha: 'NovaSenha2026', confirmacao: 'NovaSenha2026' }
    });
    assert.equal(trocar.status, 200);

    const login = await chamar('POST', '/auth/login', { corpo: { email: usuario.email, senha: 'NovaSenha2026' } });
    assert.equal(login.status, 200);
    assert.equal(login.corpo.senhaInicial, false);

    const reutilizar = await chamar('POST', '/auth/nova-senha', {
      corpo: { email: usuario.email, codigo, novaSenha: 'OutraSenha2026', confirmacao: 'OutraSenha2026' }
    });
    assert.equal(reutilizar.status, 400);
  });

  test('pedido novo invalida o código anterior', async () => {
    const primeiro = await pedirCodigo();
    const segundo = await pedirCodigo();
    if (primeiro !== segundo) {
      const r = await chamar('GET', `/auth/validar-codigo?email=${encodeURIComponent(usuario.email)}&codigo=${primeiro}`);
      assert.equal(r.status, 400);
    }
    const r2 = await chamar('GET', `/auth/validar-codigo?email=${encodeURIComponent(usuario.email)}&codigo=${segundo}`);
    assert.equal(r2.status, 200);
  });

  test('código expirado é recusado', async () => {
    const codigo = await pedirCodigo();
    await banco.executar(
      "UPDATE dbo.Usuario SET ValidadeToken_Usuario = DATEADD(DAY, -1, CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'E. South America Standard Time' AS DATETIME)) WHERE Id_Usuario = @Id",
      { Id: [banco.sql.Int, usuario.id] }
    );
    const r = await chamar('GET', `/auth/validar-codigo?email=${encodeURIComponent(usuario.email)}&codigo=${codigo}`);
    assert.equal(r.status, 400);
  });

  test('alterar a própria senha exige a senha atual (RN04)', async () => {
    const token = (await chamar('POST', '/auth/login', { corpo: { email: USUARIOS.bruno.email, senha: USUARIOS.bruno.cpf } })).corpo.token;
    const errada = await chamar('POST', '/auth/alterar-senha', {
      token,
      corpo: { senhaAtual: 'errada', novaSenha: 'Bruno2026x', confirmacao: 'Bruno2026x' }
    });
    assert.equal(errada.status, 400);
    assert.ok(errada.corpo.erro.campos.senhaAtual);

    const fraca = await chamar('POST', '/auth/alterar-senha', {
      token,
      corpo: { senhaAtual: USUARIOS.bruno.cpf, novaSenha: 'curta', confirmacao: 'curta' }
    });
    assert.equal(fraca.status, 400);
    assert.ok(fraca.corpo.erro.campos.novaSenha);
  });
});
