// Dashboard do Aluno: dados vindos da API, nada escrito no HTML. Desempenho por DISCIPLINA.
(async function () {
  const { $, mensagem, primeiroNome, formatarPercentual, formatarNumero, barras, formatarData } = Avalia.ui;
  const usuario = Avalia.montarPagina({ perfis: [Avalia.PERFIL.ALUNO] });
  $('#nome-usuario').textContent = primeiroNome(usuario.nome);
  Avalia.avisoSenhaInicial($('#avisos'));

  try {
    const [desempenho, perfil] = await Promise.all([Avalia.api.get('/desempenho/aluno'), Avalia.api.get('/perfil')]);
    $('#taxa-media').textContent = formatarPercentual(desempenho.mediaAcertos);
    $('#qtd-simulados').textContent = formatarNumero(desempenho.qtdSimulados);
    $('#ultimo-simulado').textContent = desempenho.ultimoSimulado ? formatarData(desempenho.ultimoSimulado) : '—';
    $('#pontuacao').textContent = formatarNumero(perfil.pontuacaoTotal);
    $('#mensagem-medalha').textContent = perfil.gamificacao.mensagem;

    barras(
      $('#barras-disciplinas'),
      desempenho.porDisciplina.map((d) => ({
        rotulo: d.disciplina,
        valor: d.percentual,
        detalhe: `${d.totalAcertos} de ${d.totalRespostas}`
      })),
      'Você ainda não fez nenhum simulado. Que tal começar agora?'
    );
  } catch (erro) {
    mensagem($('#mensagem'), erro.message, 'erro');
  }
})();
