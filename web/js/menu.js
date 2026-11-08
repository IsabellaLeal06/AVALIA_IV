// Menu por perfil: os itens dos três perfis ficam num ÚNICO objeto, e o menu é montado
// a partir do perfil que veio no login. Também monta o cabeçalho das páginas internas.
(function () {
  const Avalia = (window.Avalia = window.Avalia || {});

  // ativoEm: outras páginas em que o item aparece marcado (formulários e telas filhas)
  const MENUS = {
    1: [
      { texto: 'Dashboard', pagina: 'indexdashboar.html' },
      { texto: 'Simulados', pagina: 'simulados.html', ativoEm: ['analise.html', 'resultado.html'] },
      { texto: 'Desempenho', pagina: 'desempenho.html' },
      { texto: 'Criar Simulado', pagina: 'criarsimulados.html', ativoEm: ['instrucsimulado.html'] },
      { texto: 'Perfil', pagina: 'perfil.html', ativoEm: ['alterarsenha.html'] }
    ],
    2: [
      { texto: 'Dashboard', pagina: 'dashboardprofessor.html' },
      { texto: 'Turmas', pagina: 'turmas.html' },
      { texto: 'Questões', pagina: 'professorquestoes.html', ativoEm: ['criarquestao.html'] },
      { texto: 'Perfil', pagina: 'perfilprofessor.html', ativoEm: ['alterarsenha.html'] }
    ],
    3: [
      { texto: 'Dashboard', pagina: 'dashboardadmin.html' },
      { texto: 'Turmas', pagina: 'adminturmas.html', ativoEm: ['criarturma.html'] },
      { texto: 'Disciplinas', pagina: 'admindisciplinas.html', ativoEm: ['criardisciplina.html'] },
      { texto: 'Questões', pagina: 'adminquestoes.html', ativoEm: ['criarquestao.html'] },
      { texto: 'Usuários', pagina: 'adminusuarios.html', ativoEm: ['criarusuario.html'] },
      { texto: 'Medalhas', pagina: 'adminmedalhas.html', ativoEm: ['criarmedalha.html'] },
      { texto: 'Perfil', pagina: 'perfiladmin.html', ativoEm: ['alterarsenha.html'] }
    ]
  };

  function paginaAtual() {
    return window.location.pathname.split('/').pop() || 'index.html';
  }

  async function confirmarSaida() {
    const sair = await Avalia.ui.confirmar({ titulo: 'Sair da conta', mensagem: 'Deseja mesmo sair?', confirmar: 'Sair' });
    if (sair) Avalia.sessao.sair();
  }

  function montarCabecalho(usuario) {
    const { el } = Avalia.ui;
    const cabecalho = document.getElementById('cabecalho');
    if (!cabecalho) return;
    cabecalho.className = 'cabecalho';
    cabecalho.replaceChildren(
      el(
        'a',
        { class: 'logo', href: Avalia.sessao.inicio(usuario.idPerfil), 'aria-label': 'Avalia+, ir para o início' },
        el('img', { class: 'logo-imagem', src: '../assets/img/logo-avalia.svg', alt: 'Avalia+' })
      ),
      el(
        'div',
        { class: 'cabecalho-direita' },
        el(
          'a',
          { class: 'usuario-logado', href: Avalia.sessao.paginaPerfil(usuario.idPerfil), title: 'Meu perfil' },
          el('span', {}, Avalia.ui.primeiroNome(usuario.nome), el('small', { texto: usuario.perfil })),
          el('span', { class: 'avatar', 'aria-hidden': 'true', texto: Avalia.ui.iniciais(usuario.nome) })
        )
      )
    );
  }

  function montarMenu(usuario) {
    const { el } = Avalia.ui;
    const lateral = document.getElementById('menu-lateral');
    if (!lateral) return;
    const atual = paginaAtual();
    const itens = MENUS[usuario.idPerfil] || [];
    lateral.className = 'menu-lateral';
    lateral.setAttribute('aria-label', 'Menu principal');
    lateral.replaceChildren(
      el(
        'nav',
        {},
        itens.map((item) => {
          const ativo = item.pagina === atual || (item.ativoEm || []).includes(atual);
          return el('a', { href: item.pagina, 'aria-current': ativo ? 'page' : null, texto: item.texto });
        })
      ),
      el('button', { type: 'button', class: 'sair', texto: 'Sair', onclick: confirmarSaida })
    );
  }

  function linkPularConteudo() {
    if (document.querySelector('.pular-conteudo')) return;
    const principal = document.querySelector('main');
    if (principal && !principal.id) principal.id = 'conteudo';
    document.body.prepend(Avalia.ui.el('a', { class: 'pular-conteudo', href: '#conteudo', texto: 'Pular para o conteúdo' }));
  }

  /**
   * Monta uma página interna: confere a sessão e o perfil, o cabeçalho e o menu.
   * Uso: const usuario = Avalia.montarPagina({ perfis: [Avalia.PERFIL.ALUNO] });
   */
  Avalia.montarPagina = function ({ perfis, semMenu = false } = {}) {
    const usuario = Avalia.sessao.exigir(perfis);
    montarCabecalho(usuario);
    if (!semMenu) montarMenu(usuario);
    linkPularConteudo();
    return usuario;
  };

  /** RN02: recomenda (sem obrigar) a troca da senha inicial, nos dashboards. */
  Avalia.avisoSenhaInicial = function (container) {
    if (!container || !Avalia.sessao.recomendarTrocaSenha()) return;
    const { el } = Avalia.ui;
    const aviso = el(
      'div',
      { class: 'mensagem alerta', role: 'status' },
      'Você ainda usa a senha inicial (o seu CPF). Recomendamos trocá-la. ',
      el('a', { href: 'alterarsenha.html', texto: 'Trocar senha agora' }),
      ' · ',
      el('button', {
        type: 'button',
        class: 'botao pequeno contorno',
        texto: 'Agora não',
        onclick: () => {
          Avalia.sessao.senhaTrocada();
          aviso.remove();
        }
      })
    );
    container.prepend(aviso);
  };

  Avalia.menu = { MENUS, confirmarSaida };
})();
