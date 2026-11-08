// Instruções e início do simulado. O sorteio acontece no servidor (POST /simulados).
(function () {
  const { $, el, mensagem, ocupado } = Avalia.ui;
  Avalia.montarPagina({ perfis: [Avalia.PERFIL.ALUNO] });

  const escolha = JSON.parse(sessionStorage.getItem('avalia.novoSimulado') || 'null');
  if (!escolha) {
    window.location.replace('criarsimulados.html');
    return;
  }
  $('#resumo-quantidade').textContent = `${escolha.quantidade} questões`;
  $('#resumo-disciplinas').replaceChildren(...escolha.nomes.map((nome) => el('li', { texto: nome })));

  $('#botao-iniciar').addEventListener('click', async (evento) => {
    const botao = evento.currentTarget;
    ocupado(botao, true, 'Sorteando questões...');
    try {
      const simulado = await Avalia.api.post('/simulados', { disciplinas: escolha.disciplinas, quantidade: escolha.quantidade });
      sessionStorage.removeItem('avalia.novoSimulado');
      // replace: o botão Voltar do navegador não gera outro simulado
      window.location.replace(`questoes.html?id=${simulado.id}`);
    } catch (erro) {
      mensagem($('#mensagem'), erro.message, 'erro');
      ocupado(botao, false);
    }
  });
})();
