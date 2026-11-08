// Resultado do simulado (RF05), apurado pelo servidor. Sem card de tempo (RN17).
// A saudação muda com o aproveitamento, com tom encorajador nas faixas baixas.
(async function () {
  const { $, mensagem, primeiroNome, formatarPercentual, formatarNumero, formatarDataHora, barras, parametro } = Avalia.ui;
  const usuario = Avalia.montarPagina({ perfis: [Avalia.PERFIL.ALUNO] });
  const id = Number(parametro('id'));

  function saudacao(percentual) {
    const nome = primeiroNome(usuario.nome);
    if (percentual >= 80) return ['EXCELENTE,', `${nome}!`, 'Resultado de destaque. Continue assim!'];
    if (percentual >= 60) return ['PARABÉNS,', `${nome}!`, 'Bom resultado. Revise as questões que errou para ir ainda mais longe.'];
    if (percentual >= 40) return ['BOM ESFORÇO,', `${nome}!`, 'Você está no caminho. A revisão comentada ajuda a fixar o conteúdo.'];
    return ['CONTINUE PRATICANDO,', `${nome}!`, 'Cada simulado é um passo. Revise as questões e tente de novo: a evolução vem com a prática.'];
  }

  try {
    const r = await Avalia.api.get(`/simulados/${id}/resultado`);
    const [inicio, destaque, apoio] = saudacao(r.percentual);
    $('#saudacao').textContent = inicio;
    $('#saudacao-nome').textContent = ` ${destaque}`;
    $('#saudacao-apoio').textContent = apoio;
    $('#titulo-simulado').textContent = `${r.titulo} · realizado em ${formatarDataHora(r.dataRealizacao)}`;
    $('#taxa-acerto').textContent = formatarPercentual(r.percentual);
    $('#acertos').textContent = `${r.totalAcertos}/${r.totalQuestoes}`;
    $('#detalhe-acertos').textContent = `${r.totalErros} erro(s) e ${r.emBranco} em branco`;
    $('#pontos').textContent = `+${formatarNumero(r.pontosObtidos)}`;
    $('#pontuacao-total').textContent = `Pontuação acumulada: ${formatarNumero(r.pontuacaoTotal)}`;
    barras(
      $('#barras-disciplinas'),
      r.porDisciplina.map((d) => ({ rotulo: d.disciplina, valor: d.percentual, detalhe: `${d.totalAcertos} de ${d.totalQuestoes}` }))
    );
    $('#botao-revisar').href = `analise.html?id=${id}`;
    $('#conteudo-resultado').hidden = false;
  } catch (erro) {
    mensagem($('#mensagem'), erro.message, 'erro');
  }
})();
