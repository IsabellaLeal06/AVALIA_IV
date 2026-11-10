# Avalia+ | API REST

API em **Node.js + Express** que concentra as regras de negócio do Avalia+ (RNF07). As aplicações Web, Mobile e Desktop usam esta API (a Desktop foi migrada no INF14, Sprint 7).

Itens do backlog: **INF08 a INF13**, **PB01 a PB08** e **TST01 a TST03, TST06**.

## Como rodar (passo a passo)

1. Instale o **Node.js 20 LTS** ou mais novo: <https://nodejs.org>.
2. Abra um terminal na pasta `api` e instale as dependências:
   ```bash
   npm install
   ```
3. Copie o modelo de configuração:
   ```bash
   copy .env.example .env      # Windows
   cp .env.example .env        # Linux/macOS
   ```
4. Abra o `.env` e preencha:
   - `DB_CONEXAO`: a mesma string de conexão do `appsettings.Local.json` da Desktop. **Mantenha as aspas**: sem elas, uma senha com `#` é cortada, porque o `#` vira comentário.
   - `JWT_SEGREDO`: um texto aleatório com 32 caracteres ou mais. Para gerar um:
     ```bash
     node -e "console.log(require('crypto').randomBytes(48).toString('hex'))"
     ```
5. Inicie a API:
   ```bash
   npm start
   ```
6. Abra <http://localhost:3000>. A própria API serve a aplicação web (pasta `../web`).

> O IP do computador precisa estar liberado no firewall do Azure (Servidor SQL → Segurança → Rede).
> O `.env` **nunca** vai para o GitHub. Só o `.env.example`, sem valores reais, é versionado.

Usuários de teste (dados fictícios do `04_dados_teste.sql`; a senha inicial é o CPF):

| Perfil | E-mail | Senha |
|---|---|---|
| Administrador | admin@avaliamais.test | 15881399803 |
| Professor | ricardo.monteiro@avaliamais.test | 69879730917 |
| Aluno | ana.souza@aluno.avaliamais.test | 88447294072 |

## Decisões registradas

| Assunto | Decisão |
|---|---|
| **Prazo do token** | **7 dias** (`JWT_PRAZO=7d`). Prazo único, que serve também ao aplicativo móvel. Não há renovação de token. |
| **Conta desativada** | O token não basta: a API confere a situação da conta (com cache de 30 s). Conta desativada ou com perfil alterado perde o acesso sem esperar o token vencer. |
| **E-mail (RN03)** | `nodemailer` atrás de uma interface. Padrão: **modo log** (`EMAIL_MODO=log`), que mostra o e-mail com o código no console da API. Para enviar de verdade, use `EMAIL_MODO=smtp` e preencha `SMTP_*`. |
| **Código de recuperação** | 6 dígitos, válido por 15 minutos (`CODIGO_VALIDADE_MINUTOS`), gravado como hash (HMAC-SHA256), de uso único. Cinco tentativas erradas invalidam o código. |
| **Hash de senha** | BCrypt com custo 10 (`bcryptjs`), compatível com a Desktop (`BCrypt.Net-Next`). |
| **Horário** | As colunas DATETIME guardam o horário de Brasília, como nos dados de teste. A API envia datas sem fuso (`2026-10-14T19:30:00`). |
| **Imagens** | Upload em `POST /upload/imagem` (PNG, JPG, GIF ou WEBP, até 2 MB, tipo conferido pelo conteúdo e nome gerado pelo servidor). O banco guarda o caminho relativo (`uploads/questoes/...`); as respostas trazem o endereço completo. |
| **RN11** | "Questão já utilizada" = questão que entrou em algum simulado (tabela `SimuladoQuestao`), como diz a regra v2.2 e como faz a Desktop. |
| **Pontuação (INF06)** | O trigger `trg_ResultadoSimulado_AcumularPontuacao` (script 06) soma os pontos ao gravar o resultado. A API só soma por conta própria se esse trigger não existir ou estiver desabilitado, então não conta em dobro e funciona com o banco só nos scripts 01 a 04. |
| **Medalhas** | Além do que o PROMPT pedia, há o cadastro de faixas (`/medalhas`), com as mesmas regras da tela Gerenciar Medalha da Desktop, para a migração INF14. A exclusão de faixa é física, como na Desktop: a faixa não tem vínculo com aluno. |

