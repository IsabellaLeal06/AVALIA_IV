// Acesso às tabelas do ciclo do simulado: Simulado, SimuladoQuestao, RespostaAluno,
// ResultadoSimulado e Desempenho. Respostas e resultados nunca são alterados nem removidos (RN19).
const { sql, consultar, consultarUm, executar } = require('../config/banco');
const { listaDeIds, AGORA } = require('./auxiliares');

// Somente questões ativas de disciplinas ativas participam do sorteio (RN12, RN15)
const QUESTOES_SORTEAVEIS = `
  FROM dbo.Questao q
  JOIN dbo.Disciplina d ON d.Id_Disciplina = q.Id_Disciplina
  WHERE q.Ativo_Questao = 1 AND d.Ativo_Disciplina = 1`;

const simuladoRepository = {
  /** Quantas questões sorteáveis existem em cada disciplina informada. */
  contarDisponiveis(idsDisciplinas) {
    const { lista, parametros } = listaDeIds(idsDisciplinas, 'd');
    return consultar(
      `SELECT q.Id_Disciplina AS idDisciplina, COUNT(*) AS total
       ${QUESTOES_SORTEAVEIS} AND q.Id_Disciplina IN (${lista})
       GROUP BY q.Id_Disciplina`,
      parametros
    );
  },

  /** Sorteio livre no conjunto das disciplinas, sem repetição e sem divisão fixa entre elas (RN15). */
  async sortear(idsDisciplinas, quantidade) {
    const { lista, parametros } = listaDeIds(idsDisciplinas, 'd');
    const linhas = await consultar(
      `SELECT TOP (@Quantidade) q.Id_Questao AS id
       ${QUESTOES_SORTEAVEIS} AND q.Id_Disciplina IN (${lista})
       ORDER BY NEWID()`,
      { Quantidade: [sql.Int, quantidade], ...parametros }
    );
    return linhas.map((l) => l.id);
  },

  /** Grava o simulado e a ordem de apresentação das questões (SimuladoQuestao). */
  async criar({ titulo, idUsuario, idsQuestoes }, transacao) {
    const linha = await consultarUm(
      `INSERT INTO dbo.Simulado (Titulo_Simulado, QuantidadeQuestoes_Simulado, DataCriacao_Simulado, Id_Usuario)
       OUTPUT INSERTED.Id_Simulado AS id, INSERTED.DataCriacao_Simulado AS dataCriacao
       VALUES (@Titulo, @Quantidade, ${AGORA}, @Usuario)`,
      {
        Titulo: [sql.NVarChar(200), titulo],
        Quantidade: [sql.Int, idsQuestoes.length],
        Usuario: [sql.Int, idUsuario]
      },
      transacao
    );
    for (const [indice, idQuestao] of idsQuestoes.entries()) {
      await executar(
        'INSERT INTO dbo.SimuladoQuestao (Id_Simulado, Id_Questao, Ordem) VALUES (@Simulado, @Questao, @Ordem)',
        { Simulado: [sql.Int, linha.id], Questao: [sql.Int, idQuestao], Ordem: [sql.Int, indice + 1] },
        transacao
      );
    }
    return linha;
  },

  consultar(id) {
    return consultarUm(
      `SELECT s.Id_Simulado AS id, s.Titulo_Simulado AS titulo, s.QuantidadeQuestoes_Simulado AS quantidade,
              s.DataCriacao_Simulado AS dataCriacao, s.Id_Usuario AS idUsuario,
              CAST(CASE WHEN r.Id_ResultadoSimulado IS NULL THEN 0 ELSE 1 END AS BIT) AS finalizado
       FROM dbo.Simulado s
       LEFT JOIN dbo.ResultadoSimulado r ON r.Id_Simulado = s.Id_Simulado
       WHERE s.Id_Simulado = @Id`,
      { Id: [sql.Int, id] }
    );
  },

  /** Questões para a realização: SEM gabarito e SEM explicação. */
  async questoesParaRealizacao(idSimulado) {
    const questoes = await consultar(
      `SELECT sq.Ordem AS ordem, q.Id_Questao AS id, q.Enunciado_Questao AS enunciado,
              q.ImagemUrl_Questao AS imagemUrl, d.Nome_Disciplina AS nomeDisciplina
       FROM dbo.SimuladoQuestao sq
       JOIN dbo.Questao q ON q.Id_Questao = sq.Id_Questao
       JOIN dbo.Disciplina d ON d.Id_Disciplina = q.Id_Disciplina
       WHERE sq.Id_Simulado = @Id
       ORDER BY sq.Ordem`,
      { Id: [sql.Int, idSimulado] }
    );
    const alternativas = await consultar(
      `SELECT a.Id_Questao AS idQuestao, a.Letra_Alternativa AS letra, a.Texto_Alternativa AS texto
       FROM dbo.SimuladoQuestao sq
       JOIN dbo.Alternativa a ON a.Id_Questao = sq.Id_Questao
       WHERE sq.Id_Simulado = @Id
       ORDER BY a.Id_Questao, a.Letra_Alternativa`,
      { Id: [sql.Int, idSimulado] }
    );
    return questoes.map((q) => ({
      ...q,
      alternativas: alternativas.filter((a) => a.idQuestao === q.id).map(({ letra, texto }) => ({ letra, texto }))
    }));
  },

  /** Gabarito, usado só pela correção no servidor. */
  gabarito(idSimulado, transacao) {
    return consultar(
      `SELECT sq.Id_Questao AS idQuestao, q.Id_Disciplina AS idDisciplina, a.Letra_Alternativa AS correta
       FROM dbo.SimuladoQuestao sq
       JOIN dbo.Questao q ON q.Id_Questao = sq.Id_Questao
       LEFT JOIN dbo.Alternativa a ON a.Id_Questao = sq.Id_Questao AND a.Correta_Alternativa = 1
       WHERE sq.Id_Simulado = @Id
       ORDER BY sq.Ordem`,
      { Id: [sql.Int, idSimulado] },
      transacao
    );
  },

  /**
   * Grava o resultado. É o PRIMEIRO passo da finalização: o UNIQUE de Id_Simulado
   * barra uma segunda finalização simultânea antes de qualquer outra gravação (RN16).
   */
  async inserirResultado(r, transacao) {
    await executar(
      `INSERT INTO dbo.ResultadoSimulado (TotalQuestoes_ResultadoSimulado, TotalAcertos_ResultadoSimulado,
                                          Percentual_ResultadoSimulado, PontosObtidos_ResultadoSimulado,
                                          DataRealizacao_ResultadoSimulado, Id_Usuario, Id_Simulado)
       VALUES (@Total, @Acertos, @Percentual, @Pontos, ${AGORA}, @Usuario, @Simulado)`,
      {
        Total: [sql.Int, r.totalQuestoes],
        Acertos: [sql.Int, r.totalAcertos],
        Percentual: [sql.Decimal(5, 2), r.percentual],
        Pontos: [sql.Int, r.pontosObtidos],
        Usuario: [sql.Int, r.idUsuario],
        Simulado: [sql.Int, r.idSimulado]
      },
      transacao
    );
  },

  async inserirResposta(resposta, transacao) {
    await executar(
      `INSERT INTO dbo.RespostaAluno (AlternativaMarcada_RespostaAluno, Acertou_RespostaAluno, DataResposta_RespostaAluno,
                                      Id_Usuario, Id_Simulado, Id_Questao)
       VALUES (@Marcada, @Acertou, ${AGORA}, @Usuario, @Simulado, @Questao)`,
      {
        Marcada: [sql.Char(1), resposta.marcada],
        Acertou: [sql.Bit, resposta.acertou],
        Usuario: [sql.Int, resposta.idUsuario],
        Simulado: [sql.Int, resposta.idSimulado],
        Questao: [sql.Int, resposta.idQuestao]
      },
      transacao
    );
  },

  /** Acumula o desempenho do aluno na disciplina (um registro por aluno e disciplina). */
  async acumularDesempenho({ idUsuario, idDisciplina, respostas, acertos }, transacao) {
    await executar(
      `UPDATE dbo.Desempenho WITH (UPDLOCK, HOLDLOCK)
          SET TotalRespostas_Desempenho = TotalRespostas_Desempenho + @Respostas,
              TotalAcertos_Desempenho = TotalAcertos_Desempenho + @Acertos,
              Percentual_Desempenho = CAST(ROUND(100.0 * (TotalAcertos_Desempenho + @Acertos)
                                       / (TotalRespostas_Desempenho + @Respostas), 2) AS DECIMAL(5, 2)),
              UltimaAtualizacao_Desempenho = ${AGORA}
        WHERE Id_Usuario = @Usuario AND Id_Disciplina = @Disciplina;
       IF @@ROWCOUNT = 0
         INSERT INTO dbo.Desempenho (TotalRespostas_Desempenho, TotalAcertos_Desempenho, Percentual_Desempenho,
                                     UltimaAtualizacao_Desempenho, Id_Usuario, Id_Disciplina)
         VALUES (@Respostas, @Acertos, CAST(ROUND(100.0 * @Acertos / @Respostas, 2) AS DECIMAL(5, 2)),
                 ${AGORA}, @Usuario, @Disciplina);`,
      {
        Usuario: [sql.Int, idUsuario],
        Disciplina: [sql.Int, idDisciplina],
        Respostas: [sql.Int, respostas],
        Acertos: [sql.Int, acertos]
      },
      transacao
    );
  },

  /**
   * A pontuação só é incrementada, nunca reduzida (RN21).
   * Quem soma é o trigger trg_ResultadoSimulado_AcumularPontuacao (script 06), ao gravar o resultado.
   * A API só soma aqui se o trigger não existir ou estiver desabilitado, para nunca contar em dobro
   * e continuar correta com o banco só nos scripts 01 a 04.
   */
  async somarPontuacao(idUsuario, pontos, transacao) {
    await executar(
      `IF NOT EXISTS (SELECT 1 FROM sys.triggers
                       WHERE name = 'trg_ResultadoSimulado_AcumularPontuacao'
                         AND parent_id = OBJECT_ID('dbo.ResultadoSimulado')
                         AND is_disabled = 0)
         UPDATE dbo.Usuario SET PontuacaoTotal_Usuario = PontuacaoTotal_Usuario + @Pontos WHERE Id_Usuario = @Id`,
      { Id: [sql.Int, idUsuario], Pontos: [sql.Int, pontos] },
      transacao
    );
  },

  consultarResultado(idSimulado) {
    return consultarUm(
      `SELECT r.Id_Simulado AS idSimulado, s.Titulo_Simulado AS titulo,
              r.TotalQuestoes_ResultadoSimulado AS totalQuestoes, r.TotalAcertos_ResultadoSimulado AS totalAcertos,
              r.Percentual_ResultadoSimulado AS percentual, r.PontosObtidos_ResultadoSimulado AS pontosObtidos,
              r.DataRealizacao_ResultadoSimulado AS dataRealizacao, r.Id_Usuario AS idUsuario,
              (SELECT COUNT(*) FROM dbo.RespostaAluno ra
                WHERE ra.Id_Simulado = r.Id_Simulado AND ra.AlternativaMarcada_RespostaAluno IS NULL) AS emBranco,
              u.PontuacaoTotal_Usuario AS pontuacaoTotal
       FROM dbo.ResultadoSimulado r
       JOIN dbo.Simulado s ON s.Id_Simulado = r.Id_Simulado
       JOIN dbo.Usuario u ON u.Id_Usuario = r.Id_Usuario
       WHERE r.Id_Simulado = @Id`,
      { Id: [sql.Int, idSimulado] }
    );
  },

  /** Desempenho por disciplina dentro de um simulado (RF05). */
  resultadoPorDisciplina(idSimulado) {
    return consultar(
      `SELECT d.Id_Disciplina AS idDisciplina, d.Nome_Disciplina AS nomeDisciplina,
              COUNT(*) AS totalQuestoes,
              SUM(CASE WHEN ra.Acertou_RespostaAluno = 1 THEN 1 ELSE 0 END) AS totalAcertos
       FROM dbo.RespostaAluno ra
       JOIN dbo.Questao q ON q.Id_Questao = ra.Id_Questao
       JOIN dbo.Disciplina d ON d.Id_Disciplina = q.Id_Disciplina
       WHERE ra.Id_Simulado = @Id
       GROUP BY d.Id_Disciplina, d.Nome_Disciplina
       ORDER BY d.Nome_Disciplina`,
      { Id: [sql.Int, idSimulado] }
    );
  },

  /**
   * Revisão do próprio aluno (RF05): resposta marcada, correta e explicação.
   * Questões e disciplinas desativadas aparecem normalmente.
   */
  async revisao(idSimulado) {
    const questoes = await consultar(
      `SELECT sq.Ordem AS ordem, q.Id_Questao AS id, q.Enunciado_Questao AS enunciado,
              q.ImagemUrl_Questao AS imagemUrl, q.ExplicacaoResposta_Questao AS explicacao,
              d.Nome_Disciplina AS nomeDisciplina,
              ra.AlternativaMarcada_RespostaAluno AS marcada, ra.Acertou_RespostaAluno AS acertou
       FROM dbo.SimuladoQuestao sq
       JOIN dbo.Questao q ON q.Id_Questao = sq.Id_Questao
       JOIN dbo.Disciplina d ON d.Id_Disciplina = q.Id_Disciplina
       LEFT JOIN dbo.RespostaAluno ra ON ra.Id_Simulado = sq.Id_Simulado AND ra.Id_Questao = sq.Id_Questao
       WHERE sq.Id_Simulado = @Id
       ORDER BY sq.Ordem`,
      { Id: [sql.Int, idSimulado] }
    );
    const alternativas = await consultar(
      `SELECT a.Id_Questao AS idQuestao, a.Letra_Alternativa AS letra, a.Texto_Alternativa AS texto,
              a.Correta_Alternativa AS correta
       FROM dbo.SimuladoQuestao sq
       JOIN dbo.Alternativa a ON a.Id_Questao = sq.Id_Questao
       WHERE sq.Id_Simulado = @Id
       ORDER BY a.Id_Questao, a.Letra_Alternativa`,
      { Id: [sql.Int, idSimulado] }
    );
    return questoes.map((q) => ({
      ...q,
      alternativas: alternativas.filter((a) => a.idQuestao === q.id).map(({ letra, texto, correta }) => ({ letra, texto, correta }))
    }));
  },

  /** Histórico paginado: somente simulados finalizados (RN20). */
  async historico(idUsuario, pagina, tamanho) {
    const parametros = { Usuario: [sql.Int, idUsuario] };
    const total = await consultarUm(
      `SELECT COUNT(*) AS total FROM dbo.ResultadoSimulado WHERE Id_Usuario = @Usuario`,
      parametros
    );
    const itens = await consultar(
      `SELECT s.Id_Simulado AS id, s.Titulo_Simulado AS titulo, r.TotalQuestoes_ResultadoSimulado AS totalQuestoes,
              r.TotalAcertos_ResultadoSimulado AS totalAcertos, r.Percentual_ResultadoSimulado AS percentual,
              r.PontosObtidos_ResultadoSimulado AS pontosObtidos, r.DataRealizacao_ResultadoSimulado AS dataRealizacao
       FROM dbo.ResultadoSimulado r
       JOIN dbo.Simulado s ON s.Id_Simulado = r.Id_Simulado
       WHERE r.Id_Usuario = @Usuario
       ORDER BY r.DataRealizacao_ResultadoSimulado DESC, s.Id_Simulado DESC
       OFFSET @Pular ROWS FETCH NEXT @Tamanho ROWS ONLY`,
      { ...parametros, Pular: [sql.Int, (pagina - 1) * tamanho], Tamanho: [sql.Int, tamanho] }
    );
    return { total: total.total, itens };
  }
};

module.exports = simuladoRepository;
