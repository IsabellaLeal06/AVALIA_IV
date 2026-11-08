// Faixas de medalha (RN22). A medalha do aluno não é gravada: é apurada pela pontuação na consulta.
const { sql, consultar, consultarUm, executar } = require('../config/banco');

const ALUNO = 1;

const SELECAO = `
  SELECT Id_Medalha AS id, Nome_Medalha AS nome, PontosMinimos_Medalha AS pontosMinimos, ImagemUrl_Medalha AS imagemUrl
  FROM dbo.Medalha`;

const medalhaRepository = {
  /** Faixas em ordem de pontuação, com o limite da faixa seguinte e os alunos ativos em cada uma. */
  listar() {
    return consultar(
      `WITH Faixas AS (
         SELECT Id_Medalha, Nome_Medalha, PontosMinimos_Medalha, ImagemUrl_Medalha,
                LEAD(PontosMinimos_Medalha) OVER (ORDER BY PontosMinimos_Medalha) AS PontosProximaFaixa
         FROM dbo.Medalha
       )
       SELECT f.Id_Medalha AS id, f.Nome_Medalha AS nome, f.PontosMinimos_Medalha AS pontosMinimos,
              f.ImagemUrl_Medalha AS imagemUrl, f.PontosProximaFaixa AS pontosProximaFaixa,
              (SELECT COUNT(*) FROM dbo.Usuario u
                WHERE u.Id_Perfil = @Aluno AND u.Ativo_Usuario = 1
                  AND u.PontuacaoTotal_Usuario >= f.PontosMinimos_Medalha
                  AND (f.PontosProximaFaixa IS NULL OR u.PontuacaoTotal_Usuario < f.PontosProximaFaixa)) AS qtdAlunos
       FROM Faixas f
       ORDER BY f.PontosMinimos_Medalha`,
      { Aluno: [sql.Int, ALUNO] }
    );
  },

  consultar(id) {
    return consultarUm(`${SELECAO} WHERE Id_Medalha = @Id`, { Id: [sql.Int, id] });
  },

  /** Medalha corrente: maior PontosMinimos que seja <= pontuação. Nenhuma = ainda sem medalha. */
  atual(pontuacao) {
    return consultarUm(
      `SELECT TOP (1) Id_Medalha AS id, Nome_Medalha AS nome, PontosMinimos_Medalha AS pontosMinimos,
              ImagemUrl_Medalha AS imagemUrl
       FROM dbo.Medalha WHERE PontosMinimos_Medalha <= @Pontos
       ORDER BY PontosMinimos_Medalha DESC`,
      { Pontos: [sql.Int, pontuacao] }
    );
  },

  /** Próxima faixa: menor PontosMinimos maior que a pontuação. Nenhuma = nível máximo. */
  proxima(pontuacao) {
    return consultarUm(
      `SELECT TOP (1) Id_Medalha AS id, Nome_Medalha AS nome, PontosMinimos_Medalha AS pontosMinimos,
              ImagemUrl_Medalha AS imagemUrl
       FROM dbo.Medalha WHERE PontosMinimos_Medalha > @Pontos
       ORDER BY PontosMinimos_Medalha`,
      { Pontos: [sql.Int, pontuacao] }
    );
  },

  async qtdAlunosSemMedalha() {
    const linha = await consultarUm(
      `SELECT COUNT(*) AS total FROM dbo.Usuario u
       WHERE u.Id_Perfil = @Aluno AND u.Ativo_Usuario = 1
         AND u.PontuacaoTotal_Usuario < COALESCE((SELECT MIN(PontosMinimos_Medalha) FROM dbo.Medalha), 2147483647)`,
      { Aluno: [sql.Int, ALUNO] }
    );
    return linha.total;
  },

  async existeNome(nome, ignorarId = null) {
    return Boolean(
      await consultarUm(
        `SELECT 1 AS existe FROM dbo.Medalha
         WHERE Nome_Medalha = @Nome AND (@IgnorarId IS NULL OR Id_Medalha <> @IgnorarId)`,
        { Nome: [sql.NVarChar(50), nome], IgnorarId: [sql.Int, ignorarId] }
      )
    );
  },

  async existePontosMinimos(pontos, ignorarId = null) {
    return Boolean(
      await consultarUm(
        `SELECT 1 AS existe FROM dbo.Medalha
         WHERE PontosMinimos_Medalha = @Pontos AND (@IgnorarId IS NULL OR Id_Medalha <> @IgnorarId)`,
        { Pontos: [sql.Int, pontos], IgnorarId: [sql.Int, ignorarId] }
      )
    );
  },

  async incluir(medalha) {
    const linha = await consultarUm(
      `INSERT INTO dbo.Medalha (Nome_Medalha, PontosMinimos_Medalha, ImagemUrl_Medalha)
       OUTPUT INSERTED.Id_Medalha AS id
       VALUES (@Nome, @Pontos, @Imagem)`,
      {
        Nome: [sql.NVarChar(50), medalha.nome],
        Pontos: [sql.Int, medalha.pontosMinimos],
        Imagem: [sql.VarChar(500), medalha.imagemUrl]
      }
    );
    return linha.id;
  },

  async editar(id, medalha) {
    await executar(
      `UPDATE dbo.Medalha SET Nome_Medalha = @Nome, PontosMinimos_Medalha = @Pontos, ImagemUrl_Medalha = @Imagem
        WHERE Id_Medalha = @Id`,
      {
        Id: [sql.Int, id],
        Nome: [sql.NVarChar(50), medalha.nome],
        Pontos: [sql.Int, medalha.pontosMinimos],
        Imagem: [sql.VarChar(500), medalha.imagemUrl]
      }
    );
  },

  /** A faixa não tem vínculo com nenhum aluno, por isso a exclusão é física (como na Desktop). */
  async excluir(id) {
    await executar('DELETE FROM dbo.Medalha WHERE Id_Medalha = @Id', { Id: [sql.Int, id] });
  }
};

module.exports = medalhaRepository;
