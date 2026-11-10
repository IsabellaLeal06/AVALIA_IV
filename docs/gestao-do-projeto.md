# Gestão do Projeto — Avalia+ (PIM IV)

Este documento registra como a equipe se comunicou durante o desenvolvimento do PIM IV e quais riscos foram identificados, avaliados e tratados ao longo do projeto. Complementa os artefatos do Scrum, reunidos na pasta [`scrum/`](../scrum/), e o [diagrama de rede](../diagramas/rede/diagrama-de-rede.md), que apresenta o caminho crítico do cronograma.

---

## 1. Plano de Comunicação

A equipe é formada por três integrantes que estudam na mesma turma e têm rotinas de trabalho distintas fora da faculdade. A comunicação foi organizada em torno dessa condição: um canal **assíncrono**, consultado quando cada integrante tem disponibilidade, e um canal **síncrono**, aproveitando o tempo em que o grupo já está reunido na instituição.

| Canal | Tipo | Finalidade | Frequência | Participantes |
|---|---|---|---|---|
| WhatsApp — grupo da equipe | Assíncrono | Registro das Daily Scrums, decisões, dúvidas e envio de arquivos e capturas de tela | Diária, em todo dia útil da sprint | Todo o grupo |
| Encontros presenciais na faculdade | Síncrono | Sprint Planning, Sprint Review, Sprint Retrospective e alinhamentos que exigem discussão | No início e no encerramento de cada sprint, e nos intervalos das aulas | Todo o grupo |

### 1.1 Funcionamento

O **grupo de WhatsApp** é o canal principal e concentra o acompanhamento diário. Cada integrante registra por escrito o que concluiu, o que está em andamento, qual é o próximo passo e quais impedimentos encontrou. Esses registros são posteriormente consolidados nos arquivos de Daily Scrum do repositório, o que torna o canal não apenas um meio de conversa, mas a origem de um artefato do projeto. O histórico das mensagens também serve de consulta quando é preciso recuperar uma decisão tomada semanas antes.

Os **encontros presenciais** concentram as cerimônias que exigem discussão e decisão conjunta. A Sprint Planning abre a iteração com a seleção dos itens e a distribuição dos responsáveis; a Sprint Review e a Sprint Retrospective a encerram, respectivamente com a validação do incremento e a avaliação do processo. Nos intervalos das aulas, o grupo aproveita para resolver pontos que o registro escrito não esclarece bem, como a escolha entre duas abordagens técnicas.

A divisão entre os dois canais segue um critério simples: o que é **registro** vai para o WhatsApp, porque precisa ficar disponível para consulta posterior; o que é **decisão** vai para o encontro presencial, porque exige a presença dos três integrantes ao mesmo tempo.

### 1.2 Justificativa da escolha

O WhatsApp foi escolhido por ser o aplicativo que todos os integrantes já utilizam diariamente, o que elimina a necessidade de adotar e manter uma ferramenta nova e reduz o tempo de resposta. Como o acompanhamento diário foi adotado em formato assíncrono, o canal precisava ser aquele com maior probabilidade de leitura no mesmo dia, e não o tecnicamente mais completo.

Os encontros presenciais aproveitam a rotina acadêmica já comum à equipe, sem exigir a marcação de horários adicionais. Para um grupo de três pessoas que se encontra várias vezes por semana na mesma instituição, essa é a forma de comunicação síncrona de menor custo de coordenação.

### 1.3 Alternativas consideradas

- **Discord:** oferece salas de voz permanentes e histórico organizado por canal, o que seria adequado para sessões longas de trabalho conjunto. Foi descartado porque exigiria que os três integrantes adotassem uma ferramenta adicional, com baixa probabilidade de consulta diária, e porque o trabalho da equipe foi distribuído por frentes individuais, não em programação conjunta.
- **Microsoft Teams:** voltado ao ambiente corporativo, com reuniões estruturadas por agendamento. Considerado mais pesado que o necessário para uma equipe de três pessoas.
- **Google Meet:** exige criar e compartilhar um link a cada reunião e não mantém um chat persistente entre elas, o que inviabilizaria o uso do canal como registro.
- **Comentários nos cartões do Trello:** chegaram a ser usados pontualmente, mas foram descartados como canal principal porque a discussão ficaria fragmentada entre dezenas de cartões, sem uma linha do tempo única do projeto.

### 1.4 Limitação reconhecida

O formato assíncrono adotado para o acompanhamento diário preserva a função de inspeção frequente prevista no Scrum, mas não substitui integralmente a Daily Scrum presencial: o esclarecimento imediato de dúvidas, que uma conversa de quinze minutos resolve, por vezes levou algumas horas até ser respondido no grupo. A equipe considerou esse custo aceitável diante da alternativa, que seria exigir um horário comum diário fora da faculdade.

