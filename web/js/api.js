// ÚNICO ponto de chamada à API. Nenhuma outra parte da web usa fetch diretamente.
(function () {
  const Avalia = (window.Avalia = window.Avalia || {});

  class ErroApi extends Error {
    constructor(status, mensagem, campos) {
      super(mensagem);
      this.status = status;
      this.campos = campos || null;
    }
  }

  function base() {
    const configurada = (window.AVALIA_CONFIG && window.AVALIA_CONFIG.apiUrl) || '';
    if (configurada) return configurada.replace(/\/+$/, '');
    // Página aberta direto do disco (file://): usa a API local
    return window.location.protocol === 'file:' ? 'http://localhost:3000' : '';
  }

  function montarUrl(rota, parametros) {
    const busca = new URLSearchParams();
    for (const [chave, valor] of Object.entries(parametros || {})) {
      if (valor !== undefined && valor !== null && valor !== '') busca.set(chave, valor);
    }
    const consulta = busca.toString();
    return base() + rota + (consulta ? `?${consulta}` : '');
  }

  async function requisicao(metodo, rota, opcoes = {}) {
    const { corpo, parametros, formulario, manterAberta = false } = opcoes;
    const token = Avalia.sessao && Avalia.sessao.token();
    const cabecalhos = {};
    if (token) cabecalhos.Authorization = `Bearer ${token}`;
    if (corpo !== undefined) cabecalhos['Content-Type'] = 'application/json';

    let resposta;
    try {
      resposta = await fetch(montarUrl(rota, parametros), {
        method: metodo,
        headers: cabecalhos,
        body: formulario || (corpo !== undefined ? JSON.stringify(corpo) : undefined),
        // keepalive: a requisição continua mesmo se a página for fechada (finalização do simulado)
        keepalive: manterAberta
      });
    } catch {
      throw new ErroApi(0, 'Não foi possível falar com o servidor. Confira a internet e se a API está rodando.');
    }

    let dados = null;
    const texto = await resposta.text();
    if (texto) {
      try {
        dados = JSON.parse(texto);
      } catch {
        dados = null;
      }
    }

    if (!resposta.ok) {
      const erro = (dados && dados.erro) || {};
      // Sessão vencida ou conta desativada: volta para o login
      if (resposta.status === 401 && token && rota !== '/auth/login') {
        Avalia.sessao.limpar();
        window.location.replace(Avalia.sessao.pagina('login.html?motivo=sessao'));
      }
      throw new ErroApi(resposta.status, erro.mensagem || 'Não foi possível concluir a operação.', erro.campos);
    }
    return dados;
  }

  Avalia.ErroApi = ErroApi;
  Avalia.api = {
    get: (rota, parametros) => requisicao('GET', rota, { parametros }),
    post: (rota, corpo, opcoes = {}) => requisicao('POST', rota, { corpo: corpo ?? {}, ...opcoes }),
    put: (rota, corpo) => requisicao('PUT', rota, { corpo: corpo ?? {} }),
    patch: (rota, corpo) => requisicao('PATCH', rota, { corpo: corpo ?? {} }),
    excluir: (rota) => requisicao('DELETE', rota),

    /** Upload de imagem (operação própria, antes de gravar a questão ou a medalha). */
    enviarImagem(arquivo, tipo = 'questoes') {
      const formulario = new FormData();
      formulario.append('imagem', arquivo);
      return requisicao('POST', '/upload/imagem', { formulario, parametros: { tipo } });
    },

    /** Endereço completo de um arquivo servido pela API (imagens). */
    url(caminho) {
      if (!caminho) return '';
      if (/^https?:\/\//i.test(caminho)) return caminho;
      return `${base()}/${String(caminho).replace(/^\/+/, '')}`;
    }
  };
})();
