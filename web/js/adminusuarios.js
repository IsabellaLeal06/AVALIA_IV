// Listagem de usuários (Administrador). O CPF não aparece aqui: a API não o devolve na listagem (RNF02).
(function () {
  const { $, el, formatarNumero, etiquetaSituacao } = Avalia.ui;
  const { preencherTabela, botao, link, alternarSituacao, exibirMensagemGuardada, aoDigitar, preencherSelect } = Avalia.cadastro;
  const usuarioLogado = Avalia.montarPagina({ perfis: [Avalia.PERFIL.ADMINISTRADOR] });

  const corpo = $('#tabela-corpo');
  const aviso = $('#mensagem');
  exibirMensagemGuardada(aviso);

  async function carregarFiltroTurmas() {
    const turmas = await Avalia.api.get('/turmas', { situacao: 'todos' });
    preencherSelect($('#filtro-turma'), [
      { valor: '', texto: 'Todas' },
      ...turmas.map((t) => ({ valor: t.id, texto: `${t.nome} (${t.anoLetivo})${t.ativo ? '' : ' - inativa'}` }))
    ]);
  }

  async function carregarCards() {
    const ativos = await Avalia.api.get('/usuarios', { situacao: 'ativo' });
    const contar = (perfil) => formatarNumero(ativos.filter((u) => u.idPerfil === perfil).length);
    $('#qtd-alunos').textContent = contar(Avalia.PERFIL.ALUNO);
    $('#qtd-professores').textContent = contar(Avalia.PERFIL.PROFESSOR);
    $('#qtd-administradores').textContent = contar(Avalia.PERFIL.ADMINISTRADOR);
  }

  async function carregar() {
    Avalia.ui.linhaUnica(corpo, 6, 'Carregando...');
    try {
      const usuarios = await Avalia.api.get('/usuarios', {
        perfil: $('#filtro-perfil').value,
        turma: $('#filtro-turma').value,
        situacao: $('#filtro-situacao').value,
        busca: $('#filtro-busca').value
      });
      preencherTabela(corpo, usuarios, [
        { valor: (u) => u.nome },
        { valor: (u) => u.perfil },
        { valor: (u) => u.turma || '—' },
        { valor: (u) => u.email, classe: 'ocultar-celular' },
        { valor: (u) => etiquetaSituacao(u.ativo) },
        {
          classe: 'acoes',
          valor: (u) =>
            el(
              'div',
              { class: 'acoes' },
              u.ativo ? link('Editar', `criarusuario.html?id=${u.id}`, 'secundario', `Editar ${u.nome}`) : null,
              u.id === usuarioLogado.id
                ? null
                : botao(u.ativo ? 'Desativar' : 'Reativar', () => situacao(u), u.ativo ? 'perigo' : 'secundario', `${u.ativo ? 'Desativar' : 'Reativar'} ${u.nome}`)
            )
        }
      ]);
      await carregarCards();
    } catch (erro) {
      Avalia.ui.linhaUnica(corpo, 6, erro.message);
    }
  }

  function situacao(usuario) {
    return alternarSituacao({
      rota: '/usuarios',
      item: usuario,
      tipo: 'usuário',
      nome: `o usuário "${usuario.nome}"`,
      avisoDesativar: 'Ele não conseguirá mais entrar no sistema.',
      elementoMensagem: aviso,
      aoConcluir: carregar
    });
  }

  ['#filtro-perfil', '#filtro-turma', '#filtro-situacao'].forEach((s) => $(s).addEventListener('change', carregar));
  aoDigitar($('#filtro-busca'), carregar);
  carregarFiltroTurmas().catch(() => {});
  carregar();
})();
