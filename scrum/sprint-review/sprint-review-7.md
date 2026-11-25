# Sprint Review – Sprint 7

**Projeto:** Avalia+
**Período da Sprint:** 10/11 a 21/11/2026

---

# Objetivo da Sprint

Concluir o aplicativo mobile, finalizar a integração entre as aplicações e a API e realizar os ajustes finais até a apresentação.

---

# Atividades realizadas na Sprint

- Versionamento do código de todas as aplicações no repositório: Web e API (08/11), Desktop (08/11) e Mobile (10/11).
- Criação das 4 procedures de consulta e conferência (INF05).
- Criação dos 5 triggers de regra de negócio (INF06): acúmulo de pontuação, pontuação que não diminui, imutabilidade de respostas e resultados e gabarito imutável após uso.
- Ajuste da API para somar a pontuação apenas quando o trigger não está ativo, evitando contagem dobrada.
- Testes automatizados das procedures e dos triggers (`api/test/banco.test.js`).
- Migração da aplicação Desktop para a API (INF14): o Desktop deixou de acessar o banco diretamente.
- Entrega do aplicativo mobile do Aluno em Kotlin com Jetpack Compose (PB10).
- Documentação da ordem de execução dos scripts e do reset das procedures.

---

# Entregas da Sprint

- Aplicativo mobile com login, recuperação de senha, início, realização e revisão de simulados, histórico, desempenho, pontuação e perfil.
- Web, Desktop e Mobile consumindo a mesma API REST e o mesmo banco no Azure.
- Scripts `05_procedures.sql` e `06_triggers.sql` versionados.
- Suítes de testes: API (`api/test/`), Desktop (`desktop/Avalia.Desktop.Tests/`) e roteiro manual da Web (`web/TESTES.md`).
- Repositório completo, com as pastas `api`, `database`, `desktop`, `mobile`, `web`, `diagramas`, `docs` e `scrum`.

---

# Itens pendentes

- INF07 — Índices de otimização: não foram criados. A lacuna está registrada no dicionário de dados.

---

# Avaliação da Sprint

A Sprint 7 fechou o produto: as três aplicações passaram a usar a mesma API, o que concentra as regras de negócio em um só lugar (RNF07). As procedures e os triggers, previstos para a Sprint 5, foram entregues nesta sprint; a convivência do trigger de pontuação com a API exigiu um ajuste para não somar os pontos duas vezes.

Como a sprint é posterior ao corte documental, nenhum item alterou o documento entregue.

---

# Próximos passos

- Preparar a apresentação do projeto.
- Ajustes de código, se necessários, até a data da apresentação.
