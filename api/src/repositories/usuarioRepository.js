// Acesso à tabela Usuario. O CPF só sai daqui na consulta individual e no login.
const { sql, consultar, consultarUm, executar } = require('../config/banco');
const { padraoBusca, AGORA } = require('./auxiliares');

const SELECAO = `
  SELECT u.Id_Usuario             AS id,
         u.Nome_Usuario           AS nome,
         u.CPF_Usuario            AS cpf,
         u.Email_Usuario          AS email,
         u.FotoUrl_Usuario        AS fotoUrl,
         u.DataIngresso_Usuario   AS dataIngresso,
         u.Ativo_Usuario          AS ativo,
         u.PontuacaoTotal_Usuario AS pontuacaoTotal,
         u.Id_Perfil              AS idPerfil,
         p.Nome_Perfil            AS nomePerfil,
         u.Id_Turma               AS idTurma,
         t.Nome_Turma             AS nomeTurma,
         t.AnoLetivo_Turma        AS anoLetivoTurma
  FROM dbo.Usuario u
  JOIN dbo.Perfil p ON p.Id_Perfil = u.Id_Perfil
  LEFT JOIN dbo.Turma t ON t.Id_Turma = u.Id_Turma`;

const usuarioRepository = {
  /** Dados para o login, inclusive o hash da senha. */
  consultarParaLogin(email) {
    return consultarUm(
      `SELECT Id_Usuario AS id, Nome_Usuario AS nome, CPF_Usuario AS cpf, Email_Usuario AS email,
              Senha_Usuario AS senhaHash, Ativo_Usuario AS ativo, Id_Perfil AS idPerfil
       FROM dbo.Usuario WHERE Email_Usuario = @Email`,
      { Email: [sql.VarChar(150), email] }
    );
  },

  /** Situação atual da conta, conferida a cada requisição (conta desativada perde o acesso). */
  consultarSituacao(id) {
    return consultarUm(
      'SELECT Ativo_Usuario AS ativo, Id_Perfil AS idPerfil FROM dbo.Usuario WHERE Id_Usuario = @Id',
      { Id: [sql.Int, id] }
    );
  },

  consultar(id) {
    return consultarUm(`${SELECAO} WHERE u.Id_Usuario = @Id`, { Id: [sql.Int, id] });
  },

  /** Listagem SEM o CPF (RNF02). */
  listar({ idPerfil = null, idTurma = null, ativo = null, busca = null } = {}) {
    return consultar(
      `SELECT u.Id_Usuario AS id, u.Nome_Usuario AS nome, u.Email_Usuario AS email,
              u.Id_Perfil AS idPerfil, p.Nome_Perfil AS nomePerfil,
              u.Id_Turma AS idTurma, t.Nome_Turma AS nomeTurma, t.AnoLetivo_Turma AS anoLetivoTurma,
              u.DataIngresso_Usuario AS dataIngresso, u.Ativo_Usuario AS ativo
       FROM dbo.Usuario u
       JOIN dbo.Perfil p ON p.Id_Perfil = u.Id_Perfil
       LEFT JOIN dbo.Turma t ON t.Id_Turma = u.Id_Turma
       WHERE (@Perfil IS NULL OR u.Id_Perfil = @Perfil)
         AND (@IdTurma IS NULL OR u.Id_Turma = @IdTurma)
         AND (@Ativo IS NULL OR u.Ativo_Usuario = @Ativo)
         AND (@Busca IS NULL OR u.Nome_Usuario LIKE @Busca ESCAPE '\\' OR u.Email_Usuario LIKE @Busca ESCAPE '\\')
       ORDER BY u.Nome_Usuario`,
      {
        Perfil: [sql.Int, idPerfil],
        IdTurma: [sql.Int, idTurma],
        Ativo: [sql.Bit, ativo],
        Busca: [sql.NVarChar(160), padraoBusca(busca)]
      }
    );
  },

  async existeEmail(email, ignorarId = null) {
    const linha = await consultarUm(
      `SELECT 1 AS existe FROM dbo.Usuario
       WHERE Email_Usuario = @Email AND (@IgnorarId IS NULL OR Id_Usuario <> @IgnorarId)`,
      { Email: [sql.VarChar(150), email], IgnorarId: [sql.Int, ignorarId] }
    );
    return Boolean(linha);
  },

  async existeCpf(cpf, ignorarId = null) {
    const linha = await consultarUm(
      `SELECT 1 AS existe FROM dbo.Usuario
       WHERE CPF_Usuario = @Cpf AND (@IgnorarId IS NULL OR Id_Usuario <> @IgnorarId)`,
      { Cpf: [sql.Char(11), cpf], IgnorarId: [sql.Int, ignorarId] }
    );
    return Boolean(linha);
  },

  async incluir(usuario) {
    const linha = await consultarUm(
      `INSERT INTO dbo.Usuario (Nome_Usuario, CPF_Usuario, Email_Usuario, Senha_Usuario, DataIngresso_Usuario,
                                Ativo_Usuario, PontuacaoTotal_Usuario, Id_Perfil, Id_Turma)
       OUTPUT INSERTED.Id_Usuario AS id
       VALUES (@Nome, @Cpf, @Email, @Senha, @DataIngresso, 1, 0, @Perfil, @IdTurma)`,
      {
        Nome: [sql.NVarChar(150), usuario.nome],
        Cpf: [sql.Char(11), usuario.cpf],
        Email: [sql.VarChar(150), usuario.email],
        Senha: [sql.VarChar(200), usuario.senhaHash],
        DataIngresso: [sql.Date, usuario.dataIngresso],
        Perfil: [sql.Int, usuario.idPerfil],
        IdTurma: [sql.Int, usuario.idTurma]
      }
    );
    return linha.id;
  },

  /** Atualiza os dados cadastrais. A senha não muda aqui (RN04). */
  async editar(id, usuario) {
    await executar(
      `UPDATE dbo.Usuario
          SET Nome_Usuario = @Nome, CPF_Usuario = @Cpf, Email_Usuario = @Email,
              DataIngresso_Usuario = @DataIngresso, Id_Perfil = @Perfil, Id_Turma = @IdTurma
        WHERE Id_Usuario = @Id`,
      {
        Id: [sql.Int, id],
        Nome: [sql.NVarChar(150), usuario.nome],
        Cpf: [sql.Char(11), usuario.cpf],
        Email: [sql.VarChar(150), usuario.email],
        DataIngresso: [sql.Date, usuario.dataIngresso],
        Perfil: [sql.Int, usuario.idPerfil],
        IdTurma: [sql.Int, usuario.idTurma]
      }
    );
  },

  /** Exclusão lógica (RN12): nunca DELETE. */
  async alterarSituacao(id, ativo) {
    await executar('UPDATE dbo.Usuario SET Ativo_Usuario = @Ativo WHERE Id_Usuario = @Id', {
      Id: [sql.Int, id],
      Ativo: [sql.Bit, ativo]
    });
  },

  async contarDisciplinasSobResponsabilidade(id) {
    const linha = await consultarUm('SELECT COUNT(*) AS total FROM dbo.Disciplina WHERE Id_Usuario = @Id', {
      Id: [sql.Int, id]
    });
    return linha.total;
  },

  async obterHashSenha(id) {
    const linha = await consultarUm('SELECT Senha_Usuario AS senhaHash, CPF_Usuario AS cpf FROM dbo.Usuario WHERE Id_Usuario = @Id', {
      Id: [sql.Int, id]
    });
    return linha;
  },

  /** Grava a nova senha e invalida qualquer código de recuperação pendente. */
  async alterarSenha(id, senhaHash) {
    await executar(
      `UPDATE dbo.Usuario
          SET Senha_Usuario = @Senha, TokenRecuperacao_Usuario = NULL, ValidadeToken_Usuario = NULL
        WHERE Id_Usuario = @Id`,
      { Id: [sql.Int, id], Senha: [sql.VarChar(200), senhaHash] }
    );
  },

  /** Uma solicitação nova sobrescreve a anterior (RN03). */
  async gravarCodigoRecuperacao(id, codigoHash, minutosValidade) {
    await executar(
      `UPDATE dbo.Usuario
          SET TokenRecuperacao_Usuario = @Codigo, ValidadeToken_Usuario = DATEADD(MINUTE, @Minutos, ${AGORA})
        WHERE Id_Usuario = @Id`,
      { Id: [sql.Int, id], Codigo: [sql.VarChar(200), codigoHash], Minutos: [sql.Int, minutosValidade] }
    );
  },

  /** Código pendente do e-mail; `expirado` é calculado no banco, no mesmo relógio da gravação. */
  consultarRecuperacao(email) {
    return consultarUm(
      `SELECT Id_Usuario AS id, Ativo_Usuario AS ativo, TokenRecuperacao_Usuario AS codigoHash,
              CAST(CASE WHEN ValidadeToken_Usuario IS NULL OR ValidadeToken_Usuario < ${AGORA} THEN 1 ELSE 0 END AS BIT) AS expirado
       FROM dbo.Usuario WHERE Email_Usuario = @Email`,
      { Email: [sql.VarChar(150), email] }
    );
  },

  async limparCodigoRecuperacao(id) {
    await executar(
      'UPDATE dbo.Usuario SET TokenRecuperacao_Usuario = NULL, ValidadeToken_Usuario = NULL WHERE Id_Usuario = @Id',
      { Id: [sql.Int, id] }
    );
  }
};

module.exports = usuarioRepository;
