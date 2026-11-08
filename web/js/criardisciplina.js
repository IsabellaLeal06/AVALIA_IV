// Cadastro e edição de disciplina. Responsável: UM select de professores com "Sem responsável" (RN07).
(async function () {
  const { $, mensagem, mostrarErros, limparErros, ocupado, parametro } = Avalia.ui;
  const { preencherSelect, preencherMarcacao, valoresMarcados, guardarMensagem } = Avalia.cadastro;
  Avalia.montarPagina({ perfis: [Avalia.PERFIL.ADMINISTRADOR] });

  const formulario = $('#formulario');
  const aviso = $('#mensagem');
  const id = Number(parametro('id')) || null;

  try {
    const [professores, turmas, disciplina] = await Promise.all([
      Avalia.api.get('/usuarios', { perfil: Avalia.PERFIL.PROFESSOR, situacao: 'todos' }),
      Avalia.api.get('/turmas', { situacao: 'todos' }),
      id ? Avalia.api.get(`/disciplinas/${id}`) : null
    ]);
    const vinculadas = new Set((disciplina?.turmas || []).map((t) => t.id));
    const idResponsavel = disciplina?.idResponsavel ?? '';

    if (disciplina) {
      $('#titulo-acao').textContent = 'EDITAR';
      $('#subtitulo').textContent = 'Altere os dados da disciplina, o responsável e as turmas.';
      $('#botao-salvar').textContent = 'Salvar alterações';
      formulario.nome.value = disciplina.nome;
      formulario.descricao.value = disciplina.descricao || '';
    }

    // Professores ativos; o responsável atual aparece mesmo se estiver desativado
    preencherSelect(
      formulario.idResponsavel,
      [
        { valor: '', texto: 'Sem responsável' },
        ...professores
          .filter((p) => p.ativo || p.id === idResponsavel)
          .map((p) => ({ valor: p.id, texto: p.ativo ? p.nome : `${p.nome} (desativado)` }))
      ],
      idResponsavel
    );

    preencherMarcacao(
      $('#lista-turmas'),
      'turmas',
      turmas
        .filter((t) => t.ativo || vinculadas.has(t.id))
        .map((t) => ({
          valor: t.id,
          texto: `${t.nome} (${t.anoLetivo})`,
          marcado: vinculadas.has(t.id),
          desabilitado: !t.ativo,
          ajuda: t.ativo ? '' : '(inativa, vínculo mantido)'
        })),
      'Nenhuma turma ativa cadastrada.'
    );
  } catch (erro) {
    mensagem(aviso, erro.message, 'erro');
  }

  formulario.addEventListener('submit', async (evento) => {
    evento.preventDefault();
    limparErros(formulario);
    const dados = {
      nome: formulario.nome.value,
      descricao: formulario.descricao.value,
      idResponsavel: formulario.idResponsavel.value || null,
      // inclui os vínculos desabilitados (turma inativa), que são mantidos
      turmas: valoresMarcados($('#lista-turmas'), 'turmas')
    };
    const botao = $('#botao-salvar');
    ocupado(botao, true, 'Salvando...');
    try {
      const resposta = id ? await Avalia.api.put(`/disciplinas/${id}`, dados) : await Avalia.api.post('/disciplinas', dados);
      guardarMensagem(resposta.mensagem);
      window.location.href = 'admindisciplinas.html';
    } catch (erro) {
      mensagem(aviso, erro.message, 'erro');
      mostrarErros(formulario, erro.campos);
      ocupado(botao, false);
    }
  });
})();
