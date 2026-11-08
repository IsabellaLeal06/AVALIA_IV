// Ids fixos da tabela Perfil (script 03_carga_inicial.sql)
const PERFIL = Object.freeze({
  ALUNO: 1,
  PROFESSOR: 2,
  ADMINISTRADOR: 3
});

const NOME_PERFIL = Object.freeze({
  [PERFIL.ALUNO]: 'Aluno',
  [PERFIL.PROFESSOR]: 'Professor',
  [PERFIL.ADMINISTRADOR]: 'Administrador'
});

const TODOS = Object.freeze([PERFIL.ALUNO, PERFIL.PROFESSOR, PERFIL.ADMINISTRADOR]);

function perfilValido(id) {
  return TODOS.includes(Number(id));
}

module.exports = { PERFIL, NOME_PERFIL, TODOS, perfilValido };
