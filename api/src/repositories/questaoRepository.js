// Acesso às tabelas Questao e Alternativa.
const { sql, consultar, consultarUm, executar } = require('../config/banco');
const { padraoBusca, listaDeIds } = require('./auxiliares');

const questaoRepository = {
  /**
   * Listagem. `idsDisciplinas` restringe ao escopo do Professor (RN08):
   * lista vazia = nenhuma questão; null = sem restrição (Administrador).
   */
  listar({ idDisciplina = null, idsDisciplinas = null, ativo = null, busca = null } = {}) {
    if (idsDisciplinas && idsDisciplinas.length === 0) return Promise.resolve([]);
    const escopo = idsDisciplinas ? listaDeIds(idsDisciplinas, 'e') : { lista: null, parametros: {} };
    return consultar(
      `SELECT q.Id_Questao AS id, q.Enunciado_Questao AS enunciado, q.Ativo_Questao AS ativo,
              q.Id_Disciplina AS idDisciplina, d.Nome_Disciplina AS nomeDisciplina, d.Ativo_Disciplina AS disciplinaAtiva,
              q.Id_Usuario AS idAutor, u.Nome_Usuario AS nomeAutor,
              CASE WHEN q.ImagemUrl_Questao IS NULL THEN 0 ELSE 1 END AS temImagem,
              CASE WHEN EXISTS (SELECT 1 FROM dbo.SimuladoQuestao sq WHERE sq.Id_Questao = q.Id_Questao) THEN 1 ELSE 0 END AS utilizada
       FROM dbo.Questao q
       JOIN dbo.Disciplina d ON d.Id_Disciplina = q.Id_Disciplina
       JOIN dbo.Usuario u ON u.Id_Usuario = q.Id_Usuario
       WHERE (@Disciplina IS NULL OR q.Id_Disciplina = @Disciplina)
         AND (@Ativo IS NULL OR q.Ativo_Questao = @Ativo)
         AND (@Busca IS NULL OR q.Enunciado_Questao LIKE @Busca ESCAPE '\\')
         ${escopo.lista ? `AND q.Id_Disciplina IN (${escopo.lista})` : ''}
       ORDER BY d.Nome_Disciplina, q.Id_Questao`,
      {
        Disciplina: [sql.Int, idDisciplina],
        Ativo: [sql.Bit, ativo],
        Busca: [sql.NVarChar(sql.MAX), padraoBusca(busca)],
        ...escopo.parametros
      }
    );
  },

  async consultar(id) {
    const questao = await consultarUm(
      `SELECT q.Id_Questao AS id, q.Enunciado_Questao AS enunciado, q.ImagemUrl_Questao AS imagemUrl,
              q.ExplicacaoResposta_Questao AS explicacao, q.Ativo_Questao AS ativo,
              q.Id_Disciplina AS idDisciplina, d.Nome_Disciplina AS nomeDisciplina, d.Ativo_Disciplina AS disciplinaAtiva,
              d.Id_Usuario AS idResponsavelDisciplina,
              q.Id_Usuario AS idAutor, u.Nome_Usuario AS nomeAutor
       FROM dbo.Questao q
       JOIN dbo.Disciplina d ON d.Id_Disciplina = q.Id_Disciplina
       JOIN dbo.Usuario u ON u.Id_Usuario = q.Id_Usuario
       WHERE q.Id_Questao = @Id`,
      { Id: [sql.Int, id] }
    );
    if (!questao) return null;
    questao.alternativas = await consultar(
      `SELECT Letra_Alternativa AS letra, Texto_Alternativa AS texto, Correta_Alternativa AS correta
       FROM dbo.Alternativa WHERE Id_Questao = @Id ORDER BY Letra_Alternativa`,
      { Id: [sql.Int, id] }
    );
    return questao;
  },

  /** RN11: a questão entrou em algum simulado (sorteada), mesmo que ainda não respondida. */
  async foiUtilizada(id) {
    const linha = await consultarUm('SELECT TOP (1) 1 AS usada FROM dbo.SimuladoQuestao WHERE Id_Questao = @Id', {
      Id: [sql.Int, id]
    });
    return Boolean(linha);
  },

  /** Questão + 5 alternativas, na transação informada. */
  async incluir(questao, transacao) {
    const linha = await consultarUm(
      `INSERT INTO dbo.Questao (Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao,
                                Id_Disciplina, Id_Usuario)
       OUTPUT INSERTED.Id_Questao AS id
       VALUES (@Enunciado, @Imagem, @Explicacao, 1, @Disciplina, @Autor)`,
      {
        Enunciado: [sql.NVarChar(sql.MAX), questao.enunciado],
        Imagem: [sql.VarChar(500), questao.imagemUrl],
        Explicacao: [sql.NVarChar(sql.MAX), questao.explicacao],
        Disciplina: [sql.Int, questao.idDisciplina],
        Autor: [sql.Int, questao.idAutor]
      },
      transacao
    );
    for (const alternativa of questao.alternativas) {
      await executar(
        `INSERT INTO dbo.Alternativa (Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao)
         VALUES (@Letra, @Texto, @Correta, @Questao)`,
        {
          Letra: [sql.Char(1), alternativa.letra],
          Texto: [sql.NVarChar(sql.MAX), alternativa.texto],
          Correta: [sql.Bit, alternativa.correta],
          Questao: [sql.Int, linha.id]
        },
        transacao
      );
    }
    return linha.id;
  },

  /** Atualiza a questão e as 5 alternativas (pela letra), na transação informada. A autoria não muda (RN10). */
  async editar(id, questao, transacao) {
    await executar(
      `UPDATE dbo.Questao
          SET Enunciado_Questao = @Enunciado, ImagemUrl_Questao = @Imagem,
              ExplicacaoResposta_Questao = @Explicacao, Id_Disciplina = @Disciplina
        WHERE Id_Questao = @Id`,
      {
        Id: [sql.Int, id],
        Enunciado: [sql.NVarChar(sql.MAX), questao.enunciado],
        Imagem: [sql.VarChar(500), questao.imagemUrl],
        Explicacao: [sql.NVarChar(sql.MAX), questao.explicacao],
        Disciplina: [sql.Int, questao.idDisciplina]
      },
      transacao
    );
    for (const alternativa of questao.alternativas) {
      await executar(
        `UPDATE dbo.Alternativa SET Texto_Alternativa = @Texto, Correta_Alternativa = @Correta
          WHERE Id_Questao = @Questao AND Letra_Alternativa = @Letra`,
        {
          Letra: [sql.Char(1), alternativa.letra],
          Texto: [sql.NVarChar(sql.MAX), alternativa.texto],
          Correta: [sql.Bit, alternativa.correta],
          Questao: [sql.Int, id]
        },
        transacao
      );
    }
  },

  /** Exclusão lógica (RN12). */
  async alterarSituacao(id, ativo) {
    await executar('UPDATE dbo.Questao SET Ativo_Questao = @Ativo WHERE Id_Questao = @Id', {
      Id: [sql.Int, id],
      Ativo: [sql.Bit, ativo]
    });
  },

  async contarAtivas() {
    const linha = await consultarUm('SELECT COUNT(*) AS total FROM dbo.Questao WHERE Ativo_Questao = 1');
    return linha.total;
  }
};

module.exports = questaoRepository;
