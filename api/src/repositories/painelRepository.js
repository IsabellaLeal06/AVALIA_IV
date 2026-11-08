// Painel do Administrador (RF02): só quantidades de cadastros ATIVOS. Nenhum dado de desempenho (RN24).
const { consultarUm } = require('../config/banco');

const painelRepository = {
  contarAtivos() {
    return consultarUm(
      `SELECT (SELECT COUNT(*) FROM dbo.Turma WHERE Ativo_Turma = 1)           AS turmas,
              (SELECT COUNT(*) FROM dbo.Usuario WHERE Ativo_Usuario = 1)       AS usuarios,
              (SELECT COUNT(*) FROM dbo.Disciplina WHERE Ativo_Disciplina = 1) AS disciplinas,
              (SELECT COUNT(*) FROM dbo.Questao WHERE Ativo_Questao = 1)       AS questoes`
    );
  }
};

module.exports = painelRepository;
