// Faixas de medalha (Administrador). As faixas ficam na tabela Medalha, não no código (RN22).
(function () {
  const { $, el, formatarNumero, confirmar, mensagem } = Avalia.ui;
  const { preencherTabela, botao, link, exibirMensagemGuardada } = Avalia.cadastro;
  Avalia.montarPagina({ perfis: [Avalia.PERFIL.ADMINISTRADOR] });

  const corpo = $('#tabela-corpo');
  const aviso = $('#mensagem');
  exibirMensagemGuardada(aviso);
  let faixas = [];

  function textoFaixa(m) {
    return m.pontosProximaFaixa
      ? `${formatarNumero(m.pontosMinimos)} a ${formatarNumero(m.pontosProximaFaixa - 1)} pontos`
      : `${formatarNumero(m.pontosMinimos)} pontos ou mais`;
  }

  async function carregar() {
    Avalia.ui.linhaUnica(corpo, 4, 'Carregando...');
    try {
      const resposta = await Avalia.api.get('/medalhas');
      faixas = resposta.faixas;
      $('#qtd-faixas').textContent = formatarNumero(faixas.length);
      $('#qtd-sem-medalha').textContent = formatarNumero(resposta.qtdAlunosSemMedalha);
      preencherTabela(
        corpo,
        faixas,
        [
          {
            valor: (m) =>
              el(
                'span',
                { class: 'medalha-nome' },
                el('img', { src: m.imagemUrlCompleta, alt: '', width: '40', height: '40' }),
                el('strong', { texto: m.nome })
              )
          },
          { valor: textoFaixa },
          { valor: (m) => m.qtdAlunos },
          {
            classe: 'acoes',
            valor: (m) =>
              el(
                'div',
                { class: 'acoes' },
                link('Editar', `criarmedalha.html?id=${m.id}`, 'secundario', `Editar medalha ${m.nome}`),
                botao('Excluir', () => excluir(m), 'perigo', `Excluir medalha ${m.nome}`)
              )
          }
        ],
        'Nenhuma faixa cadastrada. Sem faixas, nenhum aluno terá medalha.'
      );
    } catch (erro) {
      Avalia.ui.linhaUnica(corpo, 4, erro.message);
    }
  }

  async function excluir(medalha) {
    let texto =
      `Deseja excluir a medalha "${medalha.nome}" (${textoFaixa(medalha)})?\n\n` +
      'A faixa deixa de existir. A pontuação dos alunos não muda: cada aluno passa a ver a medalha da faixa anterior (ou nenhuma).';
    if (medalha.qtdAlunos > 0) texto += `\n\nAtenção: ${medalha.qtdAlunos} aluno(s) ativo(s) estão nesta faixa hoje.`;
    if (faixas.length === 1) texto += '\n\nAtenção: esta é a única medalha cadastrada.';
    if (!(await confirmar({ titulo: 'Excluir medalha', mensagem: texto, confirmar: 'Excluir', perigo: true }))) return;
    try {
      const resposta = await Avalia.api.excluir(`/medalhas/${medalha.id}`);
      mensagem(aviso, resposta.mensagem, 'sucesso');
      carregar();
    } catch (erro) {
      mensagem(aviso, erro.message, 'erro');
    }
  }

  carregar();
})();
