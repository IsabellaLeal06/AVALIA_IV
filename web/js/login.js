// Login pela API (RF01). O perfil vem da resposta da API; o cliente nunca o informa.
// Mensagem genérica na própria tela, sem alert.
(function () {
  const { $, mensagem, ocupado, parametro } = Avalia.ui;
  const formulario = $('#formulario');
  const aviso = $('#mensagem');

  // Já logado: vai direto para o início do perfil
  if (!Avalia.sessao.expirada()) {
    window.location.replace(Avalia.sessao.inicio(Avalia.sessao.usuario().idPerfil));
    return;
  }

  if (parametro('motivo') === 'sessao') mensagem(aviso, 'Sua sessão terminou. Entre novamente.', 'info');
  if (parametro('motivo') === 'senha') mensagem(aviso, 'Senha alterada. Entre com a nova senha.', 'sucesso');

  formulario.addEventListener('submit', async (evento) => {
    evento.preventDefault();
    mensagem(aviso, '');
    const botao = $('#botao-entrar');
    ocupado(botao, true, 'Entrando...');
    try {
      const resposta = await Avalia.api.post('/auth/login', {
        email: formulario.email.value,
        senha: formulario.senha.value
      });
      Avalia.sessao.salvar({
        token: resposta.token,
        usuario: resposta.usuario,
        senhaInicial: resposta.senhaInicial
      });
      window.location.href = Avalia.sessao.inicio(resposta.usuario.idPerfil);
    } catch (erro) {
      mensagem(aviso, erro.message, 'erro');
      formulario.senha.value = '';
      formulario.senha.focus();
      ocupado(botao, false);
    }
  });

  $('#mostrar-senha').addEventListener('change', (evento) => {
    formulario.senha.type = evento.target.checked ? 'text' : 'password';
  });
})();
