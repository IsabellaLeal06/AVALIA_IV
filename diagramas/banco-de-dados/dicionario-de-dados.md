# Dicionário de Dados

Complementa o MER v2 e o DER v2.1 com a descrição detalhada de cada tabela do banco de dados relacional, incluindo tipos, restrições, valores padrão e observações relevantes para a implementação e a manutenção do sistema.

**Versão 2.1 — PIM IV.** Este documento descreve o banco **efetivamente implementado** nos scripts da pasta `database/`. Onde houver divergência entre este documento e os scripts, prevalecem os scripts.

| | |
|---|---|
| **SGBD** | Azure SQL Database / SQL Server |
| **Tabelas** | 13 |
| **Procedures** | 4 |
| **Triggers** | 5 |
| **Base documental** | MER v2, DER v2.1 (brModelo), Requisitos e Regras de Negócio |

## Scripts de criação

Executar na ordem numérica. O script `00_reset.sql` apaga tudo e só roda em ambiente de teste, após alteração manual da variável de confirmação.

| Script | Conteúdo |
|---|---|
| `00_reset.sql` | Remove todas as tabelas (**somente teste**) |
| `01_ddl.sql` | 13 tabelas, chaves primárias e valores padrão |
| `02_restricoes.sql` | UNIQUE, CHECK e FOREIGN KEY |
| `03_carga_inicial.sql` | Perfis e medalhas (obrigatório em qualquer ambiente) |
| `04_dados_teste.sql` | Massa de dados para demonstração |
| `05_procedures.sql` | 4 procedures de consulta e conferência |
| `06_triggers.sql` | 5 triggers de regra de negócio |

## Convenção de nomes

As colunas seguem o padrão **`NomeAtributo_NomeEntidade`** (`Nome_Usuario`, `Ativo_Turma`), o mesmo adotado no modelo conceitual. As chaves estrangeiras conservam o nome da chave primária referenciada (`Id_Disciplina` em `Questao`).

Os nomes compostos de **entidade** perdem o sublinhado no modelo físico:

| Modelo conceitual (MER) | Modelo físico (banco) |
|---|---|
| `Resposta_Aluno` | `RespostaAluno` |
| `Resultado_Simulado` | `ResultadoSimulado` |
| `Simulado_Questao` | `SimuladoQuestao` |
| `Turma_Disciplina` | `TurmaDisciplina` |

---

# Tabela: Perfil

Representa os tipos de usuário do sistema. O perfil determina as funcionalidades disponíveis e as plataformas às quais a conta tem acesso.

| Coluna | Tipo | Restrições | PK | FK | Nulo | Observações |
|---|---|---|---|---|---|---|
| Id_Perfil | INT | IDENTITY(1,1) | ✓ | — | NÃO | Identificador único do perfil |
| Nome_Perfil | NVARCHAR(50) | — | — | — | NÃO | Nome do perfil |

**Carga inicial — os identificadores são fixos**

| Id_Perfil | Nome_Perfil |
|---|---|
| 1 | Aluno |
| 2 | Professor |
| 3 | Administrador |

**Observações**

- O RF01 estabelece que os três perfis são fixos; não há tela de inclusão.
- Os identificadores **1, 2 e 3 não podem ser alterados**: são referenciados por constante nas quatro aplicações (API, web, desktop e mobile) e pelas procedures do script 05, que filtram alunos por `Id_Perfil = 1`.
- A coluna é `IDENTITY`, mas a carga inicial usa `SET IDENTITY_INSERT` para fixar os valores.

---

# Tabela: Turma

Agrupa os alunos da instituição. A turma é a unidade utilizada nos relatórios de desempenho consultados por professores e pelo administrador.

| Coluna | Tipo | Restrições | PK | FK | Nulo | Observações |
|---|---|---|---|---|---|---|
| Id_Turma | INT | IDENTITY(1,1) | ✓ | — | NÃO | Identificador único da turma |
| Nome_Turma | NVARCHAR(100) | UQ_Turma_NomeAno | — | — | NÃO | Nome ou código da turma |
| AnoLetivo_Turma | INT | UQ_Turma_NomeAno | — | — | NÃO | Ano letivo ao qual a turma pertence |
| Ativo_Turma | BIT | DF_Turma_Ativo → 1 | — | — | NÃO | Turma ativa (1) ou desativada (0) |

**Observações**

- **`AnoLetivo_Turma`** distingue turmas homônimas de anos diferentes — a turma 3A de 2026 e a de 2027. Sem ele, os relatórios históricos ficariam ambíguos. Fundamento: RN06.
- A unicidade é **composta**: `UQ_Turma_NomeAno (Nome_Turma, AnoLetivo_Turma)`. A mesma turma não pode ser cadastrada duas vezes no mesmo ano, mas pode se repetir em anos distintos, o que é o caso normal.
- **Não há coluna de semestre.** Conforme a RN06, o período letivo é incorporado ao próprio nome da turma.
- **`Ativo_Turma`** implementa a exclusão lógica da RN12. A turma é referenciada pelos alunos e sustenta os relatórios dos períodos anteriores, de modo que a remoção física romperia o histórico acadêmico por via indireta. A turma desativada deixa de ser oferecida na vinculação de novos alunos e permanece nos relatórios existentes.

---

# Tabela: Usuario

Representa qualquer usuário autenticado da plataforma. `Id_Perfil` determina o tipo e as permissões; `Id_Turma` é preenchido apenas para alunos.

