// Partes comuns das telas de cadastro (listagens e formulários do Administrador e do Professor).
(function () {
  const Avalia = (window.Avalia = window.Avalia || {});
  const { el, confirmar, mensagem } = Avalia.ui;
  const CHAVE_MENSAGEM = 'avalia.mensagemPendente';

  /** Preenche o corpo da tabela. colunas: [{ valor: (item) => texto ou elemento, classe }] */
  function preencherTabela(tbody, itens, colunas, vazio = 'Nenhum registro encontrado.') {
    if (!itens.length) {
      Avalia.ui.linhaUnica(tbody, colunas.length, vazio);
      return;
    }
    tbody.replaceChildren(
      ...itens.map((item) =>
        el(
          'tr',
          {},
          colunas.map((coluna) => {
            const valor = coluna.valor(item);
            return el('td', { class: coluna.classe }, valor instanceof Node ? valor : String(valor ?? '—'));
          })
        )
      )
    );
  }

  function botao(texto, aoClicar, classe = 'secundario', rotuloAcessivel) {
    return el('button', { type: 'button', class: `botao pequeno ${classe}`, 'aria-label': rotuloAcessivel, texto, onclick: aoClicar });
  }

  function link(texto, href, classe = 'secundario', rotuloAcessivel) {
    return el('a', { class: `botao pequeno ${classe}`, href, 'aria-label': rotuloAcessivel, texto });
  }

  /**
   * Desativa ou reativa (RN12) com confirmação. Rótulo "Desativar", nunca "Excluir".
   * opcoes: { rota, item, tipo, nome, avisoDesativar, avisoReativar, elementoMensagem, aoConcluir }
   */
  async function alternarSituacao(opcoes) {
    const { rota, item, tipo, nome, avisoDesativar = '', avisoReativar = '', elementoMensagem, aoConcluir } = opcoes;
    const desativar = item.ativo;
    const confirmado = await confirmar({
      titulo: desativar ? `Desativar ${tipo}` : `Reativar ${tipo}`,
      mensagem: desativar
        ? `Deseja desativar ${nome}?\n\nO registro não é apagado: o histórico é mantido e é possível reativá-lo depois.${avisoDesativar ? `\n\n${avisoDesativar}` : ''}`
        : `Deseja reativar ${nome}?${avisoReativar ? `\n\n${avisoReativar}` : ''}`,
      confirmar: desativar ? 'Desativar' : 'Reativar',
      perigo: desativar
    });
    if (!confirmado) return;
    try {
      const resposta = await Avalia.api.patch(`${rota}/${item.id}/situacao`, { ativo: !desativar });
      mensagem(elementoMensagem, [resposta.mensagem, resposta.aviso].filter(Boolean).join('\n'), resposta.aviso ? 'alerta' : 'sucesso');
      if (aoConcluir) await aoConcluir();
    } catch (erro) {
      mensagem(elementoMensagem, erro.message, 'erro');
    }
  }

  /** Mensagem mostrada na próxima página (ex.: "Turma cadastrada" depois de voltar para a lista). */
  function guardarMensagem(texto, tipo = 'sucesso') {
    sessionStorage.setItem(CHAVE_MENSAGEM, JSON.stringify({ texto, tipo }));
  }

  function exibirMensagemGuardada(elemento) {
    const guardada = sessionStorage.getItem(CHAVE_MENSAGEM);
    if (!guardada) return;
    sessionStorage.removeItem(CHAVE_MENSAGEM);
    const { texto, tipo } = JSON.parse(guardada);
    mensagem(elemento, texto, tipo);
  }

  /** Preenche um select. opcoes: [{ valor, texto, desabilitado }] */
  function preencherSelect(select, opcoes, selecionado) {
    select.replaceChildren(
      ...opcoes.map((o) =>
        el('option', { value: o.valor, disabled: o.desabilitado || null, selected: String(o.valor) === String(selecionado ?? '') || null, texto: o.texto })
      )
    );
  }

  /** Lista de caixas de marcação. itens: [{ valor, texto, marcado, desabilitado, ajuda }] */
  function preencherMarcacao(container, nome, itens, vazio = 'Nenhuma opção disponível.') {
    if (!itens.length) {
      container.replaceChildren(el('p', { class: 'ajuda', texto: vazio }));
      return;
    }
    container.replaceChildren(
      ...itens.map((item) =>
        el(
          'label',
          {},
          el('input', { type: 'checkbox', name: nome, value: item.valor, checked: item.marcado || null, disabled: item.desabilitado || null }),
          el('span', {}, item.texto, item.ajuda ? el('small', { class: 'ajuda', texto: ` ${item.ajuda}` }) : null)
        )
      )
    );
  }

  function valoresMarcados(container, nome) {
    return [...container.querySelectorAll(`input[name="${nome}"]:checked`)].map((c) => Number(c.value));
  }

  /** Atraso para a busca enquanto digita. */
  function aoDigitar(input, funcao, espera = 350) {
    let temporizador;
    input.addEventListener('input', () => {
      clearTimeout(temporizador);
      temporizador = setTimeout(funcao, espera);
    });
  }

  Avalia.cadastro = {
    preencherTabela,
    botao,
    link,
    alternarSituacao,
    guardarMensagem,
    exibirMensagemGuardada,
    preencherSelect,
    preencherMarcacao,
    valoresMarcados,
    aoDigitar
  };
})();
