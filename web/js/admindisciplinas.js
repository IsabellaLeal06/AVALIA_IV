// Listagem de disciplinas (Administrador): responsável (RN07), questões ativas e desativação (RN12).
(function () {
  const { $, el, mensagem, formatarNumero, etiquetaSituacao } = Avalia.ui;
  const { preencherTabela, botao, link, alternarSituacao, exibirMensagemGuardada, aoDigitar } = Avalia.cadastro;
  Avalia.montarPagina({ perfis: [Avalia.PERFIL.ADMINISTRADOR] });

  const corpo = $('#tabela-corpo');
  const aviso = $('#mensagem');
  exibirMensagemGuardada(aviso);

  async function carregarCards() {
    const todas = await Avalia.api.get('/disciplinas', { situacao: 'ativo' });
    $('#qtd-ativas').textContent = formatarNumero(todas.length);
    $('#qtd-sem-responsavel').textContent = formatarNumero(todas.filter((d) => !d.idResponsavel).length);
    $('#qtd-questoes').textContent = formatarNumero(todas.reduce((soma, d) => soma + d.qtdQuestoesAtivas, 0));
  }

  async function carregar() {
    Avalia.ui.linhaUnica(corpo, 6, 'Carregando...');
    try {
      const disciplinas = await Avalia.api.get('/disciplinas', { situacao: $('#filtro-situacao').value, busca: $('#filtro-busca').value });
      preencherTabela(corpo, disciplinas, [
        { valor: (d) => d.nome },
        { valor: (d) => d.nomeResponsavel || el('span', { class: 'etiqueta info', texto: 'Sem responsável' }) },
        { valor: (d) => d.qtdQuestoesAtivas },
        { valor: (d) => d.qtdTurmas, classe: 'ocultar-celular' },
        { valor: (d) => etiquetaSituacao(d.ativo) },
        {
          classe: 'acoes',
          valor: (d) =>
            el(
              'div',
              { class: 'acoes' },
              d.ativo ? link('Editar', `criardisciplina.html?id=${d.id}`, 'secundario', `Editar ${d.nome}`) : null,
              botao(d.ativo ? 'Desativar' : 'Reativar', () => situacao(d), d.ativo ? 'perigo' : 'secundario', `${d.ativo ? 'Desativar' : 'Reativar'} ${d.nome}`)
            )
        }
      ]);
      await carregarCards();
    } catch (erro) {
      Avalia.ui.linhaUnica(corpo, 6, erro.message);
    }
  }

  function situacao(disciplina) {
    return alternarSituacao({
      rota: '/disciplinas',
      item: disciplina,
      tipo: 'disciplina',
      nome: `a disciplina "${disciplina.nome}"`,
      avisoDesativar: `As ${disciplina.qtdQuestoesAtivas} questão(ões) ativa(s) desta disciplina deixarão de entrar no sorteio de novos simulados.`,
      elementoMensagem: aviso,
      aoConcluir: carregar
    });
  }

  $('#filtro-situacao').addEventListener('change', carregar);
  aoDigitar($('#filtro-busca'), carregar);
  carregar();
})();
