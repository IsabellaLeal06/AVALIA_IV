// Dashboard do Administrador: quantidades de cadastros ATIVOS (RF02). Sem desempenho (RN24).
(async function () {
  const { $, mensagem, primeiroNome, formatarNumero } = Avalia.ui;
  const usuario = Avalia.montarPagina({ perfis: [Avalia.PERFIL.ADMINISTRADOR] });
  $('#nome-usuario').textContent = primeiroNome(usuario.nome);
  Avalia.avisoSenhaInicial($('#avisos'));

  try {
    const painel = await Avalia.api.get('/painel');
    $('#qtd-turmas').textContent = formatarNumero(painel.turmas);
    $('#qtd-usuarios').textContent = formatarNumero(painel.usuarios);
    $('#qtd-disciplinas').textContent = formatarNumero(painel.disciplinas);
    $('#qtd-questoes').textContent = formatarNumero(painel.questoes);
  } catch (erro) {
    mensagem($('#mensagem'), erro.message, 'erro');
  }
})();
