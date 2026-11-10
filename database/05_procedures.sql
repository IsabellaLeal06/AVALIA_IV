/* =====================================================================
   Avalia+ | PIM IV | Banco de dados (Azure SQL Database / SQL Server)
   Script 05 - Procedures (INF05)

   Consultas e conferências reutilizáveis, todas SOMENTE LEITURA:
   1. sp_PontuacaoAluno      pontuação, medalha atual e quanto falta para a próxima (RN22, RN23)
   2. sp_DesempenhoAluno     acertos e percentual por disciplina (RF05, RF06)
   3. sp_HistoricoSimulados  simulados finalizados do aluno, paginados (RN20)
   4. sp_ConferirPontuacao   acha alunos cuja pontuação difere da soma dos resultados

   As aplicações (API, Web, Mobile e Desktop) continuam usando a API; as
   procedures servem para consulta direta no banco e para conferência.
   Podem ser executadas de novo: usam CREATE OR ALTER.

   Ordem de execução: 01 -> 02 -> 03 -> 04 -> 05 -> 06
   ===================================================================== */

SET NOCOUNT ON;
GO

/* =====================================================================
   1. sp_PontuacaoAluno
   Medalha atual = faixa de maior PontosMinimos que não passa da pontuação.
   Sem medalha ainda: MedalhaAtual vem NULL. Na última faixa: PontosFaltantes = 0.
   Exemplo: EXEC dbo.sp_PontuacaoAluno @IdUsuario = 5;
   ===================================================================== */
CREATE OR ALTER PROCEDURE dbo.sp_PontuacaoAluno
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Id_Perfil 1 = Aluno (script 03)
    IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Id_Usuario = @IdUsuario AND Id_Perfil = 1)
        THROW 50010, N'Aluno não encontrado.', 1;

    SELECT u.Id_Usuario                  AS IdUsuario,
           u.Nome_Usuario                AS Nome,
           u.PontuacaoTotal_Usuario      AS PontuacaoTotal,
           atual.Nome_Medalha            AS MedalhaAtual,
           proxima.Nome_Medalha          AS ProximaMedalha,
           proxima.PontosMinimos_Medalha AS PontosProximaMedalha,
           CASE WHEN proxima.Id_Medalha IS NULL
                THEN 0
                ELSE proxima.PontosMinimos_Medalha - u.PontuacaoTotal_Usuario
           END                           AS PontosFaltantes
    FROM dbo.Usuario u
    OUTER APPLY (SELECT TOP (1) m.Nome_Medalha
                 FROM dbo.Medalha m
                 WHERE m.PontosMinimos_Medalha <= u.PontuacaoTotal_Usuario
                 ORDER BY m.PontosMinimos_Medalha DESC) atual
    OUTER APPLY (SELECT TOP (1) m.Id_Medalha, m.Nome_Medalha, m.PontosMinimos_Medalha
                 FROM dbo.Medalha m
                 WHERE m.PontosMinimos_Medalha > u.PontuacaoTotal_Usuario
                 ORDER BY m.PontosMinimos_Medalha ASC) proxima
    WHERE u.Id_Usuario = @IdUsuario;
END
GO

/* =====================================================================
   2. sp_DesempenhoAluno
   Lê o acumulado da tabela Desempenho (atualizado a cada simulado finalizado).
   Exemplo: EXEC dbo.sp_DesempenhoAluno @IdUsuario = 5;
   ===================================================================== */
CREATE OR ALTER PROCEDURE dbo.sp_DesempenhoAluno
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Id_Usuario = @IdUsuario AND Id_Perfil = 1)
        THROW 50010, N'Aluno não encontrado.', 1;

    SELECT d.Nome_Disciplina              AS Disciplina,
           de.TotalRespostas_Desempenho   AS TotalRespostas,
           de.TotalAcertos_Desempenho     AS TotalAcertos,
           de.Percentual_Desempenho       AS Percentual,
           de.UltimaAtualizacao_Desempenho AS UltimaAtualizacao
    FROM dbo.Desempenho de
    JOIN dbo.Disciplina d ON d.Id_Disciplina = de.Id_Disciplina
    WHERE de.Id_Usuario = @IdUsuario
    ORDER BY d.Nome_Disciplina;
END
GO

/* =====================================================================
   3. sp_HistoricoSimulados
   Mais recentes primeiro. @Tamanho de 1 a 50.
   Exemplo: EXEC dbo.sp_HistoricoSimulados @IdUsuario = 5, @Pagina = 1, @Tamanho = 10;
   ===================================================================== */
CREATE OR ALTER PROCEDURE dbo.sp_HistoricoSimulados
    @IdUsuario INT,
    @Pagina    INT = 1,
    @Tamanho   INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Id_Usuario = @IdUsuario AND Id_Perfil = 1)
        THROW 50010, N'Aluno não encontrado.', 1;
    IF @Pagina < 1 OR @Tamanho < 1 OR @Tamanho > 50
        THROW 50011, N'Página deve ser 1 ou mais, e o tamanho de 1 a 50.', 1;

    SELECT s.Id_Simulado                      AS IdSimulado,
           s.Titulo_Simulado                  AS Titulo,
           r.TotalQuestoes_ResultadoSimulado  AS TotalQuestoes,
           r.TotalAcertos_ResultadoSimulado   AS TotalAcertos,
           r.Percentual_ResultadoSimulado     AS Percentual,
           r.PontosObtidos_ResultadoSimulado  AS PontosObtidos,
           r.DataRealizacao_ResultadoSimulado AS DataRealizacao
    FROM dbo.ResultadoSimulado r
    JOIN dbo.Simulado s ON s.Id_Simulado = r.Id_Simulado
    WHERE r.Id_Usuario = @IdUsuario
    ORDER BY r.DataRealizacao_ResultadoSimulado DESC, s.Id_Simulado DESC
    OFFSET (@Pagina - 1) * @Tamanho ROWS FETCH NEXT @Tamanho ROWS ONLY;
END
GO

/* =====================================================================
   4. sp_ConferirPontuacao
   A pontuação do aluno deve ser a soma dos PontosObtidos de todos os seus
   resultados (RN21). Devolve só quem está diferente: sem linhas = tudo certo.
   Não corrige nada. Exemplo: EXEC dbo.sp_ConferirPontuacao;
   ===================================================================== */
CREATE OR ALTER PROCEDURE dbo.sp_ConferirPontuacao
    @IdUsuario INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT u.Id_Usuario             AS IdUsuario,
           u.Nome_Usuario           AS Nome,
           u.PontuacaoTotal_Usuario AS PontuacaoRegistrada,
           c.Soma                   AS PontuacaoCalculada,
           u.PontuacaoTotal_Usuario - c.Soma AS Diferenca
    FROM dbo.Usuario u
    CROSS APPLY (SELECT COALESCE(SUM(r.PontosObtidos_ResultadoSimulado), 0) AS Soma
                 FROM dbo.ResultadoSimulado r
                 WHERE r.Id_Usuario = u.Id_Usuario) c
    WHERE u.Id_Perfil = 1
      AND (@IdUsuario IS NULL OR u.Id_Usuario = @IdUsuario)
      AND u.PontuacaoTotal_Usuario <> c.Soma
    ORDER BY u.Id_Usuario;
END
GO

PRINT N'05_procedures.sql concluído: 4 procedures.';
GO
