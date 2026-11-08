// Desempenho do Aluno: média geral, simulados e aproveitamento por DISCIPLINA.
// O gráfico de evolução continua na tela (decisão pendente na equipe, seção 8.3 do PROMPT):
// ele usa só o histórico que já existe (GET /simulados), sem consulta nova na API.
(async function () {
  const { $, mensagem, formatarPercentual, formatarNumero, formatarData, barras } = Avalia.ui;
  Avalia.montarPagina({ perfis: [Avalia.PERFIL.ALUNO] });

  try {
    const [desempenho, historico] = await Promise.all([
      Avalia.api.get('/desempenho/aluno'),
      Avalia.api.get('/simulados', { pagina: 1, tamanho: 10 })
    ]);
    $('#taxa-media').textContent = formatarPercentual(desempenho.mediaAcertos);
    $('#qtd-simulados').textContent = formatarNumero(desempenho.qtdSimulados);
    $('#ultimo-simulado').textContent = desempenho.ultimoSimulado ? formatarData(desempenho.ultimoSimulado) : '—';

    barras(
      $('#barras-disciplinas'),
      desempenho.porDisciplina.map((d) => ({
        rotulo: d.disciplina,
        valor: d.percentual,
        detalhe: `${d.totalAcertos} de ${d.totalRespostas} questões`
      })),
      'Faça um simulado para ver o seu desempenho por disciplina.'
    );

    const pontos = [...historico.itens].reverse();
    const canvas = $('#grafico');
    if (!pontos.length || !window.Chart) {
      canvas.replaceWith(Avalia.ui.el('p', { class: 'tabela-vazia', texto: 'O gráfico aparece depois do primeiro simulado.' }));
      return;
    }
    new window.Chart(canvas, {
      type: 'line',
      data: {
        labels: pontos.map((s) => formatarData(s.dataRealizacao)),
        datasets: [
          {
            label: 'Taxa de acerto (%)',
            data: pontos.map((s) => s.percentual),
            borderColor: '#2563eb',
            backgroundColor: 'rgba(37,99,235,0.12)',
            tension: 0.35,
            fill: true,
            pointBackgroundColor: '#2563eb',
            pointRadius: 4
          }
        ]
      },
      options: {
        responsive: true,
        plugins: { legend: { position: 'top', align: 'start' } },
        scales: { y: { min: 0, max: 100, ticks: { callback: (v) => `${v}%` } }, x: { grid: { display: false } } }
      }
    });
    canvas.setAttribute('aria-label', `Taxa de acerto nos últimos ${pontos.length} simulados`);
  } catch (erro) {
    mensagem($('#mensagem'), erro.message, 'erro');
  }
})();
