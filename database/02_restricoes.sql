/* =====================================================================
   Avalia+ | PIM IV | Banco de dados (Azure SQL Database / SQL Server)
   Script 02 - Restrições de integridade (INF03 | RN27, RNF03)

   1. UNIQUE        (RN06, RN16, RN27)
   2. CHECK         (RN09, RN14, RN18 e formato do CPF)
   3. FOREIGN KEY   (sem ON DELETE CASCADE: a exclusão é lógica, RN12)

   A regra "5 alternativas com apenas 1 correta" (RN09) não é expressável
   por CHECK e é validada na aplicação (Desktop e, depois, na API).
   Triggers e procedures pertencem aos itens INF05/INF06.

   Ordem de execução: 01 -> 02 -> 03 -> 04
   ===================================================================== */

SET NOCOUNT ON;
GO

/* =====================================================================
   1. UNIQUE
   ===================================================================== */
ALTER TABLE dbo.Usuario     ADD CONSTRAINT UQ_Usuario_Email            UNIQUE (Email_Usuario);
ALTER TABLE dbo.Usuario     ADD CONSTRAINT UQ_Usuario_CPF              UNIQUE (CPF_Usuario);
ALTER TABLE dbo.Disciplina  ADD CONSTRAINT UQ_Disciplina_Nome          UNIQUE (Nome_Disciplina);
ALTER TABLE dbo.Turma       ADD CONSTRAINT UQ_Turma_NomeAno            UNIQUE (Nome_Turma, AnoLetivo_Turma);
ALTER TABLE dbo.Medalha     ADD CONSTRAINT UQ_Medalha_Nome             UNIQUE (Nome_Medalha);
ALTER TABLE dbo.Medalha     ADD CONSTRAINT UQ_Medalha_PontosMinimos    UNIQUE (PontosMinimos_Medalha);
ALTER TABLE dbo.Alternativa ADD CONSTRAINT UQ_Alternativa_QuestaoLetra UNIQUE (Id_Questao, Letra_Alternativa);
-- RN16: tentativa única por simulado
ALTER TABLE dbo.ResultadoSimulado ADD CONSTRAINT UQ_ResultadoSimulado_Simulado UNIQUE (Id_Simulado);
GO

/* =====================================================================
   2. CHECK
   ===================================================================== */
-- RN14: o simulado possui 10, 15 ou 20 questões
ALTER TABLE dbo.Simulado ADD CONSTRAINT CK_Simulado_QuantidadeQuestoes
    CHECK (QuantidadeQuestoes_Simulado IN (10, 15, 20));

-- RN09: alternativas identificadas de A a E
ALTER TABLE dbo.Alternativa ADD CONSTRAINT CK_Alternativa_Letra
    CHECK (Letra_Alternativa IN ('A', 'B', 'C', 'D', 'E'));

-- RN18: resposta com A a E, ou NULL quando a questão fica em branco
ALTER TABLE dbo.RespostaAluno ADD CONSTRAINT CK_RespostaAluno_AlternativaMarcada
    CHECK (AlternativaMarcada_RespostaAluno IS NULL
           OR AlternativaMarcada_RespostaAluno IN ('A', 'B', 'C', 'D', 'E'));

-- CPF com exatamente 11 dígitos numéricos (sem pontos e traço)
ALTER TABLE dbo.Usuario ADD CONSTRAINT CK_Usuario_CPF
    CHECK (CPF_Usuario LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]');
GO

/* =====================================================================
   3. FOREIGN KEY (todas com NO ACTION: nenhuma exclusão em cascata)
   ===================================================================== */
-- Usuario
ALTER TABLE dbo.Usuario ADD CONSTRAINT FK_Usuario_Perfil
    FOREIGN KEY (Id_Perfil) REFERENCES dbo.Perfil (Id_Perfil);
ALTER TABLE dbo.Usuario ADD CONSTRAINT FK_Usuario_Turma
    FOREIGN KEY (Id_Turma) REFERENCES dbo.Turma (Id_Turma);

