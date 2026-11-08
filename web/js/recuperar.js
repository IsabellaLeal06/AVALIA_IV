// Recuperação de senha, etapa 1 (RN03): pede o código por e-mail.
// A resposta é a mesma exista ou não o e-mail, para não revelar quem está cadastrado.
(function () {
  const { $, mensagem, ocupado } = Avalia.ui;
  const formulario = $('#formulario');
  const aviso = $('#mensagem');

  formulario.addEventListener('submit', async (evento) => {
    evento.preventDefault();
    const botao = $('#botao-enviar');
    ocupado(botao, true, 'Enviando...');
    try {
      const email = formulario.email.value.trim();
      const resposta = await Avalia.api.post('/auth/recuperar-senha', { email });
      mensagem(aviso, `${resposta.mensagem}\nO código vale por ${resposta.validadeMinutos} minutos.`, 'sucesso');
      const continuar = $('#continuar');
      continuar.href = `novasenha.html?email=${encodeURIComponent(email)}`;
      continuar.hidden = false;
      continuar.focus();
    } catch (erro) {
      mensagem(aviso, erro.message, 'erro');
    } finally {
      ocupado(botao, false);
    }
  });
})();