---

## 2. Gerenciamento de Riscos

### 2.1 Metodologia

Foi adotada a **análise qualitativa de riscos** com matriz de probabilidade × impacto 5×5, conforme o *Guia PMBOK* (PMI, 2021) e alinhada à norma ABNT NBR ISO 31000. Cada risco recebe uma nota de 1 a 5 em probabilidade (P) e em impacto (I), e a **exposição** é o produto entre elas (P × I), variando de 1 a 25.

**Escala de probabilidade**

| Nível | Classificação | Chance estimada de ocorrência |
|:---:|---|---|
| 1 | Muito baixa | até 10% |
| 2 | Baixa | 10% a 30% |
| 3 | Média | 30% a 50% |
| 4 | Alta | 50% a 70% |
| 5 | Muito alta | acima de 70% |

**Escala de impacto** (efeito sobre prazo, escopo e qualidade)

| Nível | Classificação | Efeito no projeto |
|:---:|---|---|
| 1 | Desprezível | Ajuste rápido, sem efeito perceptível |
| 2 | Baixo | Pequeno retrabalho, sem afetar o cronograma |
| 3 | Moderado | Atraso de alguns dias ou funcionalidade entregue parcialmente |
| 4 | Alto | Compromete uma das aplicações (web, desktop ou mobile) |
| 5 | Crítico | Compromete a entrega final ou a apresentação do projeto |

**Classificação da exposição**

| Exposição (P × I) | Classificação |
|:---:|---|
| 1 a 4 | 🟩 Baixa |
| 5 a 12 | 🟨 Média |
| 15 a 25 | 🟥 Alta |

As respostas seguem as estratégias previstas no PMBOK: **evitar** (eliminar a causa), **mitigar** (reduzir a probabilidade ou o impacto), **transferir** (passar a responsabilidade a terceiros) e **aceitar** (assumir o risco e monitorá-lo).

### 2.2 Registro de riscos

O registro foi aberto na Sprint 1 e revisado a cada Sprint Retrospective. Os riscos marcados como **ocorreu** foram efetivamente observados durante o projeto e estão documentados nos registros de Daily Scrum da sprint indicada.

| ID | Risco | P | I | Exposição | Resposta | Ação adotada | Status |
|:---:|---|:---:|:---:|:---:|:---:|---|:---:|
| R01 | Divergência entre o documento de requisitos e o Modelo Entidade-Relacionamento herdados do PIM III | 5 | 3 | 🟥 15 | Mitigar | Criação do item REQ08, dedicado a registrar e resolver as divergências antes de qualquer trabalho de modelagem | Ocorreu na S1 — tratado |
| R02 | Atraso da documentação acadêmica em relação ao prazo de entrega | 4 | 5 | 🟥 20 | Mitigar | Corte documental ao final da Sprint 6, deixando a Sprint 7 livre para código e testes | Ativo — monitorado |
| R03 | Concentração de itens na Sprint 6, a mais carregada do projeto | 4 | 4 | 🟥 16 | Mitigar | Regra de deslocamento definida na Sprint Planning: em caso de atraso, desloca-se o item PB09, nunca um item de documentação | Ativo — monitorado |
| R04 | Regra de negócio validada apenas na interface e não no servidor | 3 | 4 | 🟨 12 | Mitigar | Definition of Done passou a exigir validação no servidor; itens TST03 e TST04 dedicados a esses cenários | Ocorreu na S4 — tratado |
| R05 | Falha de integração entre as aplicações web, desktop e mobile | 3 | 4 | 🟨 12 | Evitar | API e contrato de dados únicos, construídos e validados antes dos clientes | Mitigado |
| R06 | Curva de aprendizado em tecnologias novas para a equipe (Node com Express, Kotlin com Jetpack Compose, Azure SQL) | 4 | 3 | 🟨 12 | Mitigar | Desenvolvimento incremental, uma camada por vez, com a camada anterior validada antes da seguinte | Mitigado |
| R07 | Acoplamento entre os itens do Sprint Backlog, impedindo concluir entregas isoladamente | 4 | 2 | 🟨 8 | Aceitar | Validação conjunta ao final da iteração, em vez de fechamento item a item | Ocorreu na S2 e na S5 — aceito |
| R08 | Indisponibilidade de serviço de envio de e-mail para a recuperação de senha | 3 | 3 | 🟨 9 | Mitigar | Configuração do serviço no ambiente de desenvolvimento; impedimento registrado em duas Dailys consecutivas até a solução | Ocorreu na S3 — tratado |
| R09 | Indisponibilidade de um integrante em equipe de apenas três pessoas, com acúmulo de papéis | 3 | 4 | 🟨 12 | Mitigar | Responsável nominal por item do backlog e acompanhamento diário por escrito, permitindo que outro integrante assuma a atividade | Ativo |
| R10 | Vazamento de credenciais do banco de dados ou de senhas de usuários | 2 | 5 | 🟨 10 | Mitigar | Senhas armazenadas com hash (RNF01) e dados de conexão mantidos fora do repositório | Mitigado |
| R11 | Perda de código ou conflito entre versões | 2 | 4 | 🟨 8 | Mitigar | Versionamento em Git, com a branch principal reservada a código concluído e branches curtas para trabalho em andamento | Mitigado |
| R12 | Indisponibilidade do banco de dados hospedado em nuvem | 2 | 4 | 🟨 8 | Aceitar | Scripts de criação e de carga versionados no repositório, permitindo recriar o banco a qualquer momento | Aceito |

