// Realização do simulado (RN16, RN17, RN18).
//  - Questões vêm da API SEM gabarito; a correção é do servidor.
//  - "Anterior" e "Próximo", navegação numerada e avanço com a questão em branco.
//  - Na última questão, "Finalizar" com confirmação informando quantas ficaram em branco.
//  - As marcações provisórias ficam no localStorage só durante a realização (compatível com a RN18).
//  - O cronômetro só é exibido: o tempo não é enviado nem gravado (RN17).
//  - Sair da tela encerra a tentativa (RN16): a saída é interceptada com confirmação e o simulado é
//    finalizado com o que foi marcado até ali.
(async function () {
  const { $, el, mensagem, ocupado, confirmar, parametro } = Avalia.ui;
  Avalia.montarPagina({ perfis: [Avalia.PERFIL.ALUNO], semMenu: true });

  const id = Number(parametro('id'));
  if (!id) {
    window.location.replace('criarsimulados.html');
    return;
  }
  const CHAVE = `avalia.respostas.${id}`;
  const aviso = $('#mensagem');
  let questoes = [];
  let indice = 0;
  let finalizado = false;
  let respostas = {};
  try {
    respostas = JSON.parse(localStorage.getItem(CHAVE) || '{}');
  } catch {
    respostas = {};
  }

  function salvarMarcacoes() {
    try {
      localStorage.setItem(CHAVE, JSON.stringify(respostas));
    } catch {
      // sem armazenamento: as marcações ficam só na memória desta página
    }
  }

  function corpoFinalizacao() {
    return { respostas: questoes.map((q) => ({ idQuestao: q.id, alternativa: respostas[q.id] || null })) };
  }

  function emBranco() {
    return questoes.filter((q) => !respostas[q.id]).length;
  }

  // ---------------------------------------------------------------- navegação numerada
  function desenharNavegacao() {
    $('#navegacao-questoes').replaceChildren(
      ...questoes.map((q, i) => {
        const respondida = Boolean(respostas[q.id]);
        return el('button', {
          type: 'button',
          class: `numero-questao${respondida ? ' respondida' : ''}${i === indice ? ' atual' : ''}`,
          'aria-label': `Questão ${i + 1}, ${respondida ? `marcada ${respostas[q.id]}` : 'em branco'}`,
          'aria-current': i === indice ? 'step' : null,
          texto: String(i + 1),
          onclick: () => irPara(i)
        });
      })
    );
    const respondidas = questoes.length - emBranco();
    $('#progresso-texto').textContent = `${respondidas} de ${questoes.length} respondidas`;
    $('#progresso-barra').style.width = `${(respondidas / questoes.length) * 100}%`;
    $('#progresso').setAttribute('aria-valuenow', String(respondidas));
  }

  // ---------------------------------------------------------------- questão atual
  function desenharQuestao() {
    const q = questoes[indice];
    $('#titulo-questao').textContent = `QUESTÃO ${indice + 1}`;
    $('#contador-questao').textContent = `Questão ${indice + 1} de ${questoes.length} · ${q.disciplina}`;
    $('#enunciado').textContent = q.enunciado;

    const imagem = $('#imagem-questao');
    imagem.hidden = !q.imagemUrl;
    if (q.imagemUrl) {
      imagem.src = q.imagemUrl;
      imagem.alt = `Imagem da questão ${indice + 1}`;
    }

    $('#alternativas').replaceChildren(
      ...q.alternativas.map((a) =>
        el(
          'label',
          { class: `alternativa${respostas[q.id] === a.letra ? ' selecionada' : ''}` },
          el('input', {
            type: 'radio',
            name: 'alternativa',
            value: a.letra,
            checked: respostas[q.id] === a.letra || null,
            onchange: () => marcar(q.id, a.letra)
          }),
          el('span', { class: 'letra', texto: a.letra }),
          el('span', { class: 'texto-alternativa', texto: a.texto })
        )
      )
    );

    $('#botao-anterior').disabled = indice === 0;
    const ultima = indice === questoes.length - 1;
    $('#botao-proximo').hidden = ultima;
    $('#botao-finalizar').hidden = !ultima;
    $('#limpar-resposta').disabled = !respostas[q.id];
    desenharNavegacao();
  }

  function marcar(idQuestao, letra) {
    respostas[idQuestao] = letra;
    salvarMarcacoes();
    desenharQuestao();
  }

  function irPara(novoIndice) {
    indice = Math.max(0, Math.min(questoes.length - 1, novoIndice));
    desenharQuestao();
    $('#titulo-questao').focus();
  }

  // ---------------------------------------------------------------- finalização
  async function enviarFinalizacao() {
    const resultado = await Avalia.api.post(`/simulados/${id}/finalizar`, corpoFinalizacao());
    finalizado = true;
    localStorage.removeItem(CHAVE);
    return resultado;
  }

  async function finalizar() {
    const brancas = emBranco();
    const texto =
      brancas > 0
        ? `Você deixou ${brancas} questão(ões) em branco. Questões em branco contam como erro.\n\nDeseja finalizar o simulado?`
        : 'Todas as questões foram respondidas. Deseja finalizar o simulado?';
    if (!(await confirmar({ titulo: 'Finalizar simulado', mensagem: texto, confirmar: 'Finalizar' }))) return;
    const botao = $('#botao-finalizar');
    ocupado(botao, true, 'Corrigindo...');
    try {
      await enviarFinalizacao();
      window.location.replace(`resultado.html?id=${id}`);
    } catch (erro) {
      mensagem(aviso, erro.message, 'erro');
      ocupado(botao, false);
    }
  }

  // ---------------------------------------------------------------- saída da tela (RN16)
  // Links do cabeçalho: confirmação própria antes de sair
  document.addEventListener('click', async (evento) => {
    const link = evento.target.closest('a[href]');
    if (!link || finalizado || link.closest('#area-simulado') || link.classList.contains('pular-conteudo')) return;
    evento.preventDefault();
    const sair = await confirmar({
      titulo: 'Sair do simulado',
      mensagem:
        'Sair desta tela ENCERRA a tentativa: o simulado será finalizado com as respostas marcadas até agora, e não será possível retomá-lo.',
      confirmar: 'Sair e finalizar',
      perigo: true
    });
    if (!sair) return;
    try {
      await enviarFinalizacao();
    } catch {
      // mesmo com falha, a saída continua; o servidor aceita reenvio da finalização
    }
    window.location.href = link.href;
  });

  // Fechar a aba, recarregar ou voltar: o navegador pede confirmação
  window.addEventListener('beforeunload', (evento) => {
    if (finalizado) return;
    evento.preventDefault();
    evento.returnValue = '';
  });

  // Se a pessoa confirmar a saída, a tentativa é encerrada com o que foi marcado
  window.addEventListener('pagehide', () => {
    if (finalizado || !questoes.length) return;
    finalizado = true;
    Avalia.api.post(`/simulados/${id}/finalizar`, corpoFinalizacao(), { manterAberta: true }).catch(() => {});
    localStorage.removeItem(CHAVE);
  });

  // ---------------------------------------------------------------- cronômetro (só exibição, RN17)
  function iniciarCronometro() {
    const inicio = Date.now();
    const relogio = $('#cronometro');
    const doisDigitos = (n) => String(n).padStart(2, '0');
    setInterval(() => {
      const total = Math.floor((Date.now() - inicio) / 1000);
      relogio.textContent = `${doisDigitos(Math.floor(total / 3600))}:${doisDigitos(Math.floor((total % 3600) / 60))}:${doisDigitos(total % 60)}`;
    }, 1000);
  }

  // ---------------------------------------------------------------- carga
  try {
    const simulado = await Avalia.api.get(`/simulados/${id}`);
    if (simulado.finalizado) {
      finalizado = true;
      localStorage.removeItem(CHAVE);
      window.location.replace(`resultado.html?id=${id}`);
      return;
    }
    questoes = simulado.questoes;
    $('#titulo-simulado').textContent = simulado.titulo;
    $('#progresso').setAttribute('aria-valuemax', String(questoes.length));
    $('#area-simulado').hidden = false;
    $('#carregando').hidden = true;
    desenharQuestao();
    iniciarCronometro();
  } catch (erro) {
    finalizado = true;
    $('#carregando').hidden = true;
    mensagem(aviso, erro.message, 'erro');
    return;
  }

  $('#botao-anterior').addEventListener('click', () => irPara(indice - 1));
  // Avança mesmo com a questão em branco (RN18)
  $('#botao-proximo').addEventListener('click', () => irPara(indice + 1));
  $('#botao-finalizar').addEventListener('click', finalizar);
  $('#limpar-resposta').addEventListener('click', () => {
    delete respostas[questoes[indice].id];
    salvarMarcacoes();
    desenharQuestao();
  });
})();
