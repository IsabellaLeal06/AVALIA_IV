# Gerenciamento de Backlogs – Projeto Avalia+

## Descrição

Esta pasta reúne toda a documentação relacionada ao gerenciamento de Backlogs do projeto **Avalia+**, utilizando a metodologia ágil Scrum para organização, priorização e acompanhamento das atividades do sistema e da documentação acadêmica do **PIM IV**.

O gerenciamento de Backlogs permite estruturar tanto o desenvolvimento técnico da plataforma quanto as entregas acadêmicas exigidas no projeto.

---

# Estrutura do Documento

## 1. Introdução

Apresenta a proposta do gerenciamento de Backlogs no projeto Avalia+, incluindo a organização das atividades relacionadas ao desenvolvimento do sistema, modelagem, diagramas, banco de dados, API e documentação técnica.

---

## 2. Backlogs no Scrum

O projeto utiliza os principais Backlogs do framework Scrum:

### Product Backlog

Lista contendo todas as funcionalidades, melhorias e atividades previstas para o desenvolvimento da plataforma.

### Sprint Backlog

Conjunto de atividades selecionadas para execução durante cada Sprint.

---

# Product Backlog do Projeto Avalia+

O Product Backlog foi dividido em três categorias principais:

- Funcionalidades do sistema.
- Modelagem e diagramas.
- Documentação do projeto.

Essa divisão permite melhor organização e controle das atividades desenvolvidas ao longo das sprints.

---

# 1. Funcionalidades do Sistema

Esses itens representam as funcionalidades principais da plataforma Avalia+.

| ID | Funcionalidade | Descrição | Prioridade |
|---|---|---|---|
| PB01 | Autenticação de usuários | Permitir login utilizando e-mail e senha com controle de acesso por perfil. | Alta |
| PB02 | Recuperação de senha | Permitir redefinição de senha por e-mail com código único de recuperação. | Alta |
| PB03 | Cadastro de usuários | Gerenciamento de usuários (Aluno, Professor e Administrador). | Alta |
| PB04 | Gerenciamento de disciplinas | Cadastro, edição e desativação de disciplinas. | Alta |
| PB05 | Gerenciamento de turmas | Criação, edição, desativação e vinculação de disciplinas e alunos às turmas. | Alta |
| PB06 | Gerenciamento de questões | Cadastro, edição e desativação de questões por disciplina (Administrador e Professor). | Alta |
| PB07 | Geração automática de simulados | Gerar simulados automáticos com disciplinas selecionadas e quantidade configurável de questões. | Alta |
| PB08 | Realização de simulados | Permitir realização de simulados nas plataformas Web e Mobile. | Alta |
| PB09 | Correção automática | Correção automática após a finalização do simulado. | Alta |
| PB10 | Histórico de simulados | Armazenamento dos simulados realizados pelo aluno. | Média |
| PB11 | Dashboard de desempenho | Indicadores de desempenho para alunos e professores. | Média |
| PB12 | Sistema de gamificação | Pontuação acumulada e medalhas conforme desempenho do aluno. | Média |
| PB13 | Controle de permissões | Restrição de acesso conforme perfil do usuário. | Alta |
| PB14 | Plataforma Desktop | Aplicação administrativa desenvolvida em C# Windows Forms. | Alta |
| PB15 | Plataforma Mobile | Aplicativo do aluno para simulados e acompanhamento de desempenho. | Média |
| PB16 | Recursos de acessibilidade | Integração do VLibras na plataforma Web. | Média |

---

# 2. Modelagem e Diagramas do Sistema

Essas atividades representam a modelagem estrutural e funcional da plataforma.

