/* =====================================================================
   Avalia+ | PIM IV | Banco de dados (Azure SQL Database / SQL Server)
   Script 03 - Carga inicial obrigatória (INF04)

   Perfil: Ids fixos, usados pelas aplicações e pela restrição
           CK_Usuario_TurmaSomenteAluno (1 Aluno, 2 Professor, 3 Administrador).
   Medalha: faixas da gamificação (RN22). As faixas ficam em tabela para
            que a instituição possa ajustá-las sem alterar as aplicações.

   Ordem de execução: 01 -> 02 -> 03 -> 04
   ===================================================================== */

SET NOCOUNT ON;
GO

BEGIN TRANSACTION;

SET IDENTITY_INSERT dbo.Perfil ON;
INSERT INTO dbo.Perfil (Id_Perfil, Nome_Perfil) VALUES
    (1, N'Aluno'),
    (2, N'Professor'),
    (3, N'Administrador');
SET IDENTITY_INSERT dbo.Perfil OFF;

SET IDENTITY_INSERT dbo.Medalha ON;
INSERT INTO dbo.Medalha (Id_Medalha, Nome_Medalha, PontosMinimos_Medalha, ImagemUrl_Medalha) VALUES
    (1, N'Bronze',  250, 'assets/medalhas/bronze.png'),
    (2, N'Prata',  1000, 'assets/medalhas/prata.png'),
    (3, N'Ouro',   2500, 'assets/medalhas/ouro.png');
SET IDENTITY_INSERT dbo.Medalha OFF;

COMMIT TRANSACTION;
GO

PRINT N'03_carga_inicial.sql concluído: 3 perfis e 3 medalhas.';
GO
