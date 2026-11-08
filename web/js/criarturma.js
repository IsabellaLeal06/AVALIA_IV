// Cadastro e edição de turma (RN06): nome + ano letivo e grade de disciplinas.
// O mesmo formulário serve para incluir e, com ?id=, para editar.
(async function () {
  const { $, mensagem, mostrarErros, limparErros, ocupado, parametro } = Avalia.ui;
  const { preencherMarcacao, valoresMarcados, guardarMensagem } = Avalia.cadastro;
  Avalia.montarPagina({ perfis: [Avalia.PERFIL.ADMINISTRADOR] });

  const formulario = $('#formulario');
  const aviso = $('#mensagem');
  const id = Number(parametro('id')) || null;

  try {
    const [disciplinas, turma] = await Promise.all([
      Avalia.api.get('/disciplinas', { situacao: 'todos' }),
      id ? Avalia.api.get(`/turmas/${id}`) : null
    ]);
    const vinculadas = new Set((turma?.disciplinas || []).map((d) => d.id));

    if (turma) {
      $('#titulo-acao').textContent = 'EDITAR';
      $('#subtitulo').textContent = 'Altere os dados da turma e a sua grade de disciplinas.';
      $('#botao-salvar').textContent = 'Salvar alterações';
      formulario.nome.value = turma.nome;
      formulario.anoLetivo.value = turma.anoLetivo;
    } else {
      formulario.anoLetivo.value = new Date().getFullYear();
    }

    // Só disciplinas ativas podem ser marcadas; vínculos antigos com disciplinas inativas são mantidos
    preencherMarcacao(
      $('#lista-disciplinas'),
      'disciplinas',
      disciplinas
        .filter((d) => d.ativo || vinculadas.has(d.id))
        .map((d) => ({
          valor: d.id,
          texto: d.nome,
          marcado: vinculadas.has(d.id),
          desabilitado: !d.ativo,
          ajuda: d.ativo ? '' : '(inativa, vínculo mantido)'
        })),
      'Nenhuma disciplina ativa cadastrada.'
    );
  } catch (erro) {
    mensagem(aviso, erro.message, 'erro');
  }

  formulario.addEventListener('submit', async (evento) => {
    evento.preventDefault();
    limparErros(formulario);
    const dados = {
      nome: formulario.nome.value,
      anoLetivo: Number(formulario.anoLetivo.value),
      // inclui os vínculos desabilitados (disciplina inativa), que continuam na grade
      disciplinas: valoresMarcados($('#lista-disciplinas'), 'disciplinas')
    };
    const botao = $('#botao-salvar');
    ocupado(botao, true, 'Salvando...');
    try {
      const resposta = id ? await Avalia.api.put(`/turmas/${id}`, dados) : await Avalia.api.post('/turmas', dados);
      guardarMensagem(resposta.mensagem);
      window.location.href = 'adminturmas.html';
    } catch (erro) {
      mensagem(aviso, erro.message, 'erro');
      mostrarErros(formulario, erro.campos);
      ocupado(botao, false);
    }
  });
})();
