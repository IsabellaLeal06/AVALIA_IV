/* =====================================================================
   Avalia+ | PIM IV | Banco de dados (Azure SQL Database / SQL Server)
   Script 04 - Dados de teste

   TODOS OS DADOS SÃO FICTÍCIOS. Os CPFs têm dígitos verificadores válidos,
   mas não pertencem a ninguém; os e-mails usam o domínio reservado .test.

   Senha inicial = CPF (RN02). A coluna Senha_Usuario recebe o hash BCrypt
   (custo 10) de cada CPF. O CPF nunca é gravado em texto na coluna de senha.

   Conteúdo:
     - 1 administrador, 3 professores e 15 alunos
     - 3 turmas (1 desativada, com 2 alunos ativos: RN12)
     - 8 disciplinas, montadas a partir dos tomos do Material Instrucional
       Específico (CQA/UNIP) de ADS. A disciplina 8 está sem responsável (RN07)
     - 80 questões do ENADE (10 por disciplina), cada uma com 5 alternativas
       e 1 correta (RN09). O ano de origem aparece no fim do enunciado.
       * 40 com explicação e 40 sem (campo opcional)
       * questões 71 e 72 cadastradas por Marcos, antigo responsável pela
         disciplina 8, que hoje está sem responsável (RN10)
     - 2 simulados finalizados (Ana: 10 questões; Henrique: 15 questões)
       com respostas, resultado e desempenho. As questões desses simulados
       ficam "já utilizadas": a alternativa correta não pode mudar (RN11).

   Os Ids são fixos (IDENTITY_INSERT) para facilitar o roteiro de testes.
   Pré-requisito: scripts 01, 02 e 03 executados em um banco vazio.
   ===================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF EXISTS (SELECT 1 FROM dbo.Usuario)
BEGIN
    RAISERROR(N'A tabela Usuario já possui dados. Execute o 00_reset.sql e os scripts 01 a 03 antes deste script.', 16, 1);
    SET NOEXEC ON;
END
GO

BEGIN TRANSACTION;

/* ---------------------------------------------------------------- Turmas (o período integra o nome: RN06) */
SET IDENTITY_INSERT dbo.Turma ON;
INSERT INTO dbo.Turma (Id_Turma, Nome_Turma, AnoLetivo_Turma, Ativo_Turma) VALUES
    (1, N'ADS 3º Semestre A', 2026, 1),
    (2, N'ADS 3º Semestre B', 2026, 1),
    (3, N'ADS 2º Semestre A', 2025, 0);   -- desativada, com alunos ativos (RN12)
SET IDENTITY_INSERT dbo.Turma OFF;

/* ---------------------------------------------------------------- Usuários
   Perfil: 1 Aluno, 2 Professor, 3 Administrador. Senha = hash BCrypt do CPF. */
SET IDENTITY_INSERT dbo.Usuario ON;
INSERT INTO dbo.Usuario (Id_Usuario, Nome_Usuario, CPF_Usuario, Email_Usuario, Senha_Usuario,
                         DataIngresso_Usuario, Ativo_Usuario, PontuacaoTotal_Usuario, Id_Perfil, Id_Turma) VALUES
    (1, N'Administrador Avalia+', '15881399803', 'admin@avaliamais.test',
     '$2a$10$/okvwOV/qtvvyq1zH00e8uUPS4HM4sLlSQ2cSXJYjkYZlduqKFSx2', '2025-01-06', 1, 0, 3, NULL),
    (2, N'Ricardo Tavares Monteiro', '69879730917', 'ricardo.monteiro@avaliamais.test',
     '$2a$10$e8oi2RCOqlBnRgANGB/1Fu1dYx2dbbYF1YdMHmBVRWNaE18GMnRAW', '2025-01-20', 1, 0, 2, NULL),
    (3, N'Patrícia Duarte Vieira', '11417075350', 'patricia.vieira@avaliamais.test',
     '$2a$10$WIW2HKQyMNDqU9cyfGomP.a98RU6You9Djj0vmC26BKmIHCz0bW.e', '2025-01-20', 1, 0, 2, NULL),
    (4, N'Marcos Antônio Pinto', '64556815177', 'marcos.pinto@avaliamais.test',
     '$2a$10$MFU5U1Y65kQOXQXynErGwe5VOvWHpfW3ylcXLhL195YDuAYrPScF.', '2025-02-03', 1, 0, 2, NULL),
    (5, N'Ana Beatriz Souza', '88447294072', 'ana.souza@aluno.avaliamais.test',
     '$2a$10$fT9ugIU6bEJ9n4p8DPx9EeXFu8W9j9TYjK3gCXpAhe.GnbanngtB6', '2025-02-03', 1, 70, 1, 1),
    (6, N'Bruno Henrique Lima', '55761698757', 'bruno.lima@aluno.avaliamais.test',
     '$2a$10$EPCKbNeeYRPn.qS0Q7OfluGkbphbXhApvS4rueT5uJxfBrZRnEJ9.', '2025-02-03', 1, 0, 1, 1),
    (7, N'Camila Ferreira Rocha', '16897684600', 'camila.rocha@aluno.avaliamais.test',
     '$2a$10$Y9gpxMAOriklXi76XphzOuRnX4ze85XaThXrvKUhiEEq3Wwva1sG.', '2025-02-03', 1, 0, 1, 1),
    (8, N'Daniel Alves Pereira', '97880932054', 'daniel.pereira@aluno.avaliamais.test',
     '$2a$10$l6by1aPnl5lxiNUwvGrH2eNri2kfBJUzIYisSXNs7.mqu0hzQK0Dq', '2025-02-03', 1, 0, 1, 1),
    (9, N'Eduarda Martins Costa', '81967273138', 'eduarda.costa@aluno.avaliamais.test',
     '$2a$10$WLjqzfrz.q5dj4dKYYiea.EkQMLw7T2FoNhmkpz6Tmnki2WHs62PG', '2025-02-10', 1, 0, 1, 1),
    (10, N'Felipe Gomes Ribeiro', '77848648842', 'felipe.ribeiro@aluno.avaliamais.test',
     '$2a$10$UOw2g4NPucNn56XnPH.Ys.57NklbpvQt7V98iE1aHyQZUu1WWixzK', '2025-02-10', 1, 0, 1, 1),
    (11, N'Gabriela Nunes Carvalho', '76634400210', 'gabriela.carvalho@aluno.avaliamais.test',
     '$2a$10$Yk9fB/BWvWiC9oI99HbTUO18JMeS/.U9lV9bEb2SBDn4OdWxoTvZi', '2025-02-10', 0, 0, 1, 1),   -- aluno desativado (RN25)
    (12, N'Henrique Silva Barbosa', '97875347718', 'henrique.barbosa@aluno.avaliamais.test',
     '$2a$10$smWjeEG4/oX5.5kfRPFbc.UOxDxkSDH6UsWmOGPVL1Yd03IyFix4e', '2025-02-03', 1, 110, 1, 2),
    (13, N'Isadora Mendes Araújo', '87680457190', 'isadora.araujo@aluno.avaliamais.test',
     '$2a$10$zpLduMyI6VL4yLCR49rWJeKCGzPnNS0AT22bk4EzDAnn0XVw9rdqW', '2025-02-03', 1, 0, 1, 2),
    (14, N'João Pedro Cardoso', '78472671445', 'joao.cardoso@aluno.avaliamais.test',
     '$2a$10$ke0zTIoHqBq6z2On/pAVU.AxTs2q4jf36Gt6uQWCUYrYFDVGWLL9i', '2025-02-03', 1, 0, 1, 2),
    (15, N'Karina Lopes Teixeira', '75136270101', 'karina.teixeira@aluno.avaliamais.test',
     '$2a$10$LcsC1mt1vzD4NvOgZkE1Suvgvi7NIwvjKZ1Xta.XuLclNV1s2P7o2', '2025-02-10', 1, 0, 1, 2),
    (16, N'Lucas Moreira Dias', '07981933102', 'lucas.dias@aluno.avaliamais.test',
     '$2a$10$yStKM6c4HVWOX8UxaO0ZkO/PS1aPKg6F8T/do9IpGJj8LYBVdnWhu', '2025-02-10', 1, 0, 1, 2),
    (17, N'Mariana Castro Freitas', '90845205994', 'mariana.freitas@aluno.avaliamais.test',
     '$2a$10$GXIkcmwGPKfq2ZTo4EDRbeF4/qBtzZmgdaUACZSMKZohiboc20n/W', '2025-02-10', 1, 0, 1, 2),
    (18, N'Nicolas Almeida Santos', '97283792937', 'nicolas.santos@aluno.avaliamais.test',
     '$2a$10$miNtwTiNFyexZPNK3jbxBOpqO45/V51ApadwatZu6cB/5RaJXJn9O', '2025-08-04', 1, 0, 1, 3),   -- turma desativada, aluno ativo (RN12)
    (19, N'Olívia Ramos Correia', '84162294810', 'olivia.correia@aluno.avaliamais.test',
     '$2a$10$yeHzxzFnCDW3.x6YCwGJxeJB0MpoWr/dqWJceecElIKadbZ/Dbw7m', '2025-08-04', 1, 0, 1, 3);   -- turma desativada, aluno ativo (RN12)
SET IDENTITY_INSERT dbo.Usuario OFF;

/* ---------------------------------------------------------------- Disciplinas (Id_Usuario = professor responsável) */
SET IDENTITY_INSERT dbo.Disciplina ON;
INSERT INTO dbo.Disciplina (Id_Disciplina, Nome_Disciplina, Descricao_Disciplina, Ativo_Disciplina, Id_Usuario) VALUES
    (1, N'Engenharia de Software',
     N'Processos e modelos de desenvolvimento, RUP, métodos ágeis e arquitetura de software.', 1, 2),
    (2, N'Requisitos e Qualidade de Software',
     N'Engenharia de requisitos, testes, manutenção, qualidade e gerência de configuração.', 1, 2),
    (3, N'Análise e Modelagem de Sistemas',
     N'UML, casos de uso, modelagem de processos de negócio (BPM) e interação humano-computador.', 1, 2),
    (4, N'Algoritmos e Lógica de Programação',
     N'Lógica proposicional, algoritmos, recursão, ordenação e sistemas de numeração.', 1, 3),
    (5, N'Programação Orientada a Objetos e Estruturas de Dados',
     N'Classes, herança, polimorfismo, padrões de projeto, listas, pilhas, filas e árvores.', 1, 3),
    (6, N'Banco de Dados',
     N'Modelo relacional, modelagem de dados, SQL, transações, restrições e data warehouse.', 1, 4),
    (7, N'Redes de Computadores e Sistemas Operacionais',
     N'TCP/IP, IPv6, DNS, serviços de rede, sistemas distribuídos, processos e memória.', 1, 4),
    (8, N'Gestão de Projetos, Governança e Segurança da Informação',
     N'PMBOK, caminho crítico, liderança, governança de TI, segurança da informação e legislação.', 1, NULL);   -- sem responsável (RN07)
SET IDENTITY_INSERT dbo.Disciplina OFF;

/* ---------------------------------------------------------------- Turma x Disciplina */
INSERT INTO dbo.TurmaDisciplina (Id_Turma, Id_Disciplina) VALUES
    (1, 1),
    (1, 2),
    (1, 3),
    (1, 4),
    (1, 5),
    (1, 6),
    (2, 1),
    (2, 2),
    (2, 3),
    (2, 6),
    (2, 7),
    (2, 8),
    (3, 4),
    (3, 5);

/* ---------------------------------------------------------------- Questões e alternativas
   Cada questão: exatamente 5 alternativas (A a E) e apenas 1 correta (RN09). */
