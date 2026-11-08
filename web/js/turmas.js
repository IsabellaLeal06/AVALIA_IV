// Minhas turmas (Professor). Tudo consolidado: média de acertos por disciplina e quantidade de simulados.
// O painel do aluno NÃO mostra respostas questão a questão (RN24); a API usa uma rota própria para ele.
(async function () {
  const { $, el, mensagem, formatarPercentual, formatarNumero, formatarDataHora, barras, linhaUnica, formatarData } = Avalia.ui;
  Avalia.montarPagina({ perfis: [Avalia.PERFIL.PROFESSOR] });

  const aviso = $('#mensagem');
  const modalTurma = $('#modal-turma');
  const modalAlunos = $('#modal-alunos');
  const modalAluno = $('#modal-aluno');
  let turmaAtual = null;

  function listaQuestoes(container, questoes) {
    container.replaceChildren(
      ...(questoes.length
        ? questoes.map((q) =>
            el(
              'li',
              {},
              el('strong', { texto: `${formatarPercentual(q.percentualErro)} de erro` }),
              el('span', { texto: ` · Nº ${q.idQuestao} · ${q.disciplina} · ${q.totalErros} de ${q.totalRespostas} respostas` }),
              el('p', { texto: q.enunciado })
            )
          )
        : [el('li', { class: 'tabela-vazia', texto: 'Ainda não há respostas às questões das suas disciplinas.' })])
    );
  }

  async function abrirTurma(turma) {
    turmaAtual = turma;
    $('#modal-turma-titulo').textContent = `${turma.nome} (${turma.anoLetivo})`;
    $('#modal-turma-resumo').textContent =
      `${turma.qtdAlunos} aluno(s) ativo(s) · média ${formatarPercentual(turma.mediaAcertos)} · ${formatarNumero(turma.qtdSimulados)} simulado(s)`;
    $('#barras-turma').replaceChildren(el('p', { class: 'carregando', texto: 'Carregando...' }));
    $('#questoes-erro-turma').replaceChildren();
    modalTurma.showModal();
    try {
      const [detalhe, questoes] = await Promise.all([
        Avalia.api.get(`/desempenho/turmas/${turma.id}`),
        Avalia.api.get('/desempenho/questoes-erro', { turma: turma.id, limite: 5 })
      ]);
      barras(
        $('#barras-turma'),
        detalhe.porDisciplina.map((d) => ({ rotulo: d.disciplina, valor: d.mediaAcertos, detalhe: `${d.totalRespostas} respostas` }))
      );
      listaQuestoes($('#questoes-erro-turma'), questoes);
    } catch (erro) {
      $('#barras-turma').replaceChildren(el('p', { class: 'mensagem erro', texto: erro.message }));
    }
  }

  async function abrirAlunos() {
    modalTurma.close();
    $('#modal-alunos-titulo').textContent = `Alunos de ${turmaAtual.nome} (${turmaAtual.anoLetivo})`;
    const corpo = $('#tabela-alunos');
    linhaUnica(corpo, 3, 'Carregando...');
    modalAlunos.showModal();
    try {
      const alunos = await Avalia.api.get(`/desempenho/turmas/${turmaAtual.id}/alunos`);
      if (!alunos.length) {
        linhaUnica(corpo, 3, 'Nenhum aluno ativo nesta turma.');
        return;
      }
      corpo.replaceChildren(
        ...alunos.map((a) =>
          el(
            'tr',
            { class: 'clicavel', onclick: () => abrirAluno(a) },
            el('td', {}, el('button', { type: 'button', class: 'link-tabela', texto: a.nome, onclick: (e) => (e.stopPropagation(), abrirAluno(a)) })),
            el('td', { texto: formatarPercentual(a.mediaAcertos) }),
            el('td', { texto: a.ultimoSimulado ? formatarDataHora(a.ultimoSimulado) : '—' })
          )
        )
      );
    } catch (erro) {
      linhaUnica(corpo, 3, erro.message);
    }
  }

  async function abrirAluno(aluno) {
    modalAlunos.close();
    $('#modal-aluno-titulo').textContent = aluno.nome;
    $('#barras-aluno').replaceChildren(el('p', { class: 'carregando', texto: 'Carregando...' }));
    modalAluno.showModal();
    try {
      const painel = await Avalia.api.get(`/desempenho/turmas/${turmaAtual.id}/alunos/${aluno.id}`);
      $('#aluno-media').textContent = formatarPercentual(painel.mediaAcertos);
      $('#aluno-simulados').textContent = formatarNumero(painel.qtdSimulados);
      $('#aluno-ultimo').textContent = painel.ultimoSimulado ? formatarDataHora(painel.ultimoSimulado) : '—';
      barras(
        $('#barras-aluno'),
        painel.porDisciplina.map((d) => ({ rotulo: d.disciplina, valor: d.mediaAcertos, detalhe: `${d.totalRespostas} respostas` }))
      );
    } catch (erro) {
      $('#barras-aluno').replaceChildren(el('p', { class: 'mensagem erro', texto: erro.message }));
    }
  }

  $('#ver-alunos').addEventListener('click', abrirAlunos);
  $('#voltar-turma').addEventListener('click', () => {
    modalAlunos.close();
    abrirTurma(turmaAtual);
  });
  $('#voltar-alunos').addEventListener('click', () => {
    modalAluno.close();
    abrirAlunos();
  });
  document.querySelectorAll('[data-fechar]').forEach((b) => b.addEventListener('click', () => b.closest('dialog').close()));

  try {
    const [turmas, questoes] = await Promise.all([
      Avalia.api.get('/desempenho/turmas'),
      Avalia.api.get('/desempenho/questoes-erro', { limite: 10 })
    ]);
    const comMedia = turmas.filter((t) => t.mediaAcertos !== null);
    $('#taxa-media').textContent = formatarPercentual(
      comMedia.length ? comMedia.reduce((s, t) => s + t.mediaAcertos, 0) / comMedia.length : null
    );
    $('#qtd-simulados').textContent = formatarNumero(turmas.reduce((s, t) => s + t.qtdSimulados, 0));
    const ultimo = turmas.map((t) => t.ultimoSimulado).filter(Boolean).sort().pop();
    $('#ultimo-simulado').textContent = ultimo ? formatarData(ultimo) : '—';

    const corpo = $('#tabela-corpo');
    if (!turmas.length) linhaUnica(corpo, 5, 'Nenhuma turma cursa as suas disciplinas ainda.');
    else
      corpo.replaceChildren(
        ...turmas.map((t) =>
          el(
            'tr',
            { class: 'clicavel', onclick: () => abrirTurma(t) },
            el('td', {}, el('button', { type: 'button', class: 'link-tabela', texto: `${t.nome} (${t.anoLetivo})`, onclick: (e) => (e.stopPropagation(), abrirTurma(t)) })),
            el('td', { texto: formatarNumero(t.qtdAlunos) }),
            el('td', { texto: formatarPercentual(t.mediaAcertos) }),
            el('td', { class: 'ocultar-celular', texto: formatarNumero(t.qtdSimulados) }),
            el('td', { texto: t.ultimoSimulado ? formatarDataHora(t.ultimoSimulado) : '—' })
          )
        )
      );
    listaQuestoes($('#lista-questoes-erro'), questoes);
  } catch (erro) {
    mensagem(aviso, erro.message, 'erro');
  }
})();
