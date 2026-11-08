// Perfil do usuário logado e gamificação (RF07, RN21, RN22, RN23).
const usuarioRepository = require('../repositories/usuarioRepository');
const medalhaRepository = require('../repositories/medalhaRepository');
const { ErroNaoEncontrado } = require('../utils/erros');
const { PERFIL } = require('../utils/perfis');
const { data } = require('../utils/datas');
const { urlCompleta } = require('../utils/urls');

function medalhaParaResposta(medalha) {
  if (!medalha) return null;
  return {
    id: medalha.id,
    nome: medalha.nome,
    pontosMinimos: medalha.pontosMinimos,
    imagemUrl: urlCompleta(medalha.imagemUrl)
  };
}

const perfilService = {
  /**
   * GET /perfil. Para o Aluno, a medalha é APURADA aqui, comparando a pontuação com as faixas
   * da tabela Medalha (nunca gravada nem escrita no código, RN22).
   */
  async consultar(usuarioToken) {
    const usuario = await usuarioRepository.consultar(usuarioToken.id);
    if (!usuario) throw new ErroNaoEncontrado('Usuário não encontrado.');

    const perfil = {
      id: usuario.id,
      nome: usuario.nome,
      email: usuario.email,
      idPerfil: usuario.idPerfil,
      perfil: usuario.nomePerfil,
      dataIngresso: data(usuario.dataIngresso),
      turma: usuario.idTurma ? `${usuario.nomeTurma} (${usuario.anoLetivoTurma})` : null
    };
    if (usuario.idPerfil !== PERFIL.ALUNO) return perfil;

    const pontuacao = usuario.pontuacaoTotal;
    const atual = await medalhaRepository.atual(pontuacao);
    const proxima = await medalhaRepository.proxima(pontuacao);

    let mensagem;
    if (!proxima) mensagem = atual ? `Parabéns! Você alcançou o nível máximo: medalha ${atual.nome}.` : 'Nenhuma faixa de medalha cadastrada.';
    else mensagem = `Faltam ${proxima.pontosMinimos - pontuacao} pontos para a medalha ${proxima.nome}.`;

    return {
      ...perfil,
      pontuacaoTotal: pontuacao,
      gamificacao: {
        medalha: medalhaParaResposta(atual),
        // Sem medalha: a tela mostra a próxima (a primeira faixa, Bronze) acinzentada (RN23)
        semMedalha: !atual,
        proxima: proxima ? { ...medalhaParaResposta(proxima), faltam: proxima.pontosMinimos - pontuacao } : null,
        nivelMaximo: Boolean(atual) && !proxima,
        mensagem
      }
    };
  }
};

module.exports = perfilService;
