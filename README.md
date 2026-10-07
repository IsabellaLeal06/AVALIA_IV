<h1 align="center">
  <img src="./docs/assets/AVALIA +.svg" alt="Avalia+" width="300"/>
</h1>

<p align="center">
  <b>Plataforma de simulados e acompanhamento de desempenho acadêmico</b><br/>
  PIM IV – Projeto Integrado Multidisciplinar | ADS – UNIP 2026-2
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Status-Em%20Desenvolvimento-yellow?style=flat-square"/>
  <img src="https://img.shields.io/badge/C%23-239120?style=flat-square&logo=csharp&logoColor=white"/>
  <img src="https://img.shields.io/badge/SQL%20Server-CC2927?style=flat-square&logo=microsoftsqlserver&logoColor=white"/>
  <img src="https://img.shields.io/badge/Kotlin-7F52FF?style=flat-square&logo=kotlin&logoColor=white"/>
  <img src="https://img.shields.io/badge/Metodologia-Scrum-0052CC?style=flat-square"/>
</p>

---

## 📌 Descrição do Desafio

O **Avalia+** é uma plataforma de simulados e acompanhamento de desempenho acadêmico desenvolvida como evolução do projeto apresentado no PIM III.

No PIM IV, o projeto amplia a solução anterior com a construção da camada de dados, API, novas funcionalidades e aplicações integradas para diferentes plataformas.

A solução permite que estudantes realizem simulados, acompanhem seu desempenho e utilizem recursos de gamificação para incentivar a prática contínua. Professores podem contribuir com o banco de questões e acompanhar o desempenho dos alunos, enquanto administradores realizam o gerenciamento da estrutura institucional.

O sistema é composto por aplicações **Web, Desktop e Mobile**, integradas por uma mesma API e utilizando um banco de dados compartilhado.

---

## 📋 Backlog de Produto

| ID   | Funcionalidade                               | Prioridade | Plataforma             |
| ---- | -------------------------------------------- | ---------- | ---------------------- |
| PB01 | Autenticação e controle de acesso por perfil | 🔴 Alta    | Web · Desktop · Mobile |
| PB02 | Gestão de cadastros institucionais           | 🔴 Alta    | Web · Desktop          |
| PB03 | Gestão do banco de questões                  | 🔴 Alta    | Web · Desktop          |
| PB04 | Geração e realização de simulados            | 🔴 Alta    | Web · Mobile           |
| PB05 | Correção automática e revisão comentada      | 🔴 Alta    | Web · Mobile           |
| PB06 | Acompanhamento de desempenho                 | 🔴 Alta    | Web · Mobile           |
| PB07 | Gamificação por pontuação e medalhas         | 🔴 Alta    | Web · Mobile           |
| PB08 | Acessibilidade e integração com VLibras      | 🔴 Alta    | Web                    |
| PB09 | Aplicação administrativa Desktop             | 🟡 Média   | Desktop                |
| PB10 | Aplicativo Mobile do Aluno                   | 🔴 Alta    | Mobile                 |

As funcionalidades estão vinculadas aos requisitos funcionais RF01 a RF07 e aos itens definidos no Product Backlog do PIM IV.

---

## 🗓️ Cronograma de Sprints

```
Sprint 1 ────────────────────────────────
  18/08 a 29/08
  Requisitos, regras de negócio e repositório

Sprint 2 ────────────────────────────────
  01/09 a 12/09
  Modelagem e documentos do banco de dados

Sprint 3 ────────────────────────────────
  15/09 a 26/09
  Banco físico, API e autenticação

Sprint 4 ──────────────────────────────── 🔄 Em andamento
  29/09 a 10/10
  Cadastros e banco de questões

Sprint 5 ────────────────────────────────
  13/10 a 24/10
  Simulados, correção e desempenho

Sprint 6 ────────────────────────────────
  27/10 a 07/11
  Gamificação, acessibilidade e documentação

Sprint 7 ────────────────────────────────
  10/11 a 21/11
  Mobile, integração e ajustes finais
```

O cronograma oficial estabelece sete sprints de duas semanas, com início em 18 de agosto e término em 21 de novembro de 2026.

---

## 📅 Tabela de Sprints

| Sprint   | Período       | Foco principal                              | Documentação                                                |
| -------- | ------------- | ------------------------------------------- | ----------------------------------------------------------- |
| Sprint 1 | 18/08 – 29/08 | Requisitos, regras de negócio e repositório | [📄 Ver docs](./scrum/sprint-planning/sprint-1-planning.md) |
| Sprint 2 | 01/09 – 12/09 | Modelagem e documentos do banco de dados    | [📄 Ver docs](./scrum/sprint-planning/sprint-2-planning.md) |
| Sprint 3 | 15/09 – 26/09 | Banco físico, API e autenticação            | [📄 Ver docs](./scrum/sprint-planning/sprint-3-planning.md) |
| Sprint 4 | 29/09 – 10/10 | Cadastros e banco de questões               | [📄 Ver docs](./scrum/sprint-planning/sprint-4-planning.md) |
| Sprint 5 | 13/10 – 24/10 | Simulados, correção e desempenho            | [📄 Ver docs](./scrum/sprint-planning/sprint-5-planning.md) |
| Sprint 6 | 27/10 – 07/11 | Gamificação, acessibilidade e documentação  | [📄 Ver docs](./scrum/sprint-planning/sprint-6-planning.md) |
| Sprint 7 | 10/11 – 21/11 | Mobile, integração e ajustes finais         | [📄 Ver docs](./scrum/sprint-planning/sprint-7-planning.md) |

