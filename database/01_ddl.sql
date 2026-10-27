/* =====================================================================
   Avalia+ | PIM IV | Banco de dados (Azure SQL Database / SQL Server)
   Script 01 - Criação das 13 tabelas (INF02)

   Base: MER v2 + DER v2.1 (brModelo).
   Padrão de nomes: NomeAtributo_NomeEntidade. No modelo físico os nomes
   compostos de entidade ficam sem sublinhado (RespostaAluno,
   ResultadoSimulado, SimuladoQuestao, TurmaDisciplina).

   Este script cria apenas as tabelas, as chaves primárias e os valores
   padrão. UNIQUE, CHECK e FOREIGN KEY ficam no script 02.

   Ordem de execução: 01 -> 02 -> 03 -> 04
   ===================================================================== */

SET NOCOUNT ON;
GO

/* Proteção: interrompe se as tabelas já existirem.
   Para recriar o banco de teste, execute antes o 00_reset.sql. */
IF OBJECT_ID(N'dbo.Perfil', N'U') IS NOT NULL
BEGIN
    RAISERROR(N'As tabelas do Avalia+ já existem. Execute o 00_reset.sql (somente em ambiente de teste) antes deste script.', 16, 1);
    SET NOEXEC ON;
END
GO

/* ---------------------------------------------------------------------
   Perfil: Aluno, Professor e Administrador (carga no script 03)
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Perfil (
    Id_Perfil    INT IDENTITY(1,1) NOT NULL,
    Nome_Perfil  NVARCHAR(50)      NOT NULL,
    CONSTRAINT PK_Perfil PRIMARY KEY (Id_Perfil)
);
GO

/* ---------------------------------------------------------------------
   Turma: identificada por nome + ano letivo (RN06)
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Turma (
    Id_Turma         INT IDENTITY(1,1) NOT NULL,
    Nome_Turma       NVARCHAR(100)     NOT NULL,
    AnoLetivo_Turma  INT               NOT NULL,
    Ativo_Turma      BIT               NOT NULL CONSTRAINT DF_Turma_Ativo DEFAULT (1),
    CONSTRAINT PK_Turma PRIMARY KEY (Id_Turma)
);
GO

/* ---------------------------------------------------------------------
   Usuario: contas de acesso. Senha_Usuario guarda o hash BCrypt (RNF01).
   Id_Turma só é preenchido para Aluno (RN05).
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Usuario (
    Id_Usuario                INT IDENTITY(1,1) NOT NULL,
    Nome_Usuario              NVARCHAR(150)     NOT NULL,
    CPF_Usuario               CHAR(11)          NOT NULL,
    Email_Usuario             VARCHAR(150)      NOT NULL,
    Senha_Usuario             VARCHAR(200)      NOT NULL,
    FotoUrl_Usuario           VARCHAR(500)      NULL,
    DataIngresso_Usuario      DATE              NOT NULL,
    Ativo_Usuario             BIT               NOT NULL CONSTRAINT DF_Usuario_Ativo DEFAULT (1),
    PontuacaoTotal_Usuario    INT               NOT NULL CONSTRAINT DF_Usuario_PontuacaoTotal DEFAULT (0),
    TokenRecuperacao_Usuario  VARCHAR(200)      NULL,
    ValidadeToken_Usuario     DATETIME          NULL,
    Id_Perfil                 INT               NOT NULL,
    Id_Turma                  INT               NULL,
    CONSTRAINT PK_Usuario PRIMARY KEY (Id_Usuario)
);
GO

/* ---------------------------------------------------------------------
   Disciplina: Id_Usuario = professor responsável, opcional (RN07)
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Disciplina (
    Id_Disciplina         INT IDENTITY(1,1) NOT NULL,
    Nome_Disciplina       NVARCHAR(100)     NOT NULL,
    Descricao_Disciplina  NVARCHAR(500)     NULL,
    Ativo_Disciplina      BIT               NOT NULL CONSTRAINT DF_Disciplina_Ativo DEFAULT (1),
    Id_Usuario            INT               NULL,
    CONSTRAINT PK_Disciplina PRIMARY KEY (Id_Disciplina)
);
GO

/* ---------------------------------------------------------------------
   TurmaDisciplina: associativa N:N entre Turma e Disciplina
   --------------------------------------------------------------------- */
CREATE TABLE dbo.TurmaDisciplina (
    Id_Turma       INT NOT NULL,
    Id_Disciplina  INT NOT NULL,
    CONSTRAINT PK_TurmaDisciplina PRIMARY KEY (Id_Turma, Id_Disciplina)
);
GO