| Coluna | Tipo | Restrições | PK | FK | Nulo | Observações |
|---|---|---|---|---|---|---|
| Id_Usuario | INT | IDENTITY(1,1) | ✓ | — | NÃO | Identificador único do usuário |
| Nome_Usuario | NVARCHAR(150) | — | — | — | NÃO | Nome completo |
| CPF_Usuario | CHAR(11) | UQ_Usuario_CPF, CK_Usuario_CPF | — | — | NÃO | CPF, somente dígitos |
| Email_Usuario | VARCHAR(150) | UQ_Usuario_Email | — | — | NÃO | E-mail de autenticação |
| Senha_Usuario | VARCHAR(200) | — | — | — | NÃO | Hash BCrypt da senha |
| FotoUrl_Usuario | VARCHAR(500) | — | — | — | SIM | Caminho da foto de perfil |
| DataIngresso_Usuario | DATE | — | — | — | NÃO | Data de ingresso na instituição |
| Ativo_Usuario | BIT | DF_Usuario_Ativo → 1 | — | — | NÃO | Conta ativa (1) ou inativa (0) |
| PontuacaoTotal_Usuario | INT | DF_Usuario_PontuacaoTotal → 0 | — | — | NÃO | Pontos acumulados na gamificação |
| TokenRecuperacao_Usuario | VARCHAR(200) | — | — | — | SIM | Hash do código de redefinição de senha |
| ValidadeToken_Usuario | DATETIME | — | — | — | SIM | Expiração do código de redefinição |
| Id_Perfil | INT | → Perfil | — | ✓ | NÃO | Perfil de acesso |
| Id_Turma | INT | → Turma | — | ✓ | SIM | Turma do aluno |

**Observações**

- **`CPF_Usuario`** é informado pelo Administrador no cadastro e é único por identificar a pessoa de forma inequívoca (RN27). Armazenado em comprimento fixo, somente os onze dígitos, sem pontos nem traço — o que torna a verificação de unicidade previsível. `CK_Usuario_CPF` garante o formato; a validação do dígito verificador permanece na aplicação.
- **`Senha_Usuario`** armazena o **hash BCrypt**, nunca a senha legível nem valor criptografado. A distinção importa: criptografia é reversível mediante chave, hash não é. O BCrypt gera sal próprio e é deliberadamente lento, o que o torna adequado a senhas. Fundamento: RNF01.
- **Senha inicial.** Conforme a RN02, a senha inicial de todo usuário é o seu CPF, e a troca é recomendada no primeiro acesso, sem ser obrigatória. Não há coluna registrando se a troca ocorreu: a condição é derivável pela conferência do hash contra o CPF.
- **`PontuacaoTotal_Usuario`** é dado derivável de `ResultadoSimulado`, mantido em coluna própria por determinação do RNF04, para evitar o recálculo do histórico a cada exibição do perfil. É atualizado pelo trigger `trg_ResultadoSimulado_AcumularPontuacao` e protegido contra redução pelo trigger `trg_Usuario_PontuacaoNaoDiminui` (RN21). A procedure `sp_ConferirPontuacao` audita a correspondência com a soma dos resultados.
- **`TokenRecuperacao_Usuario` e `ValidadeToken_Usuario`** atendem à RN03, que prevê código único com prazo de validade enviado ao e-mail cadastrado. Permanecem `NULL` quando não há solicitação pendente, e são limpos após a redefinição, o que assegura o uso único. Armazena-se o hash do código, não o código legível, pelo mesmo motivo da senha.
- **`DataIngresso_Usuario`** é `DATE`, e não `DATETIME`: o horário do cadastro não tem uso em nenhum requisito.
- **`Ativo_Usuario`** implementa a exclusão lógica da RN12. A conta desativada deixa de autenticar, mas permanece registrada e pode ser reativada. Conforme a RN25, alunos desativados ficam **fora do cálculo das médias das turmas**, embora o histórico permaneça armazenado.
- **`Id_Turma` admite NULL** porque apenas contas de Aluno possuem turma (RN05). Professor e Administrador não pertencem a turma.
- **Não há coluna de medalha.** A medalha do aluno é apurada por consulta à tabela `Medalha`. Ver a seção correspondente.

---

# Tabela: Disciplina

Representa uma disciplina acadêmica da plataforma. Organiza o acervo de questões e é a unidade que o aluno seleciona ao gerar um simulado.

| Coluna | Tipo | Restrições | PK | FK | Nulo | Observações |
|---|---|---|---|---|---|---|
| Id_Disciplina | INT | IDENTITY(1,1) | ✓ | — | NÃO | Identificador único da disciplina |
| Nome_Disciplina | NVARCHAR(100) | UQ_Disciplina_Nome | — | — | NÃO | Nome da disciplina |
| Descricao_Disciplina | NVARCHAR(500) | — | — | — | SIM | Descrição complementar |
| Ativo_Disciplina | BIT | DF_Disciplina_Ativo → 1 | — | — | NÃO | Disciplina ativa (1) ou desativada (0) |
| Id_Usuario | INT | → Usuario | — | ✓ | SIM | Professor responsável |

**Observações**

- **`Id_Usuario`** identifica, neste contexto, o **professor responsável** pela disciplina, referenciando um usuário de perfil Professor. Admite `NULL` porque a RN07 prevê expressamente que a disciplina permaneça temporariamente sem responsável — no intervalo entre a saída de um professor e a designação de outro, ou quando a disciplina é cadastrada antes da contratação do docente.
- Disciplina sem responsável **continua disponível aos alunos**, já que as questões pertencem à disciplina e não ao professor (RN10). Fica suspenso apenas o cadastro de novas questões daquela disciplina.
- **`Nome_Disciplina` é único** para evitar cadastros duplicados do mesmo conteúdo (RN27).
- **`Ativo_Disciplina`** implementa a exclusão lógica da RN12. A disciplina é referenciada pelas questões e, por meio delas, pelos simulados já realizados, de modo que a remoção física romperia o histórico por via indireta. A disciplina desativada deixa de ser oferecida na geração de simulados e no cadastro de novas questões, permanecendo nos registros existentes.

---

# Tabela: TurmaDisciplina

Entidade associativa que resolve o relacionamento N:N entre `Turma` e `Disciplina`. Representa as disciplinas que compõem a grade curricular de cada turma.

| Coluna | Tipo | Restrições | PK | FK | Nulo | Observações |
|---|---|---|---|---|---|---|
| Id_Turma | INT | → Turma | ✓ | ✓ | NÃO | Turma |
| Id_Disciplina | INT | → Disciplina | ✓ | ✓ | NÃO | Disciplina cursada pela turma |

**Observações**