---

## 🛠️ Tecnologias Utilizadas

| Camada            | Tecnologia            |
| ----------------- | --------------------- |
| Frontend Web      | HTML, CSS, JavaScript |
| Backend / API     | C#                    |
| Banco de Dados    | SQL Server            |
| Aplicação Desktop | C# · Windows Forms    |
| Aplicação Mobile  | Kotlin                |
| Acessibilidade    | VLibras               |
| Metodologia       | Scrum                 |
| Versionamento     | Git + GitHub          |
| Gerenciamento     | Trello                |

A aplicação Desktop possui obrigatoriamente interface desenvolvida em C# com Windows Forms, enquanto o aplicativo Mobile está previsto em Kotlin.

---

## 📁 Estrutura do Projeto

```text
avalia-plus/
│
├── 📁 diagramas/
│   ├── 📁 banco-de-dados/       # MER, DER, modelo lógico e dicionário
│   ├── 📁 fluxo-de-usuario/     # Fluxos por perfil
│   └── 📁 uml/                  # Casos de Uso, Classes, Sequência,
│                                # Implantação e Implementação
│
├── 📁 docs/
│   ├── 📁 requisitos/            # Requisitos e regras de negócio
│   ├── 📁 pim/                  # Documentação acadêmica
│   └── 📁 assets/                # Imagens utilizadas na documentação
│
├── 📁 scrum/
│   ├── 📁 backlog/              # Gerenciamento de Backlog
│   ├── 📁 product-backlog/      # Product Backlog
│   ├── 📁 sprint-backlog/       # Backlogs por sprint
│   ├── 📁 sprint-planning/      # Planejamentos
│   ├── 📁 sprint-retrospective/ # Retrospectivas
│   ├── 📁 sprint-review/        # Reviews
│   └── 📁 dailys/               # Registros das Dailys
│
├── 📁 src/
│   ├── 📁 web/                  # Aplicação Web
│   ├── 📁 api/                  # API
│   ├── 📁 desktop/              # Aplicação Desktop
│   └── 📁 mobile/               # Aplicativo Mobile
│
└── 📄 README.md
```

---

## ▶️ Como Executar o Projeto

> ⚠️ O sistema está em desenvolvimento. As instruções de execução serão atualizadas conforme as aplicações forem implementadas e integradas.

### Pré-requisitos

* .NET / C#
* SQL Server
* Kotlin
* Android Studio
* Git

### Clone o repositório

```bash
git clone https://github.com/SEU_USUARIO/avalia-plus.git

cd avalia-plus
```

> As instruções específicas de configuração da API, banco de dados, aplicação Web, Desktop e Mobile serão adicionadas conforme a implementação do projeto.

---

## 📂 Documentação Completa

Acesse a pasta [`/docs`](./docs/) para consultar a documentação técnica e acadêmica do projeto.

Entre os principais documentos estão:

* Requisitos funcionais e não funcionais
* Regras de negócio
* Matriz de rastreabilidade
* Modelo Entidade-Relacionamento
* Diagrama Entidade-Relacionamento
* Modelo lógico do banco de dados
* Dicionário de dados
* Diagramas UML
* Arquitetura da solução
* Documentação das aplicações
* Gerenciamento do projeto com Scrum
* Documentação exigida pelo PIM IV

A documentação acadêmica segue as orientações estabelecidas no Manual do PIM IV, incluindo as etapas de caracterização da organização, planejamento da solução, desenvolvimento, arquitetura, banco de dados, infraestrutura e gerenciamento ágil.

---
---

## 👥 Equipe
<table align="center">
  <tr>
    <td align="center"> 
      <b>Gabriel Vinicius Rosa Pereira</b><br/>
      Product Owner · Frontend · Integração<br/>
      <sub>HTML · CSS · JavaScript · Kotlin</sub><br/><br/>
      <a href="https://github.com/GabrielVRosa">GitHub</a> · <a href="https://www.linkedin.com/in/gabriel-vinicius-6a6059352/">LinkedIn</a>
    </td>
    <td align="center">
      <b>Isabella Santos Leal</b><br/>
      Scrum Master · Banco de Dados · Documentação<br/>
      <sub>SQL Server · GitHub · Figma</sub><br/><br/>
      <a href="https://github.com/IsabellaLeal06">GitHub</a> · <a href="https://www.linkedin.com/in/isabella-santos-1148b02b9/">LinkedIn</a>
    </td>
    <td align="center">
      <b>Letícia Aparecida Santos Mota</b><br/>
      Desenvolvedora Backend · UML<br/>
      <sub>C# · API · Diagramas UML</sub><br/><br/>
      <a href="https://github.com/Jmclemota">GitHub</a> · <a href="https://www.linkedin.com/in/let%C3%ADcia-aparecida-a465b7313/">LinkedIn</a>
    </td>
  </tr>
</table>
