// Cadastro e edição de usuário (RN01). Sem campo de senha: a senha inicial é o CPF (RN02).
// A turma só aparece para o perfil Aluno (RN05).
(async function () {
  const { $, mensagem, mostrarErros, limparErros, ocupado, parametro, mascaraCpf } = Avalia.ui;
  const { preencherSelect, guardarMensagem } = Avalia.cadastro;
  Avalia.montarPagina({ perfis: [Avalia.PERFIL.ADMINISTRADOR] });

  const formulario = $('#formulario');
  const aviso = $('#mensagem');
  const id = Number(parametro('id')) || null;
  mascaraCpf(formulario.cpf);

  function atualizarTurma() {
    const aluno = Number(formulario.idPerfil.value) === Avalia.PERFIL.ALUNO;
    $('#campo-turma').hidden = !aluno;
    formulario.idTurma.required = aluno;
  }

  try {
    const [turmas, usuario] = await Promise.all([
      Avalia.api.get('/turmas', { situacao: 'todos' }),
      id ? Avalia.api.get(`/usuarios/${id}`) : null
    ]);

    // Turmas ativas; a turma atual do aluno aparece mesmo se estiver desativada
    preencherSelect(
      formulario.idTurma,
      [
        { valor: '', texto: 'Selecione a turma' },
        ...turmas
          .filter((t) => t.ativo || t.id === usuario?.idTurma)
          .map((t) => ({ valor: t.id, texto: `${t.nome} (${t.anoLetivo})${t.ativo ? '' : ' - inativa'}` }))
      ],
      usuario?.idTurma
    );

    if (usuario) {
      $('#titulo-acao').textContent = 'EDITAR';
      $('#subtitulo').textContent = 'Altere os dados cadastrais. A senha é mantida pelo próprio usuário.';
      $('#aviso-senha').hidden = true;
      $('#botao-salvar').textContent = 'Salvar alterações';
      formulario.nome.value = usuario.nome;
      formulario.cpf.value = usuario.cpf;
      formulario.cpf.dispatchEvent(new Event('input'));
      formulario.email.value = usuario.email;
      formulario.idPerfil.value = usuario.idPerfil;
      formulario.dataIngresso.value = usuario.dataIngresso;
    } else {
      formulario.dataIngresso.value = new Intl.DateTimeFormat('en-CA', { timeZone: 'America/Sao_Paulo' }).format(new Date());
    }
    atualizarTurma();
  } catch (erro) {
    mensagem(aviso, erro.message, 'erro');
  }

  formulario.idPerfil.addEventListener('change', atualizarTurma);

  formulario.addEventListener('submit', async (evento) => {
    evento.preventDefault();
    limparErros(formulario);
    const idPerfil = Number(formulario.idPerfil.value);
    const dados = {
      nome: formulario.nome.value,
      cpf: formulario.cpf.value,
      email: formulario.email.value,
      idPerfil,
      idTurma: idPerfil === Avalia.PERFIL.ALUNO ? formulario.idTurma.value || null : null,
      dataIngresso: formulario.dataIngresso.value
    };
    const botao = $('#botao-salvar');
    ocupado(botao, true, 'Salvando...');
    try {
      const resposta = id ? await Avalia.api.put(`/usuarios/${id}`, dados) : await Avalia.api.post('/usuarios', dados);
      guardarMensagem(resposta.mensagem);
      window.location.href = 'adminusuarios.html';
    } catch (erro) {
      mensagem(aviso, erro.message, 'erro');
      mostrarErros(formulario, erro.campos);
      ocupado(botao, false);
    }
  });
})();
