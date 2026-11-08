// Listagem do banco de questões, usada pelo Administrador (adminquestoes.html) e pelo Professor
// (professorquestoes.html). A coluna "Cadastrada por" substitui a da alternativa correta: o gabarito
// não é informação de gestão (RN08). O escopo do Professor é aplicado pela API.
(function () {
  const { $, el, formatarNumero, etiquetaSituacao } = Avalia.ui;
  const { preencherTabela, botao, link, alternarSituacao, exibirMensagemGuardada, aoDigitar, preencherSelect } = Avalia.cadastro;
  const { ADMINISTRADOR, PROFESSOR } = Avalia.PERFIL;
  const usuario = Avalia.montarPagina({ perfis: [ADMINISTRADOR, PROFESSOR] });
  const ehAdmin = usuario.idPerfil === ADMINISTRADOR;

  const corpo = $('#tabela-corpo');
  const aviso = $('#mensagem');
  exibirMensagemGuardada(aviso);

  async function carregarDisciplinas() {
    const disciplinas = await Avalia.api.get('/disciplinas', { situacao: 'todos' });
    preencherSelect($('#filtro-disciplina'), [
      { valor: '', texto: 'Todas' },
      ...disciplinas.map((d) => ({ valor: d.id, texto: d.ativo ? d.nome : `${d.nome} (inativa)` }))
    ]);
    $('#qtd-disciplinas').textContent = formatarNumero(disciplinas.filter((d) => d.ativo).length);
    if (!ehAdmin && !disciplinas.length) {
      Avalia.ui.mensagem(aviso, 'Você ainda não é responsável por nenhuma disciplina. Fale com o Administrador.', 'info');
    }
  }

  async function carregarCards() {
    const todas = await Avalia.api.get('/questoes', { situacao: 'todos' });
    $('#qtd-ativas').textContent = formatarNumero(todas.filter((q) => q.ativo).length);
    $('#qtd-inativas').textContent = formatarNumero(todas.filter((q) => !q.ativo).length);
  }

  function acoes(q) {
    const nome = `questão nº ${q.id}`;
    const botoes = [];
    if (q.ativo) {
      botoes.push(link('Editar', `criarquestao.html?id=${q.id}`, 'secundario', `Editar ${nome}`));
      botoes.push(botao('Desativar', () => situacao(q), 'perigo', `Desativar ${nome}`));
    } else if (ehAdmin) {
      // RN12: a reativação é do Administrador
      botoes.push(botao('Reativar', () => situacao(q), 'secundario', `Reativar ${nome}`));
    }
    return el('div', { class: 'acoes' }, botoes);
  }

  async function carregar() {
    Avalia.ui.linhaUnica(corpo, 5, 'Carregando...');
    try {
      const questoes = await Avalia.api.get('/questoes', {
        disciplina: $('#filtro-disciplina').value,
        situacao: $('#filtro-situacao').value,
        busca: $('#filtro-busca').value
      });
      preencherTabela(corpo, questoes, [
        { valor: (q) => el('span', {}, el('strong', { texto: `Nº ${q.id} · ` }), q.enunciado) },
        { valor: (q) => q.disciplina },
        { valor: (q) => q.autor, classe: 'ocultar-celular' },
        { valor: (q) => etiquetaSituacao(q.ativo) },
        { classe: 'acoes', valor: acoes }
      ]);
      await carregarCards();
    } catch (erro) {
      Avalia.ui.linhaUnica(corpo, 5, erro.message);
    }
  }

  function situacao(questao) {
    return alternarSituacao({
      rota: '/questoes',
      item: questao,
      tipo: 'questão',
      nome: `a questão nº ${questao.id}`,
      avisoDesativar: 'Ela deixará de entrar no sorteio de novos simulados. Os simulados já realizados continuam com ela no histórico.',
      elementoMensagem: aviso,
      aoConcluir: carregar
    });
  }

  ['#filtro-disciplina', '#filtro-situacao'].forEach((s) => $(s).addEventListener('change', carregar));
  aoDigitar($('#filtro-busca'), carregar);
  carregarDisciplinas().catch((erro) => Avalia.ui.mensagem(aviso, erro.message, 'erro'));
  carregar();
})();
