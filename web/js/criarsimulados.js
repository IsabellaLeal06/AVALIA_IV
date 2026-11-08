// Montagem do simulado: disciplinas ativas vindas da API (GET /disciplinas/ativas), mínimo de
// duas disciplinas (RN13) e 10, 15 ou 20 questões (RN14). O servidor confere tudo de novo.
(async function () {
  const { $, mensagem } = Avalia.ui;
  const { preencherMarcacao, valoresMarcados } = Avalia.cadastro;
  Avalia.montarPagina({ perfis: [Avalia.PERFIL.ALUNO] });

  const lista = $('#lista-disciplinas');
  const aviso = $('#mensagem');
  const botao = $('#botao-proximo');
  let disciplinas = [];

  function atualizar() {
    const marcadas = valoresMarcados(lista, 'disciplinas');
    const quantidade = Number(document.querySelector('input[name="quantidade"]:checked')?.value || 0);
    const disponiveis = disciplinas.filter((d) => marcadas.includes(d.id)).reduce((soma, d) => soma + d.qtdQuestoes, 0);

    let texto = '';
    if (marcadas.length < 2) texto = `Selecione pelo menos duas disciplinas (${marcadas.length} selecionada${marcadas.length === 1 ? '' : 's'}).`;
    else if (!quantidade) texto = 'Escolha a quantidade de questões.';
    else if (disponiveis < quantidade) texto = `As disciplinas escolhidas têm ${disponiveis} questões disponíveis. Escolha mais disciplinas ou menos questões.`;
    else texto = `Pronto: ${quantidade} questões sorteadas entre ${marcadas.length} disciplinas (${disponiveis} disponíveis).`;

    $('#resumo-escolha').textContent = texto;
    botao.disabled = !(marcadas.length >= 2 && quantidade && disponiveis >= quantidade);
  }

  try {
    disciplinas = await Avalia.api.get('/disciplinas/ativas');
    preencherMarcacao(
      lista,
      'disciplinas',
      disciplinas.map((d) => ({
        valor: d.id,
        texto: d.nome,
        desabilitado: d.qtdQuestoes === 0,
        ajuda: `(${d.qtdQuestoes} questões)`
      })),
      'Nenhuma disciplina disponível no momento.'
    );
  } catch (erro) {
    mensagem(aviso, erro.message, 'erro');
  }

  lista.addEventListener('change', atualizar);
  document.querySelectorAll('input[name="quantidade"]').forEach((r) => r.addEventListener('change', atualizar));
  atualizar();

  $('#formulario').addEventListener('submit', (evento) => {
    evento.preventDefault();
    if (botao.disabled) return;
    const marcadas = valoresMarcados(lista, 'disciplinas');
    sessionStorage.setItem(
      'avalia.novoSimulado',
      JSON.stringify({
        disciplinas: marcadas,
        nomes: disciplinas.filter((d) => marcadas.includes(d.id)).map((d) => d.nome),
        quantidade: Number(document.querySelector('input[name="quantidade"]:checked').value)
      })
    );
    window.location.href = 'instrucsimulado.html';
  });
})();
