// Dashboard do Professor: indicadores das turmas que cursam as SUAS disciplinas (RN24).
// Alunos desativados ficam fora das médias (RN25): a API já aplica.
(async function () {
  const { $, el, mensagem, primeiroNome, formatarPercentual, formatarNumero, barras, formatarData } = Avalia.ui;
  const usuario = Avalia.montarPagina({ perfis: [Avalia.PERFIL.PROFESSOR] });
  $('#nome-usuario').textContent = primeiroNome(usuario.nome);
  Avalia.avisoSenhaInicial($('#avisos'));

  try {
    const [turmas, questoes] = await Promise.all([
      Avalia.api.get('/desempenho/turmas'),
      Avalia.api.get('/desempenho/questoes-erro', { limite: 5 })
    ]);
    const comMedia = turmas.filter((t) => t.mediaAcertos !== null);
    const media = comMedia.length ? comMedia.reduce((soma, t) => soma + t.mediaAcertos, 0) / comMedia.length : null;
    const ultimo = turmas.map((t) => t.ultimoSimulado).filter(Boolean).sort().pop();

    $('#qtd-turmas').textContent = formatarNumero(turmas.length);
    $('#taxa-media').textContent = formatarPercentual(media);
    $('#qtd-simulados').textContent = formatarNumero(turmas.reduce((soma, t) => soma + t.qtdSimulados, 0));
    $('#ultimo-simulado').textContent = ultimo ? formatarData(ultimo) : '—';

    barras(
      $('#barras-turmas'),
      turmas.map((t) => ({ rotulo: `${t.nome} (${t.anoLetivo})`, valor: t.mediaAcertos, detalhe: `${t.qtdAlunos} aluno(s)` })),
      'Nenhuma turma cursa as suas disciplinas ainda.'
    );

    const lista = $('#lista-questoes-erro');
    lista.replaceChildren(
      ...(questoes.length
        ? questoes.map((q) =>
            el(
              'li',
              {},
              el('strong', { texto: `${formatarPercentual(q.percentualErro)} de erro` }),
              el('span', { texto: ` · Nº ${q.idQuestao} · ${q.disciplina}` }),
              el('p', { texto: q.enunciado })
            )
          )
        : [el('li', { class: 'tabela-vazia', texto: 'Ainda não há respostas às questões das suas disciplinas.' })])
    );
  } catch (erro) {
    mensagem($('#mensagem'), erro.message, 'erro');
  }
})();