## Estrutura

```text
api/
├── .env.example            # modelo de configuração (sem valores reais)
├── package.json
├── src/
│   ├── server.js           # inicialização
│   ├── app.js              # montagem do Express (usado também pelos testes)
│   ├── config/             # variáveis do .env e conexão mssql (consultas sempre parametrizadas)
│   ├── middleware/         # autenticação/autorização (protegido por padrão) e tratamento de erros
│   ├── routes/             # todas as rotas, com o perfil de cada uma
│   ├── controllers/        # entrada e saída HTTP (sem SQL e sem regra)
│   ├── services/           # regras de negócio (RN)
│   ├── repositories/       # acesso ao banco
│   └── utils/              # validações (CPF, e-mail, senha), erros, datas
└── test/                   # testes (node:test)
```

**Rota nova nasce protegida:** toda rota passa por `acao()`, que recusa a requisição se a rota não declarar quem pode usá-la (`permitir(...)` ou `publico`).

**Formato de erro** (igual em todas as rotas):

```json
{ "erro": { "mensagem": "Confira os campos destacados.", "campos": { "cpf": "CPF inválido. Confira os dígitos." } } }
```

Detalhes técnicos (pilha, SQL, nome de tabela) vão só para o log da API, com um código que aparece na mensagem ao cliente.

## Endpoints

Todos exigem `Authorization: Bearer <token>`, exceto o login e as três rotas de recuperação de senha.

### Autenticação e senha
| Método | Rota | Perfil | Observação |
|---|---|---|---|
| POST | `/auth/login` | aberta | `{ email, senha }` → `{ token, usuario, senhaInicial }`. Mensagem genérica na falha. |
| POST | `/auth/alterar-senha` | todos | `{ senhaAtual, novaSenha, confirmacao }`, sempre do usuário do token |
| POST | `/auth/recuperar-senha` | aberta | `{ email }`. Mesma resposta exista ou não o e-mail |
| GET | `/auth/validar-codigo?email=&codigo=` | aberta | Confere o código antes de mostrar os campos |
| POST | `/auth/nova-senha` | aberta | `{ email, codigo, novaSenha, confirmacao }`. Limpa o código (uso único) |
| GET | `/perfil` | todos | Dados do usuário; para o Aluno, pontuação e medalha apurada |

### Cadastros (Administrador)
| Método | Rota | Observação |
|---|---|---|
| GET | `/painel` | Turmas, usuários, disciplinas e questões **ativos** |
| GET | `/usuarios?perfil=&turma=&situacao=&busca=` | **Sem CPF** |
| GET | `/usuarios/:id` | CPF aparece só aqui |
| POST / PUT | `/usuarios`, `/usuarios/:id` | `{ nome, cpf, email, idPerfil, idTurma, dataIngresso }`. Senha inicial = CPF |
| PATCH | `/usuarios/:id/situacao` | `{ ativo: false }` desativa, `{ ativo: true }` reativa |
| GET / POST / PUT | `/turmas`, `/turmas/:id` | `{ nome, anoLetivo, disciplinas?: [ids] }` |
| PATCH | `/turmas/:id/situacao` | Devolve `alunosAtivos` para a tela alertar, sem impedir |
| POST | `/turmas/:id/disciplinas` | `{ idDisciplina }` inclui na grade |
| DELETE | `/turmas/:id/disciplinas/:idDisciplina` | Tira da grade (não apaga a disciplina) |
| GET / POST / PUT | `/disciplinas`, `/disciplinas/:id` | `{ nome, descricao, idResponsavel, turmas?: [ids] }`. Professor no GET vê só as suas |
| PATCH | `/disciplinas/:id/situacao` | |
| GET / POST / PUT / DELETE | `/medalhas`, `/medalhas/:id` | `{ nome, pontosMinimos, imagemUrl }` |

`situacao` aceita `ativo`, `inativo` ou `todos`.

