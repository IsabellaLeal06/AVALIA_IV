// Definição das rotas. Cada rota declara quais perfis podem usá-la (permitir) ou que é aberta (publico).
// O perfil sempre vem do token. Sem regra declarada, a ação recusa a requisição (ver acao()).
const express = require('express');
const multer = require('multer');
const { autenticar, permitir, publico, acao } = require('../middleware/autenticacao');
const { PERFIL, TODOS } = require('../utils/perfis');
const auth = require('../controllers/authController');
const cadastro = require('../controllers/cadastroController');
const questao = require('../controllers/questaoController');
const aluno = require('../controllers/alunoController');
const { TAMANHO_MAXIMO } = require('../services/uploadService');

const { ALUNO, PROFESSOR, ADMINISTRADOR } = PERFIL;
const ADMIN = permitir(ADMINISTRADOR);
const ADMIN_PROFESSOR = permitir(ADMINISTRADOR, PROFESSOR);
const SO_ALUNO = permitir(ALUNO);
const SO_PROFESSOR = permitir(PROFESSOR);
const QUALQUER_PERFIL = permitir(...TODOS);

const upload = multer({ storage: multer.memoryStorage(), limits: { fileSize: TAMANHO_MAXIMO, files: 1 } });

/** Rotas abertas: login e as três etapas da recuperação de senha. */
function rotasPublicas() {
  const r = express.Router();
  r.post('/auth/login', publico, acao(auth.login));
  r.post('/auth/recuperar-senha', publico, acao(auth.recuperarSenha));
  r.get('/auth/validar-codigo', publico, acao(auth.validarCodigo));
  r.post('/auth/nova-senha', publico, acao(auth.novaSenha));
  return r;
}

/** Rotas que exigem token. O middleware autenticar vale para TODAS as rotas deste roteador. */
function rotasProtegidas() {
  const r = express.Router();
  r.use(autenticar);

  // 6.1 Senha e perfil do próprio usuário
  r.post('/auth/alterar-senha', QUALQUER_PERFIL, acao(auth.alterarSenha));
  r.get('/perfil', QUALQUER_PERFIL, acao(aluno.perfil));

  // 6.2 Cadastros institucionais (Administrador)
  r.get('/painel', ADMIN, acao(cadastro.painel));

  r.get('/usuarios', ADMIN, acao(cadastro.listarUsuarios));
  r.get('/usuarios/:id', ADMIN, acao(cadastro.consultarUsuario));
  r.post('/usuarios', ADMIN, acao(cadastro.incluirUsuario));
  r.put('/usuarios/:id', ADMIN, acao(cadastro.editarUsuario));
  r.patch('/usuarios/:id/situacao', ADMIN, acao(cadastro.situacaoUsuario));

  r.get('/turmas', ADMIN, acao(cadastro.listarTurmas));
  r.get('/turmas/:id', ADMIN, acao(cadastro.consultarTurma));
  r.post('/turmas', ADMIN, acao(cadastro.incluirTurma));
  r.put('/turmas/:id', ADMIN, acao(cadastro.editarTurma));
  r.patch('/turmas/:id/situacao', ADMIN, acao(cadastro.situacaoTurma));
  r.post('/turmas/:id/disciplinas', ADMIN, acao(cadastro.vincularDisciplina));
  r.delete('/turmas/:id/disciplinas/:idDisciplina', ADMIN, acao(cadastro.desvincularDisciplina));

  // /disciplinas/ativas antes de /disciplinas/:id
  r.get('/disciplinas/ativas', QUALQUER_PERFIL, acao(cadastro.listarDisciplinasAtivas));
  r.get('/disciplinas', ADMIN_PROFESSOR, acao(cadastro.listarDisciplinas));
  r.get('/disciplinas/:id', ADMIN, acao(cadastro.consultarDisciplina));
  r.post('/disciplinas', ADMIN, acao(cadastro.incluirDisciplina));
  r.put('/disciplinas/:id', ADMIN, acao(cadastro.editarDisciplina));
  r.patch('/disciplinas/:id/situacao', ADMIN, acao(cadastro.situacaoDisciplina));

  // Faixas de medalha (RN22): as mesmas operações da tela Gerenciar Medalha da Desktop
  r.get('/medalhas', ADMIN, acao(cadastro.listarMedalhas));
  r.get('/medalhas/:id', ADMIN, acao(cadastro.consultarMedalha));
  r.post('/medalhas', ADMIN, acao(cadastro.incluirMedalha));
  r.put('/medalhas/:id', ADMIN, acao(cadastro.editarMedalha));
  r.delete('/medalhas/:id', ADMIN, acao(cadastro.excluirMedalha));

  // 6.3 Banco de questões (Administrador e Professor, com escopo RN08 no service)
  r.get('/questoes', ADMIN_PROFESSOR, acao(questao.listar));
  r.get('/questoes/:id', ADMIN_PROFESSOR, acao(questao.consultar));
  r.post('/questoes', ADMIN_PROFESSOR, acao(questao.incluir));
  r.put('/questoes/:id', ADMIN_PROFESSOR, acao(questao.editar));
  r.patch('/questoes/:id/situacao', ADMIN_PROFESSOR, acao(questao.situacao));
  r.post('/upload/imagem', ADMIN_PROFESSOR, upload.single('imagem'), acao(questao.uploadImagem));

  // 6.4 Ciclo do simulado (Aluno)
  r.get('/simulados', SO_ALUNO, acao(aluno.historico));
  r.post('/simulados', SO_ALUNO, acao(aluno.gerarSimulado));
  r.get('/simulados/:id', SO_ALUNO, acao(aluno.consultarSimulado));
  r.post('/simulados/:id/finalizar', SO_ALUNO, acao(aluno.finalizarSimulado));
  r.get('/simulados/:id/resultado', SO_ALUNO, acao(aluno.resultadoSimulado));
  r.get('/simulados/:id/revisao', SO_ALUNO, acao(aluno.revisaoSimulado));

  // 6.5 Desempenho. O Administrador não tem nenhuma rota aqui (RN24).
  r.get('/desempenho/aluno', SO_ALUNO, acao(aluno.desempenhoAluno));
  r.get('/desempenho/turmas', SO_PROFESSOR, acao(aluno.turmasProfessor));
  r.get('/desempenho/turmas/:id', SO_PROFESSOR, acao(aluno.turmaProfessor));
  r.get('/desempenho/turmas/:id/alunos', SO_PROFESSOR, acao(aluno.alunosDaTurma));
  r.get('/desempenho/turmas/:id/alunos/:idAluno', SO_PROFESSOR, acao(aluno.alunoParaProfessor));
  r.get('/desempenho/questoes-erro', SO_PROFESSOR, acao(aluno.questoesComMaisErros));

  return r;
}

module.exports = { rotasPublicas, rotasProtegidas };
