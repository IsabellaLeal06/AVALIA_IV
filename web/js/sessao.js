// Sessão do usuário: token, perfil e guarda de página.
// Incluído em todas as páginas internas: sem token (ou com token expirado) volta para o login.
// A guarda só melhora a navegação; quem protege os dados é a API, que confere o token em cada requisição.
(function () {
  const Avalia = (window.Avalia = window.Avalia || {});
  const CHAVE = 'avalia.sessao';

  Avalia.PERFIL = Object.freeze({ ALUNO: 1, PROFESSOR: 2, ADMINISTRADOR: 3 });

  const INICIO = {
    1: 'indexdashboar.html',
    2: 'dashboardprofessor.html',
    3: 'dashboardadmin.html'
  };

  const PAGINA_PERFIL = {
    1: 'perfil.html',
    2: 'perfilprofessor.html',
    3: 'perfiladmin.html'
  };

  /** Caminho de uma página da pasta pages, de qualquer lugar do site. */
  function pagina(nome) {
    const dentroDePages = /\/pages\//.test(window.location.pathname);
    return dentroDePages ? nome : `pages/${nome}`;
  }

  function lerPayload(token) {
    try {
      const base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
      return JSON.parse(decodeURIComponent(escape(atob(base64))));
    } catch {
      return null;
    }
  }

  Avalia.sessao = {
    pagina,

    salvar(dados) {
      localStorage.setItem(CHAVE, JSON.stringify(dados));
    },

    obter() {
      try {
        return JSON.parse(localStorage.getItem(CHAVE));
      } catch {
        return null;
      }
    },

    token() {
      return this.obter()?.token || null;
    },

    usuario() {
      return this.obter()?.usuario || null;
    },

    /** Token ausente ou com prazo vencido. */
    expirada() {
      const token = this.token();
      const payload = token && lerPayload(token);
      return !payload || !payload.exp || payload.exp * 1000 <= Date.now();
    },

    limpar() {
      localStorage.removeItem(CHAVE);
    },

    inicio(idPerfil) {
      return pagina(INICIO[idPerfil] || 'login.html');
    },

    paginaPerfil(idPerfil) {
      return pagina(PAGINA_PERFIL[idPerfil]);
    },

    /** RN02: a troca da senha inicial é recomendada (não obrigatória). */
    recomendarTrocaSenha() {
      return Boolean(this.obter()?.senhaInicial);
    },

    senhaTrocada() {
      const dados = this.obter();
      if (dados) this.salvar({ ...dados, senhaInicial: false });
    },

    /**
     * Guarda de página. Sem sessão válida: vai para o login. Perfil sem acesso à página:
     * vai para o início do próprio perfil. Devolve o usuário logado.
     */
    exigir(perfisPermitidos) {
      if (this.expirada()) {
        this.limpar();
        window.location.replace(pagina('login.html?motivo=sessao'));
        throw new Error('Sessão expirada');
      }
      const usuario = this.usuario();
      if (perfisPermitidos && !perfisPermitidos.includes(usuario.idPerfil)) {
        window.location.replace(this.inicio(usuario.idPerfil));
        throw new Error('Perfil sem acesso a esta página');
      }
      return usuario;
    },

    sair() {
      this.limpar();
      window.location.href = pagina('login.html');
    }
  };
})();
