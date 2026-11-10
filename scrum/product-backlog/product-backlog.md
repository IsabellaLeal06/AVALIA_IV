# Product Backlog — Avalia+ (PIM IV)

Este documento reúne o **Product Backlog** do projeto Avalia+: a lista única e ordenada de tudo o que o produto precisa para ser considerado concluído no PIM IV.

O backlog foi reconstruído a partir do documento de [Requisitos e Regras de Negócio](../../docs/requisitos-do-sistema.md) na versão 2.2, do Modelo Entidade-Relacionamento vigente e do cronograma oficial das sete sprints. Ele substitui integralmente a versão elaborada no PIM III.

---

## 1. Como o backlog está organizado

Os **setenta e sete itens** estão distribuídos em sete blocos. A divisão facilita a atribuição de responsáveis e a rastreabilidade com as nove etapas exigidas pelo manual do PIM IV.

| Bloco | Escopo | Itens | Responsável principal | Sprints |
|:---:|---|:---:|---|:---:|
| **REQ** | Requisitos, regras de negócio e matrizes de rastreabilidade | 8 | Isabella | S1 |
| **MD** | Modelagem do banco de dados e diagramas UML | 13 | Isabella e Letícia Ap. | S1, S2 e S6 |
| **INF** | Repositório, banco físico, API, autenticação e planos de infraestrutura | 18 | Letícia Ap. e Isabella | S1 e S3 a S7 |
| **PB** | Funcionalidades do sistema e aplicações desktop e mobile | 10 | Gabriel | S3 a S7 |
| **TST** | Testes de API, de regras de negócio, de interface e de integração | 11 | Letícia Ap. e Gabriel | S4 a S7 |
| **DOC** | Documentação acadêmica conforme as etapas do manual | 14 | Isabella | S2 a S6 |
| **GP** | Gestão do projeto: comunicação, riscos e cronograma de rede | 3 | Isabella | S1 e S2 |

### Níveis de prioridade

- **Alta** — o item bloqueia outros itens ou compõe entrega obrigatória;
- **Média** — o item é necessário à entrega, mas não bloqueia outros;
- **Baixa** — o item é desejável e pode ser reduzido em caso de atraso.

### Critérios de ordenação

A ordem dos itens obedece a quatro critérios, aplicados nesta sequência:

1. **Dependência técnica** — itens que bloqueiam outros vêm primeiro. Requisitos precedem a modelagem, que precede o banco de dados, que precede a API, que precede as aplicações.
2. **Peso na avaliação** — itens vinculados às etapas de maior pontuação no manual do PIM IV têm prioridade.
3. **Risco** — itens de maior incerteza técnica são antecipados, para que eventuais problemas apareçam com tempo hábil de correção.
4. **Esforço** — entre itens equivalentes nos critérios anteriores, executa-se primeiro o de menor esforço.

A sequência de dependências está representada graficamente no [diagrama de rede](../../diagramas/rede/diagrama-de-rede.md), que identifica o caminho crítico do projeto.

---

## 2. Bloco REQ — Requisitos e regras de negócio

Bloco de abertura do projeto, responsável por definir o alvo de todo o restante do trabalho. Nenhum item de modelagem ou de código inicia antes de sua conclusão, pois alterações de requisito nesta fase propagam-se para o banco, para os diagramas e para as três aplicações.

| ID | Item | Prioridade | Responsável | Sprint |
|:---:|---|:---:|---|:---:|
| REQ01 | Revisão dos requisitos funcionais (RF01 a RF07) | Alta | Isabella | S1 |
| REQ02 | Revisão dos requisitos não funcionais (RNF01 a RNF07) | Alta | Isabella | S1 |
| REQ03 | Revisão das regras de negócio (RN01 a RN27) | Alta | Isabella | S1 |
| REQ04 | Matriz de rastreabilidade RF × RN | Alta | Isabella | S1 |
| REQ05 | Matriz de rastreabilidade requisitos × etapas do manual | Alta | Isabella | S1 |
| REQ06 | Definição do escopo de melhorias do PIM III para o PIM IV | Alta | Gabriel | S1 |
| REQ07 | Definição do escopo de telas por plataforma (RN26) | Alta | Gabriel | S1 |
| REQ08 | Alinhamento entre os requisitos e o Modelo Entidade-Relacionamento | Alta | Isabella | S1 |

