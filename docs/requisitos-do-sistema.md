# Requisitos do Sistema – Avalia+

## 1. Requisitos Funcionais

### RF01 – Autenticação e controle de acesso por perfil

O sistema deve permitir acesso por e-mail e senha, liberando funcionalidades conforme o perfil do usuário.

A autenticação utiliza token de sessão com prazo de expiração e o servidor deve validar as permissões em cada operação.

**Plataformas:** Web, Desktop e Mobile.

### RF02 – Gestão de cadastros institucionais

O Administrador deve poder cadastrar e manter usuários, turmas e disciplinas.

Os registros institucionais não são excluídos fisicamente; são desativados e podem ser reativados.

**Plataformas:** Web e Desktop.

### RF03 – Gestão do banco de questões

O sistema deve permitir o cadastro e manutenção de questões de múltipla escolha organizadas por disciplina.

Cada questão possui exatamente cinco alternativas, de A a E, com apenas uma correta.

O Administrador atua sobre qualquer disciplina e o Professor atua somente sobre as disciplinas sob sua responsabilidade.

**Plataformas:** Web e Desktop.

### RF04 – Geração e realização de simulados

O Aluno deve poder selecionar duas ou mais disciplinas e escolher a quantidade de questões entre 10, 15 ou 20.

O sistema sorteia aleatoriamente questões ativas sem repetição dentro do mesmo simulado.

**Plataformas:** Web e Mobile.

### RF05 – Correção automática e revisão comentada

Após finalizar o simulado, o sistema deve realizar a correção automaticamente e apresentar o resultado imediatamente.

O Aluno pode revisar as questões, visualizando sua resposta, a resposta correta e a explicação quando disponível.

**Plataformas:** Web e Mobile.

### RF06 – Acompanhamento de desempenho

O sistema deve registrar o histórico dos simulados e apresentar informações de desempenho.

O Aluno acompanha seu próprio desempenho. Professores podem acompanhar indicadores relacionados às suas disciplinas e turmas.

O Administrador possui informações cadastrais, mas não indicadores de desempenho acadêmico.

**Plataformas:** Web e Mobile conforme o perfil.

### RF07 – Gamificação por pontuação e medalhas

O sistema deve atribuir pontos aos Alunos conforme seus resultados e relacionar a pontuação acumulada às faixas de medalhas previstas.

**Plataformas:** Web e Mobile.

## 2. Requisitos Não Funcionais

### RNF01 – Segurança das credenciais

As senhas não devem ser armazenadas em texto legível. O sistema deve utilizar hash e controle de sessão por token.

### RNF02 – Controle de acesso

As permissões devem ser verificadas no servidor, não apenas pela interface.

### RNF03 – Integridade dos dados

O banco deve utilizar chaves, restrições de domínio, unicidade e mecanismos necessários para preservar a consistência dos dados.

### RNF04 – Persistência da pontuação

A pontuação acumulada do usuário deve permanecer registrada conforme definido pelo modelo de dados.

### RNF05 – Consistência entre plataformas

Web, Desktop e Mobile devem utilizar a mesma API e o mesmo banco de dados, garantindo consistência das informações.

### RNF06 – Acessibilidade

A solução deve contemplar recursos de acessibilidade, incluindo a integração do VLibras na aplicação Web.

### RNF07 – Arquitetura e infraestrutura

A solução deve considerar arquitetura modular, integração por API e os aspectos de infraestrutura em nuvem, DevOps, segurança, desempenho e escalabilidade previstos no PIM IV.

## 3. Distribuição por plataforma

| Funcionalidade/Perfil | Web | Desktop | Mobile |
|---|:---:|:---:|:---:|
| Aluno | ✓ | — | ✓ |
| Professor | ✓ | — | — |
| Administrador | ✓ | ✓ | — |

## 4. Rastreabilidade

| Requisito | Principais regras relacionadas |
|---|---|
| RF01 | RN01, RN02, RN03, RN24, RN26 |
| RF02 | RN01, RN04, RN05, RN06, RN07, RN12, RN27 |
| RF03 | RN07, RN08, RN09, RN10, RN11, RN12, RN27 |
| RF04 | RN12, RN13, RN14, RN15, RN16, RN17, RN18 |
| RF05 | RN18, RN19 |
| RF06 | RN20, RN24, RN25 |
| RF07 | RN21, RN22, RN23 |
