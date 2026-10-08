# Tipos de Usuários – Avalia+

O Avalia+ possui três perfis de acesso: **Aluno, Professor e Administrador**.

O perfil é definido no cadastro do usuário e determina as funcionalidades disponíveis.

## 1. Aluno

### Objetivo

Utilizar a plataforma para realizar simulados e acompanhar o próprio desempenho acadêmico.

### Principais funcionalidades

- acessar a plataforma com e-mail e senha;
- alterar a própria senha;
- solicitar recuperação de senha;
- selecionar duas ou mais disciplinas;
- escolher simulados de 10, 15 ou 20 questões;
- realizar simulados;
- navegar entre questões;
- alterar respostas antes da finalização;
- visualizar o resultado imediatamente;
- revisar questões e explicações disponíveis;
- consultar histórico;
- acompanhar desempenho;
- acumular pontos;
- receber medalhas conforme a pontuação.

### Plataformas

- **Web:** disponível;
- **Mobile:** disponível;
- **Desktop:** não faz parte do escopo de telas do Aluno.

## 2. Professor

### Objetivo

Contribuir com o banco de questões e acompanhar o desempenho acadêmico relacionado às suas disciplinas.

### Principais funcionalidades

- acessar a plataforma;
- alterar a própria senha;
- cadastrar questões das disciplinas sob sua responsabilidade;
- editar questões permitidas pelas regras de negócio;
- desativar questões de suas disciplinas;
- consultar questões das disciplinas sob sua responsabilidade;
- acompanhar indicadores de desempenho das turmas e disciplinas sob sua responsabilidade.

### Restrições

O Professor não pode:

- cadastrar ou alterar dados institucionais de usuários, turmas e disciplinas;
- acessar questões de disciplinas que não estejam sob sua responsabilidade;
- alterar a alternativa correta de questão já utilizada em simulado;
- criar novas contas de usuário.

### Plataforma

- **Web:** disponível;
- **Desktop:** não possui telas previstas;
- **Mobile:** não possui telas previstas.

## 3. Administrador

### Objetivo

Administrar a estrutura institucional e o banco de questões da plataforma.

### Principais funcionalidades

- criar contas de Aluno, Professor e Administrador;
- cadastrar turmas;
- cadastrar disciplinas;
- definir o professor responsável pela disciplina;
- desativar e reativar usuários;
- desativar e reativar turmas;
- desativar e reativar disciplinas;
- cadastrar, consultar, editar e desativar questões;
- atuar sobre questões de qualquer disciplina;
- acompanhar informações cadastrais da base.

### Restrições

O Administrador não possui indicadores de desempenho acadêmico como funcionalidade de acompanhamento. Seu painel é voltado às informações cadastrais da base.

### Plataformas

- **Web:** disponível;
- **Desktop:** disponível;
- **Mobile:** não faz parte do escopo do Administrador.

## 4. Matriz de acesso

| Funcionalidade | Aluno | Professor | Administrador |
|---|:---:|:---:|:---:|
| Autenticação | ✓ | ✓ | ✓ |
| Alteração da própria senha | ✓ | ✓ | ✓ |
| Recuperação de senha | ✓ | ✓ | ✓ |
| Gestão de usuários | — | — | ✓ |
| Gestão de turmas | — | — | ✓ |
| Gestão de disciplinas | — | — | ✓ |
| Gestão de questões | — | Próprias disciplinas | Todas |
| Geração de simulados | ✓ | — | — |
| Realização de simulados | ✓ | — | — |
| Revisão de simulados | ✓ | — | — |
| Histórico do aluno | Próprio | Indicadores permitidos | — |
| Desempenho | Próprio | Suas disciplinas/turmas | — |
| Pontuação e medalhas | ✓ | — | — |

## 5. Distribuição por plataforma

```text
                 WEB       DESKTOP       MOBILE
Aluno             X           -            X
Professor         X           -            -
Administrador     X           X            -
```