/* ---------------------------------------------------------------------
   Questao: Id_Usuario = quem cadastrou (RN08).
   ExplicacaoResposta_Questao é opcional (RN09, decisão REQ08/MD03).
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Questao (
    Id_Questao                  INT IDENTITY(1,1) NOT NULL,
    Enunciado_Questao           NVARCHAR(MAX)     NOT NULL,
    ImagemUrl_Questao           VARCHAR(500)      NULL,
    ExplicacaoResposta_Questao  NVARCHAR(MAX)     NULL,
    Ativo_Questao               BIT               NOT NULL CONSTRAINT DF_Questao_Ativo DEFAULT (1),
    Id_Disciplina               INT               NOT NULL,
    Id_Usuario                  INT               NOT NULL,
    CONSTRAINT PK_Questao PRIMARY KEY (Id_Questao)
);
GO

/* ---------------------------------------------------------------------
   Alternativa: cinco por questão (A a E), apenas uma correta (RN09).
   A regra "5 alternativas / 1 correta" é validada na aplicação.
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Alternativa (
    Id_Alternativa       INT IDENTITY(1,1) NOT NULL,
    Letra_Alternativa    CHAR(1)           NOT NULL,
    Texto_Alternativa    NVARCHAR(MAX)     NOT NULL,
    Correta_Alternativa  BIT               NOT NULL CONSTRAINT DF_Alternativa_Correta DEFAULT (0),
    Id_Questao           INT               NOT NULL,
    CONSTRAINT PK_Alternativa PRIMARY KEY (Id_Alternativa)
);
GO

/* ---------------------------------------------------------------------
   Simulado: não possui Id_Disciplina. As disciplinas são derivadas das
   questões (Simulado -> SimuladoQuestao -> Questao -> Disciplina, RN13).
   Id_Usuario = aluno que solicitou.
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Simulado (
    Id_Simulado                  INT IDENTITY(1,1) NOT NULL,
    Titulo_Simulado              NVARCHAR(200)     NOT NULL,
    QuantidadeQuestoes_Simulado  INT               NOT NULL,
    DataCriacao_Simulado         DATETIME          NOT NULL,
    Id_Usuario                   INT               NOT NULL,
    CONSTRAINT PK_Simulado PRIMARY KEY (Id_Simulado)
);
GO

/* ---------------------------------------------------------------------
   SimuladoQuestao: composição do simulado e ordem de apresentação
   --------------------------------------------------------------------- */
CREATE TABLE dbo.SimuladoQuestao (
    Id_Simulado  INT NOT NULL,
    Id_Questao   INT NOT NULL,
    Ordem        INT NOT NULL,
    CONSTRAINT PK_SimuladoQuestao PRIMARY KEY (Id_Simulado, Id_Questao)
);
GO

/* ---------------------------------------------------------------------
   RespostaAluno: AlternativaMarcada NULL = questão em branco (conta erro)
   --------------------------------------------------------------------- */
CREATE TABLE dbo.RespostaAluno (
    Id_RespostaAluno                  INT IDENTITY(1,1) NOT NULL,
    AlternativaMarcada_RespostaAluno  CHAR(1)           NULL,
    Acertou_RespostaAluno             BIT               NOT NULL,
    DataResposta_RespostaAluno        DATETIME          NOT NULL,
    Id_Usuario                        INT               NOT NULL,
    Id_Simulado                       INT               NOT NULL,
    Id_Questao                        INT               NOT NULL,
    CONSTRAINT PK_RespostaAluno PRIMARY KEY (Id_RespostaAluno)
);
GO

/* ---------------------------------------------------------------------
   ResultadoSimulado: um por simulado (Id_Simulado UNIQUE, RN16)
   --------------------------------------------------------------------- */
CREATE TABLE dbo.ResultadoSimulado (
    Id_ResultadoSimulado              INT IDENTITY(1,1) NOT NULL,
    TotalQuestoes_ResultadoSimulado   INT               NOT NULL,
    TotalAcertos_ResultadoSimulado    INT               NOT NULL,
    Percentual_ResultadoSimulado      DECIMAL(5,2)      NOT NULL,
    PontosObtidos_ResultadoSimulado   INT               NOT NULL,
    DataRealizacao_ResultadoSimulado  DATETIME          NOT NULL,
    Id_Usuario                        INT               NOT NULL,
    Id_Simulado                       INT               NOT NULL,
    CONSTRAINT PK_ResultadoSimulado PRIMARY KEY (Id_ResultadoSimulado)
);
GO

/* ---------------------------------------------------------------------
   Desempenho: acumulado do aluno por disciplina (desnormalização para consultas rápidas)
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Desempenho (
    Id_Desempenho                 INT IDENTITY(1,1) NOT NULL,
    TotalRespostas_Desempenho     INT               NOT NULL,
    TotalAcertos_Desempenho       INT               NOT NULL,
    Percentual_Desempenho         DECIMAL(5,2)      NOT NULL,
    UltimaAtualizacao_Desempenho  DATETIME          NOT NULL,
    Id_Usuario                    INT               NOT NULL,
    Id_Disciplina                 INT               NOT NULL,
    CONSTRAINT PK_Desempenho PRIMARY KEY (Id_Desempenho)
);
GO

/* ---------------------------------------------------------------------
   Medalha: faixas de pontuação. Não se relaciona com Usuario: a medalha
   é calculada na consulta (maior PontosMinimos <= PontuacaoTotal, RN22).
   --------------------------------------------------------------------- */
CREATE TABLE dbo.Medalha (
    Id_Medalha             INT IDENTITY(1,1) NOT NULL,
    Nome_Medalha           NVARCHAR(50)      NOT NULL,
    PontosMinimos_Medalha  INT               NOT NULL,
    ImagemUrl_Medalha      VARCHAR(500)      NOT NULL,
    CONSTRAINT PK_Medalha PRIMARY KEY (Id_Medalha)
);
GO

PRINT N'01_ddl.sql concluído: 13 tabelas criadas.';
GO

SET NOEXEC OFF;
GO
