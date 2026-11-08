// Revisão comentada (RF05): resposta marcada, correta e explicação (quando houver).
// Questões em branco aparecem como "Questão deixada em branco", também nos botões numerados.
(async function () {
  const { $, el, mensagem, formatarDataHora, formatarPercentual, parametro } = Avalia.ui;
  Avalia.montarPagina({ perfis: [Avalia.PERFIL.ALUNO] });
  const id = Number(parametro('id'));
  let questoes = [];
  let indice = 0;

  function situacao(q) {
    if (q.emBranco) return { classe: 'branco', texto: 'em branco' };
    return q.acertou ? { classe: 'certa', texto: 'acertou' } : { classe: 'errada', texto: 'errou' };
  }

  function desenharNavegacao() {
    $('#navegacao-questoes').replaceChildren(
      ...questoes.map((q, i) => {
        const s = situacao(q);
        return el('button', {
          type: 'button',
          class: `numero-questao ${s.classe}${i === indice ? ' atual' : ''}`,
          'aria-label': `Questão ${i + 1}, ${s.texto}`,
          'aria-current': i === indice ? 'step' : null,
          texto: String(i + 1),
          onclick: () => {
            indice = i;
            desenhar();
          }
        });
      })
    );
  }

  function textoAlternativa(q, letra) {
    const a = q.alternativas.find((x) => x.letra === letra);
    return a ? `${letra}) ${a.texto}` : letra;
  }

  function desenhar() {
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

    const marcada = $('#resposta-marcada');
    marcada.className = `resposta ${q.emBranco ? 'branco' : q.acertou ? 'acerto' : 'erro'}`;
    $('#rotulo-marcada').textContent = q.emBranco ? 'QUESTÃO DEIXADA EM BRANCO' : q.acertou ? '✔ SUA RESPOSTA (CORRETA)' : '✖ ALTERNATIVA MARCADA';
    $('#texto-marcada').textContent = q.emBranco ? 'Questão deixada em branco: conta como erro.' : textoAlternativa(q, q.marcada);

    $('#texto-correta').textContent = textoAlternativa(q, q.correta);

    const explicacao = $('#explicacao');
    explicacao.hidden = !q.explicacao;
    $('#texto-explicacao').textContent = q.explicacao || '';

    $('#botao-anterior').disabled = indice === 0;
    $('#botao-proximo').disabled = indice === questoes.length - 1;
    desenharNavegacao();
  }

  try {
    const revisao = await Avalia.api.get(`/simulados/${id}/revisao`);
    questoes = revisao.questoes;
    $('#titulo-simulado').textContent = revisao.titulo;
    $('#subtitulo').textContent =
      `Realizado em ${formatarDataHora(revisao.dataRealizacao)} · ${revisao.totalAcertos} de ${revisao.totalQuestoes} acertos (${formatarPercentual(revisao.percentual)})`;
    $('#area-revisao').hidden = false;
    desenhar();
  } catch (erro) {
    mensagem($('#mensagem'), erro.message, 'erro');
  }

  $('#botao-anterior').addEventListener('click', () => {
    indice -= 1;
    desenhar();
  });
  $('#botao-proximo').addEventListener('click', () => {
    indice += 1;
    desenhar();
  });
})();