| ID | Atividade | Descrição | Prioridade |
|---|---|---|---|
| MD01 | Diagrama de Casos de Uso | Representação das interações do sistema. | Alta |
| MD02 | Diagrama de Classes | Modelagem das entidades e relacionamentos do sistema (backend C#). | Alta |
| MD03 | Diagrama de Sequência | Fluxo de execução das funcionalidades. | Média |
| MD04 | Fluxo de Usuário | Navegação dos usuários na plataforma Web, Desktop e Mobile. | Alta |
| MD05 | Modelo Conceitual (MER) | Estrutura conceitual do banco de dados. | Alta |
| MD06 | Modelo Lógico | Estrutura lógica das tabelas do banco de dados. | Alta |
| MD07 | Diagrama Entidade-Relacionamento (DER) | Representação gráfica do banco de dados. | Alta |
| MD08 | Dicionário de Dados | Documentação das tabelas, atributos, chaves e restrições. | Média |

Essas atividades garantem organização e padronização da estrutura do sistema.

---

# 3. Documentação do Projeto

Essas atividades correspondem à elaboração da documentação acadêmica do **PIM IV**.

| ID | Atividade | Descrição | Prioridade |
|---|---|---|---|
| DOC01 | Introdução do projeto | Apresentação geral da proposta. | Alta |
| DOC02 | Descrição do problema | Problema que o sistema pretende solucionar e evolução do PIM III. | Alta |
| DOC03 | Objetivos do sistema | Objetivos gerais e específicos. | Alta |
| DOC04 | Público-alvo | Identificação dos usuários da plataforma. | Média |
| DOC05 | Requisitos do sistema | Requisitos funcionais, não funcionais e regras de negócio. | Alta |
| DOC06 | Arquitetura do sistema | Tecnologias, API e estrutura das plataformas Web, Desktop e Mobile. | Média |
| DOC07 | Modelagem do sistema | Diagramas UML, MER, DER, Modelo Lógico e Dicionário de Dados. | Alta |
| DOC08 | Metodologia de desenvolvimento | Aplicação da metodologia Scrum. | Alta |
| DOC09 | Planejamento de Sprints | Organização das atividades por Sprint. | Alta |
| DOC10 | Conclusão do projeto | Resultados e considerações finais do PIM IV. | Média |

---

# Planejamento das Sprints

## Sprint 1 – Planejamento e Levantamento de Requisitos

### Objetivo

Organizar o projeto, revisar os requisitos da versão 2.2 do Avalia+ e estruturar o gerenciamento das atividades.

### Atividades

- Organização do repositório GitHub.
- Revisão dos requisitos funcionais.
- Revisão dos requisitos não funcionais.
- Revisão das regras de negócio.
- Criação do Product Backlog.
- Planejamento das Sprints.

---

## Sprint 2 – Modelagem do Sistema

### Objetivo

Desenvolver a modelagem estrutural e conceitual da plataforma.

### Atividades

- Revisão e atualização do MER.
- Atualização do DER.
- Modelo Lógico do banco de dados.
- Dicionário de Dados.
- Diagrama de Casos de Uso.
- Documentação da arquitetura.

---

## Sprint 3 – Banco de Dados e API

### Objetivo

Desenvolver a estrutura técnica da plataforma e iniciar a implementação da API.

### Atividades

- Script SQL do banco de dados.
- Constraints e integridade dos dados.
- Estrutura da API REST.
- Sistema de autenticação.
- Recuperação de senha.
- Integração inicial entre API e banco de dados.

---

## Sprint 4 – Funcionalidades Administrativas

### Objetivo

Desenvolver os módulos administrativos da plataforma Avalia+.

### Atividades

- Cadastro e gerenciamento de usuários.
- Cadastro de turmas.
- Cadastro de disciplinas.
- Cadastro e gerenciamento de questões.
- Upload de imagens das questões.
- Desenvolvimento da aplicação Desktop.

---

## Sprint 5 – Simulados e Desempenho

### Objetivo

Implementar o fluxo completo dos simulados e o acompanhamento de desempenho dos usuários.

### Atividades

- Geração automática de simulados.
- Realização de simulados.
- Correção automática.
- Revisão comentada.
- Histórico de simulados.
- Dashboard do aluno.
- Dashboard do professor.

---

## Sprint 6 – Gamificação, Acessibilidade e Documentação

### Objetivo

Implementar funcionalidades complementares e concluir a documentação do projeto.

### Atividades

- Sistema de pontuação.
- Sistema de medalhas.
- Integração do VLibras.
- Testes da aplicação Web.
- Testes da aplicação Desktop.
- Revisão da documentação acadêmica.

---

## Sprint 7 – Mobile, Integração e Entrega Final

### Objetivo

Finalizar o aplicativo Mobile, integrar as plataformas e preparar a entrega final do PIM IV.

### Atividades

- Desenvolvimento da aplicação Mobile.
- Integração entre Web, Desktop e Mobile.
- Testes de integração.
- Registro das evidências dos testes.
- Ajustes finais da plataforma.
- Organização da entrega final do projeto.

---

# Estrutura de Organização no Trello

O quadro do Trello foi organizado com as seguintes listas:

| Lista | Descrição |
|---|---|
| Product Backlog | Funcionalidades e atividades gerais. |
| Sprint 1 | Planejamento e levantamento de requisitos. |
| Sprint 2 | Modelagem e diagramas. |
| Sprint 3 | Banco de Dados e API. |
| Sprint 4 | Funcionalidades administrativas. |
| Sprint 5 | Simulados e desempenho. |
| Sprint 6 | Gamificação, acessibilidade e documentação. |
| Sprint 7 | Mobile, integração e entrega final. |
| Em Desenvolvimento | Atividades em execução. |
| Em Revisão | Atividades em validação. |
| Concluído | Atividades finalizadas. |

---

# Benefícios da Utilização de Backlogs

A utilização de Backlogs no projeto Avalia+ proporciona:

- organização das atividades;
- definição clara de prioridades;
- acompanhamento do progresso;
- transparência no desenvolvimento;
- melhor planejamento das entregas.

Além disso, o uso do Trello auxilia no controle visual das tarefas e no acompanhamento das atividades realizadas pela equipe.