> **Observação.** O item REQ08 foi acrescentado durante a Sprint 1, após a revisão dos requisitos revelar divergências com o modelo herdado do PIM III. É um exemplo do caráter emergente do Product Backlog: itens surgem à medida que o entendimento do produto amadurece.

---

## 3. Bloco MD — Modelagem e diagramas

O modelo conceitual encontrava-se estruturalmente concluído desde o PIM III. Este bloco trata da conferência contra a versão 2.2 dos requisitos e da atualização dos documentos derivados. Os diagramas de implantação e de implementação são inteiramente novos.

| ID | Item | Prioridade | Responsável | Sprint |
|:---:|---|:---:|---|:---:|
| MD01 | Conferência do MER contra os requisitos v2.2 | Alta | Isabella | S2 |
| MD02 | Atualização da versão do MER e do histórico de alterações | Alta | Isabella | S2 |
| MD03 | Ajuste do campo de explicação da resposta | Alta | Isabella | S2 |
| MD04 | Revisão da desativação de turma e disciplina no modelo (RN12) | Alta | Isabella | S2 |
| MD05 | Atualização do DER | Alta | Isabella | S2 |
| MD06 | Atualização do dicionário de dados | Alta | Isabella | S2 |
| MD07 | Modelo lógico do banco de dados | Alta | Isabella | S2 |
| MD08 | Ajuste do diagrama de casos de uso | Alta | Letícia Ap. | S1 |
| MD09 | Ajuste do diagrama de classes | Alta | Letícia Ap. | S2 |
| MD10 | Ajuste do diagrama de sequência | Alta | Letícia Ap. | S2 |
| MD11 | Revalidação dos diagramas UML após a implementação | Média | Letícia Ap. | S6 |
| MD12 | Diagrama de implantação | Média | Letícia Ap. | S6 |
| MD13 | Diagrama de implementação | Média | Letícia Ap. | S6 |

---

## 4. Bloco INF — Infraestrutura e arquitetura

Bloco inexistente no PIM III. Concentra a maior parte do trabalho técnico novo do semestre, uma vez que o projeto anterior não possuía banco de dados, backend nem integração entre aplicações.

| ID | Item | Prioridade | Responsável | Sprint |
|:---:|---|:---:|---|:---:|
| INF01 | Padronização do repositório de versionamento | Alta | Isabella | S1 |
| INF02 | Script DDL do banco de dados | Alta | Isabella | S3 |
| INF03 | Restrições de unicidade e de domínio (RN27, RNF03) | Alta | Isabella | S3 |
| INF04 | Carga inicial de dados: perfis, medalhas e massa de teste | Média | Isabella | S3 |
| INF05 | Procedures do banco de dados | Média | Isabella | S5 |
| INF06 | Triggers de pontuação acumulada e composição da questão | Média | Isabella | S5 |
| INF07 | Índices para otimização de consultas | Média | Isabella | S5 |
| INF08 | Estruturação da API em camadas | Alta | Letícia Ap. | S3 |
| INF09 | Endpoints REST da API | Alta | Letícia Ap. | S3 |
| INF10 | Autenticação por token com expiração e hash de senha | Alta | Letícia Ap. | S3 |
| INF11 | Envio de e-mail para recuperação de senha com código único | Alta | Letícia Ap. | S3 |
| INF12 | Upload e armazenamento de imagens | Média | Letícia Ap. | S4 |
| INF13 | Transação única da correção de simulado (RNF03) | Alta | Letícia Ap. | S5 |
| INF14 | Integração entre as aplicações e a API | Alta | Gabriel | S7 |
| INF15 | Plano de arquitetura em nuvem | Média | Gabriel | S6 |
| INF16 | Plano de containers e pipeline CI/CD | Média | Gabriel | S6 |
| INF17 | Plano de monitoramento, escalabilidade e segurança | Média | Gabriel | S6 |
| INF18 | Versionamento do código das aplicações no repositório | Alta | Gabriel | S7 |

> **Observação.** Os itens INF15 a INF17 são documentais. O manual solicita a elaboração de um plano de execução, e não a implantação efetiva da infraestrutura em nuvem.

---

## 5. Bloco PB — Funcionalidades do sistema

Cada item deste bloco corresponde diretamente a um requisito funcional do documento de requisitos, o que preserva a rastreabilidade entre o backlog e o escopo formalizado.