- A chave primária composta impede o registro duplicado da mesma disciplina para a mesma turma.
- Esta tabela sustenta a consulta prevista na RN24: o professor visualiza o desempenho consolidado das turmas que cursam as disciplinas sob sua responsabilidade, percorrendo o caminho `Disciplina → TurmaDisciplina → Turma`.

---

# Tabela: Questao

Representa uma questão do banco de questões da plataforma, organizada por disciplina. Cada questão possui cinco alternativas de resposta.

| Coluna | Tipo | Restrições | PK | FK | Nulo | Observações |
|---|---|---|---|---|---|---|
| Id_Questao | INT | IDENTITY(1,1) | ✓ | — | NÃO | Identificador único da questão |
| Enunciado_Questao | NVARCHAR(MAX) | — | — | — | NÃO | Texto do enunciado |
| ImagemUrl_Questao | VARCHAR(500) | — | — | — | SIM | Caminho de imagem complementar |
| ExplicacaoResposta_Questao | NVARCHAR(MAX) | — | — | — | SIM | Explicação exibida após a correção |
| Ativo_Questao | BIT | DF_Questao_Ativo → 1 | — | — | NÃO | Disponível para sorteio (1) ou desativada (0) |
| Id_Disciplina | INT | → Disciplina | — | ✓ | NÃO | Disciplina à qual pertence |
| Id_Usuario | INT | → Usuario | — | ✓ | NÃO | Usuário que cadastrou a questão |

**Observações**

- **`Id_Usuario`** registra a **autoria** da questão, exigida pela RN08 para fins de rastreabilidade, e é exibida na listagem de questões — razão pela qual a coluna é obrigatória.
- O vínculo expressa **autoria, não propriedade**. Conforme a RN10, a questão pertence à disciplina, e a substituição do professor responsável não transfere nem altera as questões já cadastradas. O banco de questões é cumulativo e cresce ao longo dos períodos letivos.
- **`Ativo_Questao`** implementa a exclusão lógica da RN12. A exclusão física impediria a revisão dos simulados em que a questão já apareceu. A questão desativada deixa de participar do sorteio (RN15), e a verificação de quantidade suficiente da RN14 incide somente sobre questões ativas, mas ela continua visível na revisão dos simulados anteriores.
- **`ExplicacaoResposta_Questao` é opcional**, por decisão registrada no projeto.
- **Não há coluna de dificuldade.** A RN09 afirma expressamente que a questão não possui classificação por nível de dificuldade.
- **Gabarito imutável após uso.** A RN11 proíbe alterar qual alternativa é a correta em questões já utilizadas em simulado. A proibição é garantida pelo trigger `trg_Alternativa_CorretaUtilizada`, descrito adiante. Os demais campos permanecem editáveis.

---

# Tabela: Alternativa

Representa as alternativas de resposta de uma questão. Toda questão possui cinco alternativas, identificadas de A a E, das quais apenas uma é correta.

| Coluna | Tipo | Restrições | PK | FK | Nulo | Observações |
|---|---|---|---|---|---|---|
| Id_Alternativa | INT | IDENTITY(1,1) | ✓ | — | NÃO | Identificador único da alternativa |
| Letra_Alternativa | CHAR(1) | CK_Alternativa_Letra, UQ_Alternativa_QuestaoLetra | — | — | NÃO | Letra da alternativa |
| Texto_Alternativa | NVARCHAR(MAX) | — | — | — | NÃO | Texto da alternativa |
| Correta_Alternativa | BIT | DF_Alternativa_Correta → 0 | — | — | NÃO | Alternativa correta (1) ou incorreta (0) |
| Id_Questao | INT | → Questao, UQ_Alternativa_QuestaoLetra | — | ✓ | NÃO | Questão à qual pertence |

**Observações**

- **`CK_Alternativa_Letra`** restringe a coluna aos valores `A`, `B`, `C`, `D` e `E` (RN09).
- **`UQ_Alternativa_QuestaoLetra (Id_Questao, Letra_Alternativa)`** impede duas alternativas com a mesma letra na mesma questão (RN27).
- **Regra não expressável por restrição de coluna.** A RN09 exige **exatamente cinco** alternativas por questão, com **apenas uma** correta. As duas condições envolvem o conjunto de linhas, e não uma coluna isolada, e por isso são validadas pela aplicação — no Desktop e na API.
- O trigger `trg_Alternativa_CorretaUtilizada` impede a troca da alternativa correta em questão já utilizada em simulado (RN11).

---

# Tabela: Simulado

Representa um simulado gerado para um aluno. Cada solicitação produz um simulado novo, com questões sorteadas do acervo das disciplinas escolhidas.

| Coluna | Tipo | Restrições | PK | FK | Nulo | Observações |
|---|---|---|---|---|---|---|
| Id_Simulado | INT | IDENTITY(1,1) | ✓ | — | NÃO | Identificador único do simulado |
| Titulo_Simulado | NVARCHAR(200) | — | — | — | NÃO | Título descritivo |
| QuantidadeQuestoes_Simulado | INT | CK_Simulado_QuantidadeQuestoes | — | — | NÃO | Total de questões: 10, 15 ou 20 |
| DataCriacao_Simulado | DATETIME | — | — | — | NÃO | Data e hora da geração |
| Id_Usuario | INT | → Usuario | — | ✓ | NÃO | Aluno que solicitou a geração |

**Observações**

- **Não há coluna de disciplina.** A RN13 estabelece que o simulado abrange duas ou mais disciplinas, escolhidas pelo aluno, e determina que elas **não sejam armazenadas em campo próprio**: são identificadas a partir das disciplinas das questões que o compõem, uma vez que cada questão já pertence a uma disciplina. O caminho de leitura é:

  ```
  Simulado → SimuladoQuestao → Questao → Disciplina
  ```

  ```sql
  SELECT DISTINCT d.Id_Disciplina, d.Nome_Disciplina
  FROM dbo.SimuladoQuestao sq
  JOIN dbo.Questao     q ON q.Id_Questao    = sq.Id_Questao
  JOIN dbo.Disciplina  d ON d.Id_Disciplina = q.Id_Disciplina
  WHERE sq.Id_Simulado = @IdSimulado;
  ```

