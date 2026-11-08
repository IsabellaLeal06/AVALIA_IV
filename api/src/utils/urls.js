const ambiente = require('../config/ambiente');

/**
 * Endereço completo de uma imagem gravada no banco.
 * O banco guarda o caminho relativo (uploads/questoes/x.png, assets/medalhas/bronze.png);
 * a resposta leva o endereço completo, porque o aplicativo móvel não resolve caminho relativo.
 */
function urlCompleta(caminho) {
  if (!caminho) return null;
  if (/^https?:\/\//i.test(caminho)) return caminho;
  return `${ambiente.urlPublica}/${String(caminho).replace(/\\/g, '/').replace(/^\/+/, '')}`;
}

module.exports = { urlCompleta };
