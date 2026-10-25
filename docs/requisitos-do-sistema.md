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

### RNF01 – Segurança e controle de acesso

O sistema deve garantir que cada usuário acesse exclusivamente as funcionalidades e os dados correspondentes ao seu perfil.

As senhas são armazenadas com algoritmo de hash, a autenticação ocorre por token com prazo de expiração e a autorização é verificada no servidor a cada operação. A comunicação entre as aplicações e o servidor ocorre sobre HTTPS, as consultas ao banco são parametrizadas e os códigos de recuperação de senha são aleatórios, de uso único e com prazo de validade.

Alinhado às práticas de controle de acesso da ISO/IEC 27001.

### RNF02 – Proteção de dados e conformidade com a LGPD

O sistema deve tratar os dados pessoais em conformidade com a Lei Geral de Proteção de Dados.

A coleta limita-se aos dados necessários à finalidade educacional. O CPF é tratado como dado pessoal e não é exibido em listagens ou relatórios. Os dados de desempenho são acessíveis somente a quem possui vínculo pedagógico legítimo com o aluno, e as mensagens de erro não expõem informações internas do sistema.

Conforme a Lei n. 13.709/2018.

### RNF03 – Integridade dos dados

O banco de dados deve assegurar a consistência das informações ao longo de todo o ciclo de vida do sistema.

A integridade referencial é garantida por chaves estrangeiras, as operações compostas são executadas em transação e restrições de unicidade e de domínio impedem dados duplicados ou inválidos. As remoções são lógicas, e os registros de simulados tornam-se imutáveis após a correção.

### RNF04 – Desempenho

O sistema deve responder às solicitações em tempo adequado a uma utilização fluida.

As consultas devem ser respondidas em até três segundos em condições normais de uso, e a geração de um simulado em até cinco segundos. As colunas de maior frequência de consulta recebem índices, e a pontuação acumulada é mantida em coluna própria, evitando o recálculo de todo o histórico a cada exibição do perfil.

### RNF05 – Usabilidade e responsividade

As interfaces devem ser claras e adequadas ao dispositivo em que são utilizadas.

A interface web adapta-se a diferentes resoluções, o aplicativo móvel a diferentes tamanhos de dispositivo e a aplicação desktop aproveita telas amplas. O sistema oferece retorno visual imediato, validação e confirmação das operações críticas, mantendo padronização visual e de navegação entre as plataformas.

Alinhado à ISO 9241-11 e às heurísticas de usabilidade de Nielsen.

### RNF06 – Acessibilidade e inclusão digital

O sistema deve oferecer recursos que ampliem o acesso de usuários com deficiência.

A aplicação web integra o VLibras para os três perfis e oferece modo de alto contraste, ajuste do tamanho da fonte, navegação por teclado, marcação semântica e textos alternativos. No aplicativo móvel, os elementos interativos recebem rótulos de acessibilidade compatíveis com os leitores de tela nativos.

Alinhado às diretrizes WCAG 2.1, nível AA, e à Lei n. 10.436/2002.

### RNF07 – Manutenibilidade e escalabilidade

A solução deve ser organizada de modo a permitir manutenção e evolução sem reescrita estrutural.

O backend é organizado em camadas, com separação entre apresentação, regra de negócio e persistência, e a regra de negócio é centralizada na API, e não replicada nas três aplicações. Parâmetros sujeitos a mudança institucional, como as faixas de medalhas, são mantidos em tabelas do banco. A arquitetura está preparada para execução em containers, admitindo escalonamento horizontal da API.

Alinhado às práticas de ciclo de vida de software da ISO/IEC 12207.

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