- **Consequência da derivação.** A opção evita redundância, já que a informação existe no acervo, mas uma disciplina selecionada pelo aluno cuja composição final não tenha contemplado nenhuma questão não aparece no conjunto derivado. A RN15 admite essa possibilidade ao estabelecer sorteio livre entre as disciplinas escolhidas, sem divisão fixa de quantidade entre elas.
- A **seleção mínima de duas disciplinas** é verificada na aplicação, no momento da solicitação, por não haver campo onde registrá-la.
- **`Id_Usuario`** identifica o aluno que solicitou a geração. O vínculo pertence a esta tabela, e não apenas às tabelas de resposta e resultado, porque a RN18 determina que o registro das respostas ocorra somente na finalização. Um simulado gerado e abandonado (RN16) não produz linha em `RespostaAluno` nem em `ResultadoSimulado` e, sem esta coluna, ficaria órfão na base.
- Três tabelas referenciam `Usuario` no ciclo de vida do simulado, sem redundância entre si: aqui identifica **quem solicitou a geração**; em `RespostaAluno`, **quem respondeu**; em `ResultadoSimulado`, **quem concluiu**. São momentos distintos, e apenas o primeiro existe antes da finalização.
- **`QuantidadeQuestoes_Simulado`** refere-se ao **total** do simulado, não a cada disciplina (RN14). Nenhum outro valor além de 10, 15 e 20 é aceito.
- **O tempo de realização não é armazenado.** A RN17 estabelece que o cronômetro tem função meramente informativa e não integra o resultado nem o histórico.

---

# Tabela: SimuladoQuestao

Entidade associativa que resolve o relacionamento N:N entre `Simulado` e `Questao`. Armazena também a ordem de apresentação de cada questão.

| Coluna | Tipo | Restrições | PK | FK | Nulo | Observações |
|---|---|---|---|---|---|---|
| Id_Simulado | INT | → Simulado | ✓ | ✓ | NÃO | Simulado |
| Id_Questao | INT | → Questao | ✓ | ✓ | NÃO | Questão que integra o simulado |
| Ordem | INT | — | — | — | NÃO | Posição de apresentação da questão |

**Observações**

- **`Ordem` é obrigatória.** A RN15 determina que a ordem de apresentação seja registrada junto com a composição do simulado, já que as questões são sorteadas individualmente a cada solicitação e não seguem sequência fixa.
- A chave primária composta atende à exigência de **sorteio sem repetição** da RN15: a mesma questão não pode aparecer duas vezes no mesmo simulado.
- Esta tabela sustenta a derivação das disciplinas do simulado, descrita acima, e é também a referência usada pelo trigger `trg_Alternativa_CorretaUtilizada` para determinar se uma questão já foi utilizada.
- **Registro imutável.** Conforme a RN19, a composição do simulado torna-se imutável após a correção. A exclusão lógica da RN12 não se aplica a esta tabela, razão pela qual não há coluna `Ativo`.

---

# Tabela: RespostaAluno

Registra a resposta individual do aluno a cada questão do simulado.

| Coluna | Tipo | Restrições | PK | FK | Nulo | Observações |
|---|---|---|---|---|---|---|
| Id_RespostaAluno | INT | IDENTITY(1,1) | ✓ | — | NÃO | Identificador único da resposta |
| AlternativaMarcada_RespostaAluno | CHAR(1) | CK_RespostaAluno_AlternativaMarcada | — | — | SIM | Letra selecionada pelo aluno |
| Acertou_RespostaAluno | BIT | — | — | — | NÃO | Acerto (1) ou erro (0) |
| DataResposta_RespostaAluno | DATETIME | — | — | — | NÃO | Data e hora do registro |
| Id_Usuario | INT | → Usuario | — | ✓ | NÃO | Aluno que respondeu |
| Id_Simulado | INT | → Simulado | — | ✓ | NÃO | Simulado a que se refere |
| Id_Questao | INT | → Questao | — | ✓ | NÃO | Questão respondida |

**Observações**

- **`AlternativaMarcada_RespostaAluno`** guarda apenas a **letra**, não uma chave estrangeira para `Alternativa`. A substituição foi avaliada e descartada: teria alto impacto em uma tabela central, com reflexo em todas as rotinas de correção e revisão, sem ganho proporcional. A consistência é assegurada pela restrição de domínio.

  Efeito colateral favorável da decisão: a edição de uma questão pode recriar suas alternativas sem romper o histórico de respostas.

- Permanece **`NULL` quando a questão foi deixada em branco** (RN18).
- **`Acertou_RespostaAluno` é obrigatório.** A RN18 determina que a questão em branco seja contabilizada como erro, ou seja, valor `0`, sem gerar pontos. Não existe resposta registrada sem resultado apurado.
- **Momento do registro.** Conforme a RN18, o aluno pode alterar livremente as marcações durante a realização, e o registro definitivo ocorre na finalização. Até esse momento os dados ficam apenas na aplicação.
- **Imutabilidade garantida pelo banco.** A RN19 estabelece que os registros se tornam imutáveis após a correção. O trigger `trg_RespostaAluno_Imutavel` recusa qualquer `UPDATE` ou `DELETE` nesta tabela, com o erro 50022.

---

# Tabela: ResultadoSimulado

Registra o resultado consolidado de um aluno ao finalizar um simulado.

| Coluna | Tipo | Restrições | PK | FK | Nulo | Observações |
|---|---|---|---|---|---|---|
| Id_ResultadoSimulado | INT | IDENTITY(1,1) | ✓ | — | NÃO | Identificador único do resultado |
| TotalQuestoes_ResultadoSimulado | INT | — | — | — | NÃO | Total de questões do simulado |
| TotalAcertos_ResultadoSimulado | INT | — | — | — | NÃO | Número de questões corretas |
| Percentual_ResultadoSimulado | DECIMAL(5,2) | — | — | — | NÃO | Percentual de acertos |
| PontosObtidos_ResultadoSimulado | INT | — | — | — | NÃO | Pontos creditados na correção |
| DataRealizacao_ResultadoSimulado | DATETIME | — | — | — | NÃO | Data e hora da conclusão |
| Id_Usuario | INT | → Usuario | — | ✓ | NÃO | Aluno |
| Id_Simulado | INT | → Simulado, UQ_ResultadoSimulado_Simulado | — | ✓ | NÃO | Simulado corrigido, **único** |