-- Disciplina (professor responsável)
ALTER TABLE dbo.Disciplina ADD CONSTRAINT FK_Disciplina_Usuario
    FOREIGN KEY (Id_Usuario) REFERENCES dbo.Usuario (Id_Usuario);

-- TurmaDisciplina
ALTER TABLE dbo.TurmaDisciplina ADD CONSTRAINT FK_TurmaDisciplina_Turma
    FOREIGN KEY (Id_Turma) REFERENCES dbo.Turma (Id_Turma);
ALTER TABLE dbo.TurmaDisciplina ADD CONSTRAINT FK_TurmaDisciplina_Disciplina
    FOREIGN KEY (Id_Disciplina) REFERENCES dbo.Disciplina (Id_Disciplina);

-- Questao (disciplina e autoria)
ALTER TABLE dbo.Questao ADD CONSTRAINT FK_Questao_Disciplina
    FOREIGN KEY (Id_Disciplina) REFERENCES dbo.Disciplina (Id_Disciplina);
ALTER TABLE dbo.Questao ADD CONSTRAINT FK_Questao_Usuario
    FOREIGN KEY (Id_Usuario) REFERENCES dbo.Usuario (Id_Usuario);

-- Alternativa
ALTER TABLE dbo.Alternativa ADD CONSTRAINT FK_Alternativa_Questao
    FOREIGN KEY (Id_Questao) REFERENCES dbo.Questao (Id_Questao);

-- Simulado (aluno que solicitou)
ALTER TABLE dbo.Simulado ADD CONSTRAINT FK_Simulado_Usuario
    FOREIGN KEY (Id_Usuario) REFERENCES dbo.Usuario (Id_Usuario);

-- SimuladoQuestao
ALTER TABLE dbo.SimuladoQuestao ADD CONSTRAINT FK_SimuladoQuestao_Simulado
    FOREIGN KEY (Id_Simulado) REFERENCES dbo.Simulado (Id_Simulado);
ALTER TABLE dbo.SimuladoQuestao ADD CONSTRAINT FK_SimuladoQuestao_Questao
    FOREIGN KEY (Id_Questao) REFERENCES dbo.Questao (Id_Questao);

-- RespostaAluno
ALTER TABLE dbo.RespostaAluno ADD CONSTRAINT FK_RespostaAluno_Usuario
    FOREIGN KEY (Id_Usuario) REFERENCES dbo.Usuario (Id_Usuario);
ALTER TABLE dbo.RespostaAluno ADD CONSTRAINT FK_RespostaAluno_Simulado
    FOREIGN KEY (Id_Simulado) REFERENCES dbo.Simulado (Id_Simulado);
ALTER TABLE dbo.RespostaAluno ADD CONSTRAINT FK_RespostaAluno_Questao
    FOREIGN KEY (Id_Questao) REFERENCES dbo.Questao (Id_Questao);

-- ResultadoSimulado
ALTER TABLE dbo.ResultadoSimulado ADD CONSTRAINT FK_ResultadoSimulado_Usuario
    FOREIGN KEY (Id_Usuario) REFERENCES dbo.Usuario (Id_Usuario);
ALTER TABLE dbo.ResultadoSimulado ADD CONSTRAINT FK_ResultadoSimulado_Simulado
    FOREIGN KEY (Id_Simulado) REFERENCES dbo.Simulado (Id_Simulado);

-- Desempenho
ALTER TABLE dbo.Desempenho ADD CONSTRAINT FK_Desempenho_Usuario
    FOREIGN KEY (Id_Usuario) REFERENCES dbo.Usuario (Id_Usuario);
ALTER TABLE dbo.Desempenho ADD CONSTRAINT FK_Desempenho_Disciplina
    FOREIGN KEY (Id_Disciplina) REFERENCES dbo.Disciplina (Id_Disciplina);
GO

PRINT N'02_restricoes.sql concluído.';
GO