| ID | Item | Prioridade | Responsável | Sprint |
|:---:|---|:---:|---|:---:|
| PB01 | RF01 — Autenticação e controle de acesso por perfil | Alta | Gabriel | S3 |
| PB02 | RF02 — Gestão de cadastros institucionais | Alta | Gabriel | S4 |
| PB03 | RF03 — Gestão do banco de questões | Alta | Gabriel | S4 |
| PB04 | RF04 — Geração e realização de simulados | Alta | Gabriel | S5 |
| PB05 | RF05 — Correção automática e revisão comentada | Alta | Gabriel | S5 |
| PB06 | RF06 — Acompanhamento de desempenho | Alta | Gabriel | S5 |
| PB07 | RF07 — Gamificação por pontuação e medalhas | Alta | Gabriel | S6 |
| PB08 | RNF06 — Integração do VLibras e recursos de acessibilidade | Alta | Gabriel | S6 |
| PB09 | Aplicação desktop em C# com Windows Forms | Média | Letícia Ap. | S6 |
| PB10 | Aplicativo mobile em Kotlin com Jetpack Compose | Alta | Gabriel | S7 |

---

## 6. Bloco TST — Testes

Bloco também inexistente no PIM III. As evidências de teste compõem a documentação final e demonstram a qualidade da solução entregue.

| ID | Item | Prioridade | Responsável | Sprint |
|:---:|---|:---:|---|:---:|
| TST01 | Testes da API e dos endpoints | Alta | Letícia Ap. | S4 |
| TST02 | Testes de autenticação, primeiro acesso e recuperação de senha | Alta | Letícia Ap. | S4 |
| TST03 | Testes de visibilidade de dados por perfil (RN24) | Alta | Letícia Ap. | S4 |
| TST04 | Testes de desativação em cascata (RN12) | Alta | Letícia Ap. | S4 |
| TST05 | Testes de simulado, correção e sorteio multidisciplina | Alta | Gabriel | S5 |
| TST06 | Testes do frontend web | Alta | Gabriel | S6 |
| TST07 | Testes da aplicação desktop | Média | Letícia Ap. | S6 |
| TST08 | Testes de acessibilidade e do VLibras | Média | Gabriel | S6 |
| TST09 | Testes do aplicativo mobile | Alta | Gabriel | S7 |
| TST10 | Testes de integração entre as três aplicações e a API | Alta | Gabriel | S7 |
| TST11 | Registro das evidências de teste | Média | Isabella | S7 |

---

## 7. Bloco DOC — Documentação acadêmica

Itens organizados conforme as nove etapas exigidas pelo manual do PIM IV, acrescidos dos elementos de formatação e da revisão final.

| ID | Item | Prioridade | Responsável | Sprint |
|:---:|---|:---:|---|:---:|
| DOC01 | Elementos pré-textuais em ABNT | Alta | Isabella | S6 |
| DOC02 | Introdução | Alta | Isabella | S2 |
| DOC03 | Etapa 1 — Caracterização da organização | Alta | Isabella | S2 |
| DOC04 | Etapa 2 — Planejamento da solução tecnológica | Alta | Gabriel | S2 |
| DOC05 | Etapa 3 — Responsabilidade social e diversidade | Alta | Isabella | S3 |
| DOC06 | Etapa 4 — Desenvolvimento da solução web | Alta | Gabriel | S5 |
| DOC07 | Etapa 5 — Desenvolvimento da solução mobile | Alta | Gabriel | S6 |
| DOC08 | Etapa 6 — Arquitetura de software | Alta | Letícia Ap. | S5 |
| DOC09 | Etapa 7 — Projeto do banco de dados | Alta | Isabella | S5 |
| DOC10 | Etapa 8 — Infraestrutura em nuvem e DevOps | Média | Gabriel | S6 |
| DOC11 | Etapa 9 — Gerenciamento ágil do projeto | Alta | Isabella | S6 |
| DOC12 | Conclusão | Alta | Isabella | S6 |
| DOC13 | Referências em ABNT | Alta | Isabella | S6 |
| DOC14 | Apêndices, anexos e revisão final | Alta | Isabella | S6 |

---

## 8. Bloco GP — Gestão do projeto

Itens de gerenciamento que o manual cobra além dos artefatos do Scrum. Foram posicionados no início do projeto porque orientam a forma de trabalho das sprints seguintes: o plano de comunicação define como a equipe se coordena, o registro de riscos antecipa os problemas conhecidos e o diagrama de rede identifica a cadeia de atividades que não admite atraso.

