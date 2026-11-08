// Upload de imagens (questões e medalhas): operação própria, concluída antes de gravar o registro.
// Valida tipo (pelo conteúdo, não pela extensão) e tamanho, e gera o nome no servidor.
const fs = require('fs/promises');
const path = require('path');
const crypto = require('crypto');
const ambiente = require('../config/ambiente');
const { ErroValidacao } = require('../utils/erros');
const { urlCompleta } = require('../utils/urls');

const TAMANHO_MAXIMO = 2 * 1024 * 1024; // 2 MB, como na Desktop
const PASTAS = { questoes: 'questoes', medalhas: 'medalhas' };

/** Identifica a imagem pelos primeiros bytes do arquivo. */
function tipoDaImagem(buffer) {
  if (!buffer || buffer.length < 12) return null;
  if (buffer.subarray(0, 8).equals(Buffer.from([0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a]))) return '.png';
  if (buffer[0] === 0xff && buffer[1] === 0xd8 && buffer[2] === 0xff) return '.jpg';
  if (buffer.subarray(0, 6).toString('ascii') === 'GIF87a' || buffer.subarray(0, 6).toString('ascii') === 'GIF89a') return '.gif';
  if (buffer.subarray(0, 4).toString('ascii') === 'RIFF' && buffer.subarray(8, 12).toString('ascii') === 'WEBP') return '.webp';
  return null;
}

/**
 * Converte o que o cliente enviou (caminho relativo ou endereço completo devolvido pelo upload)
 * no caminho relativo gravado no banco. Só aceita arquivos da pasta de uploads do tipo informado.
 * Medalhas aceitam também as imagens que acompanham a aplicação web (assets/medalhas/...).
 */
function caminhoDeImagem(valor, tipo) {
  if (valor === undefined || valor === null || String(valor).trim() === '') return null;
  let caminho = String(valor).trim().replace(/\\/g, '/');
  const prefixo = `${ambiente.urlPublica}/`;
  if (caminho.startsWith(prefixo)) caminho = caminho.slice(prefixo.length);
  caminho = caminho.replace(/^\/+/, '');
  const nomeArquivo = '[A-Za-z0-9._-]+\\.(png|jpe?g|gif|webp)';
  const permitido = new RegExp(`^uploads/${PASTAS[tipo]}/${nomeArquivo}$`, 'i');
  const assetsWeb = new RegExp(`^assets/medalhas/${nomeArquivo}$`, 'i');
  if (permitido.test(caminho) || (tipo === 'medalhas' && assetsWeb.test(caminho))) return caminho;
  return null;
}

const uploadService = {
  TAMANHO_MAXIMO,
  caminhoDeImagem,

  async salvarImagem(arquivo, tipo = 'questoes') {
    if (!PASTAS[tipo]) throw new ErroValidacao('Tipo de imagem inválido.');
    if (!arquivo) throw new ErroValidacao('Envie o arquivo da imagem no campo "imagem".', { imagem: 'Selecione a imagem.' });
    if (arquivo.size > TAMANHO_MAXIMO) throw new ErroValidacao('A imagem deve ter no máximo 2 MB.', { imagem: 'Máximo de 2 MB.' });

    const extensao = tipoDaImagem(arquivo.buffer);
    if (!extensao) {
      throw new ErroValidacao('Formato de imagem não suportado. Use PNG, JPG, GIF ou WEBP.', { imagem: 'Formato não suportado.' });
    }

    const pasta = path.join(ambiente.pastaUploads, PASTAS[tipo]);
    await fs.mkdir(pasta, { recursive: true });
    // Nome gerado no servidor: o nome original do arquivo não é usado
    const nome = `${crypto.randomUUID()}${extensao}`;
    await fs.writeFile(path.join(pasta, nome), arquivo.buffer);

    const caminho = `uploads/${PASTAS[tipo]}/${nome}`;
    return { caminho, url: urlCompleta(caminho) };
  }
};

module.exports = uploadService;
