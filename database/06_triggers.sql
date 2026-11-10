/* =====================================================================
   Avalia+ | PIM IV | Banco de dados (Azure SQL Database / SQL Server)
   Script 06 - Triggers (INF06)

   1. trg_ResultadoSimulado_AcumularPontuacao   soma os pontos no Usuario (RN21)
   2. trg_Usuario_PontuacaoNaoDiminui           barra qualquer redução de pontuação (RN21)
   3. trg_RespostaAluno_Imutavel                respostas não mudam nem somem (RN19)
   4. trg_ResultadoSimulado_Imutavel            resultados não mudam nem somem (RN19)
   5. trg_Alternativa_CorretaUtilizada          questão já usada em simulado mantém a correta (RN11)

   Com o trigger 1 ativo, a API NÃO soma a pontuação por conta própria: ela confere
   se o trigger está ativo (sys.triggers) e só soma quando ele não existe ou está
   desabilitado. Por isso a API pode ser atualizada antes ou depois deste script.

   Quando uma regra é violada, o trigger desfaz a transação e devolve o erro
   (50021 a 50024), que a API mostra como falha de operação.
   Podem ser executados de novo: usam CREATE OR ALTER.

   Ordem de execução: 01 -> 02 -> 03 -> 04 -> 05 -> 06
   ===================================================================== */

SET NOCOUNT ON;
GO

/* =====================================================================
   1. Pontuação acumulada
   Cada resultado novo atualiza o total do aluno = soma de todos os seus resultados.
   ===================================================================== */
CREATE OR ALTER TRIGGER dbo.trg_ResultadoSimulado_AcumularPontuacao
ON dbo.ResultadoSimulado
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Soma só o que acabou de entrar (tabela inserted). O UPDATE é atômico: duas finalizações
    -- simultâneas do mesmo aluno não se perdem, e nenhuma linha de outra transação é lida.
    UPDATE u
       SET PontuacaoTotal_Usuario = u.PontuacaoTotal_Usuario + t.Pontos
      FROM dbo.Usuario u
      JOIN (SELECT i.Id_Usuario, SUM(i.PontosObtidos_ResultadoSimulado) AS Pontos
              FROM inserted i
             GROUP BY i.Id_Usuario) t ON t.Id_Usuario = u.Id_Usuario;
END
GO

/* =====================================================================
   2. A pontuação nunca diminui (RN21)
   ===================================================================== */
CREATE OR ALTER TRIGGER dbo.trg_Usuario_PontuacaoNaoDiminui
ON dbo.Usuario
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF UPDATE(PontuacaoTotal_Usuario)
       AND EXISTS (SELECT 1
                     FROM inserted i
                     JOIN deleted d ON d.Id_Usuario = i.Id_Usuario
                    WHERE i.PontuacaoTotal_Usuario < d.PontuacaoTotal_Usuario)
    BEGIN
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW 50021, N'A pontuação do aluno só aumenta e nunca é reduzida (RN21).', 1;
    END
END
GO

/* =====================================================================
   3 e 4. Respostas e resultados são imutáveis (RN19)
   O trigger dispara mesmo quando nenhuma linha é afetada; por isso a
   primeira linha confere se houve linha alterada ou excluída.
   ===================================================================== */
CREATE OR ALTER TRIGGER dbo.trg_RespostaAluno_Imutavel
ON dbo.RespostaAluno
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM deleted) RETURN;

    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW 50022, N'Respostas de simulado não podem ser alteradas nem excluídas (RN19).', 1;
END
GO

CREATE OR ALTER TRIGGER dbo.trg_ResultadoSimulado_Imutavel
ON dbo.ResultadoSimulado
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM deleted) RETURN;

    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW 50023, N'Resultados de simulado não podem ser alterados nem excluídos (RN19).', 1;
END
GO

/* =====================================================================
   5. Alternativa correta de questão já utilizada (RN11)
   "Utilizada" = a questão aparece em algum simulado (SimuladoQuestao).
   Só barra quando o valor de Correta_Alternativa realmente muda; editar o texto continua permitido.
   ===================================================================== */
CREATE OR ALTER TRIGGER dbo.trg_Alternativa_CorretaUtilizada
ON dbo.Alternativa
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF UPDATE(Correta_Alternativa)
       AND EXISTS (SELECT 1
                     FROM inserted i
                     JOIN deleted d ON d.Id_Alternativa = i.Id_Alternativa
                    WHERE i.Correta_Alternativa <> d.Correta_Alternativa
                      AND EXISTS (SELECT 1 FROM dbo.SimuladoQuestao sq WHERE sq.Id_Questao = i.Id_Questao))
    BEGIN
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW 50024, N'Esta questão já foi utilizada em simulado; a alternativa correta não pode ser alterada (RN11).', 1;
    END
END
GO

PRINT N'06_triggers.sql concluído: 5 triggers.';
GO
