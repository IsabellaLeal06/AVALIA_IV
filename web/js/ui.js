// Utilitários de interface: montagem segura de elementos, formatação, mensagens na tela,
// destaque de campos com erro e diálogo de confirmação (sem alert/confirm do navegador).
(function () {
  const Avalia = (window.Avalia = window.Avalia || {});

  /** Cria um elemento. Texto sempre por textContent (nunca HTML vindo da API). */
  function el(tag, atributos = {}, ...filhos) {
    const elemento = document.createElement(tag);
    for (const [chave, valor] of Object.entries(atributos || {})) {
      if (valor === undefined || valor === null || valor === false) continue;
      if (chave === 'class') elemento.className = valor;
      else if (chave === 'texto') elemento.textContent = valor;
      else if (chave.startsWith('on') && typeof valor === 'function') elemento.addEventListener(chave.slice(2), valor);
      else if (chave === 'dados') Object.assign(elemento.dataset, valor);
      else elemento.setAttribute(chave, valor === true ? '' : valor);
    }
    for (const filho of filhos.flat()) {
      if (filho === null || filho === undefined || filho === false) continue;
      elemento.append(filho instanceof Node ? filho : document.createTextNode(String(filho)));
    }
    return elemento;
  }

  const $ = (seletor, raiz = document) => raiz.querySelector(seletor);

  function formatarData(valor) {
    if (!valor) return '—';
    const [ano, mes, dia] = String(valor).slice(0, 10).split('-');
    return `${dia}/${mes}/${ano}`;
  }

  function formatarDataHora(valor) {
    if (!valor) return '—';
    return `${formatarData(valor)} às ${String(valor).slice(11, 16)}`;
  }

  function formatarPercentual(valor) {
    if (valor === null || valor === undefined || Number.isNaN(Number(valor))) return '—';
    return `${Number(valor).toLocaleString('pt-BR', { maximumFractionDigits: 1 })}%`;
  }

  function formatarNumero(valor) {
    return Number(valor || 0).toLocaleString('pt-BR');
  }

  function primeiroNome(nome) {
    return String(nome || '').trim().split(/\s+/)[0] || '';
  }

  function iniciais(nome) {
    const partes = String(nome || '').trim().split(/\s+/);
    return ((partes[0]?.[0] || '') + (partes.length > 1 ? partes[partes.length - 1][0] : '')).toUpperCase();
  }

  /** Mensagem na tela (substitui o alert). tipo: sucesso | erro | alerta | info */
  function mensagem(elemento, texto, tipo = 'info') {
    if (!elemento) return;
    if (!texto) {
      elemento.hidden = true;
      elemento.textContent = '';
      return;
    }
    elemento.className = `mensagem ${tipo}`;
    elemento.setAttribute('role', tipo === 'erro' || tipo === 'alerta' ? 'alert' : 'status');
    elemento.textContent = texto;
    elemento.hidden = false;
  }

  function limparErros(formulario) {
    formulario.querySelectorAll('.com-erro').forEach((c) => c.classList.remove('com-erro'));
    formulario.querySelectorAll('.erro-campo').forEach((e) => e.remove());
    formulario.querySelectorAll('[aria-invalid]').forEach((c) => c.removeAttribute('aria-invalid'));
  }

  /** Destaca os campos devolvidos pela API (campos: { nome: 'mensagem' }). */
  function mostrarErros(formulario, campos) {
    limparErros(formulario);
    let primeiro = null;
    for (const [nome, texto] of Object.entries(campos || {})) {
      // data-campo indica onde mostrar o erro quando ele não pertence a um único controle
      const controle = formulario.querySelector(`[data-campo~="${nome}"]`) || formulario.querySelector(`[name="${nome}"]`);
      const caixa = controle?.closest('.campo');
      if (!caixa) continue;
      caixa.classList.add('com-erro');
      const id = `erro-${nome}`;
      caixa.append(el('small', { class: 'erro-campo', id, texto }));
      if (controle.matches('input, select, textarea')) {
        controle.setAttribute('aria-invalid', 'true');
        controle.setAttribute('aria-describedby', id);
      }
      primeiro = primeiro || controle;
    }
    if (primeiro && primeiro.focus) primeiro.focus();
  }

  /** Botão ocupado durante a requisição (evita clique duplo). */
  function ocupado(botao, sim, textoOcupado = 'Aguarde...') {
    if (!botao) return;
    if (sim) {
      botao.dataset.textoOriginal = botao.textContent;
      botao.textContent = textoOcupado;
      botao.disabled = true;
      botao.setAttribute('aria-busy', 'true');
    } else {
      botao.textContent = botao.dataset.textoOriginal || botao.textContent;
      botao.disabled = false;
      botao.removeAttribute('aria-busy');
    }
  }

  /** Diálogo de confirmação acessível. Resolve true (confirmou) ou false. */
  function confirmar({ titulo = 'Confirmar', mensagem: texto = '', confirmar: rotulo = 'Confirmar', cancelar = 'Cancelar', perigo = false } = {}) {
    return new Promise((resolve) => {
      const dialogo = el(
        'dialog',
        { class: 'dialogo', 'aria-labelledby': 'dialogo-titulo', 'aria-describedby': 'dialogo-texto' },
        el(
          'div',
          { class: 'dialogo-conteudo' },
          el('h2', { id: 'dialogo-titulo', texto: titulo }),
          el('p', { id: 'dialogo-texto', texto }),
          el(
            'div',
            { class: 'botoes' },
            el('button', { type: 'button', class: 'botao contorno', value: 'nao', texto: cancelar }),
            el('button', { type: 'button', class: `botao${perigo ? ' perigo' : ''}`, value: 'sim', texto: rotulo })
          )
        )
      );
      dialogo.addEventListener('click', (evento) => {
        const botao = evento.target.closest('button');
        if (botao) dialogo.close(botao.value);
      });
      dialogo.addEventListener('close', () => {
        resolve(dialogo.returnValue === 'sim');
        dialogo.remove();
      });
      document.body.append(dialogo);
      dialogo.showModal();
      dialogo.querySelector('button[value="nao"]').focus();
    });
  }

  function etiquetaSituacao(ativo) {
    return el('span', { class: `etiqueta ${ativo ? 'ativo' : 'inativo'}`, texto: ativo ? 'Ativo' : 'Inativo' });
  }

  /** Barras de desempenho. itens: [{ rotulo, valor (0 a 100 ou null), detalhe }] */
  function barras(container, itens, vazio = 'Ainda não há dados.') {
    container.replaceChildren();
    if (!itens.length) {
      container.append(el('p', { class: 'tabela-vazia', texto: vazio }));
      return;
    }
    for (const item of itens) {
      const valor = item.valor === null || item.valor === undefined ? null : Math.max(0, Math.min(100, Number(item.valor)));
      const rotuloValor = valor === null ? 'sem respostas' : formatarPercentual(valor);
      container.append(
        el(
          'div',
          { class: 'barra-item' },
          el('div', { class: 'barra-rotulo' }, el('span', { texto: item.rotulo }), el('span', { texto: item.detalhe ? `${rotuloValor} · ${item.detalhe}` : rotuloValor })),
          el(
            'div',
            { class: 'barra-trilho', role: 'progressbar', 'aria-label': item.rotulo, 'aria-valuemin': '0', 'aria-valuemax': '100', 'aria-valuenow': valor ?? 0 },
            el('div', { class: 'barra-preenchida', style: `width:${valor ?? 0}%` })
          )
        )
      );
    }
  }

  /** Linha única de tabela com texto (vazia, carregando ou erro). */
  function linhaUnica(tbody, colunas, texto) {
    tbody.replaceChildren(el('tr', {}, el('td', { colspan: colunas, class: 'tabela-vazia', texto })));
  }

  function parametro(nome) {
    return new URLSearchParams(window.location.search).get(nome);
  }

  /** CPF com máscara enquanto digita: 000.000.000-00 */
  function mascaraCpf(input) {
    input.addEventListener('input', () => {
      const d = input.value.replace(/\D/g, '').slice(0, 11);
      input.value = d
        .replace(/^(\d{3})(\d)/, '$1.$2')
        .replace(/^(\d{3})\.(\d{3})(\d)/, '$1.$2.$3')
        .replace(/\.(\d{3})(\d)/, '.$1-$2');
    });
  }

  Avalia.ui = {
    el,
    $,
    formatarData,
    formatarDataHora,
    formatarPercentual,
    formatarNumero,
    primeiroNome,
    iniciais,
    mensagem,
    limparErros,
    mostrarErros,
    ocupado,
    confirmar,
    etiquetaSituacao,
    barras,
    linhaUnica,
    parametro,
    mascaraCpf
  };
})();