| ID | Item | Prioridade | Responsável | Sprint |
|:---:|---|:---:|---|:---:|
| GP01 | Plano de comunicação da equipe | Alta | Isabella | S1 |
| GP02 | Registro de riscos com matriz de probabilidade × impacto | Alta | Isabella | S1 |
| GP03 | Diagrama de rede do projeto e identificação do caminho crítico | Alta | Isabella | S2 |

> **Observação.** O item GP03 foi alocado na Sprint 2, e não na Sprint 1, porque o diagrama de rede depende do backlog consolidado: sem a lista de atividades e suas dependências, não é possível calcular o caminho crítico. O registro de riscos (GP02), por sua vez, é revisado a cada Sprint Retrospective, e não apenas na sprint em que foi criado.

Os dois documentos produzidos por este bloco são:

- [`docs/gestao-do-projeto.md`](../../docs/gestao-do-projeto.md) — plano de comunicação e gerenciamento de riscos;
- [`diagramas/rede/diagrama-de-rede.md`](../../diagramas/rede/diagrama-de-rede.md) — diagrama de rede pelo Método do Caminho Crítico.

---

## 9. Definition of Done

Os critérios de conclusão variam conforme a natureza do item, uma vez que um diagrama, um trecho de código e uma seção de documento não se verificam da mesma forma.

| Tipo de item | Critérios de conclusão |
|---|---|
| **Código** (PB, INF) | Funcionalidade implementada conforme o requisito de origem; regras de negócio aplicadas no servidor, e não apenas na interface; código versionado com commit padronizado; testado pelo responsável; revisado por outro integrante; sem erros de execução; comportamento consistente entre as plataformas envolvidas. |
| **Modelagem e diagramas** (MD) | Diagrama gerado na ferramenta definida; coerente com os requisitos v2.2 e com o banco de dados implementado; exportado em imagem e versionado; revisado por outro integrante. |
| **Requisitos** (REQ) | Redigido no documento oficial de requisitos; sem contradição com os demais requisitos, regras e com o MER; refletido nas matrizes de rastreabilidade; aprovado pelos três integrantes. |
| **Testes** (TST) | Cenários de teste definidos; testes executados; falhas corrigidas ou registradas como pendência; evidência registrada por captura de tela ou relatório. |
| **Documentação** (DOC, GP) | Seção redigida integralmente; formatada conforme as normas da ABNT quando aplicável; revisada por outro integrante; fontes citadas e referenciadas. |

---

## 10. Rastreabilidade com as etapas do manual

A matriz abaixo relaciona cada etapa avaliativa do manual do PIM IV aos itens do backlog responsáveis por atendê-la, de modo que nenhuma exigência fique sem item correspondente.

| Etapa do manual | Pontos | Itens do backlog |
|---|:---:|---|
| Formatação ABNT | 1,0 | DOC01, DOC13, DOC14 |
| 1 — Caracterização da organização | 1,0 | DOC03 |
| 2 — Planejamento da solução tecnológica | 1,0 | DOC04, REQ06 |
| 3 — Responsabilidade social e diversidade | 0,5 | DOC05, PB08 |
| 4 — Desenvolvimento da solução web | 2,0 | PB01 a PB08, INF08 a INF13, DOC06 |
| 5 — Desenvolvimento da solução mobile | 2,0 | PB10, DOC07 |
| 6 — Arquitetura de software | — | MD09, MD10, INF08, DOC08 |
| 7 — Projeto do banco de dados | 1,0 | MD01 a MD07, INF02 a INF07, DOC09 |
| 8 — Infraestrutura em nuvem e DevOps | 0,5 | MD12, INF15 a INF17, DOC10 |
| 9 — Gerenciamento ágil do projeto | 1,0 | DOC11, GP01 a GP03, backlog, cronograma e quadro Kanban |

---

## 11. Atualização do backlog

O Product Backlog é um documento vivo. Os ajustes decorrem das validações realizadas nas cerimônias do Scrum — Sprint Planning, Daily Scrum, Sprint Review e Sprint Retrospective — e devem permanecer alinhados ao documento de Requisitos e Regras de Negócio v2.2, ao Modelo Entidade-Relacionamento e ao cronograma das sete sprints.

Sempre que um item for acrescentado, removido ou realocado, o diagrama de rede deve ser revisto, pois a alteração pode mudar o caminho crítico do projeto.
