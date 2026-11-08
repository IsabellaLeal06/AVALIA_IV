// "Meu perfil" dos três perfis (perfil.html, perfilprofessor.html, perfiladmin.html).
// Aluno: pontuação e medalha APURADAS pela API (RN22). Sem medalha: a primeira faixa aparece
// acinzentada; no nível máximo, mensagem própria. Sem barra de progresso (RN23).
(async function () {
  const { $, el, mensagem, formatarData, formatarNumero, formatarPercentual, formatarDataHora } = Avalia.ui;
  const { ALUNO, PROFESSOR, ADMINISTRADOR } = Avalia.PERFIL;
  const usuario = Avalia.montarPagina({ perfis: [Number(document.body.dataset.perfil)] });

  $('#sair-conta').addEventListener('click', Avalia.menu.confirmarSaida);

  function cardResumo(rotulo, valor) {
    return el('div', { class: 'card' }, el('p', { texto: rotulo }), el('strong', { texto: valor }));
  }

  function mostrarGamificacao(perfil) {
    const g = perfil.gamificacao;
    const caixa = $('#gamificacao');
    caixa.hidden = false;
    $('#pontuacao').textContent = `${formatarNumero(perfil.pontuacaoTotal)} pontos`;
    $('#mensagem-medalha').textContent = g.mensagem;

    // Medalha corrente; sem medalha, a próxima (primeira faixa) acinzentada
    const exibida = g.medalha || g.proxima;
    const imagem = $('#imagem-medalha');
    if (exibida) {
      imagem.src = exibida.imagemUrl;
      imagem.alt = g.medalha ? `Medalha ${g.medalha.nome}` : `Medalha ${g.proxima.nome} ainda não conquistada`;
      imagem.classList.toggle('acinzentada', g.semMedalha);
      imagem.hidden = false;
      $('#nome-medalha').textContent = g.medalha ? `Medalha ${g.medalha.nome}` : 'Ainda sem medalha';
    } else {
      imagem.hidden = true;
      $('#nome-medalha').textContent = 'Sem faixas de medalha cadastradas';
    }
  }

  try {
    const perfil = await Avalia.api.get('/perfil');
    $('#campo-nome').textContent = perfil.nome;
    $('#campo-email').textContent = perfil.email;
    $('#campo-perfil').textContent = perfil.perfil;
    $('#campo-ingresso').textContent = formatarData(perfil.dataIngresso);
    if (perfil.turma) {
      $('#campo-turma').textContent = perfil.turma;
      $('#caixa-turma').hidden = false;
    }
    $('#avatar-grande').textContent = Avalia.ui.iniciais(perfil.nome);

    const resumo = $('#resumo');
    if (usuario.idPerfil === ALUNO) {
      mostrarGamificacao(perfil);
      const desempenho = await Avalia.api.get('/desempenho/aluno');
      resumo.replaceChildren(
        cardResumo('Taxa média de acerto', formatarPercentual(desempenho.mediaAcertos)),
        cardResumo('Simulados realizados', formatarNumero(desempenho.qtdSimulados)),
        cardResumo('Último simulado', desempenho.ultimoSimulado ? formatarDataHora(desempenho.ultimoSimulado) : '—')
      );
    } else if (usuario.idPerfil === PROFESSOR) {
      const [turmas, disciplinas] = await Promise.all([
        Avalia.api.get('/desempenho/turmas'),
        Avalia.api.get('/disciplinas', { situacao: 'ativo' })
      ]);
      resumo.replaceChildren(
        cardResumo('Disciplinas sob sua responsabilidade', formatarNumero(disciplinas.length)),
        cardResumo('Turmas acompanhadas', formatarNumero(turmas.length)),
        cardResumo('Simulados realizados pelas turmas', formatarNumero(turmas.reduce((s, t) => s + t.qtdSimulados, 0)))
      );
    } else if (usuario.idPerfil === ADMINISTRADOR) {
      // Somente quantitativos de cadastro (RN24)
      const painel = await Avalia.api.get('/painel');
      resumo.replaceChildren(
        cardResumo('Turmas ativas', formatarNumero(painel.turmas)),
        cardResumo('Disciplinas ativas', formatarNumero(painel.disciplinas)),
        cardResumo('Usuários ativos', formatarNumero(painel.usuarios))
      );
    }
  } catch (erro) {
    mensagem($('#mensagem'), erro.message, 'erro');
  }
})();
