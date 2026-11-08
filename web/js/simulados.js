// Histórico de simulados do Aluno (RN20), vindo da API e paginado. Nada fica no localStorage.
(function () {
  const { $, el, formatarPercentual, formatarDataHora, linhaUnica } = Avalia.ui;
  Avalia.montarPagina({ perfis: [Avalia.PERFIL.ALUNO] });

  const corpo = $('#tabela-corpo');
  let pagina = 1;

  async function carregar() {
    linhaUnica(corpo, 5, 'Carregando...');
    try {
      const resposta = await Avalia.api.get('/simulados', { pagina, tamanho: 10 });
      if (!resposta.itens.length) {
        linhaUnica(corpo, 5, 'Nenhum simulado realizado ainda.');
      } else {
        corpo.replaceChildren(
          ...resposta.itens.map((s) => {
            const abrir = () => (window.location.href = `analise.html?id=${s.id}`);
            return el(
              'tr',
              { class: 'clicavel', onclick: abrir },
              el('td', {}, el('a', { href: `analise.html?id=${s.id}`, texto: s.titulo })),
              el('td', { texto: `${s.totalAcertos}/${s.totalQuestoes}` }),
              el('td', { texto: formatarPercentual(s.percentual) }),
              el('td', { class: 'ocultar-celular', texto: `+${s.pontosObtidos}` }),
              el('td', { texto: formatarDataHora(s.dataRealizacao) })
            );
          })
        );
      }
      $('#info-pagina').textContent = `Página ${resposta.pagina} de ${resposta.totalPaginas} · ${resposta.total} simulado(s)`;
      $('#anterior').disabled = resposta.pagina <= 1;
      $('#proxima').disabled = resposta.pagina >= resposta.totalPaginas;
    } catch (erro) {
      linhaUnica(corpo, 5, erro.message);
    }
  }

  $('#anterior').addEventListener('click', () => {
    pagina -= 1;
    carregar();
  });
  $('#proxima').addEventListener('click', () => {
    pagina += 1;
    carregar();
  });
  carregar();
})();