### Banco de questões (Administrador e Professor)
| Método | Rota | Observação |
|---|---|---|
| GET | `/questoes?disciplina=&situacao=&busca=` | Escopo pelo token (RN08). Traz quem cadastrou e `utilizada` (RN11) |
| GET | `/questoes/:id` | Alternativas, correta e `utilizada` (RN11) |
| POST / PUT | `/questoes`, `/questoes/:id` | `{ idDisciplina, enunciado, explicacao, imagemUrl, correta: "C", alternativas: [{ letra, texto }] }` |
| PATCH | `/questoes/:id/situacao` | Professor desativa; só o Administrador reativa |
| POST | `/upload/imagem?tipo=questoes\|medalhas` | multipart, campo `imagem` |

### Simulado (Aluno)
| Método | Rota | Observação |
|---|---|---|
| GET | `/disciplinas/ativas` | Com a quantidade de questões disponíveis |
| POST | `/simulados` | `{ disciplinas: [ids], quantidade: 10\|15\|20 }` |
| GET | `/simulados/:id` | Questões **sem gabarito e sem explicação** |
| POST | `/simulados/:id/finalizar` | `{ respostas: [{ idQuestao, alternativa: "A"\|null }] }`. Reenvio devolve o resultado já apurado (`jaFinalizado: true`) |
| GET | `/simulados/:id/resultado` | Resultado com desempenho por disciplina |
| GET | `/simulados/:id/revisao` | Gabarito e explicação, só depois de finalizar |
| GET | `/simulados?pagina=&tamanho=` | Histórico paginado |

### Desempenho
| Método | Rota | Perfil |
|---|---|---|
| GET | `/desempenho/aluno` | Aluno |
| GET | `/desempenho/turmas` | Professor |
| GET | `/desempenho/turmas/:id` | Professor |
| GET | `/desempenho/turmas/:id/alunos` | Professor |
| GET | `/desempenho/turmas/:id/alunos/:idAluno` | Professor (painel consolidado) |
| GET | `/desempenho/questoes-erro?turma=&limite=` | Professor |

O Administrador não tem nenhuma rota de desempenho (RN24). O escopo do professor está escrito uma única vez em `src/repositories/desempenhoRepository.js` (`escopoProfessor`).

## Testes

```bash
npm test
```

- Os testes de **unidade** (CPF, senha, correção, caminhos de imagem) rodam sempre.
- Os testes de **integração** usam um **banco de TESTE** informado em `DB_CONEXAO_TESTE` no `.env`, com os scripts 01 a 06 da pasta `database` (sem o 05 e o 06, só os testes de procedures e triggers são pulados). Eles criam simulados, trocam senhas e desativam registros. **Nunca aponte para o banco de produção.** Sem `DB_CONEXAO_TESTE`, eles são pulados com um aviso.
- Banco de teste local (opcional, com Docker):
  ```bash
  docker run -d --name sqlavalia -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=Teste#Local2026" -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
  ```
  Crie o banco `avalia`, rode os scripts 01 a 06 e use no `.env`:
  `DB_CONEXAO_TESTE="Server=tcp:localhost,1433;Initial Catalog=avalia;User ID=sa;Password=Teste#Local2026;Encrypt=True;TrustServerCertificate=True;"`

Cobertura: autenticação, conta desativada e mensagem genérica; recuperação de senha (válido, expirado, reutilizado e pedido novo); visibilidade por perfil (professor fora da turma, aluno em simulado de outro, administrador em desempenho, CPF fora da listagem); desativação em cascata; sorteio (mínimo de 2 disciplinas, 10/15/20, sem repetição, só ativas de disciplinas ativas); correção (branco = erro, pontos = acertos × 10, pontuação = soma dos PontosObtidos); tentativa única (inclusive finalizações simultâneas); RN07, RN09 e RN11; gamificação.

## Executar em container (opcional)

A API não guarda estado em memória que impeça várias instâncias, exceto o cache de 30 s da situação da conta e o contador de tentativas de código. Basta uma imagem `node:20-alpine` com `npm ci --omit=dev` e `npm start`, as variáveis do `.env` passadas pelo ambiente, e a pasta `uploads` num volume.