**Observações**

- **`Id_Simulado` é único.** Esta é a restrição que torna efetiva a **tentativa única** da RN16: cada simulado gerado admite um só resultado. Sem ela, a regra existiria apenas na aplicação, e uma falha de validação permitiria dois resultados para o mesmo simulado. O aluno que deseja praticar novamente gera um simulado novo, sem limite diário.
- **`PontosObtidos_ResultadoSimulado`** registra os pontos efetivamente creditados na correção, exigidos pelo RF05 na tela de resultado. O valor é hoje calculável como `TotalAcertos × 10` (RN21), mas é armazenado para preservar a fidelidade do histórico: se a instituição alterar o valor do ponto, os resultados anteriores continuam exibindo a pontuação creditada à época.
- **A inserção aciona o acúmulo de pontuação.** O trigger `trg_ResultadoSimulado_AcumularPontuacao` soma os pontos a `Usuario.PontuacaoTotal_Usuario`. A soma dos `PontosObtidos` de um aluno corresponde, portanto, à sua pontuação total — correspondência auditada pela procedure `sp_ConferirPontuacao`.
- **Imutabilidade garantida pelo banco.** O trigger `trg_ResultadoSimulado_Imutavel` recusa `UPDATE` e `DELETE`, com o erro 50023 (RN19). A correção é automática e definitiva, sem intervenção do professor.
- **Registro permanente.** A RN20 determina que todos os simulados realizados permaneçam no histórico do aluno por tempo indeterminado, independentemente da plataforma utilizada.

---

# Tabela: Desempenho

Armazena o desempenho acumulado de cada aluno por disciplina. Atualizado após a correção de cada simulado.

| Coluna | Tipo | Restrições | PK | FK | Nulo | Observações |
|---|---|---|---|---|---|---|
| Id_Desempenho | INT | IDENTITY(1,1) | ✓ | — | NÃO | Identificador único do registro |
| TotalRespostas_Desempenho | INT | — | — | — | NÃO | Total de questões respondidas |
| TotalAcertos_Desempenho | INT | — | — | — | NÃO | Total de acertos na disciplina |
| Percentual_Desempenho | DECIMAL(5,2) | — | — | — | NÃO | Percentual de acertos |
| UltimaAtualizacao_Desempenho | DATETIME | — | — | — | NÃO | Data da última atualização |
| Id_Usuario | INT | → Usuario | — | ✓ | NÃO | Aluno |
| Id_Disciplina | INT | → Disciplina | — | ✓ | NÃO | Disciplina apurada |

**Observações**

- A tabela armazena dados **deriváveis** de `RespostaAluno`. É desnormalização motivada por desempenho, conforme o RNF04, que fixa o tempo máximo de resposta das consultas.
- É a granularidade por disciplina desta tabela que viabiliza o desempenho segmentado exigido pelo RF05 no resultado de um simulado multidisciplinar, e é a fonte lida pela procedure `sp_DesempenhoAluno`.
- Conforme a RN25, alunos com conta desativada permanecem registrados aqui, mas ficam fora do cálculo das médias apresentadas ao Professor.
- **Ponto em aberto:** não existe restrição de unicidade sobre `(Id_Usuario, Id_Disciplina)`. Ver a seção "Pontos em aberto".

---

# Tabela: Medalha

Armazena as faixas de pontuação que definem o nível alcançado pelo aluno na gamificação.

| Coluna | Tipo | Restrições | PK | FK | Nulo | Observações |
|---|---|---|---|---|---|---|
| Id_Medalha | INT | IDENTITY(1,1) | ✓ | — | NÃO | Identificador único da medalha |
| Nome_Medalha | NVARCHAR(50) | UQ_Medalha_Nome | — | — | NÃO | Nome da medalha |
| PontosMinimos_Medalha | INT | UQ_Medalha_PontosMinimos | — | — | NÃO | Pontuação mínima da faixa |
| ImagemUrl_Medalha | VARCHAR(500) | — | — | — | NÃO | Imagem exibida no perfil do aluno |

**Carga inicial**

| Id_Medalha | Nome_Medalha | PontosMinimos_Medalha |
|---|---|---|
| 1 | Bronze | 250 |
| 2 | Prata | 1000 |
| 3 | Ouro | 2500 |

**Observações**

- **As faixas ficam em tabela, não fixas no código.** A RN22 prevê que a instituição ajuste os limites sem alteração nas aplicações web, desktop e mobile. Os valores acima são **iniciais, não imutáveis**.
- **`Nome_Medalha` e `PontosMinimos_Medalha` são únicos.** Duas faixas com o mesmo nome, ou com a mesma pontuação mínima, tornariam indeterminada a apuração da medalha corrente (RN27).
- **`ImagemUrl_Medalha` é obrigatório.** O RF07 e a RN23 determinam que o perfil exiba a imagem da medalha.
- **A tabela não possui chave estrangeira nem é referenciada por nenhuma outra**, e também **não possui coluna `Ativo`**: a RN12 lista apenas Usuário, Questão, Turma e Disciplina, de modo que aqui a exclusão é física.

## Apuração da medalha

A medalha **não é gravada** no cadastro do aluno. É apurada por consulta, comparando `Usuario.PontuacaoTotal_Usuario` com as faixas desta tabela, conforme determina a RN22.

**Motivo.** Gravar a medalha produziria dado derivado sujeito a desatualização. Como a RN22 permite que a instituição altere as faixas, qualquer alteração nos limites tornaria incorreta a medalha gravada em todos os alunos, até que uma rotina varresse a base inteira. Apurando por consulta, a alteração passa a valer imediatamente para todos, sem rotina alguma. Como a apuração envolve apenas a leitura de três registros e uma comparação numérica, o custo é irrelevante.