### 2.3 Matriz de probabilidade × impacto

Cada célula apresenta a exposição (P × I) e os riscos posicionados nela.

| Probabilidade ↓ / Impacto → | 1 — Desprezível | 2 — Baixo | 3 — Moderado | 4 — Alto | 5 — Crítico |
|---|:---:|:---:|:---:|:---:|:---:|
| **5 — Muito alta** | 🟨 5 | 🟨 10 | 🟥 15<br>**R01** | 🟥 20 | 🟥 25 |
| **4 — Alta** | 🟩 4 | 🟨 8<br>**R07** | 🟨 12<br>**R06** | 🟥 16<br>**R03** | 🟥 20<br>**R02** |
| **3 — Média** | 🟩 3 | 🟨 6 | 🟨 9<br>**R08** | 🟨 12<br>**R04, R05, R09** | 🟥 15 |
| **2 — Baixa** | 🟩 2 | 🟩 4 | 🟨 6 | 🟨 8<br>**R11, R12** | 🟨 10<br>**R10** |
| **1 — Muito baixa** | 🟩 1 | 🟩 2 | 🟩 3 | 🟩 4 | 🟨 5 |

### 2.4 Análise

Três riscos foram classificados com exposição alta.

O **R01** foi identificado logo na abertura do projeto e se concretizou na Sprint 1: a conferência dos requisitos contra o modelo de dados do PIM III revelou divergências, entre elas a obrigatoriedade do campo de explicação da resposta, tratada como opcional nos requisitos e obrigatória no modelo. A resposta adotada — criar um item específico de backlog para resolvê-las antes de iniciar a modelagem — evitou que a inconsistência se propagasse para o banco de dados e para as três aplicações.

O **R02** e o **R03** permanecem ativos até a entrega final e estão relacionados entre si: a Sprint 6 concentra tanto o fechamento da documentação acadêmica quanto itens de implementação. A resposta foi estrutural, e não apenas de acompanhamento: o cronograma estabelece um corte documental ao final dessa sprint e proíbe alocar na Sprint 7 qualquer item que componha o documento entregue. Assim, um eventual atraso na última iteração afeta somente o produto demonstrado na apresentação.

Entre os riscos de exposição média, dois merecem registro por terem de fato ocorrido. O **R04** apareceu na Sprint 4, quando regras corretamente documentadas — como o limite de cinco alternativas por questão — haviam sido aplicadas apenas na interface; a correção foi replicar a validação no servidor e criar itens de teste dedicados. O **R07** ocorreu nas Sprints 2 e 5, em que os itens eram fortemente acoplados e não podiam ser concluídos de forma independente; nesse caso a equipe optou por **aceitar** o risco e ajustar a forma de validação, em vez de refatiar o Sprint Backlog no meio da iteração.

Os demais riscos foram mitigados principalmente pela ordem de desenvolvimento adotada — requisitos, modelagem, banco, API e só então as aplicações — e pela estrutura de comunicação descrita na seção 1.

---

## Referências

ASSOCIAÇÃO BRASILEIRA DE NORMAS TÉCNICAS. **NBR ISO 31000**: gestão de riscos — diretrizes. Rio de Janeiro: ABNT, 2018.

PROJECT MANAGEMENT INSTITUTE. **Um guia do conhecimento em gerenciamento de projetos (Guia PMBOK)**. 7. ed. Newtown Square: PMI, 2021.

SCHWABER, Ken; SUTHERLAND, Jeff. **O guia do Scrum**. 2020. Disponível em: https://scrumguides.org. Acesso em: nov. 2026.
