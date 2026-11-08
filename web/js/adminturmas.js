// Listagem de turmas (Administrador): filtro de situação para chegar às inativas e desativar/reativar (RN12).
(function () {
  const { $, el, mensagem, formatarNumero, etiquetaSituacao } = Avalia.ui;
  const { preencherTabela, botao, link, alternarSituacao, exibirMensagemGuardada, aoDigitar } = Avalia.cadastro;
  Avalia.montarPagina({ perfis: [Avalia.PERFIL.ADMINISTRADOR] });

  const corpo = $('#tabela-corpo');
  const aviso = $('#mensagem');
  exibirMensagemGuardada(aviso);

  async function carregarCards() {
    const todas = await Avalia.api.get('/turmas', { situacao: 'todos' });
    const ativas = todas.filter((t) => t.ativo);
    $('#qtd-ativas').textContent = formatarNumero(ativas.length);
    $('#qtd-inativas').textContent = formatarNumero(todas.length - ativas.length);
    $('#qtd-alunos').textContent = formatarNumero(ativas.reduce((soma, t) => soma + t.qtdAlunosAtivos, 0));
  }

  async function carregar() {
    Avalia.ui.linhaUnica(corpo, 6, 'Carregando...');
    try {
      const turmas = await Avalia.api.get('/turmas', { situacao: $('#filtro-situacao').value, busca: $('#filtro-busca').value });
      preencherTabela(corpo, turmas, [
        { valor: (t) => t.nome },
        { valor: (t) => t.anoLetivo },
        { valor: (t) => t.qtdAlunosAtivos },
        { valor: (t) => t.qtdDisciplinas, classe: 'ocultar-celular' },
        { valor: (t) => etiquetaSituacao(t.ativo) },
        {
          classe: 'acoes',
          valor: (t) =>
            el(
              'div',
              { class: 'acoes' },
              t.ativo ? link('Editar', `criarturma.html?id=${t.id}`, 'secundario', `Editar ${t.nome}`) : null,
              botao(t.ativo ? 'Desativar' : 'Reativar', () => situacao(t), t.ativo ? 'perigo' : 'secundario', `${t.ativo ? 'Desativar' : 'Reativar'} ${t.nome}`)
            )
        }
      ]);
      await carregarCards();
    } catch (erro) {
      Avalia.ui.linhaUnica(corpo, 6, erro.message);
    }
  }

  function situacao(turma) {
    // RN12: alerta quantos alunos ativos estão vinculados, mas não impede
    const aviso2 =
      turma.qtdAlunosAtivos > 0
        ? `Atenção: há ${turma.qtdAlunosAtivos} aluno(s) ativo(s) nesta turma. Eles NÃO serão desativados e continuarão com acesso ao sistema.`
        : 'A turma deixará de aparecer no cadastro de novos alunos.';
    return alternarSituacao({
      rota: '/turmas',
      item: turma,
      tipo: 'turma',
      nome: `a turma "${turma.nome} (${turma.anoLetivo})"`,
      avisoDesativar: aviso2,
      elementoMensagem: aviso,
      aoConcluir: carregar
    });
  }

  $('#filtro-situacao').addEventListener('change', carregar);
  aoDigitar($('#filtro-busca'), carregar);
  carregar();
})();