A lógica está implementada na procedure **`sp_PontuacaoAluno`**, que devolve em uma única chamada a medalha corrente, a próxima faixa e quantos pontos faltam para alcançá-la (RN23):

- **Medalha corrente** — faixa de maior `PontosMinimos_Medalha` que não excede a pontuação do aluno. Nenhum resultado indica aluno que ainda não alcançou a primeira faixa.
- **Próxima faixa** — faixa de menor `PontosMinimos_Medalha` superior à pontuação atual. Quando o aluno está na última faixa, `PontosFaltantes` vem como `0`.

---

# Restrições de integridade

Consolidação das restrições declaradas nas tabelas acima. Fundamento geral: RNF03, segundo o qual a integridade é garantida por chaves estrangeiras e restrições de domínio, e não apenas pela aplicação.

## Chaves primárias — 13

Uma por tabela. Compostas em `TurmaDisciplina (Id_Turma, Id_Disciplina)` e `SimuladoQuestao (Id_Simulado, Id_Questao)`; simples e `IDENTITY(1,1)` nas demais.

## Chaves estrangeiras — 18

Todas com **`NO ACTION`**, sem exclusão em cascata. A escolha é deliberada: o modelo adota exclusão lógica (RN12), de modo que a cascata nunca deveria ser acionada, e mantê-la desligada transforma uma eventual tentativa de exclusão física em erro explícito, em vez de remoção silenciosa de histórico acadêmico.

| Restrição | Tabela | Coluna | Referencia |
|---|---|---|---|
| FK_Usuario_Perfil | Usuario | Id_Perfil | Perfil |
| FK_Usuario_Turma | Usuario | Id_Turma | Turma |
| FK_Disciplina_Usuario | Disciplina | Id_Usuario | Usuario |
| FK_TurmaDisciplina_Turma | TurmaDisciplina | Id_Turma | Turma |
| FK_TurmaDisciplina_Disciplina | TurmaDisciplina | Id_Disciplina | Disciplina |
| FK_Questao_Disciplina | Questao | Id_Disciplina | Disciplina |
| FK_Questao_Usuario | Questao | Id_Usuario | Usuario |
| FK_Alternativa_Questao | Alternativa | Id_Questao | Questao |
| FK_Simulado_Usuario | Simulado | Id_Usuario | Usuario |
| FK_SimuladoQuestao_Simulado | SimuladoQuestao | Id_Simulado | Simulado |
| FK_SimuladoQuestao_Questao | SimuladoQuestao | Id_Questao | Questao |
| FK_RespostaAluno_Usuario | RespostaAluno | Id_Usuario | Usuario |
| FK_RespostaAluno_Simulado | RespostaAluno | Id_Simulado | Simulado |
| FK_RespostaAluno_Questao | RespostaAluno | Id_Questao | Questao |
| FK_ResultadoSimulado_Usuario | ResultadoSimulado | Id_Usuario | Usuario |
| FK_ResultadoSimulado_Simulado | ResultadoSimulado | Id_Simulado | Simulado |
| FK_Desempenho_Usuario | Desempenho | Id_Usuario | Usuario |
| FK_Desempenho_Disciplina | Desempenho | Id_Disciplina | Disciplina |

## Restrições de unicidade — 8

| Restrição | Tabela | Coluna(s) | Justificativa | Fundamento |
|---|---|---|---|---|
| UQ_Usuario_Email | Usuario | Email_Usuario | Credencial de autenticação | RF01 / RN27 |
| UQ_Usuario_CPF | Usuario | CPF_Usuario | Identificação inequívoca da pessoa | RN27 |
| UQ_Disciplina_Nome | Disciplina | Nome_Disciplina | Evita cadastro duplicado do mesmo conteúdo | RN27 |
| UQ_Turma_NomeAno | Turma | Nome_Turma, AnoLetivo_Turma | Turma única por ano letivo | RN06 / RN27 |
| UQ_Medalha_Nome | Medalha | Nome_Medalha | Evita faixas homônimas | RN27 |
| UQ_Medalha_PontosMinimos | Medalha | PontosMinimos_Medalha | Apuração determinada da medalha | RN22 / RN27 |
| UQ_Alternativa_QuestaoLetra | Alternativa | Id_Questao, Letra_Alternativa | Letra única dentro da mesma questão | RN27 |
| UQ_ResultadoSimulado_Simulado | ResultadoSimulado | Id_Simulado | Tentativa única por simulado | RN16 |

## Restrições de domínio — 4

| Restrição | Tabela.Coluna | Valores permitidos | Fundamento |
|---|---|---|---|
| CK_Simulado_QuantidadeQuestoes | Simulado.QuantidadeQuestoes_Simulado | 10, 15 ou 20 | RN14 |
| CK_Alternativa_Letra | Alternativa.Letra_Alternativa | A, B, C, D ou E | RN09 |
| CK_RespostaAluno_AlternativaMarcada | RespostaAluno.AlternativaMarcada_RespostaAluno | A, B, C, D, E ou NULL | RN18 |
| CK_Usuario_CPF | Usuario.CPF_Usuario | Exatamente 11 dígitos numéricos | RN01 |

## Valores padrão — 6

| Restrição | Coluna | Valor |
|---|---|---|
| DF_Turma_Ativo | Turma.Ativo_Turma | 1 |
| DF_Usuario_Ativo | Usuario.Ativo_Usuario | 1 |
| DF_Usuario_PontuacaoTotal | Usuario.PontuacaoTotal_Usuario | 0 |
| DF_Disciplina_Ativo | Disciplina.Ativo_Disciplina | 1 |
| DF_Questao_Ativo | Questao.Ativo_Questao | 1 |
| DF_Alternativa_Correta | Alternativa.Correta_Alternativa | 0 |

## Exclusão lógica

Quatro tabelas possuem coluna `Ativo`, conforme a RN12. As demais não a possuem por decisão expressa: os registros de simulados realizados, suas respostas, seus resultados e a composição das questões sorteadas são imutáveis após a correção (RN19) e nunca são excluídos nem desativados.

