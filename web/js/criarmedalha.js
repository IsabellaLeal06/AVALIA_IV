// Cadastro e edição de faixa de medalha (RN22): nome, pontuação mínima e imagem.
// A imagem pode ser uma das que acompanham a web (assets/medalhas) ou enviada pelo upload.
(async function () {
  const { $, mensagem, mostrarErros, limparErros, ocupado, parametro } = Avalia.ui;
  const { guardarMensagem } = Avalia.cadastro;
  Avalia.montarPagina({ perfis: [Avalia.PERFIL.ADMINISTRADOR] });

  const formulario = $('#formulario');
  const aviso = $('#mensagem');
  const id = Number(parametro('id')) || null;
  let imagemUrl = '';

  function mostrarImagem(caminho) {
    imagemUrl = caminho || '';
    const previa = $('#previa-imagem');
    previa.hidden = !imagemUrl;
    if (imagemUrl) previa.src = Avalia.api.url(imagemUrl);
  }

  formulario.imagemPadrao.addEventListener('change', () => {
    if (formulario.imagemPadrao.value) mostrarImagem(formulario.imagemPadrao.value);
  });

  $('#arquivo-imagem').addEventListener('change', async (evento) => {
    const arquivo = evento.target.files[0];
    if (!arquivo) return;
    $('#estado-imagem').textContent = 'Enviando imagem...';
    try {
      const enviado = await Avalia.api.enviarImagem(arquivo, 'medalhas');
      formulario.imagemPadrao.value = '';
      mostrarImagem(enviado.caminho);
      $('#estado-imagem').textContent = 'Imagem enviada.';
    } catch (erro) {
      $('#estado-imagem').textContent = '';
      mostrarErros(formulario, { imagemUrl: erro.message });
    }
  });

  if (id) {
    try {
      const medalha = await Avalia.api.get(`/medalhas/${id}`);
      $('#titulo-acao').textContent = 'EDITAR';
      $('#botao-salvar').textContent = 'Salvar alterações';
      formulario.nome.value = medalha.nome;
      formulario.pontosMinimos.value = medalha.pontosMinimos;
      const padrao = [...formulario.imagemPadrao.options].find((o) => o.value === medalha.imagemUrl);
      formulario.imagemPadrao.value = padrao ? medalha.imagemUrl : '';
      mostrarImagem(medalha.imagemUrl);
    } catch (erro) {
      mensagem(aviso, erro.message, 'erro');
    }
  }

  formulario.addEventListener('submit', async (evento) => {
    evento.preventDefault();
    limparErros(formulario);
    const dados = { nome: formulario.nome.value, pontosMinimos: Number(formulario.pontosMinimos.value), imagemUrl };
    const botao = $('#botao-salvar');
    ocupado(botao, true, 'Salvando...');
    try {
      const resposta = id ? await Avalia.api.put(`/medalhas/${id}`, dados) : await Avalia.api.post('/medalhas', dados);
      guardarMensagem(resposta.mensagem);
      window.location.href = 'adminmedalhas.html';
    } catch (erro) {
      mensagem(aviso, erro.message, 'erro');
      mostrarErros(formulario, erro.campos);
      ocupado(botao, false);
    }
  });
})();
