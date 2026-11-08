// Troca da própria senha (RN04), para os três perfis. Exige a senha atual.
(function () {
  const { $, mensagem, ocupado, mostrarErros, limparErros } = Avalia.ui;
  const usuario = Avalia.montarPagina({});
  const formulario = $('#formulario');
  const aviso = $('#mensagem');
  $('#link-voltar').href = Avalia.sessao.paginaPerfil(usuario.idPerfil);

  formulario.addEventListener('submit', async (evento) => {
    evento.preventDefault();
    limparErros(formulario);
    const botao = $('#botao-salvar');
    ocupado(botao, true, 'Salvando...');
    try {
      const resposta = await Avalia.api.post('/auth/alterar-senha', {
        senhaAtual: formulario.senhaAtual.value,
        novaSenha: formulario.novaSenha.value,
        confirmacao: formulario.confirmacao.value
      });
      Avalia.sessao.senhaTrocada();
      formulario.reset();
      mensagem(aviso, resposta.mensagem, 'sucesso');
    } catch (erro) {
      mensagem(aviso, erro.message, 'erro');
      mostrarErros(formulario, erro.campos);
    } finally {
      ocupado(botao, false);
    }
  });
})();
