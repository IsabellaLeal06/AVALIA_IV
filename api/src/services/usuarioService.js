// Cadastro de usuários pelo Administrador (RF02, RN01, RN02, RN05, RN12, RN27).
const usuarioRepository = require('../repositories/usuarioRepository');
const turmaRepository = require('../repositories/turmaRepository');
const authService = require('./authService');
const { ErroValidacao, ErroNaoEncontrado, ErroConflito } = require('../utils/erros');
const v = require('../utils/validacoes');
const { PERFIL, NOME_PERFIL, perfilValido } = require('../utils/perfis');
const { data, hojeBrasilia } = require('../utils/datas');

const TAMANHO_MAXIMO_NOME = 150;

/** Resposta da listagem: SEM CPF (RNF02). A omissão é feita aqui, não no cliente. */
function paraListagem(u) {
  return {
    id: u.id,
    nome: u.nome,
    email: u.email,
    idPerfil: u.idPerfil,
    perfil: u.nomePerfil,
    idTurma: u.idTurma,
    turma: u.idTurma ? `${u.nomeTurma} (${u.anoLetivoTurma})` : null,
    dataIngresso: data(u.dataIngresso),
    ativo: u.ativo
  };
}

/** Consulta individual: o CPF aparece só aqui. */
function paraDetalhe(u) {
  return { ...paraListagem(u), cpf: u.cpf };
}

function normalizar(dados) {
  const idPerfil = Number(dados.idPerfil);
  return {
    nome: v.normalizarTexto(dados.nome),
    cpf: v.normalizarCpf(dados.cpf),
    cpfInformado: dados.cpf,
    email: v.normalizarEmail(dados.email),
    idPerfil,
    // RN05: somente Aluno possui turma; para os outros perfis o campo fica nulo
    idTurma: idPerfil === PERFIL.ALUNO ? v.idOuNulo(dados.idTurma) : null,
    dataIngresso: dados.dataIngresso ? String(dados.dataIngresso).slice(0, 10) : hojeBrasilia()
  };
}

async function validar(usuario, atual, usuarioToken) {
  const campos = {};

  if (usuario.nome.length < 3) campos.nome = 'Informe o nome completo.';
  else if (usuario.nome.length > TAMANHO_MAXIMO_NOME) campos.nome = `O nome deve ter no máximo ${TAMANHO_MAXIMO_NOME} caracteres.`;

  if (!usuario.cpf) campos.cpf = 'Informe o CPF.';
  else if (!v.cpfValido(usuario.cpfInformado)) campos.cpf = 'CPF inválido. Confira os dígitos.';

  if (!usuario.email) campos.email = 'Informe o e-mail.';
  else if (!v.emailValido(usuario.email)) campos.email = 'E-mail inválido.';

  if (!perfilValido(usuario.idPerfil)) campos.idPerfil = 'Selecione o perfil.';

  const dataIngresso = new Date(`${usuario.dataIngresso}T00:00:00Z`);
  const limite = new Date(`${hojeBrasilia()}T00:00:00Z`);
  limite.setUTCFullYear(limite.getUTCFullYear() + 1);
  if (!/^\d{4}-\d{2}-\d{2}$/.test(usuario.dataIngresso) || Number.isNaN(dataIngresso.getTime()) ||
      dataIngresso.getUTCFullYear() < 2000 || dataIngresso > limite) {
    campos.dataIngresso = 'Data de ingresso inválida.';
  }

  // RN05: turma obrigatória para Aluno, e somente turmas ativas (a turma atual do aluno é mantida)
  if (usuario.idPerfil === PERFIL.ALUNO) {
    if (!usuario.idTurma || Number.isNaN(usuario.idTurma)) {
      campos.idTurma = 'Selecione a turma do aluno.';
    } else {
      const turma = await turmaRepository.consultar(usuario.idTurma);
      if (!turma) campos.idTurma = 'Turma não encontrada.';
      else if (!turma.ativo && atual?.idTurma !== turma.id) campos.idTurma = 'A turma selecionada está desativada. Escolha uma turma ativa.';
    }
  }

  if (atual && atual.idPerfil !== usuario.idPerfil && perfilValido(usuario.idPerfil)) {
    if (atual.id === usuarioToken.id) {
      campos.idPerfil = 'Você não pode alterar o seu próprio perfil.';
    } else if (atual.idPerfil === PERFIL.PROFESSOR) {
      const disciplinas = await usuarioRepository.contarDisciplinasSobResponsabilidade(atual.id);
      if (disciplinas > 0) {
        campos.idPerfil = `Este professor é responsável por ${disciplinas} disciplina(s). Defina outro responsável antes de alterar o perfil.`;
      }
    }
  }

  // Unicidade (RN27): verificada antes para dar uma mensagem clara; o banco mantém o UNIQUE como última defesa
  if (!campos.email && (await usuarioRepository.existeEmail(usuario.email, atual?.id))) {
    campos.email = 'Já existe um usuário com este e-mail.';
  }
  if (!campos.cpf && (await usuarioRepository.existeCpf(usuario.cpf, atual?.id))) {
    campos.cpf = 'Já existe um usuário com este CPF.';
  }

  ErroValidacao.verificar(campos);
}