| Tabela | Coluna | Efeito da desativação |
|---|---|---|
| Usuario | Ativo_Usuario | Deixa de autenticar; sai do cálculo das médias de turma (RN25) |
| Turma | Ativo_Turma | Deixa de ser oferecida na vinculação de novos alunos |
| Disciplina | Ativo_Disciplina | Deixa de ser oferecida na geração de simulados e no cadastro de questões |
| Questao | Ativo_Questao | Sai do sorteio; continua visível na revisão de simulados anteriores |

---

# Procedures

Quatro procedures de **somente leitura**, todas recriáveis por `CREATE OR ALTER`. As aplicações acessam os dados pela API; as procedures servem à consulta direta no banco e à conferência.

| Procedure | Parâmetros | Finalidade | Fundamento |
|---|---|---|---|
| `sp_PontuacaoAluno` | `@IdUsuario` | Pontuação, medalha atual, próxima faixa e pontos faltantes | RN22, RN23 |
| `sp_DesempenhoAluno` | `@IdUsuario` | Acertos e percentual por disciplina | RF05, RF06 |
| `sp_HistoricoSimulados` | `@IdUsuario`, `@Pagina`, `@Tamanho` | Simulados finalizados, paginados, mais recentes primeiro | RN20 |
| `sp_ConferirPontuacao` | `@IdUsuario` (opcional) | Alunos cuja pontuação difere da soma dos resultados | RN21 |

**Observações**

- As três primeiras recusam identificador que não corresponda a um Aluno, com o erro **50010**.
- `sp_HistoricoSimulados` limita o tamanho da página a 50 registros e recusa valores fora da faixa com o erro **50011**.
- `sp_ConferirPontuacao` **não corrige nada**: devolve apenas as divergências. Nenhuma linha retornada significa base íntegra. É a auditoria da desnormalização descrita em `Usuario.PontuacaoTotal_Usuario`.

---

# Triggers

Cinco triggers, recriáveis por `CREATE OR ALTER`. Implementam as regras de negócio que não são expressáveis por restrição de coluna, desfazendo a transação e devolvendo erro próprio quando a regra é violada.

| Trigger | Tabela | Evento | Finalidade | Erro | Fundamento |
|---|---|---|---|---|---|
| `trg_ResultadoSimulado_AcumularPontuacao` | ResultadoSimulado | AFTER INSERT | Soma os pontos obtidos à pontuação total do aluno | — | RN21 |
| `trg_Usuario_PontuacaoNaoDiminui` | Usuario | AFTER UPDATE | Recusa qualquer redução da pontuação acumulada | 50021 | RN21 |
| `trg_RespostaAluno_Imutavel` | RespostaAluno | AFTER UPDATE, DELETE | Recusa alteração e exclusão de respostas | 50022 | RN19 |
| `trg_ResultadoSimulado_Imutavel` | ResultadoSimulado | AFTER UPDATE, DELETE | Recusa alteração e exclusão de resultados | 50023 | RN19 |
| `trg_Alternativa_CorretaUtilizada` | Alternativa | AFTER UPDATE | Impede trocar a alternativa correta de questão já usada em simulado | 50024 | RN11 |

**Observações relevantes à implementação**

- **Acúmulo de pontuação — divisão de responsabilidade com a API.** Com `trg_ResultadoSimulado_AcumularPontuacao` ativo, a API **não** soma a pontuação por conta própria: ela consulta `sys.triggers` e só executa a soma quando o trigger não existe ou está desabilitado. Isso evita contagem dobrada e permite que a API seja atualizada antes ou depois do script 06.
- **Concorrência.** O trigger soma apenas as linhas da tabela `inserted`, em um único `UPDATE` atômico. Duas finalizações simultâneas do mesmo aluno não se perdem, e nenhuma linha de outra transação é lida.
- **Imutabilidade.** Os triggers de `RespostaAluno` e `ResultadoSimulado` disparam mesmo quando nenhuma linha é afetada; por isso verificam a tabela `deleted` antes de recusar a operação.
- **Edição de alternativa.** `trg_Alternativa_CorretaUtilizada` barra apenas a mudança do valor de `Correta_Alternativa`. Editar o **texto** da alternativa continua permitido, inclusive em questões já utilizadas. Uma questão é considerada "utilizada" quando aparece em `SimuladoQuestao`.

---

# Regras de negócio e onde estão implementadas

Rastreabilidade entre as regras e os mecanismos que as garantem. A coluna "Onde" indica quem recusa a operação quando a regra é violada.

| Regra | Onde | Mecanismo |
|---|---|---|
| RN01 — CPF obrigatório no cadastro | Banco | `CK_Usuario_CPF` + `NOT NULL` |
| RN02 — senha inicial é o CPF, troca recomendada | Aplicação | Hash BCrypt do CPF na inclusão |
| RN03 — código de recuperação com validade | Aplicação | `TokenRecuperacao_Usuario`, `ValidadeToken_Usuario` |
| RN05 — somente Aluno possui turma | Aplicação | Ver "Pontos em aberto" |
| RN06 — turma única por nome e ano | Banco | `UQ_Turma_NomeAno` |
| RN07 — disciplina pode ficar sem responsável | Banco | `Disciplina.Id_Usuario` admite NULL |
| RN08 — registro da autoria da questão | Banco | `Questao.Id_Usuario` `NOT NULL` + FK |
| RN09 — alternativas de A a E | Banco | `CK_Alternativa_Letra` |
| RN09 — cinco alternativas, uma correta | Aplicação | Validação no Desktop e na API |
| RN11 — gabarito imutável após uso | Banco | `trg_Alternativa_CorretaUtilizada` |
| RN12 — exclusão lógica | Banco | Colunas `Ativo` em 4 tabelas; FK sem cascata |
| RN13 — duas ou mais disciplinas por simulado | Aplicação | Sem campo próprio; derivação pelas questões |
| RN14 — 10, 15 ou 20 questões | Banco | `CK_Simulado_QuantidadeQuestoes` |
| RN15 — sorteio sem repetição | Banco | PK composta de `SimuladoQuestao` |
| RN16 — tentativa única por simulado | Banco | `UQ_ResultadoSimulado_Simulado` |
| RN18 — questão em branco conta como erro | Banco | `CK_RespostaAluno_AlternativaMarcada` + `Acertou` `NOT NULL` |
| RN19 — respostas e resultados imutáveis | Banco | `trg_RespostaAluno_Imutavel`, `trg_ResultadoSimulado_Imutavel` |
| RN21 — pontuação cumulativa, nunca reduzida | Banco | `trg_ResultadoSimulado_AcumularPontuacao`, `trg_Usuario_PontuacaoNaoDiminui` |
| RN22 — faixas de medalha configuráveis | Banco | Tabela `Medalha`; apuração em `sp_PontuacaoAluno` |
| RN23 — pontos faltantes para a próxima faixa | Banco | `sp_PontuacaoAluno` |
| RN25 — aluno desativado fora das médias | Aplicação | Filtro `Ativo_Usuario = 1` nas consultas |
| RN27 — dados que não se repetem | Banco | 8 restrições UNIQUE |

