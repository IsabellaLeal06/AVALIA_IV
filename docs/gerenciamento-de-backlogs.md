# Gerenciamento de Backlogs – Projeto Avalia+

Este documento apresenta a organização dos Backlogs do projeto Avalia+, desenvolvido no PIM IV com utilização do framework Scrum.

## 1. Product Backlog

O Product Backlog reúne todos os itens necessários para a evolução do Avalia+.

Os itens estão organizados em seis grupos:

1. **REQ** – Requisitos e regras de negócio;
2. **MD** – Modelagem e diagramas;
3. **INF** – Infraestrutura e arquitetura;
4. **PB** – Funcionalidades do sistema;
5. **TST** – Testes;
6. **DOC** – Documentação acadêmica.

## 2. Sprint Backlog

O Sprint Backlog é o conjunto de itens selecionados do Product Backlog para uma sprint específica.

O planejamento atual possui sete sprints:

| Sprint | Período | Foco |
|---|---|---|
| S1 | 18/08 a 29/08/2026 | Requisitos, regras de negócio e repositório |
| S2 | 01/09 a 12/09/2026 | Modelagem e documentos do banco |
| S3 | 15/09 a 26/09/2026 | Banco físico, API e autenticação |
| S4 | 29/09 a 10/10/2026 | Cadastros e banco de questões |
| S5 | 13/10 a 24/10/2026 | Simulados, correção e desempenho |
| S6 | 27/10 a 07/11/2026 | Gamificação, acessibilidade e documentação |
| S7 | 10/11 a 21/11/2026 | Mobile, integração e ajustes finais |

## 3. Priorização

A priorização considera, nesta ordem:

1. dependência técnica;
2. peso na avaliação;
3. risco;
4. esforço.

Itens que bloqueiam outras atividades são executados primeiro.

## 4. Definition of Done

Um item é considerado concluído quando atende aos critérios correspondentes ao seu tipo.

### Código e infraestrutura

- implementação conforme o requisito de origem;
- regras de negócio aplicadas corretamente;
- código versionado no GitHub;
- testes realizados;
- revisão por outro integrante;
- ausência de erros conhecidos de execução.

### Modelagem e diagramas

- coerência com os requisitos v2.2;
- coerência com o banco implementado;
- exportação e versionamento;
- revisão por outro integrante.

### Documentação

- seção concluída;
- formatação conforme as orientações acadêmicas;
- revisão por outro integrante;
- referências e fontes registradas quando aplicável.

## 5. Rastreabilidade

O Backlog mantém relação direta com os requisitos e com as etapas previstas no Manual do PIM IV.

A organização permite acompanhar desde os requisitos iniciais até as funcionalidades, banco de dados, testes e documentação acadêmica.

## 6. Corte documental

A documentação acadêmica deve estar integralmente concluída ao final da Sprint 6, em **07/11/2026**.

Nenhum item que componha o documento final deve ser alocado na Sprint 7.

A Sprint 7 fica destinada a código, integração e testes finais.