const usuarioService = {
  async listar(filtros) {
    const idPerfil = v.idOuNulo(filtros.perfil);
    const idTurma = v.idOuNulo(filtros.turma);
    const lista = await usuarioRepository.listar({
      idPerfil: Number.isNaN(idPerfil) ? null : idPerfil,
      idTurma: Number.isNaN(idTurma) ? null : idTurma,
      ativo: v.filtroSituacao(filtros.situacao, 'todos'),
      busca: filtros.busca
    });
    return lista.map(paraListagem);
  },

  async consultar(id) {
    const usuario = await usuarioRepository.consultar(id);
    if (!usuario) throw new ErroNaoEncontrado('Usuário não encontrado.');
    return paraDetalhe(usuario);
  },

  /** RN01: só o Administrador cria contas. RN02: a senha inicial é o CPF. */
  async incluir(dados, usuarioToken) {
    const usuario = normalizar(dados);
    await validar(usuario, null, usuarioToken);
    usuario.senhaHash = await authService.gerarHashSenha(usuario.cpf);
    const id = await usuarioRepository.incluir(usuario);
    return {
      id,
      mensagem: 'Usuário cadastrado com sucesso. A senha inicial é o CPF (somente números).'
    };
  },

  async editar(id, dados, usuarioToken) {
    const atual = await usuarioRepository.consultar(id);
    if (!atual) throw new ErroNaoEncontrado('Usuário não encontrado.');
    if (!atual.ativo) throw new ErroConflito('O usuário está desativado e não pode ser editado. Reative-o antes.');

    const usuario = normalizar(dados);
    await validar(usuario, atual, usuarioToken);
    await usuarioRepository.editar(id, usuario);
    return { id, mensagem: 'Usuário atualizado com sucesso.' };
  },

  /** PATCH /usuarios/:id/situacao. Desativação lógica (RN12). */
  async alterarSituacao(id, ativo, usuarioToken) {
    if (ativo === null) throw new ErroValidacao('Informe a situação: ativo true ou false.', { ativo: 'Valor inválido.' });
    const usuario = await usuarioRepository.consultar(id);
    if (!usuario) throw new ErroNaoEncontrado('Usuário não encontrado.');
    if (!ativo && id === usuarioToken.id) throw new ErroConflito('Você não pode desativar a sua própria conta.');
    if (usuario.ativo === ativo) {
      throw new ErroConflito(ativo ? 'O usuário já está ativo.' : 'O usuário já está desativado.');
    }

    await usuarioRepository.alterarSituacao(id, ativo);
    const resposta = {
      id,
      ativo,
      mensagem: ativo ? `Usuário "${usuario.nome}" reativado.` : `Usuário "${usuario.nome}" desativado.`
    };
    if (!ativo && usuario.idPerfil === PERFIL.PROFESSOR) {
      const disciplinas = await usuarioRepository.contarDisciplinasSobResponsabilidade(id);
      if (disciplinas > 0) {
        resposta.aviso = `Este professor é responsável por ${disciplinas} disciplina(s). Defina outro responsável em Disciplinas.`;
      }
    }
    return resposta;
  },

  perfis() {
    return Object.entries(NOME_PERFIL).map(([id, nome]) => ({ id: Number(id), nome }));
  }
};

module.exports = usuarioService;
