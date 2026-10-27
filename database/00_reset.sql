/* =====================================================================
   Avalia+ | PIM IV | Banco de dados
   Script 00 - RESET (SOMENTE PARA AMBIENTE DE TESTE)

   Apaga TODAS as tabelas do Avalia+ e todos os dados.
   Depois dele, execute novamente: 01 -> 02 -> 03 -> 04.

   Proteção: o script só roda se @confirmar for alterado para 1.
   NUNCA execute em um banco com dados reais.
   ===================================================================== */

SET NOCOUNT ON;
GO

DECLARE @confirmar BIT = 0;   -- altere para 1 para confirmar o reset

IF @confirmar = 0
BEGIN
    RAISERROR(N'Reset não executado. Altere @confirmar para 1 se este for um banco de TESTE.', 16, 1);
    SET NOEXEC ON;
END
GO

/* Remove as tabelas da mais dependente para a menos dependente.
   Como as FKs são removidas junto com as tabelas filhas, a ordem evita
   erros de referência. */
DROP TABLE IF EXISTS dbo.RespostaAluno;
DROP TABLE IF EXISTS dbo.ResultadoSimulado;
DROP TABLE IF EXISTS dbo.SimuladoQuestao;
DROP TABLE IF EXISTS dbo.Simulado;
DROP TABLE IF EXISTS dbo.Desempenho;
DROP TABLE IF EXISTS dbo.Alternativa;
DROP TABLE IF EXISTS dbo.Questao;
DROP TABLE IF EXISTS dbo.TurmaDisciplina;
DROP TABLE IF EXISTS dbo.Disciplina;
DROP TABLE IF EXISTS dbo.Usuario;
DROP TABLE IF EXISTS dbo.Turma;
DROP TABLE IF EXISTS dbo.Perfil;
DROP TABLE IF EXISTS dbo.Medalha;
GO

PRINT N'00_reset.sql concluído: tabelas removidas. Execute agora 01 -> 02 -> 03 -> 04.';
GO

SET NOEXEC OFF;
GO
