// Acessibilidade (RNF06), incluída em TODAS as páginas por este arquivo único:
//  - VLibras (tradução para Libras), com o bloco fixo e o script oficial do governo.
(function () {
  const Avalia = (window.Avalia = window.Avalia || {});
  /** VLibras: bloco fixo e script oficial, inseridos antes do fechamento do body. */
  function iniciarVLibras() {
    if (document.querySelector('[vw]')) return;
    const bloco = document.createElement('div');
    bloco.setAttribute('vw', '');
    bloco.className = 'enabled';
    bloco.innerHTML =
      '<div vw-access-button class="active"></div>' +
      '<div vw-plugin-wrapper><div class="vw-plugin-top-wrapper"></div></div>';
    document.body.append(bloco);

    const script = document.createElement('script');
    script.src = 'https://vlibras.gov.br/app/vlibras-plugin.js';
    script.onload = () => {
      if (!window.VLibras) return;
      new window.VLibras.Widget('https://vlibras.gov.br/app');
      // O widget se inicia no evento load; se a página já carregou, dispara a inicialização
      if (document.readyState === 'complete' && typeof window.onload === 'function') window.onload();
    };
    document.body.append(script);
  }

  Avalia.acessibilidade = { iniciarVLibras };

  document.addEventListener('DOMContentLoaded', iniciarVLibras);
})();
