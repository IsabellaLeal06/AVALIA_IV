# Escopo do Projeto – Avalia+

## 1. Visão geral

O **Avalia+** é uma plataforma de simulados e acompanhamento de desempenho acadêmico desenvolvida para uma instituição de ensino.

O PIM IV representa a continuação do PIM III. O protótipo web existente é ampliado para uma solução integrada, com camada de dados, API, aplicação administrativa Desktop e aplicativo Mobile para alunos.

## 2. Objetivo

Desenvolver uma solução tecnológica integrada capaz de:

- autenticar usuários e controlar o acesso conforme o perfil;
- manter os cadastros institucionais;
- manter um banco de questões;
- permitir a criação de questões por professores responsáveis pelas disciplinas;
- gerar simulados automaticamente a partir de duas ou mais disciplinas;
- corrigir simulados automaticamente;
- apresentar revisão comentada;
- registrar e acompanhar o desempenho acadêmico;
- utilizar pontuação e medalhas como elementos de gamificação;
- disponibilizar recursos de acessibilidade;
- integrar Web, Desktop e Mobile por meio de uma API comum.

## 3. Plataformas

### Web

É a plataforma de referência do sistema e contempla funcionalidades dos três perfis.

- Aluno;
- Professor;
- Administrador.

O cadastro de questões pelo Professor e a integração com o VLibras estão previstos na aplicação Web.

### Desktop

É a aplicação administrativa destinada ao Administrador.

- tecnologia obrigatória para a interface: **C# com Windows Forms**;
- telas de cadastro e edição do Administrador;
- consumo da mesma API e banco utilizados pelas demais aplicações.

### Mobile

É o aplicativo destinado ao Aluno.

Contempla o ciclo de realização de simulados e acompanhamento de desempenho em mobilidade.

## 4. Funcionalidades incluídas

O escopo funcional está organizado nos seguintes requisitos:

- **RF01:** Autenticação e controle de acesso por perfil;
- **RF02:** Gestão de cadastros institucionais;
- **RF03:** Gestão do banco de questões;
- **RF04:** Geração e realização de simulados;
- **RF05:** Correção automática e revisão comentada;
- **RF06:** Acompanhamento de desempenho;
- **RF07:** Gamificação por pontuação e medalhas.

## 5. Requisitos não funcionais

O projeto também contempla requisitos relacionados a:

- segurança e proteção das credenciais;
- desempenho;
- integridade dos dados;
- persistência da pontuação;
- acessibilidade;
- compatibilidade entre as aplicações;
- arquitetura de software.

## 6. Banco de dados

O modelo atual possui **13 entidades e 18 relacionamentos**.

Entre as entidades estão:

- Perfil;
- Usuário;
- Turma;
- Disciplina;
- Turma_Disciplina;
- Questão;
- Alternativa;
- Simulado;
- Simulado_Questão;
- Resposta_Aluno;
- Resultado_Simulado;
- Desempenho;
- Medalha.

## 7. Fora do escopo

Não fazem parte do escopo definido:

- autocadastro de usuários;
- importação de listas de usuários;
- correção manual de simulados;
- classificação das questões por dificuldade;
- limite de tempo para realização do simulado;
- reaproveitamento do mesmo simulado para novas tentativas;
- exclusão física de usuários, turmas, disciplinas e questões.

## 8. Evolução em relação ao PIM III

O PIM IV mantém a proposta central do Avalia+, mas amplia sua estrutura com:

- banco de dados corporativo;
- API e serviços de integração;
- autenticação por token;
- recuperação de senha;
- participação do Professor no banco de questões;
- gamificação por pontuação e medalhas;
- acessibilidade com VLibras;
- aplicação Desktop administrativa;
- aplicativo Mobile;
- integração entre as plataformas.

## 9. Limites de desenvolvimento

As três aplicações compartilham a mesma API e o mesmo banco de dados. A divisão de telas por plataforma representa o escopo de desenvolvimento de cada aplicação, não uma restrição de autenticação.
