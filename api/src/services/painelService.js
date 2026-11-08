// Painel do Administrador (RF02): turmas, usuários, disciplinas e questões ATIVOS. Sem desempenho (RN24).
const painelRepository = require('../repositories/painelRepository');

const painelService = {
  contar() {
    return painelRepository.contarAtivos();
  }
};

module.exports = painelService;