---

# Pontos em aberto

Divergências e lacunas identificadas na conferência deste documento contra os scripts. Nenhuma impede o funcionamento do sistema; todas merecem decisão do grupo.

**1. Não existem índices.** Nenhum script cria índice. O SQL Server indexa automaticamente as chaves primárias e as restrições `UNIQUE`, mas **não** as chaves estrangeiras — e são dezoito. Sem eles, cada junção e cada verificação de integridade referencial percorre a tabela inteira. O RNF04 determina índices nas colunas de maior frequência de consulta e fixa tempo máximo de resposta; as colunas candidatas são, no mínimo, `Questao (Id_Disciplina, Ativo_Questao)` para o sorteio da RN15, `SimuladoQuestao (Id_Questao)` para a derivação de disciplinas e a verificação da RN11, `ResultadoSimulado (Id_Usuario, DataRealizacao_ResultadoSimulado)` para o histórico da RN20, e as demais colunas de chave estrangeira.

**2. `CK_Usuario_TurmaSomenteAluno` é citada, mas não existe.** O comentário de abertura do script `03_carga_inicial.sql` menciona que os identificadores de perfil são usados "pela restrição `CK_Usuario_TurmaSomenteAluno`". Essa restrição não é criada em nenhum script. A RN05 — apenas contas de Aluno possuem turma — está, portanto, garantida somente pela aplicação. A restrição correspondente seria:

```sql
ALTER TABLE dbo.Usuario ADD CONSTRAINT CK_Usuario_TurmaSomenteAluno
    CHECK (Id_Perfil = 1 OR Id_Turma IS NULL);
```

Observe que ela garante apenas a direção inequívoca: quem **não** é Aluno não pode ter turma. A direção oposta — obrigar todo Aluno a ter turma — não é imposta porque as fontes divergem: o MER registra a cardinalidade `(0,1)`, que admite aluno sem turma, enquanto o RF02 trata a turma como obrigatória no cadastro do Aluno.

**3. `Desempenho` não possui restrição de unicidade.** A descrição da entidade — "desempenho acumulado de **cada** aluno **por** disciplina" — implica um único registro por par. Sem a restrição, linhas duplicadas para o mesmo par distorceriam as médias apresentadas ao Professor e o resultado de `sp_DesempenhoAluno`. A restrição correspondente seria:

```sql
ALTER TABLE dbo.Desempenho ADD CONSTRAINT UQ_Desempenho_UsuarioDisciplina
    UNIQUE (Id_Usuario, Id_Disciplina);
```

**4. Coerência aritmética não é verificada pelo banco.** Nada impede `TotalAcertos_ResultadoSimulado` maior que `TotalQuestoes_ResultadoSimulado`, nem percentual fora da faixa de 0 a 100, nem pontuação negativa. São restrições de domínio simples de acrescentar e que protegem contra defeito na aplicação.

**5. Armazenamento dos arquivos de imagem.** As colunas `ImagemUrl_Questao`, `FotoUrl_Usuario` e `ImagemUrl_Medalha` guardam o **caminho** do arquivo, não a imagem. Nenhum requisito define formatos aceitos, tamanho máximo ou local definitivo de armazenamento, e o banco não tem como garantir que o arquivo referenciado exista.

**6. Rotina de backup.** O RNF02 determina rotina periódica de backup. Não há script correspondente, por depender da política de retenção e do ambiente de hospedagem.

---

# Legenda

| Símbolo / Tipo | Descrição |
|---|---|
| PK | Primary Key — chave primária que identifica unicamente cada registro da tabela |
| FK | Foreign Key — chave estrangeira que referencia a chave primária de outra tabela |
| PK + FK | Coluna que atua simultaneamente como chave primária e estrangeira |
| → Tabela | A coluna referencia a chave primária da tabela indicada |
| IDENTITY(1,1) | Valor gerado automaticamente pelo banco, começando em 1 e incrementando de 1 |
| UNIQUE | O valor não pode se repetir na tabela |
| UQ_… (A, B) | Restrição de unicidade composta: a combinação das colunas não pode se repetir |
| CK_… | Restrição de domínio: a coluna aceita apenas os valores previstos |
| DF_… → v | Valor padrão atribuído automaticamente quando nenhum é informado |
| NOT NULL / NÃO | O campo é obrigatório e não pode conter valor nulo |
| NULL / SIM | O campo é opcional e pode conter valor nulo |
| INT | Número inteiro |
| BIT | Tipo booleano: 1 = verdadeiro / 0 = falso |
| CHAR(n) | Texto de comprimento fixo de n caracteres |
| VARCHAR(n) | Texto de comprimento variável, até n caracteres, sem acentuação Unicode |
| NVARCHAR(n) | Texto de comprimento variável Unicode, adequado a acentuação |
| NVARCHAR(MAX) | Texto Unicode de comprimento variável sem limite prático |
| DATE | Data, sem horário |
| DATETIME | Data e horário |
| DECIMAL(5,2) | Número decimal com até 5 dígitos e 2 casas decimais |

---

[Voltar aos diagramas do banco de dados](./README.md)
