// Cadastro e edição de questão (Administrador e Professor). Exatamente 5 alternativas, A a E,
// uma correta (RN09). Explicação opcional. Sem campo de dificuldade. Disciplina filtrada pelo perfil (RN08).
// Na edição, se a API informar que a questão já foi usada em simulado, a alternativa correta fica bloqueada (RN11).
(async function () {
  const { $, el, mensagem, mostrarErros, limparErros, ocupado, parametro } = Avalia.ui;
  const { preencherSelect, guardarMensagem } = Avalia.cadastro;
  const { ADMINISTRADOR, PROFESSOR } = Avalia.PERFIL;
  const usuario = Avalia.montarPagina({ perfis: [ADMINISTRADOR, PROFESSOR] });

  const LETRAS = ['A', 'B', 'C', 'D', 'E'];
  const paginaLista = usuario.idPerfil === ADMINISTRADOR ? 'adminquestoes.html' : 'professorquestoes.html';
  const formulario = $('#formulario');
  const aviso = $('#mensagem');
  const id = Number(parametro('id')) || null;
  let imagemUrl = null;

  document.querySelectorAll('a[href="adminquestoes.html"]').forEach((a) => (a.href = paginaLista));

  // Cinco alternativas fixas, A a E
  $('#alternativas').replaceChildren(
    ...LETRAS.map((letra) =>
      el(
        'div',
        { class: 'campo alternativa' },
        el('label', { for: `alternativa${letra}`, texto: `Alternativa ${letra}` }),
        el(
          'div',
          { class: 'alternativa-linha' },
          el(
            'label',
            { class: 'marcar-correta', title: `Marcar ${letra} como correta` },
            el('input', { type: 'radio', name: 'correta', value: letra, 'aria-label': `${letra} é a correta` }),
            el('span', { texto: 'Correta' })
          ),
          el('textarea', { id: `alternativa${letra}`, name: `alternativa${letra}`, rows: '2', required: true })
        )
      )
    )
  );

  function mostrarImagem(url) {
    imagemUrl = url;
    const previa = $('#previa-imagem');
    if (url) {
      previa.src = Avalia.api.url(url);
      previa.hidden = false;
      $('#remover-imagem').hidden = false;
    } else {
      previa.removeAttribute('src');
      previa.hidden = true;
      $('#remover-imagem').hidden = true;
      $('#arquivo-imagem').value = '';
    }
  }

  // Upload: operação própria, concluída antes de gravar a questão
  $('#arquivo-imagem').addEventListener('change', async (evento) => {
    const arquivo = evento.target.files[0];
    if (!arquivo) return;
    $('#estado-imagem').textContent = 'Enviando imagem...';
    try {
      const enviado = await Avalia.api.enviarImagem(arquivo, 'questoes');
      mostrarImagem(enviado.caminho);
      $('#estado-imagem').textContent = 'Imagem enviada.';
    } catch (erro) {
      mostrarImagem(null);
      $('#estado-imagem').textContent = '';
      mostrarErros(formulario, erro.campos || { imagemUrl: erro.message });
    }
  });
  $('#remover-imagem').addEventListener('click', () => mostrarImagem(null));

  try {
    const [disciplinas, questao] = await Promise.all([
      Avalia.api.get('/disciplinas', { situacao: 'todos' }),
      id ? Avalia.api.get(`/questoes/${id}`) : null
    ]);

    // Disciplinas ativas do escopo; sem responsável, o cadastro de NOVAS questões fica suspenso (RN07)
    const opcoes = disciplinas
      .filter((d) => d.ativo || d.id === questao?.idDisciplina)
      .map((d) => {
        const semResponsavel = !d.idResponsavel && !questao;
        return {
          valor: d.id,
          texto: `${d.nome}${!d.ativo ? ' (inativa)' : ''}${semResponsavel ? ' (sem responsável: cadastro suspenso)' : ''}`,
          desabilitado: semResponsavel
        };
      });
    preencherSelect(formulario.idDisciplina, [{ valor: '', texto: 'Selecione a disciplina' }, ...opcoes], questao?.idDisciplina);

    if (questao) {
      $('#titulo-acao').textContent = 'EDITAR';
      $('#subtitulo').textContent = `Questão nº ${questao.id}, cadastrada por ${questao.autor}.`;
      $('#botao-salvar').textContent = 'Salvar alterações';
      formulario.enunciado.value = questao.enunciado;
      formulario.explicacao.value = questao.explicacao || '';
      for (const alternativa of questao.alternativas) {
        formulario[`alternativa${alternativa.letra}`].value = alternativa.texto;
      }
      formulario.querySelector(`input[name="correta"][value="${questao.correta}"]`).checked = true;
      mostrarImagem(questao.imagemUrl);

      if (questao.utilizada) {
        formulario.querySelectorAll('input[name="correta"]').forEach((r) => (r.disabled = true));
        mensagem(
          $('#aviso-rn11'),
          'Esta questão já foi usada em simulado: a alternativa correta não pode ser alterada (RN11). ' +
            'Enunciado, imagem, explicação e o texto das alternativas continuam editáveis. ' +
            'Se a resposta estiver errada, desative esta questão e cadastre uma nova.',
          'alerta'
        );
      }
    }
  } catch (erro) {
    mensagem(aviso, erro.message, 'erro');
  }

  formulario.addEventListener('submit', async (evento) => {
    evento.preventDefault();
    limparErros(formulario);
    const correta = formulario.querySelector('input[name="correta"]:checked')?.value || null;
    const dados = {
      idDisciplina: formulario.idDisciplina.value ? Number(formulario.idDisciplina.value) : null,
      enunciado: formulario.enunciado.value,
      explicacao: formulario.explicacao.value,
      imagemUrl,
      correta,
      alternativas: LETRAS.map((letra) => ({ letra, texto: formulario[`alternativa${letra}`].value }))
    };
    const botao = $('#botao-salvar');
    ocupado(botao, true, 'Salvando...');
    try {
      const resposta = id ? await Avalia.api.put(`/questoes/${id}`, dados) : await Avalia.api.post('/questoes', dados);
      guardarMensagem(resposta.mensagem);
      window.location.href = paginaLista;
    } catch (erro) {
      mensagem(aviso, erro.message, 'erro');
      mostrarErros(formulario, erro.campos);
      ocupado(botao, false);
    }
  });
})();