-- Questão 1 | Engenharia de Software | correta: D
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (1, N'Um analista foi contratado para desenvolver um sistema de pesquisa de DVDs em lojas virtuais. O sistema deverá solicitar ao usuário um título de DVD, que será usado para realizar a pesquisa nas bases de dados das lojas conveniadas. Ao detectar a disponibilidade do DVD solicitado, o sistema armazenará temporariamente os dados das lojas (nome, preço, data prevista para entrega do produto) e exibirá as informações ordenadas por preço. Após analisar as informações, o cliente poderá efetuar a compra. O contratante deverá testar algumas operações do sistema antes de ele ser finalizado. Há tempo suficiente para que o analista atenda a essa solicitação e efetue eventuais modificações exigidas pelo contratante. Com relação a essa situação, julgue os itens a seguir quanto ao modelo de ciclo de vida.
I. O entendimento do sistema como um todo e a execução sequencial das fases sem retorno produzem um sistema que pode ser validado pelo contratante.
II. A elaboração do protótipo pode ser utilizada para resolver dúvidas de comunicação, o que aumenta os riscos de inclusão de novas funcionalidades não prioritárias.
III. A definição das restrições deve ser a segunda fase a ser realizada no desenvolvimento do projeto, correspondendo à etapa de engenharia.
IV. Um processo iterativo permite que versões progressivas mais completas do sistema sejam construídas e avaliadas.
São corretos apenas os itens
(ENADE 2008)',
     NULL, N'O contratante quer testar o sistema e pedir ajustes durante o desenvolvimento, o que favorece protótipos e um processo iterativo. O item II está correto: o protótipo esclarece dúvidas, mas pode estimular pedidos de funções não prioritárias. O item IV também: cada iteração entrega uma versão mais completa para avaliação. O item I descreve o modelo em cascata, que não permite essa validação ao longo do processo, e no item III a definição de restrições pertence ao levantamento de requisitos.', 1, 1, 2);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (1, 'A', N'I e II.', 0, 1),
    (2, 'B', N'I e III.', 0, 1),
    (3, 'C', N'II e III.', 0, 1),
    (4, 'D', N'II e IV.', 1, 1),
    (5, 'E', N'III e IV.', 0, 1);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 2 | Engenharia de Software | correta: D
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (2, N'O Rational Unified Process (RUP) é um processo de engenharia de software cujo objetivo é assegurar a produção de software de alta qualidade, satisfazendo as necessidades dos usuários no prazo e nos custos previstos. O RUP contém uma estrutura que pode ser adaptada e estendida, pois é formado por duas estruturas principais, denominadas dimensões, que representam os aspectos dinâmicos e estáticos do processo. O aspecto dinâmico é expresso em ciclos, fases, iterações e marcos. O estático, por sua vez, contém as disciplinas, os fluxos, os artefatos e os trabalhadores.
Com base na iteração do RUP, julgue as asserções e a relação proposta entre elas.
I. A cada iteração das fases do RUP, geram-se ou não artefatos de software
PORQUE
II. Os artefatos produzidos dependem da ênfase que é dada a cada disciplina.
Assinale a opção correta.
(ENADE 2008)',
     NULL, NULL, 1, 1, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (6, 'A', N'As duas asserções são proposições verdadeiras, e a segunda justifica a primeira.', 0, 2),
    (7, 'B', N'As duas asserções são proposições verdadeiras, e a segunda não justifica a primeira.', 0, 2),
    (8, 'C', N'A primeira asserção é uma proposição verdadeira, e a segunda, uma proposição falsa.', 0, 2),
    (9, 'D', N'A primeira asserção é uma proposição falsa, e a segunda, uma proposição verdadeira.', 1, 2),
    (10, 'E', N'As duas asserções são proposições falsas.', 0, 2);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 3 | Engenharia de Software | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (3, N'Com relação à forma como o RUP trata a análise de requisitos, assinale a opção correta.
(ENADE 2008)',
     NULL, N'No RUP, os requisitos são trabalhados em várias iterações, mas a maior parte da análise acontece na fase de elaboração, quando a arquitetura e a maioria dos casos de uso são detalhados. As outras alternativas restringem a análise a uma única fase ou proíbem mudanças, o que contraria o caráter iterativo do processo.', 1, 1, 2);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (11, 'A', N'A análise de requisitos ocorre na fase de construção, quando são descritos todos os casos de uso, e em seguida modelados por meio de diagramas de casos de uso UML.', 0, 3),
    (12, 'B', N'A análise de requisitos ocorre na fase de elaboração, em que são feitas entrevistas com usuários e definição do escopo do projeto.', 0, 3),
    (13, 'C', N'A maior parte da análise de requisitos ocorre durante a fase de elaboração.', 1, 3),
    (14, 'D', N'Por se tratar de um processo iterativo e evolutivo, a análise de requisitos ocorre na fase de construção juntamente com a programação, o que permite que os requisitos sejam revistos.', 0, 3),
    (15, 'E', N'A análise de requisitos deve acontecer antes da programação e testes do sistema, não podendo sofrer alterações a partir do momento que estejam definidos.', 0, 3);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 4 | Engenharia de Software | correta: A
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (4, N'Leia o texto a seguir.
Várias técnicas relacionadas à programação extrema (XP) são diretamente ligadas ao código, incluindo a refatoração, programação em pares e integração contínua. A programação em pares é a prática preferida dos desenvolvedores XP trabalhando em pares em um computador.
SCOTT, K. O Processo Unificado Explicado. Porto Alegre: Artmed, 2002. BECK, K. Programação Extrema (XP) Explicada.
Porto Alegre: Artmed, 2000.
A programação em pares auxilia no desenvolvimento de código de melhor qualidade quando os pares
(ENADE 2011)',
     NULL, NULL, 1, 1, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (16, 'A', N'elaboram e utilizam padrões de codificação conjuntamente, os quais, quando utilizados corretamente e apropriadamente, reduzem problemas individuais.', 1, 4),
    (17, 'B', N'estão acostumados ao desenvolvimento e à propriedade coletiva, limitando-se a fazer pequenas mudanças na ocorrência de erros em tempo de execução.', 0, 4),
    (18, 'C', N'minimizam os riscos de insucesso no projeto através da utilização de ferramentas para a geração automática de testes funcionais e protótipos de interface.', 0, 4),
    (19, 'D', N'escrevem testes em separado e discutem os resultados posteriormente, o que dá a eles a chance de se sintonizarem antes de começarem a implementação.', 0, 4),
    (20, 'E', N'trabalham em projetos complexos onde a codificação seja desenvolvida de forma conjunta, minimizando erros e agregando valor onde quer que o sistema necessite.', 0, 4);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 5 | Engenharia de Software | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (5, N'Nos anos 1970, os sistemas executavam em mainframes com aplicativos escritos em linguagens estruturadas e com todas as funcionalidades em um único módulo, com grande quantidade de linhas de código. Acessos a bancos de dados não relacionais, regras de negócios e tratamento de telas para terminais "burros" ficavam no mesmo programa. Posteriormente, uma importante mudança ocorreu: a substituição dos terminais "burros" por microcomputadores, permitindo que todo o tratamento da interface, e de algumas regras de negócios, passassem a ser feitas nas estações clientes. Surgiam as aplicações cliente-servidor. A partir dos anos 1990 até os dias atuais, as mudanças foram mais radicais, os bancos de dados passaram a ser relacionais e distribuídos. As linguagens passaram a ser orientadas a objetos, cuja modelagem encapsula dados e oferece funcionalidades através de métodos. A interface passou a ser web. Vive-se a era das aplicações em três camadas. Considerando a evolução da arquitetura de software de sistemas de informação, conforme citado no texto acima, avalie as seguintes afirmações:
I. A separação dos sistemas em três camadas lógicas torna os sistemas mais complexos, requerendo pessoal mais especializado.
II. A separação dos sistemas em três camadas lógicas torna os sistemas mais flexíveis, permitindo que as partes possam ser alteradas de forma independente.
III. A separação dos sistemas em três camadas lógicas aumentou o acoplamento, dificultando a manutenção.
É correto o que se afirma em
(ENADE 2014)',
     NULL, N'Separar apresentação, regras de negócio e dados em três camadas torna o sistema mais flexível, porque cada camada pode ser alterada de forma independente (II), mas também mais complexo, exigindo pessoal mais especializado (I). A afirmativa III está errada: a separação em camadas diminui o acoplamento e facilita a manutenção.', 1, 1, 2);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (21, 'A', N'II, apenas.', 0, 5),
    (22, 'B', N'III, apenas.', 0, 5),
    (23, 'C', N'I e II, apenas.', 1, 5),
    (24, 'D', N'I e III, apenas.', 0, 5),
    (25, 'E', N'I, II e III.', 0, 5);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 6 | Engenharia de Software | correta: B
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (6, N'O Rational Unified Process (RUP) é subdividido em fases, que indicam a ênfase que é dada ao projeto em um dado instante, e em fluxos de trabalho, que definem os grupos de atividades a serem realizadas ao longo das fases.
Considerando que no desenvolvimento de software utilizam-se técnicas de análise e projeto orientado a objetos e que as atividades sejam distribuídas de acordo com o RUP, o momento em que devem ser elaborados os diagramas de classes e de interação entre os objetos do sistema é
(ENADE 2014)',
     NULL, NULL, 1, 1, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (26, 'A', N'na fase de concepção, no fluxo de modelagem de negócios.', 0, 6),
    (27, 'B', N'na fase de elaboração, no fluxo de análise e design.', 1, 6),
    (28, 'C', N'na fase de concepção, no fluxo de análise e design.', 0, 6),
    (29, 'D', N'na fase de elaboração, no fluxo de requisitos.', 0, 6),
    (30, 'E', N'na fase de concepção, no fluxo de requisitos.', 0, 6);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 7 | Engenharia de Software | correta: D
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (7, N'Leia o texto a seguir.
Os modelos de processo foram propostos para trazer ordem ao caos existente na área de desenvolvimento de software. A história mostra que esses modelos trouxeram considerável contribuição no trabalho da engenharia de software.
PRESSMAN, R. S. Engenharia de software: uma abordagem profissional. 8. ed. Porto Alegre: AMGH, 2016 (com adaptações).
A respeito dos modelos de processo, avalie as afirmativas.
I. São atividades do modelo incremental: especificação, desenvolvimento e validação.
II. No modelo espiral, a fase de modelagem é responsável, entre outras atividades, pela estimativa, cronograma e análise de risco.
III. O modelo em cascata sugere uma abordagem sequencial e sistemática para o desenvolvimento de software, iniciando na especificação de requisitos e finalizando com a entrega do software concluído.
É correto o que se afirma em
(ENADE 2017)',
     NULL, N'O modelo incremental repete especificação, desenvolvimento e validação a cada incremento (I), e o modelo em cascata segue fases sequenciais, da especificação de requisitos até a entrega (III). A afirmativa II está errada: no modelo espiral, estimativa, cronograma e análise de riscos fazem parte do planejamento, não da modelagem.', 1, 1, 2);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (31, 'A', N'II, apenas.', 0, 7),
    (32, 'B', N'III, apenas.', 0, 7),
    (33, 'C', N'I e II, apenas.', 0, 7),
    (34, 'D', N'I e III, apenas.', 1, 7),
    (35, 'E', N'I, II e III.', 0, 7);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 8 | Engenharia de Software | correta: A
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (8, N'Leia o texto a seguir.
Arquitetura de software é uma representação que permite analisar a efetividade do projeto no entendimento dos requisitos declarados. Durante a fase de concepção da arquitetura, podem-se considerar alternativas de arquitetura em um estágio em que mudanças ainda são realizadas com menor esforço, diminuindo riscos associados à construção do software ainda na fase inicial.
PRESSMAN, R. S. Engenharia de Software: uma abordagem profissional. 8. ed. Porto Alegre: AMGH, 2016 (com adaptações).
A respeito dos estilos e padrões arquiteturais contidos na engenharia de software, avalie as afirmativas.
I. Em arquiteturas orientadas a objetos, a comunicação entre os componentes do software é realizada por intermédio da troca de mensagens.
II. As arquiteturas monolíticas consistem de um sistema dividido em pequenas partes, possibilitando que estas tenham sua manutenção, execução e evolução individual.
III. No padrão arquitetural Modelo-Visão-Controle (MVC), a camada de Modelo armazena as interações realizadas no Controle, podendo ser apresentados/manipulados posteriormente na Visão.
IV. Nas arquiteturas microsserviços, o software possui componentes altamente acoplados, dificultando a manutenção.
É correto apenas o que se afirma em
(ENADE 2021)',
     NULL, NULL, 1, 1, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (36, 'A', N'I e III.', 1, 8),
    (37, 'B', N'II e III.', 0, 8),
    (38, 'C', N'II e IV.', 0, 8),
    (39, 'D', N'I, II e IV.', 0, 8),
    (40, 'E', N'I, III e IV.', 0, 8);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 9 | Engenharia de Software | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (9, N'Uma fábrica de software está realizando entrevistas para contratação de um profissional que esteja alinhado às exigências do atual mundo corporativo. Sabe-se que processos ágeis de desenvolvimento têm se tornado essenciais para empresas que desejam realizar entregas rápidas e frequentes de produtos e/ou serviços de software. Essa empresa possui uma equipe de desenvolvimento que faz uso de processos ágeis como o Scrum e eXtreme Programming (XP) e o acompanhamento por meio do quadro Kanban. Sendo assim, um conjunto de características deve ser verificado durante a entrevista para garantir que o candidato a ser contratado possua conhecimentos necessários para atuar juntamente a esta equipe. Com base no texto e nos processos ágeis de desenvolvimento de software, avalie as afirmativas.
I. Métodos ágeis são baseados em ciclos iterativo e incremental que se concentram no desenvolvimento rápido e na flexibilidade às mudanças, com a participação do cliente no processo de software.
II. Uma forte característica da XP é a garantia da qualidade do código produzido e, para isso, os desenvolvedores produzem testes automatizados antes mesmo de codificar uma funcionalidade.
III. O planejamento no Scrum é baseado na elaboração dos itens do product backlog, que é uma lista de funcionalidades desejadas pelo cliente, sendo o Scrum Master o responsável por gerenciá-lo.
IV. O quadro Kanban permite monitorar a evolução das tarefas necessárias durante o processo ágil de desenvolvimento de software, possibilitando um acompanhamento de forma visual das atividades em construção.
É correto apenas o que se afirma em
(ENADE 2021)',
     NULL, N'Métodos ágeis são iterativos e incrementais e contam com a participação do cliente (I); a XP escreve testes automatizados antes do código (II); e o quadro Kanban dá visibilidade ao andamento das tarefas (IV). A afirmativa III está errada porque quem gerencia o Product Backlog é o Product Owner, e não o Scrum Master.', 1, 1, 2);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (41, 'A', N'II.', 0, 9),
    (42, 'B', N'I e III.', 0, 9),
    (43, 'C', N'I, II e IV.', 1, 9),
    (44, 'D', N'I, III e IV.', 0, 9),
    (45, 'E', N'II, III e IV.', 0, 9);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 10 | Engenharia de Software | correta: E
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (10, N'Uma determinada empresa do ramo de piscicultura solicitou a uma instituição especializada no desenvolvimento de softwares de gestão a construção de um sistema que fosse capaz de gerenciar o processo de criação dos peixes, que começava ainda na fase de alevinos, estendia-se para a engorda, finalizando na fase adulta com a venda do peixe. O sistema deveria disponibilizar funcionalidades para armazenar informações sobre toda a etapa da criação, mortandade e rações utilizadas na engorda dos peixes, viabilizando uma melhor tomada de decisão dos investidores. Como não havia nenhuma solução e a demanda por peixes estava aumentando, a empresa solicitou extrema urgência na construção da aplicação. Considerando o processo de desenvolvimento de software que deverá ser utilizado para construir o sistema mencionado no texto, avalie as afirmativas.
I. Para desenvolver o sistema, deve ser adotado um processo baseado no modelo cascata, pois ele se adapta melhor a cenários com muitas mudanças.
II. Por se tratar de uma aplicação com escopo pequeno, o modelo espiral deverá ser utilizado no desenvolvimento do sistema solicitado, já que reduzirá riscos por meio da prototipação de especificações de cada fase dos peixes e diminuirá o tempo de entrega.
III. O modelo incremental deve ser utilizado na construção do software descrito no cenário, já que partes do sistema referentes à fase inicial dos peixes poderiam ser disponibilizadas, enquanto as demais estão nas etapas de concepção e desenvolvimento.
IV. Em razão da extrema urgência no desenvolvimento do software solicitado, a empresa deve elaborar seu processo utilizando o modelo RAD (Rapid Application Development).
É correto apenas o que se afirma em
(ENADE 2021)',
     NULL, NULL, 1, 1, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (46, 'A', N'I.', 0, 10),
    (47, 'B', N'IV.', 0, 10),
    (48, 'C', N'I e II.', 0, 10),
    (49, 'D', N'II e III.', 0, 10),
    (50, 'E', N'III e IV.', 1, 10);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 11 | Requisitos e Qualidade de Software | correta: E
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (11, N'Os alunos de uma disciplina deveriam escolher um sistema de média complexidade, contendo no mínimo 100 funcionalidades, para ser modelado em UML e codificado em uma linguagem orientada a objetos. Um dos grupos de alunos estabeleceu a seguinte estratégia para identificação e seleção do sistema.
I. Cada integrante do grupo deveria criar um nickname (apelido) em um software de chat.
II. O grupo deveria se reunir em um horário predeterminado.
III. Durante o chat, os seguintes procedimentos deveriam ser realizados:
• cada integrante deveria sugerir um ou mais sistemas e justificar sua escolha, e não poderia criticar as ideias dos outros;
• à medida que as ideias fossem digitadas, o líder deveria copiá-las para um editor de texto e controlar o tempo de sugestão;
• quando o limite de tempo fosse atingido, o líder disponibilizaria todas as sugestões para serem analisadas pelo grupo;
• as 5 melhores ideias seriam selecionadas e colocadas em votação para a escolha da melhor ideia, segundo critérios predefinidos.
Nessa situação, a estratégia utilizada pelo grupo de alunos é uma adaptação da técnica de levantamento e elicitação de requisitos denominada
(ENADE 2008)',
     NULL, N'O grupo gerou livremente o maior número de ideias, sem críticas, e só depois analisou e votou nas melhores. Essas são as características do brainstorming; o chat é apenas o meio usado para aplicar a técnica.', 1, 2, 2);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (51, 'A', N'joint application design.', 0, 11),
    (52, 'B', N'PIECES (perfomance informação/dados economia controle eficiência e serviços).', 0, 11),
    (53, 'C', N'facilitaded application specification techniques.', 0, 11),
    (54, 'D', N'entrevista.', 0, 11),
    (55, 'E', N'brainstorming.', 1, 11);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 12 | Requisitos e Qualidade de Software | correta: A
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (12, N'Uma indústria de alimentos compra sementes de vários fornecedores. No recebimento das cargas, as sementes passam por uma operação de classificação por cor, em uma esteira adquirida do fabricante MAQ, equipada com sensores e software de processamento de imagens. Na etapa seguinte do processo, as sementes são separadas em lotes, pelo critério de tamanho, e são, então, empacotadas. A separação dos lotes é realizada por um mecanismo robótico, controlado por computador e que, pelo fato de sofrer contínuo desgaste, necessita ser substituído a cada 1.000 horas de uso. Durante a última troca, em razão da indisponibilidade do equipamento produzido pela empresa MAQ, a indústria instalou, com sucesso, um equipamento robótico similar.
Considerando o processo descrito, julgue os itens a seguir, relacionados aos fatores de qualidade.
I. As operações de classificação e separação de sementes se interrelacionam e não podem falhar, pois essa falha acarretaria prejuízos. O atributo de qualidade correspondente a essas operações, e que deve ser observado pelo software, é a interoperabilidade.
II. Caso o responsável pela instalação do sistema robotizado não tenha encontrado dificuldade em fazê-lo comunicar-se com o equipamento de outra marca, é correto concluir que o sistema que controla o robô é portável.
III. A maneira como ocorre a interação com o sistema computacional sugere que alguns requisitos, como ergonomia, sejam observados na interface. Por isso, é correto concluir que o software utilizado pela indústria contempla o fator denominado usabilidade.
Assinale a opção correta.
(ENADE 2008)',
     NULL, NULL, 1, 2, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (56, 'A', N'Apenas um item está certo.', 1, 12),
    (57, 'B', N'Apenas os itens I e II estão certos.', 0, 12),
    (58, 'C', N'Apenas os itens I e III estão certos.', 0, 12),
    (59, 'D', N'Apenas os itens II e III estão certos.', 0, 12),
    (60, 'E', N'Todos os itens estão certos.', 0, 12);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 13 | Requisitos e Qualidade de Software | correta: B
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (13, N'O levantamento de requisitos é uma etapa fundamental do projeto de sistemas. Dependendo da situação encontrada, uma ou mais técnicas podem ser utilizadas para a elicitação dos requisitos.
A respeito dessas técnicas, avalie as afirmativas.
I. Workshop de requisitos consiste na realização de reuniões estruturadas e delimitadas entre os analistas de requisitos do projeto e representantes do cliente.
II. Cenário consiste na observação das ações do funcionário na realização de uma determinada tarefa, para verificar os passos necessários para sua conclusão.
III. As entrevistas são realizadas com os stakeholders e podem ser abertas ou fechadas.
IV. A prototipagem é uma versão inicial do sistema, baseado em requisitos levantados em outros sistemas da organização.
É correto apenas o que se afirma em
(ENADE 2011)',
     NULL, N'Workshop de requisitos são reuniões estruturadas entre analistas e representantes do cliente (I) e entrevistas podem ser abertas ou fechadas (III). Observar o funcionário executando a tarefa é a técnica de observação (etnografia), não cenário (II), e o protótipo é baseado nos requisitos do próprio sistema em desenvolvimento, não nos de outros sistemas (IV).', 1, 2, 2);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (61, 'A', N'I e II.', 0, 13),
    (62, 'B', N'I e III.', 1, 13),
    (63, 'C', N'II e IV.', 0, 13),
    (64, 'D', N'I, III e IV.', 0, 13),
    (65, 'E', N'II, III e IV.', 0, 13);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 14 | Requisitos e Qualidade de Software | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (14, N'A engenharia de software considera diversos aspectos para a garantia da qualidade. Os requisitos funcionais definem como um sistema deverá se comportar em relação as suas funcionalidades básicas, já os requisitos não funcionais avaliam outros aspectos do software. São exemplos de requisitos não funcionais a serem considerados em um software
(ENADE 2014)',
     NULL, NULL, 1, 2, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (66, 'A', N'segurança, desempenho, estresse e sistema.', 0, 14),
    (67, 'B', N'usabilidade, segurança, aceitação e confiabilidade.', 0, 14),
    (68, 'C', N'usabilidade, segurança, desempenho e confiabilidade', 1, 14),
    (69, 'D', N'segurança, aceitação, testabilidade e confidencialidade.', 0, 14),
    (70, 'E', N'usabilidade, confidencialidade, aceitação e confiabilidade.', 0, 14);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 15 | Requisitos e Qualidade de Software | correta: D
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (15, N'Leia o texto a seguir.
A engenharia de requisitos, do ponto de vista do processo de software, é uma ação de engenharia de software importante, que se inicia durante a atividade de comunicação e continua na de modelagem. Ela deve ser adaptada às necessidades do processo, do projeto, do produto e das pessoas que estão realizando o trabalho.
PRESSMAN, R. S. Engenharia de software: uma abordagem profissional. 8. ed. Porto Alegre: AMGH, 2016 (com adaptações).
Considere os requisitos, a seguir, de um sistema para uma universidade, na qual se pretenda gerenciar o setor acadêmico.
● R1: o sistema deve permitir que cada professor realize o lançamento de notas das turmas nas quais lecionou;
● R2: o sistema deverá ser desenvolvido de forma a possibilitar seu transporte para outro sistema operacional em, no máximo, sessenta dias;
● R3: o sistema deve permitir que um estudante realize a sua matrícula nas disciplinas oferecidas em semestre letivo;
● R4: o sistema atualiza a nota do estudante, permitindo sua visualização, em até dois segundos depois do momento que o professor a registra;
● R5: o sistema deve permitir que o auxiliar de serviços acadêmicos realize o cadastro de um estudante em não mais do que dez minutos de orientação.
Nessa situação, representam descrições de requisitos não funcionais apenas os requisitos
(ENADE 2017)',
     NULL, N'R2 trata de portabilidade, R4 de desempenho (tempo de resposta) e R5 de usabilidade (facilidade de aprendizado): são requisitos não funcionais. R1 e R3 descrevem funções que o sistema deve oferecer (lançar notas e realizar matrícula), portanto são requisitos funcionais.', 1, 2, 2);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (71, 'A', N'R1, R2 e R3.', 0, 15),
    (72, 'B', N'R1, R2 e R5.', 0, 15),
    (73, 'C', N'R1, R3 e R4.', 0, 15),
    (74, 'D', N'R2, R4 e R5.', 1, 15),
    (75, 'E', N'R3, R4 e R5.', 0, 15);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 16 | Requisitos e Qualidade de Software | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (16, N'Leia o texto a seguir.
A etapa de definição de requisitos é voltada para estabelecer quais as funções são requeridas pelo sistema e as restrições sobre a operação e o desenvolvimento do software. Os requisitos de software podem ser classificados como requisitos funcionais e não funcionais.
SOMMERVILLE, I. Engenharia de Software, 10. ed. São Paulo: Pearson Education, 2019 (com adaptações).
Considerando as informações do texto, assinale a alternativa em que o item é um requisito funcional.
(ENADE 2021)',
     NULL, NULL, 1, 2, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (76, 'A', N'O software deve ser operacionalizado no sistema Linux.', 0, 16),
    (77, 'B', N'O tempo de desenvolvimento não deve ultrapassar seis meses.', 0, 16),
    (78, 'C', N'O software deve emitir relatórios de compras a cada quinze dias.', 1, 16),
    (79, 'D', N'O tempo de resposta do sistema não deve ultrapassar 30 segundos.', 0, 16),
    (80, 'E', N'A base de dados deve ser protegida para acesso apenas de usuários autorizados.', 0, 16);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 17 | Requisitos e Qualidade de Software | correta: B
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (17, N'Um software com defeito pode ser consequência de problemas no levantamento dos requisitos, uma vez que o requisito pode ser ambíguo porque o cliente não estava convicto de sua real necessidade ou porque a equipe o interpretou mal e registrou uma especificação de forma incorreta. Por esses motivos, as verificações, as validações e os testes são fundamentais para se certificar da qualidade do software resultante.
Considerando esse contexto, avalie as afirmativas.
I. O teste funcional certifica se o software desempenha as funções especificadas nos requisitos.
II. O teste de desempenho valida a conformidade da especificação do processo de desenvolvimento de software.
III. O teste de aceitação é realizado pelo cliente a fim de validar se aquilo que foi implementado é o que foi solicitado.
IV. O teste de instalação, invariavelmente, é executado no local determinado pelo cliente para instalação do software.
V. As técnicas de verificação e validação de software asseguram que o sistema que está sendo desenvolvido seja adequado ao seu propósito.
É correto apenas o que se afirma em
(ENADE 2017)',
     NULL, N'O teste funcional verifica se o software cumpre as funções especificadas (I), o teste de aceitação é feito pelo cliente para validar o que foi pedido (III) e verificação e validação garantem que o sistema atende ao seu propósito (V). O teste de desempenho avalia tempo de resposta e uso de recursos, não o processo de desenvolvimento (II), e o teste de instalação nem sempre é feito no local do cliente (IV).', 1, 2, 2);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (81, 'A', N'I e IV.', 0, 17),
    (82, 'B', N'I, III e V.', 1, 17),
    (83, 'C', N'II, III e V.', 0, 17),
    (84, 'D', N'II, IV e V.', 0, 17),
    (85, 'E', N'I, II, III e V.', 0, 17);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 18 | Requisitos e Qualidade de Software | correta: D
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (18, N'Em razão da covid-19, muitos estabelecimentos viram no delivery uma forma de manter os negócios funcionando. As mudanças de hábitos dos brasileiros revelaram um aumento do interesse nesse tipo de serviço.
Nesse contexto, um restaurante solicitou o desenvolvimento de um sistema web que possibilite gerenciar automaticamente os pedidos de entrega. O sistema foi projetado com base em tecnologias web e modelado com UML (Unified Modeling Language). Durante o processo de desenvolvimento, foram descritos alguns itens importantes que merecem atenção no processo de teste de software.
Considere as seguintes descrições definidas pelo analista de teste:
1. Verificar se o método “Finalizar Pedido” da classe “Pedido” está funcionando corretamente.
2. Garantir que o sistema funcione corretamente em diferentes navegadores de internet.
3. O sistema deve passar por testes rigorosos na estrutura lógica interna do software.
4. O sistema deve garantir, no mínimo, o registro de 100 pedidos simultâneos.
5. O usuário deve testar o sistema em um ambiente controlado sob a supervisão dos desenvolvedores.
Considerando o texto e as descrições apresentadas, avalie as afirmativas.
I. A descrição 1 deve ser verificada com teste unitário.
II. A descrição 2 deve ser executada com a abordagem caixa-branca.
III. A descrição 3 deve ser executada com a abordagem caixa-preta.
IV. A descrição 4 deve ser verificada com teste carga.
V. A descrição 5 deve ser classificada como teste alfa.
É correto apenas o que se afirma em
(ENADE 2021)',
     NULL, NULL, 1, 2, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (86, 'A', N'II e III.', 0, 18),
    (87, 'B', N'I, II e IV.', 0, 18),
    (88, 'C', N'I, III e V.', 0, 18),
    (89, 'D', N'I, IV e V.', 1, 18),
    (90, 'E', N'II, III, IV e V.', 0, 18);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 19 | Requisitos e Qualidade de Software | correta: B
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (19, N'Após realizar uma análise de mercado em busca de soluções para aprimorar o seu negócio, uma empresa adquiriu um sistema de ERP (enterprise resource planning) contendo um conjunto de módulos que integra todos os departamentos existentes. Após um ano de utilização, houve uma mudança na legislação e, para atender as novas exigências, foi necessária uma manutenção no sistema ERP. Considerando essa situação hipotética, é correto afirmar que a empresa irá realizar uma manutenção
(ENADE 2008)',
     NULL, N'A manutenção adaptativa ajusta o software a mudanças no ambiente externo, como uma nova legislação. A corretiva corrige defeitos, a perfectiva (ou aperfeiçoadora) melhora funções e desempenho e a preventiva antecipa problemas futuros.', 1, 2, 2);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (91, 'A', N'corretiva.', 0, 19),
    (92, 'B', N'adaptativa.', 1, 19),
    (93, 'C', N'aperfeiçoadora.', 0, 19),
    (94, 'D', N'preventiva.', 0, 19),
    (95, 'E', N'perfectiva.', 0, 19);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 20 | Requisitos e Qualidade de Software | correta: D
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (20, N'A gerência de configuração de software, também chamada de gerência de mudança e configuração é o processo que mantém atualizadas as informações dos elementos de configuração.
A esse respeito, avalie as afirmativas.
I. A auditoria de software é um processo que consiste em verificar se a linha de base foi atendida ou se os requisitos do sistema foram atendidos.
II. A linha de base é o marco de referência, a partir do qual serão feitos os controles de mudança, e pode ser a versão 1.0 de um software.
III. Um item de configuração é um elemento unitário, que compõe aquele software e deve ser gerenciado.
É correto o que se afirma em
(ENADE 2014)',
     NULL, NULL, 1, 2, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (96, 'A', N'I, apenas.', 0, 20),
    (97, 'B', N'II, apenas.', 0, 20),
    (98, 'C', N'I e III, apenas.', 0, 20),
    (99, 'D', N'II e III, apenas.', 1, 20),
    (100, 'E', N'I, II e III.', 0, 20);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 21 | Análise e Modelagem de Sistemas | correta: D
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (21, N'Durante as eleições o eleitor deve comparecer à sua seção e zona, munido de um documento válido. Ao chegar ao local, apresenta o documento ao mesário, que verifica se o eleitor está apto a votar. Caso afirmativo, o mesário informa ao sistema o número do título de eleitor. O sistema valida o título e habilita o voto eletrônico para o eleitor. O eleitor informa os números de seus candidatos, podendo anular ou confirmar seu voto. Ao final do dia, termina o processo eleitoral da seção, o mesário finaliza o sistema, que gera os dados em tela ou em papel do resultado da urna, listando os votos para cada candidato. A totalização das urnas ocorre em um processo distinto em que o resultado final da eleição é apresentado à população.
Partindo dessa descrição, assinale a opção correta que corresponde à modelagem conceitual, utilizando diagrama de caso de uso com UML.
(ENADE 2008)',
     NULL, N'Informar o número do candidato é o fluxo principal, e anular ou confirmar o voto são comportamentos que o estendem, por isso o relacionamento <<extends>>. A verificação do documento é feita pelo mesário fora do sistema, a População não interage com o sistema da seção e <<implements>> não é um relacionamento entre casos de uso.', 1, 3, 2);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (101, 'A', N'Verificar o Documento do eleitor e Habilitar o Voto Eletrônico são casos de uso.', 0, 21),
    (102, 'B', N'No processo eleitoral da seção, os atores são: Eleitor, Mesário e População.', 0, 21),
    (103, 'C', N'O caso de uso Informar Título tem uma associação do tipo <<extends>> com o caso de uso Validar Título.', 0, 21),
    (104, 'D', N'O caso de uso Informar Número Candidato tem uma associação do tipo <<extends>> com os casos de uso Anular Voto e Confirmar Voto.', 1, 21),
    (105, 'E', N'Gerar Dados em Tela e Gerar Dados em Papel têm uma associação do tipo <<implements>> com o caso de uso Gerar Dados.', 0, 21);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 22 | Análise e Modelagem de Sistemas | correta: B
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (22, N'Avalie as afirmativas sobre a UML (Linguagem de Modelagem Unificada).
I. A UML é uma metodologia para o desenvolvimento de software orientado a objetos, uma vez que fornece um conjunto de representações gráficas e sua semântica para a modelagem de software.
II. O diagrama de casos de uso procura, por meio de uma linguagem simples, demonstrar o comportamento externo do sistema. Esse diagrama apresenta o sistema sob a perspectiva do usuário, e é dentre todos da UML, o mais abstrato, flexível e informal.
III. Um relacionamento de extensão de um caso de uso “A” para um caso de uso “B” significa que toda vez que “A” for executado ele incorporará o comportamento definido em “B”.
IV. Os diagramas de comportamento da UML demonstram como ocorrem as trocas de mensagens entre os objetos do sistema para se atingir um determinado objetivo.
É correto apenas o que se afirma em
(ENADE 2011)',
     NULL, NULL, 1, 3, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (106, 'A', N'I e II.', 0, 22),
    (107, 'B', N'II e IV.', 1, 22),
    (108, 'C', N'III e IV.', 0, 22),
    (109, 'D', N'I, II e III.', 0, 22),
    (110, 'E', N'II, III e IV.', 0, 22);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 23 | Análise e Modelagem de Sistemas | correta: B
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (23, N'Leia o texto a seguir.
Casos de uso podem ser organizados agrupando-os em pacotes do mesmo modo como são organizadas as classes. Também podem ser organizados pela especificação de relacionamentos de generalização, inclusão e extensão, existentes entre eles.
JACOBSON, I.; BOOCH, G.; RUMBAUGH, J. UML - Guia do Usuário. Campus, 2006 (com adaptações).
Considerando os relacionamentos existentes entre os casos de uso, avalie as afirmativas a seguir.
I. Para casos de uso, a generalização significa que o caso de uso filho herda o comportamento e o significado do caso de uso pai e no caso de uso filho deverá acrescentar ou sobrescrever o comportamento de seu pai.
II. Um relacionamento de inclusão entre casos de uso significa que o caso de uso base incorpora explicitamente o comportamento de outro caso de uso em uma localização especificada. O caso de uso base poderá permanecer isolado, mas, sob certas condições, seu comportamento poderá ser incluído pelo comportamento de outro caso de uso.
III. Um relacionamento estendido entre casos de uso significa que o caso de uso base incorpora implicitamente o comportamento de outro caso de uso em um local especificado indiretamente pelo caso de uso estendido. O caso de uso estendido nunca permanece isolado, mas é apenas instanciado como parte de alguma base maior que o estende.
IV. Um relacionamento estendido é utilizado para a modelagem de parte de um caso de uso que o usuário poderá considerar como um comportamento opcional do sistema e para a modelagem de um subfluxo separado, que é executado somente sob determinadas condições.
É correto apenas o que se afirma em
(ENADE 2014)',
     NULL, N'Na generalização, o caso de uso filho herda o comportamento do pai e pode acrescentar ou sobrescrever partes dele (I), e a extensão modela comportamentos opcionais ou executados apenas sob certas condições (IV). As afirmativas II e III trocam as definições: no «include» o caso base depende do caso incluído, enquanto no «extend» o caso base é completo e pode existir sozinho.', 1, 3, 2);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (111, 'A', N'I e II.', 0, 23),
    (112, 'B', N'I e IV.', 1, 23),
    (113, 'C', N'II e III.', 0, 23),
    (114, 'D', N'I, III e IV.', 0, 23),
    (115, 'E', N'II, III e IV.', 0, 23);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 24 | Análise e Modelagem de Sistemas | correta: E
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (24, N'A linguagem gráfica EPC/ARIS é utilizada para modelar processos de negócios. Para isso, utiliza diversos recursos para descrever, representar ou indicar, por exemplo, atividades, funções, processos e fluxos. Nesse contexto, avalie as afirmativas a seguir.
I. A ligação entre dois processos é indicada por um conector.
II. A descrição de um processo deve iniciar e terminar em um evento.
III. As funções, ou atividades, são representadas por um retângulo com bordas arredondadas.
É correto apenas o que se afirma em
(ENADE 2011)',
     NULL, NULL, 1, 3, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (116, 'A', N'I.', 0, 24),
    (117, 'B', N'II.', 0, 24),
    (118, 'C', N'III.', 0, 24),
    (119, 'D', N'I e II.', 0, 24),
    (120, 'E', N'II e III.', 1, 24);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 25 | Análise e Modelagem de Sistemas | correta: E
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (25, N'Leia o texto a seguir.
Os processos de negócio aumentam o entendimento de “o que precisa ser feito” e de “como precisa ser feito” dentro de uma organização.
BALDAM, R. 2009, 2. ed.
Sabendo disso, a fase da gestão de processos de negócio que evita a estagnação dos processos organizacionais é
(ENADE 2011)',
     NULL, N'No ciclo de gestão de processos de negócio (BPM), a otimização usa os resultados do monitoramento para melhorar os processos continuamente, evitando que eles fiquem estagnados. Planejamento, modelagem, gerenciamento e monitoração são etapas anteriores do ciclo.', 1, 3, 2);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (121, 'A', N'a fase de gerenciamento.', 0, 25),
    (122, 'B', N'a fase de planejamento.', 0, 25),
    (123, 'C', N'a fase de monitoração.', 0, 25),
    (124, 'D', N'a fase de modelagem.', 0, 25),
    (125, 'E', N'a fase de otimização.', 1, 25);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 26 | Análise e Modelagem de Sistemas | correta: B
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (26, N'O uso das técnicas de BPM (Business Process Management) visa à melhoria contínua do desempenho e à busca de resultados consistentes e alinhados aos objetivos estratégicos da empresa, sejam os processos automatizados ou não, e seus benefícios incluem o aumento de produtividade e a redução de erros, custos e desperdício.
Considerando as ferramentas de TI que contribuem para a eficácia da implementação de BPM nas organizações, avalie as afirmativas a seguir, em relação às oportunidades de melhoria nos processos de negócio.
I. A utilização de SOA (Service Oriented Architecture) contribui para a criação de sistemas corporativos que se adaptam aos requisitos de BPM.
II. Para a implementação de softwares que respeitem a sintaxe BPM, deve-se ter acesso ao código fonte para identificar o padrão de armazenamento de dados.
III. A ferramenta de BPM chamada BAM (Business Activity Monitoring) é utilizada para a modelagem de processos e o gerenciamento de dados.
IV. A utilização de BPMS (Business Process Management Systems) permite gerar aplicações e fornecer suporte à execução de operações de negócio.
É correto apenas o que se afirma em
(ENADE 2017)',
     NULL, NULL, 1, 3, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (126, 'A', N'I e II.', 0, 26),
    (127, 'B', N'I e IV.', 1, 26),
    (128, 'C', N'III e IV.', 0, 26),
    (129, 'D', N'I, II e III.', 0, 26),
    (130, 'E', N'II, III e IV.', 0, 26);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 27 | Análise e Modelagem de Sistemas | correta: A
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (27, N'Leia o texto a seguir.
A estrutura das organizações tradicionais apoia-se sobre vários pressupostos superados e que deram base ao surgimento de empresas voltadas para dentro, para suas próprias atividades, em estruturas hierárquicas pesadas e rígidas. Por outro lado, empresas mais inovadoras estão se organizando por processos, no esforço para aperfeiçoar seu desempenho empresarial, para terem maior eficiência, melhor adaptação à mudança e melhor integração de seus esforços.
GONÇALVES, J. E. L. Processo, que processo? Revista de Administração de empresas, v 40, n. 4, p. 8-19, 2000 (com adaptações).
Considerando o contexto apresentado, avalie as asserções e a relação proposta entre elas.
I. Uma gestão organizacional que valoriza mais o fluxo dos processos do que as estruturas departamentais tende a alcançar melhor desempenho e competitividade e, nesse contexto, os sistemas de informação representam recursos importantes para gerenciar e integrar processos e promover a melhoria do desempenho organizacional.
PORQUE
II. O desempenho das organizações está diretamente relacionado à eficiência e à eficácia de seus processos de negócio, sendo por meio deles que as organizações transformam suas estratégias em ações efetivas que implementam seus produtos e/ou serviços.
A respeito dessas asserções, assinale a opção correta.
(ENADE 2017)',
     NULL, N'As duas asserções são verdadeiras e a segunda justifica a primeira: como o desempenho da organização depende da eficiência e da eficácia de seus processos, uma gestão orientada a processos, apoiada por sistemas de informação que os integram, tende a alcançar melhor desempenho e competitividade.', 1, 3, 2);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (131, 'A', N'As asserções I e II são proposições verdadeiras, e a asserção II justifica a I.', 1, 27),
    (132, 'B', N'As asserções I e II são proposições verdadeiras, e a asserção II não justifica a I.', 0, 27),
    (133, 'C', N'A asserção I é uma proposição verdadeira, e a II é uma proposição falsa.', 0, 27),
    (134, 'D', N'A asserção I é uma proposição falsa, e a II é uma proposição verdadeira.', 0, 27),
    (135, 'E', N'As asserções I e II são proposições falsas.', 0, 27);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 28 | Análise e Modelagem de Sistemas | correta: B
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (28, N'Leia o texto a seguir.
Os mapas de navegação enfocam como as pessoas se movimentam por um site ou aplicação. Cada página do site ou local da aplicação é representado com uma caixa ou cabeçalho e todas as páginas que puderem ser acessadas a partir dela devem usá-la como referência. Uma técnica bastante utilizada é colocar todos os fluxos possíveis no mapa de navegação, já que isso destacará seções onde há necessidade de uma revisão no projeto da interface.
BENYON, D. Interação humano-computador. 2. ed. São Paulo: Pearson Prentice Hall, 2011.
Com relação ao tema, avalie as asserções e a relação proposta entre elas.
I. Os mapas de navegação são redesenhados muitas vezes no decorrer do ciclo de vida do projeto
PORQUE
II. A interface deve ser projetada para atender pessoas (capacidades e limitações motoras, neurológicas, cognitivas etc.), atividades que as pessoas querem realizar (questões temporais, com ou sem cooperação, se são críticas em termos de segurança etc.), contextos nos quais a interação acontece (ambiente físico, contexto social ou organizacional etc.), e ater-se às tecnologias empregadas (hardware, plataformas, normas, linguagens de programação, políticas de acesso em ambientes de trabalho e residencial etc.). As combinações desses elementos são muito diferentes, por exemplo, em um quiosque público, em um sistema de agenda compartilhada, em uma cabine de avião ou em um telefone celular.
Acerca dessas asserções, assinale a opção correta.
(ENADE 2011)',
     NULL, NULL, 1, 3, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (136, 'A', N'As duas asserções são proposições verdadeiras, e a segunda justifica a primeira.', 0, 28),
    (137, 'B', N'As duas asserções são proposições verdadeiras, e a segunda não justifica a primeira.', 1, 28),
    (138, 'C', N'A primeira asserção é uma proposição verdadeira, e a segunda é uma proposição falsa.', 0, 28),
    (139, 'D', N'A primeira asserção é uma proposição falsa, e a segunda é uma proposição verdadeira.', 0, 28),
    (140, 'E', N'As duas asserções são proposições falsas.', 0, 28);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 29 | Análise e Modelagem de Sistemas | correta: B
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (29, N'Leia o texto a seguir.
À medida que o projeto de uma interface do usuário evolui, quatro questões de projeto comuns quase sempre vêm à tona: tempo de resposta do sistema, recursos de ajuda do usuário, informações de tratamento de erros e atribuição de nomes a comandos.
PRESSMAN, R. S. Engenharia de software: uma abordagem profissional. 8. ed. São Paulo: AMGH, 2016 (com adaptações).
Nesse contexto, avalie as afirmativas abaixo, a respeito do projeto de interface e dos requisitos de software.
I. A análise de interfaces tem por objetivo identificar o perfil dos usuários e auxiliar na especificação do requisito não funcional de segurança pertinente ao perfil de acesso dos usuários.
II. O requisito não funcional de segurança descreve as características relacionadas a inteligibilidade, modo de usar ou operar o sistema.
III. O requisito não funcional de confiabilidade descreve as características relacionadas ao tempo de resposta para o processamento de dados ou informações.
IV. O requisito não funcional de usabilidade descreve diretrizes de acessibilidade para pessoas com deficiência.
É correto apenas o que se afirma em
(ENADE 2017)',
     NULL, N'A análise de interfaces identifica o perfil dos usuários e ajuda a definir requisitos de segurança ligados ao perfil de acesso (I), e a usabilidade inclui diretrizes de acessibilidade para pessoas com deficiência (IV). Inteligibilidade e facilidade de uso são características de usabilidade, não de segurança (II), e tempo de resposta é requisito de desempenho, não de confiabilidade (III).', 1, 3, 2);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (141, 'A', N'I e III.', 0, 29),
    (142, 'B', N'I e IV.', 1, 29),
    (143, 'C', N'II e IV.', 0, 29),
    (144, 'D', N'I, II e III.', 0, 29),
    (145, 'E', N'II, III e IV.', 0, 29);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 30 | Análise e Modelagem de Sistemas | correta: A
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (30, N'Leia o texto a seguir.
A engenharia de requisitos é uma área que inclui quatro subprocessos relacionados de alto nível. Esses subprocessos são: 1) avaliação se o sistema será útil para a empresa (estudo de viabilidade); 2) obtenção de requisitos (elicitação de requisitos); 3) conversão desses requisitos em alguma forma padrão (especificação); 4) verificação se os requisitos realmente definem o sistema que o cliente deseja (validação).
SOMMERVILLE, I. Engenharia de Software. São Paulo: Pearson Addison-Wesley, 2017 (com adaptações).
Uma equipe de Tecnologia da Informação (TI) de uma empresa de consultoria desenvolverá um software de Suporte Técnico para uma grande empresa fornecedora de equipamentos eletrônicos. O estudo de viabilidade do software já foi realizado e aprovado. A equipe de Tecnologia da Informação seguirá os três subprocessos seguintes de alto nível de engenharia de requisitos descritos no texto de Sommerville, ou seja, os subprocessos de elicitação de requisitos, especificação e validação. Para esses três subprocessos, quais são os artefatos que podem ser utilizados por essa equipe de Tecnologia da Informação?
(ENADE 2021)',
     NULL, NULL, 1, 3, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (146, 'A', N'Documento de entrevista com usuários; modelo de caso de uso para os requisitos funcionais; prototipação de telas.', 1, 30),
    (147, 'B', N'Documento de estudo de viabilidade; modelo de caso de uso para os requisitos funcionais; prototipação de telas.', 0, 30),
    (148, 'C', N'Matriz de rastreabilidade; modelo de caso de uso para os requisitos não-funcionais; prototipação de telas.', 0, 30),
    (149, 'D', N'Documento de entrevista com usuários; modelo de caso de uso para os requisitos não- funcionais; matriz de rastreabilidade.', 0, 30),
    (150, 'E', N'Documento de estudo de viabilidade; modelo de caso de uso para os requisitos funcionais; matriz de rastreabilidade.', 0, 30);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 31 | Algoritmos e Lógica de Programação | correta: A
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (31, N'A pilha é uma estrutura de dados que permite a inserção/remoção de itens dinamicamente seguindo a norma de último a entrar, primeiro a sair. Suponha que para uma estrutura de dados, tipo pilha, são definidos os comandos:
• PUSH (p, n): Empilha um número “n” em uma estrutura de dados do tipo pilha “p”;
• POP (p): Desempilha o elemento no topo da pilha.
Considere que, em uma estrutura de dados tipo pilha “p”, inicialmente vazia, sejam executados os seguintes comandos:
PUSH (p, 10)
PUSH (p, 5)
PUSH (p, 3)
PUSH (p, 40)
POP (p)
PUSH (p, 11)
PUSH (p, 4)
PUSH (p, 7)
POP (p)
POP (p)
Após a execução dos comandos, o elemento no topo da pilha “p” e a soma dos elementos armazenados na pilha “p” são, respectivamente,
(ENADE 2011)',
     NULL, N'Após os quatro primeiros PUSH, a pilha contém 10, 5, 3 e 40 (topo). O POP remove o 40; em seguida são empilhados 11, 4 e 7, e os dois POP finais removem o 7 e o 4. Restam 10, 5, 3 e 11: o topo é 11 e a soma é 29.', 1, 4, 3);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (151, 'A', N'11 e 29.', 1, 31),
    (152, 'B', N'11 e 80.', 0, 31),
    (153, 'C', N'4 e 80.', 0, 31),
    (154, 'D', N'7 e 29.', 0, 31),
    (155, 'E', N'7 e 40.', 0, 31);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 32 | Algoritmos e Lógica de Programação | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (32, N'Os termos da sequência de Fibonacci são definidos por:
Fibonacci(0) = 0
Fibonacci(1) = 1
Fibonacci(n) = Fibonacci(n-1) + Fibonacci(n-2)
Uma solução recursiva para o cálculo do i-ésimo termo da seqüência é dada pela função a seguir.
1 funcao fibonacci(inteiro longo n)
2 se((n=0) OU (n=1)) entao
3     retorne n
4 senao
5    retorne fibonacci(n-1) + fibonacci(n-2)
6 fim se
7 fim
Acerca da execução recursiva dessa função, assinale a opção incorreta.
(ENADE 2008)',
     NULL, NULL, 1, 4, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (156, 'A', N'À medida que o valor de n cresce, há um aumento no número de chamadas recursivas.', 0, 32),
    (157, 'B', N'Na linha 4, a ordem de execução é calcular o valor para fibonacci(n-1) e somente depois calcular o valor para fibonacci(n-2).', 0, 32),
    (158, 'C', N'O método recursivo é o mais eficiente para o cálculo do i-ésimo termo da seqüência de Fibonacci, pois realiza duas chamadas por passo da recursão, cada uma mais simples do que a chamada original.', 1, 32),
    (159, 'D', N'As condições de parada da recursão são: o valor de n é 0 ou o valor de n é 1.', 0, 32),
    (160, 'E', N'O uso da recursão para o problema da série de Fibonacci não é indicado, pois ele gera rapidamente uma explosão de chamadas do método.', 0, 32);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 33 | Algoritmos e Lógica de Programação | correta: B
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (33, N'José discutindo futebol com João declarou que se o time X venceu o último jogo, então o time Y é campeão. João depois de algumas horas descobriu que a declaração era falsa. Em consequência, teve certeza de que é verdade que
(ENADE 2014)',
     NULL, N'A frase "se X venceu, então Y é campeão" é uma condicional (p → q), que só é falsa quando p é verdadeira e q é falsa. Se a declaração é falsa, então o time X venceu o último jogo e o time Y não é campeão.', 1, 4, 3);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (161, 'A', N'o time X não venceu o último jogo e o time Y é campeão.', 0, 33),
    (162, 'B', N'o time X venceu o último jogo e o time Y não é campeão.', 1, 33),
    (163, 'C', N'ou o time X não venceu o último jogo ou o time Y é campeão.', 0, 33),
    (164, 'D', N'o time X não venceu o último jogo e o time Y não é campeão.', 0, 33),
    (165, 'E', N'se o time Y não é campeão, então o time X não venceu o último jogo.', 0, 33);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 34 | Algoritmos e Lógica de Programação | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (34, N'A álgebra booleana apresenta um operador unário ~, conhecido como NÃO, e os operadores binários * e +, conhecidos como E e OU, respectivamente. A tabela verdade é utilizada para validar uma fórmula composta de operadores da álgebra booleana. A seguir, é apresentada a tabela verdade para as proposições p, q e r diante da fórmula G, em que V representa uma proposição verdadeira e F uma proposição falsa.
p | q | r | G
V | V | V | V
V | V | F | F
V | F | V | F
V | F | F | F
F | V | V | V
F | V | F | V
F | F | V | V
F | F | F | V
Considerando as informações contidas na tabela acima e a ordem de precedência dos operadores booleanos, a fórmula G apresenta a mesma tabela verdade da proposição
(ENADE 2017)',
     NULL, NULL, 1, 4, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (166, 'A', N'p + ~q * r', 0, 34),
    (167, 'B', N'p + q * ~r', 0, 34),
    (168, 'C', N'~p + q * r', 1, 34),
    (169, 'D', N'~p + ~q * r', 0, 34),
    (170, 'E', N'~p + q * ~r', 0, 34);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 35 | Algoritmos e Lógica de Programação | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (35, N'Leia o texto a seguir.
Uma função é denominada recursiva quando ela é chamada novamente dentro de seu corpo. Implementações recursivas tendem a ser menos eficientes, porém facilitam a codificação e seu entendimento.
CELES, W.; CERQUEIRA, R.; RANGEL, J. L. Introdução a estrutura de dados. Rio de Janeiro, 2004 (com adaptações).
Considere a função recursiva f(), a qual foi escrita em linguagem C:
1 int f( int v[], int n){
2     if(n == 0)
3        return 0;
4     else{
5        int s;
6        s = f(v, n-1);
7        if( v[n-1] > 0 ) s = s + v[n-1];
8            return s;
9     }
10 }
Suponha que a função f() é acionada com os seguintes parâmetros de entrada:
f({2,-4,7,0,-1,4},6);
Nesse caso, o valor de retorno da função f() será
(ENADE 2014)',
     NULL, N'A função percorre o vetor de forma recursiva e soma apenas os elementos positivos. No vetor {2, -4, 7, 0, -1, 4}, os positivos são 2, 7 e 4, cuja soma é 13.', 1, 4, 3);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (171, 'A', N'8.', 0, 35),
    (172, 'B', N'10.', 0, 35),
    (173, 'C', N'13.', 1, 35),
    (174, 'D', N'15.', 0, 35),
    (175, 'E', N'18.', 0, 35);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 36 | Algoritmos e Lógica de Programação | correta: D
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (36, N'Leia o texto a seguir.
A ordenação por inserção funciona de modo semelhante à forma como algumas pessoas ordenam cartas de baralho. Inicia-se com a mão esquerda vazia e as cartas empilhadas na mesa. Remove-se da pilha uma carta de cada vez, inserindo-a na posição correta na mão esquerda. Para se identificar a posição correta de uma carta, deve-se compará-la com as cartas presentes na mão esquerda, no sentido da direita para a esquerda. Em todos os momentos, as cartas na mão esquerda estão ordenadas, tendo sido obtidas no topo da pilha da mesa.
CORMEN, T. H. et al. Introduction to algorithms. 3. ed. Cambridge: The MIT Press, 2009 (com adaptações).
Um programador implementou um algoritmo de ordenação semelhante à forma de ordenação de cartas descrita no texto. Ao realizar um teste com um vetor de nove posições (vetor[1..9]), verificou que o algoritmo não funcionava corretamente.
01 para i <- 2 até 9 faça
02     valor <- vetor[i]
03     j <- i – 1
04     enquanto ((j >= 1) e (valor < vetor[j])) faça
05          vetor[i] <- vetor[j]
06            j <- j – 1
07            se (j = 0) então
08                interrompa
09            fim se
10     fim enquanto
11     vetor[j + 1] <- valor
12 fim para
Com base nessas informações, assinale a opção em que se apresentam a linha e o respectivo comando a ser substituído, para que o algoritmo ordene corretamente um vetor de inteiros de forma crescente.
(ENADE 2017)',
     NULL, NULL, 1, 4, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (176, 'A', N'Linha 01; para i <- 1 até 9 faça', 0, 36),
    (177, 'B', N'Linha 03; j <- i', 0, 36),
    (178, 'C', N'Linha 04; enquanto ((j >= 1) ou (valor < vetor[j])) faça', 0, 36),
    (179, 'D', N'Linha 05; vetor[j+1] <- vetor[j]', 1, 36),
    (180, 'E', N'Linha 11; vetor[j] <- valor', 0, 36);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 37 | Algoritmos e Lógica de Programação | correta: A
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (37, N'Sistemas especialistas são aqueles que fazem uso intensivo do conhecimento especializado para resolver problemas de modo semelhante àquele de um especialista humano. Uma das diversas utilizações de sistemas especialistas é na elaboração de diagnósticos destinados a inferir sobre o mau funcionamento de um sistema, a partir de observações, de modo a prescrever soluções para a anomalia detectada. A medicina, atualmente, já se beneficia do uso de sistemas especialistas em conjunto com técnicas de inferência estatística. Uma das formas de trabalhar com sistemas especialistas é por meio do uso de “regras de produção”, que são instruções do tipo “se - então”. As regras de produção aplicam-se à memória de trabalho, que contém dados, e se tiverem êxito, contribuirão com alguma nova informação para a memória.
A metodologia de trabalho de um médico do serviço de emergência inclui a utilização de um sistema especialista para diagnóstico de dengue, para o qual se encontra a seguinte regra de produção.
Se ((temperatura > 38º) e (dores musculares intensas))
então (quadro de dengue, com 70% de chance);
senão (quadro de resfriado comum com 30% de chance);
Levando em conta a regra de produção do sistema especialista utilizado pelo médico, é correto afirmar que
(ENADE 2021)',
     NULL, N'A regra de produção atribui 70% de chance de dengue a quem apresenta os dois sintomas, ou seja, probabilidade 0,7 para cada pessoa nessa condição. Uma probabilidade não garante quantidades exatas (C), e a regra não permite concluir o que dizem as demais alternativas.', 1, 4, 3);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (181, 'A', N'cada pessoa que procura atendimento apresentando ambos os sintomas, possui a probabilidade “0,7” de estar contaminada pela dengue.', 1, 37),
    (182, 'B', N'se 50 pessoas procurarem o atendimento apresentando pelo menos um dos dois sintomas, provavelmente 15 pessoas estarão com resfriado comum.', 0, 37),
    (183, 'C', N'se 100 pessoas procurarem o atendimento apresentando ambos os sintomas, a quantidade de pessoas com dengue será igual a 70.', 0, 37),
    (184, 'D', N'cada pessoa que chega apresentando um dos dois sintomas estará com resfriado comum.', 0, 37),
    (185, 'E', N'a cada 100 pessoas que procuram atendimento, no máximo 30 estarão doentes.', 0, 37);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 38 | Algoritmos e Lógica de Programação | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (38, N'O sistema de numeração, inserido na arquitetura e no funcionamento dos computadores, sempre foi muito utilizado na área computacional e pode ser escrito em diferentes bases numéricas, como: binária, octal, decimal ou hexadecimal. Durante a execução dos programas, a Unidade Central de Processamento (CPU) trabalha com os dados e instruções convertidos para dois estados distintos: 0 (zero) e 1 (um), que podem ser entendidos como “com energia” e “sem energia”, a chamada linguagem binária ou 0 e 1. Durante o processamento dos dados, as instruções e os dados são armazenados no formato binário na memória principal do computador. A CPU, trabalhando com dados no formato binário, aumenta sua capacidade e velocidade no processamento dos dados.
Alguns exemplos dos dados, representados em diferentes bases, são utilizados nos sistemas computacionais diariamente: o endereçamento IP dos computadores em uma rede são configurados na base decimal pontuada se for o IPv4, exemplo: 192.168.70.10; o número do endereço MAC-Address da placa de rede do computador é hexadecimal, exemplo: 00-15-5D-01-F2-00; já o sistema octal foi muito utilizado na computação, como uma alternativa mais compacta do sistema binário, na programação em linguagem de máquina.
Os profissionais que atuam na área da Tecnologia da Informação (TI) precisam constantemente interpretar, fazer cálculos ou converter dados entre as bases binária, decimal, octal ou hexadecimal. A tabela abaixo mostra os dígitos, a notação e alguns exemplos nas bases numéricas: 2, 8, 10 e 16.
Sistema na Base | Dígitos utilizados | Notação e exemplos
Binário – base 2 | 0, 1 | (1001)2; (11011)2
Octal – base 8 | 0, 1, 2, 3, 4, 5, 6, 7 | (27)8 ; (35)8; (16)8
Decimal – base 10 | 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 | (45)10; (120)10
Hexadecimal – base 16 | 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, A, B, C, D, E, F | (1A)16 ; (48)16 ; (1E2)16
STALLINGS, W. Arquitetura e Organização de Computadores. São Paulo: Pearson Education do Brasil, 2017 (com adaptações).
Fazendo a conversão da soma de: (1001)2 + (15)8 + (FF)16 , qual é o número equivalente na base decimal?
(ENADE 2021)',
     NULL, NULL, 1, 4, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (186, 'A', N'(188)10', 0, 38),
    (187, 'B', N'(215)10', 0, 38),
    (188, 'C', N'(277)10', 1, 38),
    (189, 'D', N'(316)10', 0, 38),
    (190, 'E', N'(345)10', 0, 38);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 39 | Algoritmos e Lógica de Programação | correta: D
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (39, N'Leia o texto a seguir.
Um algoritmo é qualquer procedimento computacional bem definido que toma algum valor ou conjunto de valores como entrada e produz algum valor ou conjunto de valores como saída.
CORMEN, T. H. et al. Algoritmos: teoria e prática. Rio de Janeiro: Elsevier, 2002 (com adaptações).
Considere o algoritmo abaixo.
Algoritmo Calcular
    var
        mat [1..3][1..5] de inteiro =
            {{1, 2, -1, 2, 3}, {1, -3, 4, 2, 0}, {-3, 5, 2, 3, 4}}
        sl[1..3] de inteiro = {0, 0, 0}
        x, i, j : inteiro
        x <- 0
    início
        para i <- 1 até 3 faça
            para j <- 1 até 5 faça
                sl[i] <- sl[i] + mat[i][j]
            fimpara
            x <- x + sl[i]
        fimpara
        imprima x
    fim
No fim da execução do código apresentado, será exibido o valor
(ENADE 2021)',
     NULL, N'O algoritmo soma os elementos de cada linha da matriz em sl[i] e acumula essas somas em x. As linhas somam 7, 4 e 11, portanto o valor impresso é 7 + 4 + 11 = 22.', 1, 4, 3);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (191, 'A', N'4.', 0, 39),
    (192, 'B', N'7.', 0, 39),
    (193, 'C', N'11.', 0, 39),
    (194, 'D', N'22.', 1, 39),
    (195, 'E', N'30.', 0, 39);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 40 | Algoritmos e Lógica de Programação | correta: B
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (40, N'Com relação ao valor lógico, avalie as afirmativas.
I. ¬(p ∧ ¬q)
II. p → (q → p)
III. (p ∨ ¬q) → ¬p
IV. (p ∧ q) ∨ (¬p ∧ ¬q)
É tautologia apenas o que se afirma em
(ENADE 2011)',
     NULL, NULL, 1, 4, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (196, 'A', N'I.', 0, 40),
    (197, 'B', N'II.', 1, 40),
    (198, 'C', N'I e III.', 0, 40),
    (199, 'D', N'II e IV.', 0, 40),
    (200, 'E', N'III e IV.', 0, 40);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 41 | Programação Orientada a Objetos e Estruturas de Dados | correta: B
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (41, N'Uma pizzaria fez uma ampliação de suas instalações e o gerente aproveitou para melhorar o sistema informatizado, que era limitado e não atendia a todas as funções necessárias. O gerente, então, contratou uma empresa para ampliar o software. No desenvolvimento do novo sistema, a empresa aproveitou partes do sistema antigo e estendeu os componentes de maneira a usar código validado, acrescentando as novas funções solicitadas. Que conceito de orientação a objetos está descrito na situação hipotética acima?
(ENADE 2008)',
     NULL, N'Aproveitar partes do sistema antigo e estender componentes já validados, acrescentando novas funções, é a ideia da herança: novas classes reaproveitam o código de classes existentes e o especializam.', 1, 5, 3);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (201, 'A', N'Sobrecarga.', 0, 41),
    (202, 'B', N'Herança.', 1, 41),
    (203, 'C', N'Sobreposição.', 0, 41),
    (204, 'D', N'Abstração.', 0, 41),
    (205, 'E', N'Mensagem.', 0, 41);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 42 | Programação Orientada a Objetos e Estruturas de Dados | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (42, N'Com relação a conceitos de orientação a objetos, julgue os itens.
I. As variáveis ou métodos declarados com modificador de acesso private só são acessíveis a métodos da classe em que são declarados.
II. Uma classe deve possuir uma única declaração de método construtor.
III. Uma instância de uma classe abstrata herda atributos e métodos de sua superclasse direta.
IV. O polimorfismo permite substituir a lógica condicional múltipla (lógica switch ou faça caso).
São corretos apenas os itens
(ENADE 2008)',
     NULL, NULL, 1, 5, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (206, 'A', N'I e II.', 0, 42),
    (207, 'B', N'I e III.', 0, 42),
    (208, 'C', N'I e IV.', 1, 42),
    (209, 'D', N'II e III.', 0, 42),
    (210, 'E', N'II e IV.', 0, 42);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 43 | Programação Orientada a Objetos e Estruturas de Dados | correta: B
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (43, N'Eclipse, Netbeans, Jdeveloper são exemplos de ambientes integrados de desenvolvimento, chamados de IDE, que têm por objetivo facilitar o desenvolvimento de softwares, provendo maior produtividade e gerenciamento de projetos. A especificação JavaBeans foi criada para ser um padrão de desenvolvimento de componentes que possam ser facilmente usados por outros desenvolvedores em diferentes IDE.
Com relação ao tema, avalie as asserções e a relação proposta entre elas.
I. Seja para o Netbeans ou para o Eclipse, é possível adquirir componentes de terceiros que facilitem a implementação do seu projeto
PORQUE
II. Como o código desses componentes está em linguagem intermediária, ou seja, independente da arquitetura de um computador real, só é necessário que a máquina virtual esteja instalada no computador onde o aplicativo será executado e a máquina virtual será a responsável pela interpretação do código para a linguagem de máquina do computador em execução.
Acerca dessas asserções, assinale a alternativa correta.
(ENADE 2011)',
     NULL, N'As duas asserções são verdadeiras: há componentes de terceiros (JavaBeans) para IDEs como NetBeans e Eclipse, e o bytecode Java é executado pela máquina virtual em qualquer arquitetura. Mas o que permite usar esses componentes em diferentes IDEs é a especificação JavaBeans, e não a portabilidade do bytecode; por isso a segunda asserção não justifica a primeira.', 1, 5, 3);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (211, 'A', N'As duas asserções são proposições verdadeiras, e a segunda justifica a primeira.', 0, 43),
    (212, 'B', N'As duas asserções são proposições verdadeiras, e a segunda não justifica a primeira.', 1, 43),
    (213, 'C', N'A primeira asserção é uma proposição verdadeira, e a segunda é uma proposição falsa.', 0, 43),
    (214, 'D', N'A primeira asserção é uma proposição falsa, e a segunda é uma proposição verdadeira.', 0, 43),
    (215, 'E', N'As duas asserções são proposições falsas.', 0, 43);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 44 | Programação Orientada a Objetos e Estruturas de Dados | correta: A
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (44, N'Leia o texto a seguir.
Padrões de criação (creational patterns) abstraem a forma como objetos são criados, tornando o sistema independente de como os objetos são criados, compostos e representados. Um padrão de criação de classe usa a herança para variar a classe que é instanciada, enquanto que um padrão de criação de objeto delegará a instanciação para outro objeto. Há dois temas recorrentes nesses padrões. Primeiro, todos encapsulam conhecimento sobre quais classes concretas são usadas pelo sistema. Segundo, ocultam o modo como essas classes são criadas e montadas. Tudo que o sistema sabe no geral sobre os objetos é que suas classes são definidas por classes abstratas. Os padrões de criação são classificados em Abstract Factory, Builder, Factory Method, Prototype e Singleton.
GAMMA, E. et al. Design Patterns: Elements of Reusable Object-Oriented Software. Reading, MA: Addison-Wesley, 1994 (com adaptações).
O padrão Abstract Factory é usado quando
(ENADE 2011)',
     NULL, NULL, 1, 5, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (216, 'A', N'o sistema deve ser independente da maneira como seus produtos são criados, relacionados e representados.', 1, 44),
    (217, 'B', N'o algoritmo de criação de um objeto deve ser independente das suas partes e da maneira como ele é montado.', 0, 44),
    (218, 'C', N'houver uma única instância de uma classe e esta for acessada a partir de um ponto de acesso conhecido.', 0, 44),
    (219, 'D', N'classes delegam responsabilidade a alguma das subclasses e deseja-se localizar qual é a subclasse acessada.', 0, 44),
    (220, 'E', N'as classes utilizadas para instanciação são especificadas em tempo de execução ou carregadas dinamicamente.', 0, 44);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 45 | Programação Orientada a Objetos e Estruturas de Dados | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (45, N'O paradigma de programação orientado a objetos tem sido largamente utilizado no desenvolvimento de sistemas.
Considerando o conceito de herança, avalie as afirmativas.
I. Herança é uma propriedade que facilita a implementação de reuso.
II. Quando uma subclasse é criada, ela herda todas as características da superclasse, não podendo possuir propriedades e métodos próprios.
III. Herança múltipla é uma propriedade na qual uma superclasse possui diversas subclasses.
IV. Extensão é uma das formas de se implementar herança.
É correto apenas o que se afirma em
(ENADE 2011)',
     NULL, N'A herança favorece o reuso (I) e é implementada por extensão de classes, como o extends do Java (IV). A subclasse pode ter atributos e métodos próprios (II é falsa), e herança múltipla ocorre quando uma subclasse herda de várias superclasses, não o contrário (III é falsa).', 1, 5, 3);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (221, 'A', N'I.', 0, 45),
    (222, 'B', N'III.', 0, 45),
    (223, 'C', N'I e IV.', 1, 45),
    (224, 'D', N'II e III.', 0, 45),
    (225, 'E', N'II e IV.', 0, 45);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 46 | Programação Orientada a Objetos e Estruturas de Dados | correta: A
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (46, N'A área de desenvolvimento de software está se tornando cada vez mais complexa. Para lidar com essa realidade, os desenvolvedores contam com linguagens de programação baseadas no paradigma de orientação a objetos, cujos pilares são abstração, encapsulamento, herança e polimorfismo. No código a seguir, observa-se a implementação de classes relacionadas.
public abstract class Impressora {
    String nome;
    Impressora() {}
    Impressora(String n) {
        this.nome = n;
    }
    public void imprimir() {}
}
public class Laser extends Impressora {
    public Laser() {}
    public void imprimir() {
        System.out.println("Imprimindo na Laser");
    }
}
class Matricial extends Impressora {
    public Matricial () {}
    public void imprimir() {
        System.out.println("Imprimindo na Matricial");
    }
}
public class JatoDeTinta extends Impressora {
    public JatoDeTinta() {}
    public void imprimir(){
        System.out.println("Imprimindo na Jato de tinta");
    }
}
public class Main {
    public static void main(String args[]) {
        Impressora imp[] = new Impressora[3];
        imp[0] = new Laser();
        imp[1] = new JatoDeTinta();
        imp[2] = new Matricial();
        for(int i = imp.length - 1; i >= 0; i--){
            imp[i].imprimir();
        }
    }
}
Com base nas informações do texto e no código apresentado, avalie as afirmativas a seguir.
I. A execução do código, via classe Main, resulta na seguinte saída:
   Imprimindo na Laser
   Imprimindo na Matricial
   Imprimindo na Jato de tinta
II. O código faz uso da técnica denominada polimorfismo.
III. O código não será compilado, pois o vetor imp foi instanciado por meio da classe abstrata Impressora.
É correto o que se afirma em
(ENADE 2017)',
     NULL, NULL, 1, 5, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (226, 'A', N'II, apenas.', 1, 46),
    (227, 'B', N'III, apenas.', 0, 46),
    (228, 'C', N'I e II, apenas.', 0, 46),
    (229, 'D', N'I e III, apenas.', 0, 46),
    (230, 'E', N'I, II e III.', 0, 46);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 47 | Programação Orientada a Objetos e Estruturas de Dados | correta: A
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (47, N'Para fins estatísticos, uma empresa precisa armazenar os trajetos que seus representantes comerciais percorrem entre pontos de venda. É importante que para cada local visitado sejam armazenadas, além da informação do próprio local, o local de origem do representante (ponto de venda anterior), o local de destino (ponto de venda posterior), as distâncias percorridas e os tempos de viagem. Esse procedimento permite que estes trajetos possam ser analisados, de forma rápida, do local de origem ao local de destino, bem como no sentido inverso, do local de destino (final do trajeto) ao local de origem (início do trajeto).
O analista responsável pelo sistema, que utilizará os dados armazenados e produzirá os relatórios estatísticos, projetou o seguinte esboço de uma classe que representa um ponto de venda:
public class Local {
     private String nome_estabelecimento;
     private String endereco;
     private Local origem;
     private Local destino;
     private float distancia_origem;
     private float tempo_origem;
}
A respeito do esboço da classe, avalie as afirmativas.
I. O esboço acima representa uma lista duplamente encadeada.
II. A utilização de um nó de uma estrutura de dados do tipo árvore de busca multivias de grau três seria a solução ideal para o problema porque providenciaria a economia de recursos de memória e de disco.
III. A utilização de uma árvore de pesquisa binária para a solução do problema é normal, desde que o atributo de ordenação da árvore seja distancia_origem.
É correto o que se afirma em
(ENADE 2014)',
     NULL, N'Cada objeto Local guarda uma referência para o local anterior (origem) e outra para o próximo (destino), o que permite percorrer o trajeto nos dois sentidos: é a estrutura de uma lista duplamente encadeada. Árvores não representam a sequência do trajeto, por isso II e III estão incorretas.', 1, 5, 3);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (231, 'A', N'I, apenas.', 1, 47),
    (232, 'B', N'II, apenas.', 0, 47),
    (233, 'C', N'I e III, apenas.', 0, 47),
    (234, 'D', N'II e III, apenas.', 0, 47),
    (235, 'E', N'I, II e III.', 0, 47);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 48 | Programação Orientada a Objetos e Estruturas de Dados | correta: A
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (48, N'O fragmento de código C++, a seguir, contém a implementação da inserção de um nó em uma árvore binária.
...
struct noArvore {
     int dado;
     noArvore *esquerda;
     noArvore *direita;
};
noArvore *insere(noArvore *arvore, int valor) {
     if (arvore == NULL) {
           arvore = new noArvore;
           arvore->esquerda = NULL;
           arvore->direita = NULL;
           arvore->dado = valor;
     } else if(valor < arvore->dado) {
           arvore->esquerda = insere (arvore->esquerda, valor);
     } else {
           arvore->direita = insere (arvore->direita, valor);
     }
     return(arvore);
}
...
Com base no exposto, assinale a opção que apresenta o fragmento de programa para imprimir os valores contidos nos nós, percorrendo a árvore em pré-ordem.
(ENADE 2017)',
     NULL, NULL, 1, 5, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (236, 'A', N'void preorder(noArvore *raiz) {
    if (raiz != NULL) {
            cout << raiz->dado << " ";
            preorder(raiz->esquerda);
            preorder(raiz->direita);
    }
}', 1, 48),
    (237, 'B', N'void preorder(noArvore *raiz) {
 if (raiz != NULL) {
     preorder(raiz->direita);
     preorder(raiz->esquerda);
     cout << raiz->dado << " ";
 }
}', 0, 48),
    (238, 'C', N'void preorder(noArvore *raiz) {
    if (raiz != NULL) {
        preorder(raiz->esquerda);
        cout << raiz->dado << " ";
        preorder(raiz->direita);
    }
}', 0, 48),
    (239, 'D', N'void preorder(noArvore *raiz) {
     if (raiz != NULL) {
             preorder(raiz->esquerda);
             preorder(raiz->direita);
             cout << raiz->dado << " ";
     }
}', 0, 48),
    (240, 'E', N'void preorder(noArvore *raiz) {
    if (raiz != NULL) {
        preorder(raiz->direita);
        cout << raiz->dado << " ";
        preorder(raiz->esquerda);
    }
}', 0, 48);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 49 | Programação Orientada a Objetos e Estruturas de Dados | correta: E
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (49, N'O código Java, a seguir, contém a implementação de uma Pilha utilizando a estratégia encadeada.
public class No{
     public int dado;
     public No prox;
     public No(int dado){
          this.dado = dado;
     }
}
public class PilhaEncadeada{
     private No topo;
     public int pop(){
          if (topo == null)
                return -1;
          No lixo = topo;
          topo = topo.prox;
          lixo.prox = null;
          return lixo.dado;
     }
     public int top(){
          if (topo == null)
                return -1;
          return (topo.dado);
     }
}
Com base no exposto, assinale a opção que possui a implementação correta do método push para essa Pilha.
(ENADE 2021)',
     NULL, N'Na pilha encadeada, o novo nó deve apontar para o topo atual (novo.prox = topo) e então passar a ser o topo (topo = novo). As outras opções perdem os elementos já empilhados, sobrescrevem dados ou causam erro de referência nula.', 1, 5, 3);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (241, 'A', N'public void push(int elemento){
       No novo = new No(elemento);
       if(topo == null)
              topo = novo;
       else
              topo.prox = novo;
}', 0, 49),
    (242, 'B', N'public void push(int elemento) {
       No novo = new No(elemento);
       topo = novo;
}', 0, 49),
    (243, 'C', N'public void push(int elemento) {
       topo.dado = elemento;
}', 0, 49),
    (244, 'D', N'public void push(int elemento) {
           No novo = new No(elemento);
           while(topo != null)
                 topo = topo.prox;
           topo.prox = novo;
}', 0, 49),
    (245, 'E', N'public void push(int elemento) {
       No novo = new No(elemento);
       novo.prox = topo;
       topo = novo;
}', 1, 49);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 50 | Programação Orientada a Objetos e Estruturas de Dados | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (50, N'Considere que as variáveis pilha e fila correspondem, respectivamente, às estruturas de dados do tipo Pilha e Fila. Para testar as duas estruturas, um programador realizou a série de operações a seguir.
Pilha pilha = new Pilha();
Fila fila = new Fila();
pilha.push(‘A’);
pilha.push(‘B’);
pilha.push(‘C’);
fila.enqueue(pilha.top());
fila.enqueue(pilha.top());
fila.enqueue(‘D’);
pilha.push(fila.dequeue());
fila.enqueue(fila.dequeue());
fila.enqueue(pilha.pop());
pilha.push(‘E’);
fila.enqueue(‘E’);
pilha.pop();
Após essas operações, ao imprimir o conteúdo de pilha e fila, respectivamente, serão exibidos os seguintes resultados:
(ENADE 2021)',
     NULL, NULL, 1, 5, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (246, 'A', N'pilha: topo → C → A → E.
fila: início → D → A → A → E.', 0, 50),
    (247, 'B', N'pilha: topo →A.
fila: início → D → B → C → E.', 0, 50),
    (248, 'C', N'pilha: topo → C → B → A.
fila: início → D → C → C → E.', 1, 50),
    (249, 'D', N'pilha: topo → B → A.
fila: início → D → B → C → E.', 0, 50),
    (250, 'E', N'pilha: topo → C → B → A.
fila: início → D → B → C → E.', 0, 50);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 51 | Banco de Dados | correta: E
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (51, N'Uma livraria usa um sistema informatizado para realizar vendas pela Internet. Optou-se por um sistema gerenciador de banco de dados, no qual aplicaram-se regras de corretude e integridade. Cada cliente se cadastra, faz login no sistema e escolhe títulos. Os livros são colocados em um carrinho de compras até que o cliente confirme ou descarte o pedido. As tabelas do sistema são: clientes, livros, carrinho, vendas. Dois clientes acessam o sítio no mesmo horário e escolhem alguns livros. O estado do sistema nesse instante é o seguinte:
Cliente | Livros no carrinho
Benjamim | Cálculo I; Estatística básica; Inglês intermediário
Alice | Estatística básica; História geral
A livraria possui um único exemplar do livro Estatística básica. O computador usado por Alice sofre uma pane de energia antes que ela confirme o pedido. No mesmo instante também ocorre uma pane de energia no computador da loja. Em seguida, o computador de Alice e o computador da loja voltam a ter energia e a funcionar. Ela volta ao sistema e retoma seu carrinho de compras intacto.
Com relação a essa situação, julgue os itens seguintes.
I. A consistência de uma tabela do banco de dados foi violada temporariamente, para disponibilizar dois exemplares do livro Estatística básica.
II. Como o pedido de Alice continuou válido apesar da interrupção, o sistema gerenciador de banco de dados não emprega atomicidade.
III. Todas as transações devem ser fechadas depois do retorno da energia, para que os clientes possam recuperar seus carrinhos de compras.
Assinale a opção correta.
(ENADE 2008)',
     NULL, N'Nenhum item está certo. A consistência do banco não foi violada: o livro estava apenas no carrinho, e nenhuma venda foi confirmada (I). O carrinho ter sido mantido não indica falta de atomicidade, que garante que cada transação seja executada por inteiro ou desfeita (II). Após a falha, transações não confirmadas são desfeitas pelo SGBD, sem necessidade de fechar todas as transações para recuperar os carrinhos (III).', 1, 6, 4);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (251, 'A', N'Apenas um item está certo.', 0, 51),
    (252, 'B', N'Apenas os itens I e II estão certos.', 0, 51),
    (253, 'C', N'Apenas os itens I e III estão certos.', 0, 51),
    (254, 'D', N'Apenas os itens II e III estão certos.', 0, 51),
    (255, 'E', N'Nenhum item está certo.', 1, 51);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 52 | Banco de Dados | correta: A
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (52, N'O modelo de entidade-relacionamento a seguir representa, de forma sucinta, uma solução para persistência de dados de uma biblioteca (PK = chave primária; FK = chave estrangeira).
Livro (liv_cod PK, titulo, autor_cod FK → Autor)
Autor (aut_cod PK, nome)
Aluno (matricula PK, nome, data_nasc)
Emprestimo (emp_cod PK, data_emp, data_dev, aluno_mat FK → Aluno, livro_cod FK → Livro)
Considerando que um livro está emprestado quando possuir um registro vinculado a ele na tabela "Emprestimo", e essa tupla não possuir valor na coluna "data_dev", o comando SQL que deve ser utilizado para listar os títulos dos livros disponíveis para empréstimo é:
(ENADE 2014)',
     NULL, NULL, 1, 6, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (256, 'A', N'select titulo from livro
except
select l.titulo from emprestimo e inner join livro l
on e.livro_cod = l.liv_cod where e.data_dev is null', 1, 52),
    (257, 'B', N'select titulo from livro
union
select l.titulo from emprestimo e inner join livro l
on e.livro_cod = l.liv_cod where e.data_dev is null', 0, 52),
    (258, 'C', N'select titulo from livro
except
select l.titulo from emprestimo e inner join livro l
on e.livro_cod = l.liv_cod where e.data_dev is not null', 0, 52),
    (259, 'D', N'select titulo from livro
union select l.titulo from emprestimo e left join livro l
on e.livro_cod = l.liv_cod where e.data_dev is null', 0, 52),
    (260, 'E', N'select titulo from livro
except
select l.titulo from emprestimo e right join livro l
on e.livro_cod = l.liv_cod where e.data_dev is not null', 0, 52);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 53 | Banco de Dados | correta: A
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (53, N'O modelo lógico de dados fornece uma visão da maneira como os dados serão armazenados. O modelo a seguir representa um ambiente observado em um escritório contábil (PK = chave primária; FK = chave estrangeira).
Contribuinte (CPF PK, nome, endereco, telefone)
Declaracao_Imposto_de_Renda (Contribuinte_CPF PK/FK → Contribuinte, ano_exercicio PK, ano_base, valor, restituicao_ou_pagamento)
Malha_Fina (Identificador PK, ano_exercicio)
Contribuinte_Malha_Fina (Malha_Fina_Identificador PK/FK → Malha_Fina, Contribuinte_CPF PK/FK → Contribuinte, valor, quantidade_de_prestacoes)
Em relação ao modelo, avalie as afirmativas.
I. A entidade Declaração Imposto de Renda é uma entidade fraca.
II. O relacionamento entre Contribuinte e Malha Fina é do tipo N:M (muitos para muitos).
III. O atributo CPF da entidade Contribuinte tem a função de chave estrangeira na entidade Declaração Imposto de Renda e no relacionamento Contribuinte_MalhaFina.
IV. A entidade Malha Fina não possui chave primária, somente chave estrangeira.
V. O relacionamento Contribuinte_MalhaFina é um relacionamento ternário.
É correto apenas o que se afirma em
(ENADE 2014)',
     NULL, N'A Declaração de Imposto de Renda depende do Contribuinte para ser identificada (sua chave inclui o CPF), por isso é uma entidade fraca (I). A tabela associativa Contribuinte_Malha_Fina liga contribuintes e malhas finas, o que caracteriza um relacionamento N:M (II), e o CPF aparece como chave estrangeira na declaração e na associativa (III). Malha Fina tem chave primária (Identificador) e o relacionamento é binário, não ternário.', 1, 6, 4);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (261, 'A', N'I, II e III.', 1, 53),
    (262, 'B', N'I, II e IV.', 0, 53),
    (263, 'C', N'I, IV e V.', 0, 53),
    (264, 'D', N'II, III e V.', 0, 53),
    (265, 'E', N'III, IV e V.', 0, 53);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 54 | Banco de Dados | correta: E
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (54, N'Leia o texto a seguir.
O modelo relacional representa o banco de dados como uma coleção de relações (tabelas). Na terminologia formal do modelo relacional, uma linha é chamada de "tupla", o título da coluna é denominado "atributo" e a tabela é chamada de "relação". O tipo de dado que descreve os tipos de valores que podem aparecer em cada coluna é denominado "domínio". Um banco de dados relacional pode impor vários tipos de restrições nos dados armazenados.
ELMASRI, R. NAVATHE, S. B. Sistema de Banco de Dados: Fundamentos e Aplicações. Rio de Janeiro: LTC, 2002.
Restrições que permitem controlar situações como, por exemplo, "o salário de um empregado não deve exceder o salário do supervisor do empregado" e utilizam mecanismos chamados triggers (gatilhos) na sua implementação, são do tipo
(ENADE 2014)',
     NULL, NULL, 1, 6, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (266, 'A', N'restrições de domínio.', 0, 54),
    (267, 'B', N'restrições de unicidade.', 0, 54),
    (268, 'C', N'restrições de integridade referencial.', 0, 54),
    (269, 'D', N'restrições de integridade da entidade.', 0, 54),
    (270, 'E', N'restrições de integridade semântica.', 1, 54);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 55 | Banco de Dados | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (55, N'Um cliente solicitou a uma empresa a criação de um banco de dados para armazenar o resultado de uma eleição presidencial, com dados sobre os partidos políticos, os candidatos e a votação obtida por cada candidato em cada unidade da federação. A partir do diagrama entidade-relacionamento elaborado, foram criadas as tabelas a seguir.
Partido (numero, nome, sigla)
Candidato (numero, nome) — cada candidato é filiado a um partido, e o número do candidato é igual ao número do partido.
Votacao (partido, votos, estado) — um registro por partido em cada estado.
Os dados registrados a partir do resultado dessa eleição são:
Partido: (91, Partido 1, P1), (92, Partido 2, P2), (93, Partido 3, P3), (94, Partido 4, P4), (95, Partido 5, P5).
Candidato: (91, Candidato 1), (92, Candidato 2), (93, Candidato 3), (94, Candidato 4), (95, Candidato 5).
Votacao: cada partido possui um registro de votos para cada um dos estados AC, PE e SP (por exemplo: 91, 12345, AC; 91, 98323, PE; 91, 1726453, SP).
Com base nas informações e na situação apresentada, qual o comando SQL que seleciona corretamente os nomes dos candidatos, seus partidos e o total de votos de cada partido nessa eleição?
(ENADE 2017)',
     NULL, N'Para totalizar os votos por candidato e partido é preciso somar os votos de todos os estados com SUM e agrupar por nome do candidato e do partido com GROUP BY. Sem GROUP BY a consulta não funciona (A), COUNT conta registros em vez de somar votos (B e E) e funções de agregação não podem ser usadas no GROUP BY (D).', 1, 6, 4);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (271, 'A', N'SELECT c.nome, p.nome, SUM(v.votos) FROM Partido p, Candidato c, Votacao v WHERE c.numero = p.numero and v.partido = c.numero;', 0, 55),
    (272, 'B', N'SELECT c.nome, p.nome, COUNT(v.votos) FROM Partido p, Candidato c, Votacao v WHERE c.numero = p.numero and v.partido = c.numero GROUP BY c.nome, p.nome;', 0, 55),
    (273, 'C', N'SELECT c.nome, p.nome, SUM(v.votos) FROM Partido p, Candidato c, Votacao v WHERE c.numero = p.numero and v.partido = c.numero GROUP BY c.nome, p.nome;', 1, 55),
    (274, 'D', N'SELECT c.nome, p.nome, v.votos FROM Partido p, Candidato c, Votacao v WHERE c.numero = p.numero and v.partido = c.numero GROUP BY c.nome, p.nome, SUM(v.votos);', 0, 55),
    (275, 'E', N'SELECT c.nome, p.nome, COUNT(v.votos) FROM Partido p, Candidato c, Votacao v WHERE c.numero = p.numero and v.partido = c.numero GROUP BY c.nome, p.nome, v.votos;', 0, 55);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 56 | Banco de Dados | correta: A
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (56, N'Uma empresa da indústria farmacêutica está implantando um data warehouse que servirá de base para geração de suas informações gerenciais. Durante o processo de análise, descobre-se que dois sistemas transacionais adquiridos de empresas diferentes armazenam dados cadastrais de forma diversa: o primeiro utiliza "m" e "f " para representar os sexos masculino e feminino, respectivamente. Já o segundo sistema representa os mesmos sexos como "1" e "2", respectivamente. Além disso, outros sistemas transacionais da própria empresa alimentam-se de dados desses sistemas.
Sabendo que os dados de ambos os sistemas transacionais alimentarão o data warehouse que está sendo implantado na referida empresa, avalie as afirmativas.
I. A empresa deve definir um padrão e utilizar uma solução de ETL (Extract, Transform and Load) para adequar os dados de entrada para o data warehouse ao padrão definido.
II. A empresa deve armazenar, na tabela de fatos do data warehouse, a sigla do sistema de origem para saber interpretar os dados.
III. A empresa deve solicitar aos fornecedores dos aplicativos que a representação de seus dados ocorra de forma padronizada.
É correto o que se afirma em
(ENADE 2017)',
     NULL, NULL, 1, 6, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (276, 'A', N'I, apenas.', 1, 56),
    (277, 'B', N'II, apenas.', 0, 56),
    (278, 'C', N'I e III, apenas.', 0, 56),
    (279, 'D', N'II e III, apenas.', 0, 56),
    (280, 'E', N'I, II e III.', 0, 56);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 57 | Banco de Dados | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (57, N'O modelo de dados relacional a seguir representa de forma simplificada o banco de dados para um sistema de informação bancário (pk = chave primária; fk = chave estrangeira).
CATEGORIA_CLIENTE (COD_CATEGORIA_CLIENTE pk, NOM_CATEGORIA_CLIENTE)
CLIENTE (COD_CLIENTE pk, NOME, NUM_CPF_CNPJ, NUM_CELULAR, DES_ENDERECO, COD_CATEGORIA_CLIENTE fk)
AGENCIA (NUM_AGENCIA pk, NOM_AGENCIA)
TIPO_CONTA (COD_TIPO_CONTA pk, DES_TIPO_CONTA)
CONTA (NUM_CONTA pk, VAL_SALDO, COD_TIPO_CONTA fk, COD_CLIENTE fk, NUM_AGENCIA fk)
TIPO_MOVIMENTACAO (COD_TIPO_MOVIMENTACAO pk, DES_TIPO_MOVIMENTACAO, VAL_TAXA, IND_DEBITO_CREDITO)
HISTORICO_MOVIMENTACAO (COD_HISTORICO_MOVIMENTACAO pk, NUM_CONTA fk, COD_TIPO_MOVIMENTACAO fk, VAL_MOVIMENTADO, DTA_MOVIMENTACAO)
Usando a linguagem SQL (Structured Query Language), indique a consulta que recupera os nomes dos clientes e o valor médio movimentado por eles em sua conta até o momento. Primeiro devem ser exibidos os clientes com maior média de valor movimentado.
(ENADE 2021)',
     NULL, N'A consulta deve ligar CLIENTE, CONTA e HISTORICO_MOVIMENTACAO pelas chaves corretas, calcular a média com AVG, agrupar por cliente e ordenar pela média em ordem decrescente. A alternativa A não calcula a média, a B usa AVG sem GROUP BY, a D liga colunas erradas e a E agrupa apenas pelo nome, misturando clientes com nomes iguais.', 1, 6, 4);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (281, 'A', N'SELECT CL.NOME, HM.VAL_MOVIMENTADO
FROM CLIENTE CL INNER JOIN CONTA CO ON CL.COD_CLIENTE = CO.COD_CLIENTE
INNER JOIN HISTORICO_MOVIMENTACAO HM ON CO.NUM_CONTA = HM.NUM_CONTA
ORDER BY HM.VAL_MOVIMENTADO', 0, 57),
    (282, 'B', N'SELECT CL.NOME, AVG(HM.VAL_MOVIMENTADO)
FROM CLIENTE CL INNER JOIN CONTA CO ON CL.COD_CLIENTE = CO.COD_CLIENTE
INNER JOIN HISTORICO_MOVIMENTACAO HM ON CO.NUM_CONTA = HM.NUM_CONTA
ORDER BY HM.VAL_MOVIMENTADO DESC', 0, 57),
    (283, 'C', N'SELECT CL.NOME, AVG(HM.VAL_MOVIMENTADO)
FROM CLIENTE CL INNER JOIN CONTA CO ON CL.COD_CLIENTE = CO.COD_CLIENTE
INNER JOIN HISTORICO_MOVIMENTACAO HM ON CO.NUM_CONTA = HM.NUM_CONTA
GROUP BY CL.COD_CLIENTE, CL.NOME
ORDER BY AVG(HM.VAL_MOVIMENTADO) DESC', 1, 57),
    (284, 'D', N'SELECT CL.NOME, AVG(HM.VAL_MOVIMENTADO)
FROM CLIENTE CL LEFT OUTER JOIN CONTA CO ON CL.COD_CLIENTE = CO.NUM_CONTA
LEFT OUTER JOIN HISTORICO_MOVIMENTACAO HM ON CO.NUM_CONTA = HM.COD_HISTORICO_MOVIMENTACAO
GROUP BY CL.NOME
ORDER BY AVG(HM.VAL_MOVIMENTADO) DESC', 0, 57),
    (285, 'E', N'SELECT CL.NOME, AVG(HM.VAL_MOVIMENTADO)
FROM CLIENTE CL LEFT OUTER JOIN CONTA CO ON CL.COD_CLIENTE = CO.COD_CLIENTE
LEFT OUTER JOIN HISTORICO_MOVIMENTACAO HM ON CO.NUM_CONTA = HM.NUM_CONTA
GROUP BY CL.NOME
ORDER BY AVG(HM.VAL_MOVIMENTADO) DESC', 0, 57);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 58 | Banco de Dados | correta: A
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (58, N'Considere o modelo de dados a seguir (pk = chave primária; fk = chave estrangeira).
tb_vendedor (id_vendedor pk, ds_nome)
tb_cliente (id_cliente pk, ds_nome)
tb_pedido (id_pedido pk, dt_pedido, id_cliente fk, id_vendedor fk)
tb_item_pedido (id_pedido pk/fk, id_produto pk/fk, qt_produto, vl_unitario)
tb_produto (id_produto pk, ds_produto, vl_unitario, qt_estoque, id_fornecedor fk)
tb_fornecedor (id_fornecedor pk, ds_fornecedor)
A empresa que se utilizou do modelo de dados apresentado desejava obter um relatório com as quantidades e o total de vendas por vendedor, a cada ano, conforme a tabela a seguir.
Ano | Vendedor | Quantidade de vendas | Total de vendas
2015 | Vendedor A | 53 | 54 658
2015 | Vendedor B | 57 | 33 874
2015 | Vendedor C | 65 | 85 489
2016 | Vendedor A | 80 | 45 987
2016 | Vendedor B | 73 | 86 458
2016 | Vendedor C | 49 | 89 452
2016 | Vendedor D | 95 | 78 698
Com base nessas informações, assinale a opção com a consulta SQL que gera corretamente os dados do relatório solicitado pela empresa.
(ENADE 2017)',
     NULL, NULL, 1, 6, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (286, 'A', N'SELECT
  YEAR(p.dt_pedido) AS ano,
  v.ds_nome AS vendedor,
  COUNT(DISTINCT p.id_pedido) AS qtd_vendas,
  SUM(ip.qt_produto * ip.vl_unitario) AS tot_vendas
FROM tb_vendedor v
INNER JOIN tb_pedido p ON v.id_vendedor = p.id_vendedor
INNER JOIN tb_item_pedido ip ON p.id_pedido = ip.id_pedido
GROUP BY year(p.dt_pedido), v.ds_nome
ORDER BY year(p.dt_pedido), v.ds_nome;', 1, 58),
    (287, 'B', N'SELECT
  YEAR(dt_pedido) AS ano,
  v.ds_nome AS vendedor,
  COUNT(DISTINCT p.id_pedido) AS qtd_vendas,
  SUM(ip.qt_produto * ip.vl_unitario) AS tot_vendas
FROM tb_vendedor v
INNER JOIN tb_pedido p ON v.id_vendedor = p.id_vendedor
INNER JOIN tb_item_pedido ip ON p.id_pedido = ip.id_pedido
GROUP BY dt_pedido, ds_nome
ORDER BY year(p.dt_pedido), v.ds_nome;', 0, 58),
    (288, 'C', N'SELECT
  YEAR(dt_pedido) AS ano,
  v.ds_nome AS vendedor,
  AVG(p.id_pedido) AS qtd_vendas,
  SUM(ip.qt_produto * ip.vl_unitario) AS tot_vendas
FROM tb_vendedor v
OUTER JOIN tb_pedido p ON v.id_vendedor = p.id_vendedor
OUTER JOIN tb_item_pedido ip ON p.id_pedido = ip.id_pedido
GROUP BY year(p.dt_pedido), v.ds_nome
ORDER BY year(p.dt_pedido), v.ds_nome;', 0, 58),
    (289, 'D', N'SELECT
  YEAR(p.dt_pedido) AS ano,
  v.ds_nome AS vendedor,
  COUNT(p.id_pedido) AS qtd_vendas,
  SUM(ip.qt_produto * ip.vl_unitario) AS tot_vendas
FROM tb_vendedor v
INNER JOIN tb_pedido p ON v.id_vendedor = p.id_vendedor
INNER JOIN tb_item_pedido ip ON p.id_pedido = ip.id_pedido
GROUP BY p.dt_pedido, v.ds_nome
ORDER BY p.dt_pedido, v.ds_nome', 0, 58),
    (290, 'E', N'SELECT
  YEAR(p.dt_pedido) AS ano,
  v.ds_nome AS vendedor,
  AVG(DISTINCT p.id_pedido) AS qtd_vendas,
  SUM(ip.qt_produto * ip.vl_unitario) AS tot_vendas
FROM tb_pedido p
INNER JOIN tb_item_pedido ip ON p.id_pedido = ip.id_pedido
RIGHT JOIN tb_vendedor v ON p.id_vendedor = v.id_vendedor
GROUP BY year(p.dt_pedido), v.ds_nome
ORDER BY year(p.dt_pedido), v.ds_nome;', 0, 58);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 59 | Banco de Dados | correta: D
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (59, N'Em uma rede local de uma instituição bancária, foi implantado um sistema gerenciador de banco de dados (SGDB), tendo o administrador efetuado um comando GRANT para a realização das configurações de acesso ao SGDB na rede.
Com o objetivo de realizar autenticação por aplicações para acesso ao banco de dados, de forma segura, devem ser enviados ao servidor
(ENADE 2017)',
     NULL, N'Para se autenticar no SGBD pela rede, a aplicação precisa informar onde está o servidor (IP), qual banco deseja acessar (nome físico do banco) e a credencial (senha). O endereço MAC não é usado nessa autenticação.', 1, 6, 4);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (291, 'A', N'o IP do servidor e a senha.', 0, 59),
    (292, 'B', N'a senha e o endereço MAC.', 0, 59),
    (293, 'C', N'o IP do servidor e o endereço MAC.', 0, 59),
    (294, 'D', N'o IP do servidor, o nome físico do banco e a senha.', 1, 59),
    (295, 'E', N'o IP do cliente, o nome físico do banco, a senha e o endereço MAC.', 0, 59);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 60 | Banco de Dados | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (60, N'Leia o texto a seguir.
Certamente, uma empresa com um volume de 10.000 notas fiscais por mês (e com vários itens por nota fiscal) não usará cópias dessas notas como meio de entrada de dados sobre receitas nos subsistemas de contas a receber e de contabilidade. Isso equivaleria a classificar e efetuar 10.000 ou mais lançamentos, respectivamente, para cada cliente, de acordo com o seu código contábil. Por outro lado, de alguma forma, o sistema de contas a receber deve "informar" ao sistema de contabilidade os recebimentos de clientes. Da mesma forma, pode ser inviável o registro contábil de um por um dos comprovantes de recebimento.
BIO, S. R. Sistema de informação: um enfoque gerencial. 2. ed. São Paulo: Atlas, 2008 (com adaptações).
A situação descrita no texto é um típico caso de problema a ser resolvido com
(ENADE 2017)',
     NULL, NULL, 1, 6, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (296, 'A', N'data warehouse, um depósito de dados orientado por assunto, integrado, não volátil, variável com o tempo, para apoiar as decisões gerenciais.', 0, 60),
    (297, 'B', N'SOA (Service Oriented Architecture), uma arquitetura proposta para interoperabilidade de sistemas por meio de um conjunto de interfaces de serviços.', 0, 60),
    (298, 'C', N'ERP (Enterprise Resource Planning), um software que integra todos os dados e processos de uma organização em um único sistema.', 1, 60),
    (299, 'D', N'GED (Gestão Eletrônica de Documentos), um conjunto de tecnologias que permite a uma empresa gerenciar seus documentos em forma digital.', 0, 60),
    (300, 'E', N'gestão do conhecimento, uma área de atuação transversal relacionada, sobretudo, à gestão estratégica, teoria das organizações, sistema de informação, gestão da tecnologia, e às áreas mais tradicionais.', 0, 60);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 61 | Redes de Computadores e Sistemas Operacionais | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (61, N'O conceito de máquina virtual (MV) foi usado, na década de 1970, no sistema operacional IBM System 370. Atualmente, centros de dados (datacenters) usam MVs para migrar tarefas entre servidores conectados em rede e, assim, equilibrar carga de processamento. Além disso, plataformas atuais de desenvolvimento de software empregam MVs (Java,.NET). Uma MV pode ser construída para emular um processador ou um computador completo. Um código desenvolvido para uma máquina real pode ser executado de forma transparente em uma MV.
Com relação a essas informações, assinale a opção correta.
(ENADE 2008)',
     NULL, N'A máquina virtual executa os aplicativos em um ambiente isolado e controlado, o que aumenta o controle de segurança. Ela não dá acesso direto ao hardware (A), consome recursos adicionais em vez de economizá-los (B), pode emular uma CPU diferente da real (D) e o sistema executado nela continua sujeito a vírus (E).', 1, 7, 4);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (301, 'A', N'O conceito de transparência mencionado indica que a MV permite que um aplicativo acesse diretamente o hardware da máquina.', 0, 61),
    (302, 'B', N'Uma das vantagens mais significativas de uma MV é a economia de carga de CPU e de memória RAM na execução de um aplicativo.', 0, 61),
    (303, 'C', N'Uma MV oferece maior controle de segurança, uma vez que aplicativos são executados em um ambiente controlado.', 1, 61),
    (304, 'D', N'Para emular uma CPU dual-core, uma MV deve ser instalada e executada em um computador com CPU dual-core.', 0, 61),
    (305, 'E', N'Como uma MV não é uma máquina real, um sistema operacional nela executado fica automaticamente imune a vírus.', 0, 61);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 62 | Redes de Computadores e Sistemas Operacionais | correta: D
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (62, N'Leia o texto a seguir.
O serviço DNS (Domain Name System) traduz nomes alfanuméricos de hosts em endereços numéricos, de acordo com o protocolo IP (Internet Protocol). Essa ação é comumente chamada de resolução de endereço.
TANENBAUM, A. S. Redes de Computadores. Rio de Janeiro: Campus, 2003 (com adaptações).
Considere um conjunto de computadores conectados em uma rede local, os quais têm à sua disposição um servidor DNS capaz de resolver endereços, sejam eles internos ou externos. Nesse contexto, avalie as afirmativas a seguir.
I. O servidor DNS também executa funções de cliente DNS quando não é autoritativo para determinado endereço.
II. A adoção do IPv6 (Internet Protocol, versão 6) dispensará serviços de DNS, pois suas funções serão incorporadas pelo próprio protocolo IP.
III. O cache DNS permite que determinada requisição do cliente DNS possa ser resolvida sem que seja necessário recorrer a outro serviço DNS.
IV. O protocolo DNS depende de um banco de dados distribuído.
É correto apenas o que se afirma em
(ENADE 2014)',
     NULL, NULL, 1, 7, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (306, 'A', N'I e II.', 0, 62),
    (307, 'B', N'I e III.', 0, 62),
    (308, 'C', N'II e IV.', 0, 62),
    (309, 'D', N'I, III e IV.', 1, 62),
    (310, 'E', N'II, III e IV.', 0, 62);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 63 | Redes de Computadores e Sistemas Operacionais | correta: E
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (63, N'Leia o texto a seguir.
O barramento é o elemento de conexão entre todos os componentes do computador, como memória, CPU e dispositivos de entrada e saída. O barramento de dados é o meio por onde serão trafegados os dados. O barramento de endereços transporta a informação do endereço do dispositivo que poderá acessar o barramento de dados; e o barramento de controle serve para determinar o sentido do fluxo de dados (se os dados são de entrada ou saída da CPU), e se os dados devem ser destinados à memória ou aos dispositivos de I/O e também para controlar o clock no barramento.
STALLINGS, W. Arquitetura e Organização de Computadores. 8 ed. São Paulo: Pearson Prentice Hall, 2010 (com adaptações).
Considerando um computador com um barramento de dados de 4 bits e barramento de endereços de 3 bits, ele poderá endereçar, respectivamente, quantas posições de memória e quantos dispositivos de I/O?
(ENADE 2014)',
     NULL, N'Com 3 bits de endereço é possível formar 2³ = 8 combinações. Como o barramento de endereços seleciona tanto posições de memória quanto dispositivos de E/S, o computador endereça 8 posições de memória e 8 dispositivos de E/S.', 1, 7, 4);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (311, 'A', N'4 e 4.', 0, 63),
    (312, 'B', N'4 e 8.', 0, 63),
    (313, 'C', N'8 e 3.', 0, 63),
    (314, 'D', N'8 e 4.', 0, 63),
    (315, 'E', N'8 e 8.', 1, 63);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 64 | Redes de Computadores e Sistemas Operacionais | correta: A
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (64, N'Leia o texto a seguir.
As redes de computadores estão por toda parte. A internet é uma delas, assim como as várias redes das quais ela é composta, que compartilham recursos por meio de Sistemas Distribuídos. Como o número de dispositivos tem crescido exponencialmente, em vez de paralelizar tarefas em várias máquinas, tornou-se cada vez mais importante empregar sistemas distribuídos, comunicando e coordenando dispositivos e, assim, contribuindo para o cotidiano de empresas, governos e demais instituições.
COULOURIS, G. et al. Sistemas Distribuídos: conceitos e projeto, 5. ed. Porto Alegre: Bookman, 2013 (com adaptações).
Nesse contexto, assinale a opção que apresenta exemplos atuais de sistemas distribuídos.
(ENADE 2017)',
     NULL, NULL, 1, 7, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (316, 'A', N'Pesquisa na web, jogos on-line, e-commerce e o banco de dados das agências bancárias.', 1, 64),
    (317, 'B', N'Jogos on-line, e-commerce, pesquisa no Windows e e-mails.', 0, 64),
    (318, 'C', N'E-commerce, jogos em mídia física, pesquisa no Windows e redes sociais.', 0, 64),
    (319, 'D', N'Pesquisa na web, jogos em mídia física, e-commerce e e-mails.', 0, 64),
    (320, 'E', N'Pesquisa no Windows, pesquisa na web, jogos on-line e o banco de dados das agências bancárias.', 0, 64);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 65 | Redes de Computadores e Sistemas Operacionais | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (65, N'Leia o texto a seguir.
Conceitualmente, cada processo tem sua própria CPU (Central Processing Unit) virtual. É claro que, na realidade, a CPU troca a execução, a todo momento, de um processo para outro, mas, para entender esse sistema, é muito mais fácil pensar em um conjunto de processos sendo executado (pseudo) paralelamente do que tentar controlar o modo como a CPU faz esses chaveamentos.
TANENBAUN, A. S. Sistemas operacionais modernos. 3. ed. São Paulo: Pearson, 2010 (com adaptações).
De acordo com o exposto, o conceito descrito denomina-se
(ENADE 2017)',
     NULL, N'Na multiprogramação, a CPU alterna rapidamente entre vários processos, dando a impressão de que cada um tem sua própria CPU e de que todos executam em paralelo (pseudoparalelismo).', 1, 7, 4);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (321, 'A', N'thread.', 0, 65),
    (322, 'B', N'multiprocessador.', 0, 65),
    (323, 'C', N'multiprogramação.', 1, 65),
    (324, 'D', N'processo monothread.', 0, 65),
    (325, 'E', N'máquina de estados finitos.', 0, 65);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 66 | Redes de Computadores e Sistemas Operacionais | correta: D
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (66, N'Leia o texto a seguir.
Nas organizações, as redes de computadores e os equipamentos são soluções tecnológicas planejadas e construídas com base em padrões. Um dos padrões responsáveis pela comunicação e pelo endereçamento em redes é o TCP/IP (Transmission Control Protocol/Internet Protocol), que permite a comunicação entre diferentes computadores, sistemas operacionais e aplicativos que aceitam esse protocolo.
SOUSA, L. B. Projetos e implementação de redes: fundamentos, soluções, arquitetura e planejamento. São Paulo: Erica,
2013 (com adaptações).
Com base no protocolo de comunicação de dados em rede TCP/IP, mencionado no texto acima, avalie as afirmativas.
I. Os programas que controlam a comunicação dos dados entre equipamentos e aplicativos em redes são chamados de protocolos de armazenamento de dados.
II. Dois sistemas operacionais com plataforma de hardware distintas comunicam-se entre si por meio de um protocolo comum, o TCP/IP.
III. O nome TCP/IP se refere a dois protocolos: o TCP, protocolo responsável pelo controle e qualidade da comunicação entre a origem e o destino, e o IP, protocolo responsável pelo endereçamento nas redes.
É correto o que se afirma em
(ENADE 2017)',
     NULL, NULL, 1, 7, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (326, 'A', N'I, apenas.', 0, 66),
    (327, 'B', N'II, apenas.', 0, 66),
    (328, 'C', N'I e III, apenas.', 0, 66),
    (329, 'D', N'II e III, apenas.', 1, 66),
    (330, 'E', N'I, II e III.', 0, 66);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 67 | Redes de Computadores e Sistemas Operacionais | correta: B
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (67, N'Leia o texto a seguir.
O modo clássico de encarar um sistema operacional é como um gerenciador de recursos. Desse ponto de vista, o sistema operacional é responsável pelo hardware do sistema. Nesse papel, ele recebe solicitações de acesso a recursos por parte das aplicações e concede ou nega tais acessos. Ao conceder solicitações de alocação, ele deve dispor com cuidado os recursos, de modo que os programas não interfiram uns nos outros.
Por exemplo, é uma péssima ideia permitir que os programas tenham acesso sem restrição à memória uns dos outros. Se um programa com defeito (ou malicioso) escreve no espaço de memória do outro programa, o segundo programa travará, na melhor das hipóteses, ou produzirá resultados incorretos, na pior das hipóteses. Ou ainda, se o programa ofensivo modificar a memória do sistema operacional, poderá afetar o comportamento de todo o sistema.
STUART. B. L. Princípios de sistemas operacionais: projetos e aplicações. São Paulo: Cengage Learning, 2011 (com adaptações).
Considerando que o texto alerta para a possibilidade de um programa interferir no outro, a atividade realizada por um sistema operacional que garante essa proteção é
(ENADE 2021)',
     NULL, N'O gerenciamento de memória garante que cada programa use apenas o seu próprio espaço de memória, impedindo que um processo com defeito ou malicioso escreva na memória de outro processo ou do sistema operacional.', 1, 7, 4);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (331, 'A', N'o programa antivírus.', 0, 67),
    (332, 'B', N'o gerenciamento de memória.', 1, 67),
    (333, 'C', N'o gerenciamento de arquivos.', 0, 67),
    (334, 'D', N'o gerenciamento de processos.', 0, 67),
    (335, 'E', N'o gerenciamento de entrada e saída.', 0, 67);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 68 | Redes de Computadores e Sistemas Operacionais | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (68, N'Em um cenário de administração de Servidores de Redes, um administrador realiza a publicação de um site no servidor web com a funcionalidade de vhost (virtual host) no arquivo de configuração do sistema operacional. Em seguida, ele cria o diretório e armazena nesse local todo o conteúdo necessário para o correto funcionamento do site. Após essas ações, no entanto, o administrador percebe que o site não está funcionando, mesmo após o reinício do daemon do serviço do servidor web.
Considerando que o servidor de DNS (Domain Name System) está configurado corretamente, o procedimento pendente a ser realizado pelo administrador para o correto funcionamento do site é
(ENADE 2014)',
     NULL, NULL, 1, 7, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (336, 'A', N'habilitar a funcionalidade de DNS do vhost do site.', 0, 68),
    (337, 'B', N'iniciar o daemon do serviço web em modo seguro.', 0, 68),
    (338, 'C', N'verificar as permissões dos arquivos declarados no vhost.', 1, 68),
    (339, 'D', N'habilitar a porta 80 para atender o vhost no arquivo de configuração.', 0, 68),
    (340, 'E', N'adicionar a entrada NS no servidor de DNS apontando para o endereço do vhost do site.', 0, 68);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 69 | Redes de Computadores e Sistemas Operacionais | correta: D
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (69, N'O protocolo IPv6 foi desenvolvido para substituir o IPv4, tendo sua implementação ocasionado várias mudanças importantes, como a capacidade de endereçamento expandida, o cabeçalho aprimorado de 40 bytes e a rotulação de fluxo e prioridade.
Considerando essas informações, avalie as afirmativas a seguir, relativas à descrição dos campos de cabeçalho IPv6.
I. Em "endereço de origem" e "endereço de destino", cada campo possui 64 bits, tendo sido expandidos os 32 bits usados no IPv4.
II. Em se tratando do cabeçalho IPv6, insere-se o valor 32 no campo "versão" de 4 bits que é usado para identificar a versão do protocolo IP.
III. O campo "próximo cabeçalho" identifica o protocolo ao qual os dados presentes no datagrama serão entregues, por exemplo, TCP ou UDP.
IV. No IPv6, o campo "classe de tráfego" de 8 bits é semelhante ao campo "tipo de serviço" do IPv4, ambos utilizados para diferenciar os tipos de pacotes IP.
V. O valor do campo "limite de saltos" é decrementado em um para cada roteador que repassa o pacote; caso a contagem do limite de salto chegue a zero, o pacote será descartado.
É correto apenas o que se afirma em
(ENADE 2017)',
     NULL, N'No IPv6, o campo próximo cabeçalho indica o protocolo da camada superior (III), a classe de tráfego equivale ao tipo de serviço do IPv4 (IV) e o limite de saltos diminui a cada roteador, descartando o pacote ao chegar a zero (V). Os endereços têm 128 bits, não 64 (I), e o campo versão recebe o valor 6, não 32 (II).', 1, 7, 4);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (341, 'A', N'II e IV.', 0, 69),
    (342, 'B', N'I, II e III.', 0, 69),
    (343, 'C', N'I, III e V.', 0, 69),
    (344, 'D', N'III, IV e V.', 1, 69),
    (345, 'E', N'I, II, IV e V.', 0, 69);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 70 | Redes de Computadores e Sistemas Operacionais | correta: B
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (70, N'As empresas que utilizam as tecnologias da informação estão cada vez mais compromissadas com as questões de sustentabilidade e meio ambiente. As tecnologias convergentes visam, entre as diversas vantagens, a proporcionar economia nos custos, possibilitando às empresas praticarem um preço final competitivo, preservando o meio ambiente e mantendo o negócio sustentável.
Nesse contexto, avalie as asserções a seguir e a relação proposta entre elas.
I. A tecnologia de VoIP é utilizada para substituir redes telefônicas convencionais portando o tráfego de voz para as redes de dados e proporcionando economia nos custos de telefonia da empresa.
PORQUE
II. Sustentabilidade é a habilidade em manter alguma condição, ou seja, possibilitar que um processo exista e permaneça.
A respeito dessas asserções, assinale a opção correta.
(ENADE 2014)',
     NULL, NULL, 1, 7, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (346, 'A', N'As asserções I e II são proposições verdadeiras, e a asserção II justifica a I.', 0, 70),
    (347, 'B', N'As asserções I e II são proposições verdadeiras, e a asserção II não justifica a I.', 1, 70),
    (348, 'C', N'A asserção I é uma proposição verdadeira, e a II é uma proposição falsa.', 0, 70),
    (349, 'D', N'A asserção I é uma proposição falsa, e a II é uma proposição verdadeira.', 0, 70),
    (350, 'E', N'As asserções I e II são falsas.', 0, 70);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 71 | Gestão de Projetos, Governança e Segurança da Informação | correta: B
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (71, N'Após atuar como programador em uma empresa de desenvolvimento de software por aproximadamente 10 anos, um funcionário que se destacou por nunca atrasar um cronograma foi nomeado gerente de projetos. Ao assumir o primeiro projeto, o funcionário foi informado que sua principal responsabilidade era a realização da entrega conforme o cronograma estabelecido no contrato. Para o gerenciamento de tempo, o gerente de projetos irá utilizar o PMBOK.
Considerando essa situação, é correto afirmar que o gerente de projetos deverá coordenar processos de
(ENADE 2008)',
     NULL, N'No PMBOK, o gerenciamento do tempo (cronograma) envolve definir e sequenciar as atividades, estimar recursos e durações e desenvolver e controlar o cronograma. As demais alternativas descrevem as áreas de qualidade, escopo, custos e aquisições.', 1, 8, 4);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (351, 'A', N'planejamento, garantia e controle da qualidade.', 0, 71),
    (352, 'B', N'definição e sequenciamento de atividades, estimativa de recursos e duração da atividade, desenvolvimento e controle do cronograma.', 1, 71),
    (353, 'C', N'planejamento, definição, verificação e controle do escopo.', 0, 71),
    (354, 'D', N'estimativa de custos, realização do orçamento e controle de custos.', 0, 71),
    (355, 'E', N'planejamento de compra e contratações, seleção de fornecedores e encerramento do contrato.', 0, 71);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 72 | Gestão de Projetos, Governança e Segurança da Informação | correta: D
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (72, N'O gerenciamento de projetos de sistemas é essencial para a engenharia de sofware. Um mau gerenciamento acarreta atraso na entrega do programa, custo maior do que o programado e falha no atendimento de requisitos. O quadro a seguir representa de forma hipotética algumas tarefas de um projeto de desenvolvimento de software. Esse quadro mostra as atividades, sua duração e as respectivas interdependências.
Tarefa | Duração (dias) | Dependências
T1 | 5 | -
T2 | 8 | T1
T3 | 2 | T2
T4 | 4 | -
T5 | 2 | T3, T4
T6 | 4 | -
T7 | 3 | T5
Com base na análise do quadro, o tempo mínimo necessário para terminar o projeto é de
(ENADE 2014)',
     NULL, NULL, 1, 8, 4);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (356, 'A', N'13 dias.', 0, 72),
    (357, 'B', N'15 dias.', 0, 72),
    (358, 'C', N'16 dias.', 0, 72),
    (359, 'D', N'20 dias.', 1, 72),
    (360, 'E', N'28 dias.', 0, 72);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 73 | Gestão de Projetos, Governança e Segurança da Informação | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (73, N'O método do caminho crítico identifica a sequência de atividades a que o gerente de projetos deve dar mais atenção, porque, caso uma delas atrase, todo o projeto estará atrasado. A tabela a seguir apresenta a estrutura de atividades de um projeto.
Atividade | Atividade predecessora | Duração (dias)
A | -- | 3
B | A | 5
C | -- | 12
D | -- | 4
E | D | 2
F | D | 5
G | F | 7
H | E e G | 8
I | B | 11
J | C, H e I | 3
Com base nas informações apresentadas na tabela, avalie as afirmativas.
I. O caminho crítico é composto pelas atividades A, B, I e J.
II. O caminho crítico tem 27 dias.
III. A atividade H faz parte do caminho crítico.
IV. O projeto em pauta possui dois caminhos críticos.
É correto apenas o que se afirma em
(ENADE 2017)',
     NULL, N'Os caminhos possíveis são A-B-I-J (22 dias), C-J (15 dias), D-E-H-J (17 dias) e D-F-G-H-J (27 dias). O caminho crítico é o mais longo, D-F-G-H-J, com 27 dias, e inclui a atividade H; existe apenas um caminho crítico.', 1, 8, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (361, 'A', N'I e II.', 0, 73),
    (362, 'B', N'I e IV.', 0, 73),
    (363, 'C', N'II e III.', 1, 73),
    (364, 'D', N'I, III e IV.', 0, 73),
    (365, 'E', N'II, III e IV.', 0, 73);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 74 | Gestão de Projetos, Governança e Segurança da Informação | correta: E
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (74, N'Um projeto de integração de um ERP com um CRM não implementou todos os serviços esperados e há falhas nos serviços já implementados. Uma auditoria avaliou a situação e identificou que há desentendimentos entre os membros da equipe; os membros da qualidade não compreendem o que deve ser avaliado; demandas de novos solicitantes aparecem constantemente; e o patrocinador demora muito a responder às solicitações da equipe. Tudo isso fez com que o projeto ficasse bastante atrasado e com orçamento excedido. Considerando o cenário descrito, o gerente de projeto deve priorizar o gerenciamento de
(ENADE 2021)',
     NULL, NULL, 1, 8, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (366, 'A', N'custo.', 0, 74),
    (367, 'B', N'tempo.', 0, 74),
    (368, 'C', N'escopo.', 0, 74),
    (369, 'D', N'qualidade.', 0, 74),
    (370, 'E', N'partes interessadas.', 1, 74);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 75 | Gestão de Projetos, Governança e Segurança da Informação | correta: A
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (75, N'Leia o texto a seguir.
Pode-se definir liderança como a capacidade de influenciar um grupo para alcançar metas. A origem dessa influência pode ser formal, como a que é conferida por um alto cargo na organização. Como essas posições remetem a um certo grau de autoridade, uma pessoa pode assumir um papel de liderança apenas em função do cargo que ocupa. Nem todos os líderes são administradores e nem todos os executivos são líderes. O fato de a organização conferir a seus executivos alguns direitos formais não lhes assegura a capacidade de liderança eficaz. A liderança não sancionada – aquela capaz de influenciar os outros, que emerge fora da estrutura formal da organização – geralmente é tão importante quanto a influência formal, ou até mais. Em outras palavras, o líder pode surgir naturalmente dentro de um grupo ou por indicação formal. Nesse sentido, uma das habilidades interpessoais cujo valor é alto para as organizações é a liderança.
ROBBINS, S. P. Comportamento organizacional. 11. ed. São Paulo: Prentice Hall, 2009 (com adaptações).
Considerando essas informações, assinale a opção em que se caracteriza corretamente a liderança na área de projetos.
(ENADE 2017)',
     NULL, N'A liderança em projetos depende de habilidades interpessoais, como comunicação e trabalho em equipe, que o gestor deve desenvolver no grupo. As outras alternativas tratam o estilo autocrático como garantia de sucesso, dispensam as habilidades técnicas ou excluem perfis introspectivos, o que não condiz com as teorias de liderança.', 1, 8, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (371, 'A', N'Gestores de projetos devem prezar pelo desenvolvimento das habilidades interpessoais dos membros de seu grupo, como a liderança e o trabalho em equipe, essenciais para o sucesso dos projetos e o alcance dos objetivos estratégicos.', 1, 75),
    (372, 'B', N'Líderes com perfis mais autocráticos têm maior chance de sucesso no gerenciamento de projetos, visto que essa é uma atividade fundamental para que os objetivos estratégicos das organizações sejam alcançados.', 0, 75),
    (373, 'C', N'Líderes devem matar o status quo, pois sua posição é resultado de uma escolha da administração que eles devem representar, cabendo-lhes fazer com que os objetivos estratégicos sejam alcançados e os projetos sejam entregues no prazo, dentro do escopo planejado, respeitando as estimativas orçamentárias.', 0, 75),
    (374, 'D', N'Líderes de projetos devem ter como características pessoais determinação, confiança e vontade, sendo-lhes dispensáveis as habilidades técnicas, pois elas podem ser desempenhadas pelos membros da equipe.', 0, 75),
    (375, 'E', N'Profissionais do tipo introspectivo são inadequados para funções de liderança, pois as teorias modernas de gerenciamento de projetos consideram a comunicação como um dos principais papéis do líder.', 0, 75);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 76 | Gestão de Projetos, Governança e Segurança da Informação | correta: D
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (76, N'Leia o texto a seguir.
A unidade de TI de determinada empresa é vista como ruim, pois não produz níveis de serviços adequados. Há alguns meses, após a fusão dessa empresa com uma outra do mesmo ramo, foi nomeado um novo executivo, que se sente preocupado com o desempenho da unidade de TI e pretende melhorá-lo. Uma questão contínua é que novos produtos da empresa são introduzidos sem que a equipe de TI seja avisada, o que força situações que exigem construção de infraestrutura e elaboração dos sistemas de suporte necessários. Pelo fato de os produtos da empresa acumularem muitos dados e haver muitos clientes, um novo produto pode implicar em um aumento repentino da demanda por serviços de infraestrutura de TI. É caro e difícil satisfazer essa demanda para esse fim específico, e os resultados alcançados têm gerado insatisfação contínua em relação à unidade de TI.
HUNTER, R.; WESTERMAN, G. O verdadeiro valor de TI. São Paulo: M. Books, 2011 (com adaptações).
Com base nessa situação, avalie as asserções e a relação proposta entre elas.
I. A criação de um comitê composto exclusivamente pelo pessoal da área de TI, com autonomia para tomar as decisões que afetam as demandas por serviços e infraestrutura de TI, seria uma opção acertada.
PORQUE
II. A empresa possui um arranjo decisório inadequado para suas expectativas; a pouca credibilidade da área de TI e a falta de envolvimento dessa área nas decisões comprometem o alinhamento estratégico entre a TI e o negócio, problema relacionado à governança de TI.
A respeito dessas asserções, assinale a opção correta.
(ENADE 2017)',
     NULL, NULL, 1, 8, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (376, 'A', N'As asserções I e II são proposições verdadeiras, e a asserção II justifica a I.', 0, 76),
    (377, 'B', N'As asserções I e II são proposições verdadeiras, e a asserção II não justifica a I.', 0, 76),
    (378, 'C', N'A asserção I é uma proposição verdadeira, e a II é uma proposição falsa.', 0, 76),
    (379, 'D', N'A asserção I é uma proposição falsa, e a II é uma proposição verdadeira.', 1, 76),
    (380, 'E', N'As asserções I e II são proposições falsas.', 0, 76);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 77 | Gestão de Projetos, Governança e Segurança da Informação | correta: E
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (77, N'Leia o texto a seguir.
A norma internacional ABNT NBR ISO/IEC 27001:2013 [substituta da norma 17799:2005, citada no texto original], define práticas para a análise e elaboração de um projeto para a gestão da segurança da informação. Os conceitos mais importantes consistem em confiabilidade, integridade, disponibilidade e autenticidade, entendidos também como práticas essenciais para criar sistemas que evitam perda de dados e deixam as informações mais seguras. A aplicação dessas práticas garante a drástica redução dos riscos de uma organização sofrer um ataque.
LYRA, M. R. Segurança e auditoria em sistemas de informação. São Paulo: Ciência Moderna, 2009 (com adaptações).
Com base no texto, avalie as afirmativas.
I. A confidencialidade é a capacidade de um sistema permitir que usuários autorizados acessem determinadas informações e impedir o acesso a elas aos demais usuários.
II. A autenticidade e a integridade garantem que a informação esteja correta, seja verdadeira e não esteja corrompida.
III. A disponibilidade garante que a informação esteja disponível para todos os usuários que precisarem dela, sempre que necessário.
É correto o que se afirma em
(ENADE 2017)',
     NULL, N'As três afirmativas estão corretas: a confidencialidade restringe o acesso a quem é autorizado, a autenticidade e a integridade garantem que a informação é verdadeira e não foi corrompida, e a disponibilidade garante o acesso sempre que necessário.', 1, 8, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (381, 'A', N'I, apenas.', 0, 77),
    (382, 'B', N'II, apenas.', 0, 77),
    (383, 'C', N'I e III, apenas.', 0, 77),
    (384, 'D', N'II e III, apenas.', 0, 77),
    (385, 'E', N'I, II e III.', 1, 77);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 78 | Gestão de Projetos, Governança e Segurança da Informação | correta: C
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (78, N'Leia o texto a seguir.
Recentemente, variantes de ransomware provocaram uma onda de sequestros digitais, que afetou hospitais, operadoras de telecomunicações e usuários residenciais.
Os ransomwares são utilizados com o objetivo de entrar nos sistemas corporativos por meio da rede e criptografar todos os dados da empresa, de modo que possam exigir um resgate financeiro para liberá-los.
Disponível em <http://www.junipernetworks.com.br>. Acesso em 10 jul. 2017 (com adaptações).
Considerando as informações do texto, avalie as afirmativas a seguir, em relação às medidas de prevenção que devem compor a política de segurança corporativa contra esse tipo de malware.
I. Deve-se manter o treinamento constante dos usuários em relação ao uso de e-mail e sítios seguros, suspeitos ou falsos.
II. Deve-se garantir a existência de uma boa política de combate e recuperação contra desastres em relação às cópias de segurança.
III. Deve-se prevenir a contaminação por meio de ferramentas capazes de detectar ataques de força bruta, característicos desses worms, e cujo objetivo é contaminar os computadores da rede.
É correto o que se afirma em
(ENADE 2017)',
     NULL, NULL, 1, 8, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (386, 'A', N'I, apenas.', 0, 78),
    (387, 'B', N'III, apenas.', 0, 78),
    (388, 'C', N'I e II, apenas.', 1, 78),
    (389, 'D', N'II e III, apenas.', 0, 78),
    (390, 'E', N'I, II e III.', 0, 78);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 79 | Gestão de Projetos, Governança e Segurança da Informação | correta: B
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (79, N'Uma empresa de médio porte sofreu um ataque hacker e alguns de seus sistemas foram comprometidos. Depois de alguns dias, a equipe de tecnologia de informação (TI) dessa empresa conseguiu restabelecer os sistemas, tendo a gestora de TI apresentado um plano para a diretoria com o intuito de criar um setor especializado em segurança da informação para evitar novos ataques. Após a análise dos riscos e benefícios do plano, a diretoria aprovou a implantação do referido setor de segurança da informação.
A partir do cenário descrito, avalie as asserções e a relação proposta entre elas.
I. A implantação de um setor de segurança da informação está associada ao desenvolvimento de uma política de segurança de informação que é um conjunto de normas, métodos e procedimentos utilizados para a manutenção da segurança da informação, devendo ser formalizada e divulgada a todos os usuários.
PORQUE
II. A implantação de um sistema de gestão da segurança da informação (SGSI) preserva a confidencialidade, a integridade e a disponibilidade da informação, por meio de um processo de gestão de riscos, e fornece, para as partes interessadas, a confiança de que os riscos são adequadamente gerenciados, sendo importante que o SGSI esteja integrado aos processos da organização e à sua estrutura global.
A respeito dessas asserções, assinale a opção correta.
(ENADE 2017)',
     NULL, N'As duas asserções são verdadeiras. A política de segurança é o conjunto formalizado de normas e procedimentos que acompanha a criação do setor de segurança, e o SGSI preserva confidencialidade, integridade e disponibilidade por meio da gestão de riscos. Porém, a segunda asserção descreve o SGSI e não explica por que o setor exige uma política de segurança; por isso não justifica a primeira.', 1, 8, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (391, 'A', N'As asserções I e II são proposições verdadeiras, e a II justifica a I.', 0, 79),
    (392, 'B', N'As asserções I e II são proposições verdadeiras, e a II não justifica a I.', 1, 79),
    (393, 'C', N'A asserção I é uma proposição verdadeira, e a II é uma proposição falsa.', 0, 79),
    (394, 'D', N'A asserção I é uma proposição falsa, e a II é uma proposição verdadeira.', 0, 79),
    (395, 'E', N'As asserções I e II são proposições falsas.', 0, 79);
SET IDENTITY_INSERT dbo.Alternativa OFF;

-- Questão 80 | Gestão de Projetos, Governança e Segurança da Informação | correta: B
SET IDENTITY_INSERT dbo.Questao ON;
INSERT INTO dbo.Questao (Id_Questao, Enunciado_Questao, ImagemUrl_Questao, ExplicacaoResposta_Questao, Ativo_Questao, Id_Disciplina, Id_Usuario) VALUES
    (80, N'Leia o texto a seguir.
A Lei N° 12.965, de 23 de abril de 2014, originalmente denominada Marco Civil da Internet, estabelece princípios, garantias, direitos e deveres para o uso da Internet no Brasil, contribuindo principalmente nas relações de consumo e segurança da informação pessoal para com os prestadores de serviço de acesso à Internet.
BRASIL. Lei N° 12.965, de 23 de abril de 2014.
Suponha que o cliente de determinado provedor de acesso à Internet contrate um serviço com velocidade de conexão de 5 Megabits/s (Mbps), mas tenha continuamente enfrentado problemas de perda de conexão e, adicionalmente, não obtém a velocidade contratada, apesar de o cliente realizar todos os pagamentos em dia.
Nessa situação, avalie as seguintes asserções e a relação proposta entre elas.
I. A manutenção da qualidade contratada da conexão à Internet é uma obrigação prevista no Marco Civil da Internet.
PORQUE
II. Não pode haver suspensão da conexão à Internet, salvo por débito diretamente decorrente de sua utilização.
A respeito dessas asserções, assinale a opção correta.
(ENADE 2014)',
     NULL, NULL, 1, 8, 1);
SET IDENTITY_INSERT dbo.Questao OFF;
SET IDENTITY_INSERT dbo.Alternativa ON;
INSERT INTO dbo.Alternativa (Id_Alternativa, Letra_Alternativa, Texto_Alternativa, Correta_Alternativa, Id_Questao) VALUES
    (396, 'A', N'As asserções I e II são proposições verdadeiras, e a asserção II justifica a I.', 0, 80),
    (397, 'B', N'As asserções I e II são proposições verdadeiras, e a asserção II não justifica a I.', 1, 80),
    (398, 'C', N'A asserção I é uma proposição verdadeira, e a II é uma proposição falsa.', 0, 80),
    (399, 'D', N'A asserção I é uma proposição falsa, e a II é uma proposição verdadeira.', 0, 80),
    (400, 'E', N'As asserções I e II são proposições falsas.', 0, 80);
SET IDENTITY_INSERT dbo.Alternativa OFF;

/* ---------------------------------------------------------------- Histórico de simulados (RN11, RN20, RN21)
   PontosObtidos = acertos x 10. PontuacaoTotal_Usuario = soma dos PontosObtidos. */
-- Simulado 1: Ana Beatriz Souza | 10 questões | 7 acertos
SET IDENTITY_INSERT dbo.Simulado ON;
INSERT INTO dbo.Simulado (Id_Simulado, Titulo_Simulado, QuantidadeQuestoes_Simulado, DataCriacao_Simulado, Id_Usuario) VALUES
    (1, N'Simulado de 10 questões - 14/10/2026', 10, '2026-10-14T19:30:00', 5);
SET IDENTITY_INSERT dbo.Simulado OFF;
INSERT INTO dbo.SimuladoQuestao (Id_Simulado, Id_Questao, Ordem) VALUES
    (1, 1, 1),
    (1, 31, 2),
    (1, 51, 3),
    (1, 3, 4),
    (1, 34, 5),
    (1, 53, 6),
    (1, 5, 7),
    (1, 36, 8),
    (1, 55, 9),
    (1, 58, 10);
SET IDENTITY_INSERT dbo.RespostaAluno ON;
INSERT INTO dbo.RespostaAluno (Id_RespostaAluno, AlternativaMarcada_RespostaAluno, Acertou_RespostaAluno, DataResposta_RespostaAluno, Id_Usuario, Id_Simulado, Id_Questao) VALUES
    (1, 'D', 1, '2026-10-14T19:52:10', 5, 1, 1),
    (2, 'A', 1, '2026-10-14T19:52:10', 5, 1, 31),
    (3, 'E', 1, '2026-10-14T19:52:10', 5, 1, 51),
    (4, 'C', 1, '2026-10-14T19:52:10', 5, 1, 3),
    (5, 'A', 0, '2026-10-14T19:52:10', 5, 1, 34),
    (6, NULL, 0, '2026-10-14T19:52:10', 5, 1, 53),   -- em branco: conta como erro (RN18)
    (7, 'C', 1, '2026-10-14T19:52:10', 5, 1, 5),
    (8, 'D', 1, '2026-10-14T19:52:10', 5, 1, 36),
    (9, 'A', 0, '2026-10-14T19:52:10', 5, 1, 55),
    (10, 'A', 1, '2026-10-14T19:52:10', 5, 1, 58);
SET IDENTITY_INSERT dbo.RespostaAluno OFF;
SET IDENTITY_INSERT dbo.ResultadoSimulado ON;
INSERT INTO dbo.ResultadoSimulado (Id_ResultadoSimulado, TotalQuestoes_ResultadoSimulado, TotalAcertos_ResultadoSimulado,
                                   Percentual_ResultadoSimulado, PontosObtidos_ResultadoSimulado, DataRealizacao_ResultadoSimulado, Id_Usuario, Id_Simulado) VALUES
    (1, 10, 7, 70.00, 70, '2026-10-14T19:52:10', 5, 1);
SET IDENTITY_INSERT dbo.ResultadoSimulado OFF;

-- Simulado 2: Henrique Silva Barbosa | 15 questões | 11 acertos
SET IDENTITY_INSERT dbo.Simulado ON;
INSERT INTO dbo.Simulado (Id_Simulado, Titulo_Simulado, QuantidadeQuestoes_Simulado, DataCriacao_Simulado, Id_Usuario) VALUES
    (2, N'Simulado de 15 questões - 20/10/2026', 15, '2026-10-20T20:05:00', 12);
SET IDENTITY_INSERT dbo.Simulado OFF;
INSERT INTO dbo.SimuladoQuestao (Id_Simulado, Id_Questao, Ordem) VALUES
    (2, 11, 1),
    (2, 21, 2),
    (2, 61, 3),
    (2, 71, 4),
    (2, 12, 5),
    (2, 23, 6),
    (2, 63, 7),
    (2, 72, 8),
    (2, 14, 9),
    (2, 26, 10),
    (2, 65, 11),
    (2, 75, 12),
    (2, 17, 13),
    (2, 68, 14),
    (2, 79, 15);
SET IDENTITY_INSERT dbo.RespostaAluno ON;
INSERT INTO dbo.RespostaAluno (Id_RespostaAluno, AlternativaMarcada_RespostaAluno, Acertou_RespostaAluno, DataResposta_RespostaAluno, Id_Usuario, Id_Simulado, Id_Questao) VALUES
    (11, 'E', 1, '2026-10-20T20:38:45', 12, 2, 11),
    (12, 'D', 1, '2026-10-20T20:38:45', 12, 2, 21),
    (13, 'C', 1, '2026-10-20T20:38:45', 12, 2, 61),
    (14, 'B', 1, '2026-10-20T20:38:45', 12, 2, 71),
    (15, 'B', 0, '2026-10-20T20:38:45', 12, 2, 12),
    (16, 'B', 1, '2026-10-20T20:38:45', 12, 2, 23),
    (17, 'E', 1, '2026-10-20T20:38:45', 12, 2, 63),
    (18, 'D', 1, '2026-10-20T20:38:45', 12, 2, 72),
    (19, 'C', 1, '2026-10-20T20:38:45', 12, 2, 14),
    (20, NULL, 0, '2026-10-20T20:38:45', 12, 2, 26),   -- em branco: conta como erro (RN18)
    (21, 'A', 0, '2026-10-20T20:38:45', 12, 2, 65),
    (22, 'A', 1, '2026-10-20T20:38:45', 12, 2, 75),
    (23, 'B', 1, '2026-10-20T20:38:45', 12, 2, 17),
    (24, 'C', 1, '2026-10-20T20:38:45', 12, 2, 68),
    (25, 'A', 0, '2026-10-20T20:38:45', 12, 2, 79);
SET IDENTITY_INSERT dbo.RespostaAluno OFF;
SET IDENTITY_INSERT dbo.ResultadoSimulado ON;
INSERT INTO dbo.ResultadoSimulado (Id_ResultadoSimulado, TotalQuestoes_ResultadoSimulado, TotalAcertos_ResultadoSimulado,
                                   Percentual_ResultadoSimulado, PontosObtidos_ResultadoSimulado, DataRealizacao_ResultadoSimulado, Id_Usuario, Id_Simulado) VALUES
    (2, 15, 11, 73.33, 110, '2026-10-20T20:38:45', 12, 2);
SET IDENTITY_INSERT dbo.ResultadoSimulado OFF;

/* ---------------------------------------------------------------- Desempenho por aluno e disciplina */
SET IDENTITY_INSERT dbo.Desempenho ON;
INSERT INTO dbo.Desempenho (Id_Desempenho, TotalRespostas_Desempenho, TotalAcertos_Desempenho, Percentual_Desempenho,
                            UltimaAtualizacao_Desempenho, Id_Usuario, Id_Disciplina) VALUES
    (1, 3, 3, 100.00, '2026-10-14T19:52:10', 5, 1),   -- Ana Beatriz Souza | Engenharia de Software
    (2, 3, 2, 66.67, '2026-10-14T19:52:10', 5, 4),   -- Ana Beatriz Souza | Algoritmos e Lógica de Programação
    (3, 4, 2, 50.00, '2026-10-14T19:52:10', 5, 6),   -- Ana Beatriz Souza | Banco de Dados
    (4, 4, 3, 75.00, '2026-10-20T20:38:45', 12, 2),   -- Henrique Silva Barbosa | Requisitos e Qualidade de Software
    (5, 3, 2, 66.67, '2026-10-20T20:38:45', 12, 3),   -- Henrique Silva Barbosa | Análise e Modelagem de Sistemas
    (6, 4, 3, 75.00, '2026-10-20T20:38:45', 12, 7),   -- Henrique Silva Barbosa | Redes de Computadores e Sistemas Operacionais
    (7, 4, 3, 75.00, '2026-10-20T20:38:45', 12, 8);   -- Henrique Silva Barbosa | Gestão de Projetos, Governança e Segurança da Informação
SET IDENTITY_INSERT dbo.Desempenho OFF;

COMMIT TRANSACTION;
GO

/* ---------------------------------------------------------------- Conferência
   A pontuação acumulada deve ser igual à soma dos PontosObtidos (MER 5.1). */
SELECT u.Nome_Usuario,
       u.PontuacaoTotal_Usuario,
       SUM(r.PontosObtidos_ResultadoSimulado) AS SomaPontosObtidos
FROM dbo.Usuario u
JOIN dbo.ResultadoSimulado r ON r.Id_Usuario = u.Id_Usuario
GROUP BY u.Nome_Usuario, u.PontuacaoTotal_Usuario;
GO

PRINT N'04_dados_teste.sql concluído.';
GO

SET NOEXEC OFF;
GO
