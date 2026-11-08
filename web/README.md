# Avalia+ | Aplicação Web

Aplicação web do Avalia+ em **HTML, CSS e JavaScript puro** (entregável do PIM III, ajustado para a versão 2.2 dos requisitos e integrado à API). Tem as telas dos três perfis: Aluno, Professor e Administrador (RN26).

## Como abrir

A web não acessa o banco: ela conversa com a **API** (pasta `../api`).

1. Siga o passo a passo do `api/README.md` e rode `npm start` na pasta `api`.
2. Abra <http://localhost:3000>. A própria API serve esta pasta, sem precisar de outro servidor.

Se preferir o **Live Server** do VS Code (porta 5500), deixe a API rodando e, em `js/config.js`, troque `apiUrl: ''` por `apiUrl: 'http://localhost:3000'`. A origem `http://127.0.0.1:5500` já vem liberada no `.env.example` da API.

## Estrutura

```text
web/
├── index.html              # página inicial (a única indexável: title, description, Open Graph)
├── pages/                  # todas as outras páginas (com noindex)
├── css/
│   ├── base.css            # variáveis, cabeçalho, menu, cards, tabelas, formulários, botões
│   ├── publico.css         # início, login e recuperação de senha
│   ├── simulado.css        # montagem, realização, resultado e revisão
│   ├── perfil.css          # meu perfil e alterar senha
│   ├── professor.css       # dashboard e turmas do professor
│   └── cadastro.css        # formulários de questão e medalha
├── js/
│   ├── config.js           # endereço da API
│   ├── api.js              # ÚNICO ponto de chamada à API
│   ├── sessao.js           # token, perfil e guarda de página
│   ├── menu.js             # menu montado a partir do perfil (itens num objeto único) e cabeçalho
│   ├── ui.js               # mensagens na tela, erros de campo, diálogo de confirmação, formatação
│   ├── acessibilidade.js   # VLibras (um arquivo para todas as páginas)
│   ├── cadastro.js         # partes comuns das listagens e formulários
│   ├── vendor/chart.umd.js # Chart.js 4.4.1 (licença MIT), local para funcionar sem internet
│   └── <tela>.js           # um arquivo por tela
└── assets/
    ├── img/                # logo, ilustrações e fundo do login (otimizado: 8,5 MB → 114 KB)
    └── medalhas/           # bronze.png, prata.png e ouro.png (caminhos usados pela tabela Medalha)
```

## Páginas

| Perfil | Páginas |
|---|---|
| Aberta | `index.html`, `login.html`, `recuperar.html`, `novasenha.html` |
| Todos | `alterarsenha.html` |
| Aluno | `indexdashboar.html`, `simulados.html`, `desempenho.html`, `criarsimulados.html`, `instrucsimulado.html`, `questoes.html`, `resultado.html`, `analise.html`, `perfil.html` |
| Professor | `dashboardprofessor.html`, `turmas.html`, `professorquestoes.html`, `criarquestao.html`, `perfilprofessor.html` |
| Administrador | `dashboardadmin.html`, `adminturmas.html`, `admindisciplinas.html`, `adminquestoes.html`, `adminusuarios.html`, `adminmedalhas.html`, `criarturma.html`, `criardisciplina.html`, `criarusuario.html`, `criarquestao.html`, `criarmedalha.html`, `perfiladmin.html` |

Os formulários de cadastro servem para incluir e, com `?id=`, para editar.

## O que mudou em relação ao PIM III

**Integração (seção 8.1 do PROMPT)**
- Login pela API, com token e redirecionamento pelo perfil que vem na resposta. Mensagem genérica na tela, sem `alert`.
- Guarda de sessão em todas as páginas internas (`sessao.js`). Quem digita o endereço de outra área volta para o próprio início. A API confere o perfil de novo em cada requisição.
- Simulado sorteado pela API (`POST /simulados`). O `questoes.json` saiu da web, e o gabarito não chega ao navegador durante a prova.
- Histórico e resultado vêm da API. O `localStorage` guarda só as marcações provisórias durante a realização.
- Recuperação de senha com código por e-mail, conferido antes de liberar os campos da nova senha.
- Nenhum dado fixo no HTML: nomes, taxas, quantidades e datas vêm da API.
- Menu montado em `menu.js`, com os itens dos três perfis num objeto único, e um dashboard por perfil.

**Divergências com a v2.2 (seção 8.2)**
- Simulado com 10, 15 ou 20 questões, mínimo de duas disciplinas (botão desabilitado até lá) e disciplinas vindas da API.
- Usuário com CPF, perfil em `select`, turma só para Aluno, data de ingresso e sem campo de senha.
- Turma com ano letivo. Disciplina com um `select` de professor ("Sem responsável" primeiro) e turmas em seleção múltipla.
- Questão com explicação opcional, disciplina filtrada pelo perfil e sem campo de dificuldade. Telas `adminquestoes.html`, `professorquestoes.html` e `criarquestao.html` criadas.
- Listagens com desativar/reativar (com confirmação), coluna e filtro de situação. Rótulo "Desativar", nunca "Excluir". Nenhuma coluna de ID ou CPF.
- Cards de desempenho removidos das telas do Administrador; ficam contadores da própria entidade. Dashboard do admin com o contador de usuários.
- Realização: "Anterior", avanço em branco, navegação numerada, "Finalizar" na última com confirmação de quantas ficaram em branco, sem menu lateral.
- Resultado sem tempo, com pontos obtidos, desempenho por disciplina e saudação conforme o aproveitamento.
- Revisão com explicação (quando houver), questões em branco identificadas e título com o simulado e a data.
- Perfil do aluno com pontuação, medalha, quantos pontos faltam e Bronze acinzentada para quem ainda não pontuou. Sem barra de progresso.
- Menu do professor com "Questões". Indicador de questões com maior índice de erro.
- Instruções avisando que sair da tela encerra a tentativa. A saída é interceptada com confirmação.
- Textos de "ENADE" e "competências" trocados por acervo da instituição e disciplina.
- `profile icon // makeit later` removido.

**Novas telas:** cadastro de faixas de medalha (`adminmedalhas.html` e `criarmedalha.html`) e `alterarsenha.html`.

## Acessibilidade (RNF06) e SEO

- **VLibras** em todas as páginas, pelo arquivo único `js/acessibilidade.js` (bloco fixo e script oficial do governo).
- Navegação por teclado com foco sempre visível, link "Pular para o conteúdo", `alt` em todas as imagens, `label` em todos os campos, erros associados ao campo (`aria-describedby`) e diálogos de confirmação acessíveis (`<dialog>`).
- **SEO:** `title`, `meta description` e Open Graph no `index.html`; `noindex` nas páginas internas.

## Pendências registradas

- **Gráficos de evolução (seção 8.3):** a decisão está pendente com a equipe. Em `desempenho.html` o gráfico ficou, alimentado só pelo histórico que já existe (`GET /simulados`), sem consulta nova na API. Em `turmas.html` o gráfico era um desenho fixo (sem dados); ele saiu do modal da turma, porque alimentá-lo exigiria a consulta de série temporal que o PROMPT pede para não criar.

Roteiro de testes manuais: [`TESTES.md`](./TESTES.md).
