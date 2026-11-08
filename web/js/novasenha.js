// Recuperação de senha, etapa 2 (RN03): confere o código ANTES de mostrar os campos da nova senha.
// Sem o código válido, ninguém redefine a senha de outra pessoa só por conhecer o endereço desta página.
(function () {
  const { $, mensagem, ocupado, parametro, mostrarErros, limparErros } = Avalia.ui;
  const formulario = $('#formulario');
  const aviso = $('#mensagem');
  const etapaSenha = $('#etapa-senha');

  formulario.email.value = parametro('email') || '';
  (formulario.email.value ? formulario.codigo : formulario.email).focus();

  async function validarCodigo() {
    const botao = $('#botao-validar');
    ocupado(botao, true, 'Conferindo...');
    try {
      await Avalia.api.get('/auth/validar-codigo', { email: formulario.email.value.trim(), codigo: formulario.codigo.value.trim() });
      mensagem(aviso, 'Código confirmado. Agora crie a nova senha.', 'sucesso');
      formulario.email.readOnly = true;
      formulario.codigo.readOnly = true;
      botao.hidden = true;
      etapaSenha.hidden = false;
      formulario.novaSenha.focus();
    } catch (erro) {
      mensagem(aviso, erro.message, 'erro');
      ocupado(botao, false);
    }
  }

  async function trocarSenha() {
    limparErros(formulario);
    const botao = $('#botao-salvar');
    ocupado(botao, true, 'Salvando...');
    try {
      await Avalia.api.post('/auth/nova-senha', {
        email: formulario.email.value.trim(),
        codigo: formulario.codigo.value.trim(),
        novaSenha: formulario.novaSenha.value,
        confirmacao: formulario.confirmacao.value
      });
      window.location.href = 'login.html?motivo=senha';
    } catch (erro) {
      mensagem(aviso, erro.message, 'erro');
      mostrarErros(formulario, erro.campos);
      ocupado(botao, false);
    }
  }

  formulario.addEventListener('submit', (evento) => {
    evento.preventDefault();
    if (etapaSenha.hidden) validarCodigo();
    else trocarSenha();
  });
})();
