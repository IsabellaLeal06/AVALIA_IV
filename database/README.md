# Banco de Dados — Avalia+

Esta pasta contém os arquivos relacionados à estrutura, configuração e preparação do banco de dados do projeto Avalia+.

## Arquivos

| Arquivo | Descrição |
|---|---|
| `00_reset.sql` | Script destinado à reinicialização do banco de dados. Verifique seu conteúdo antes da execução, pois ele pode excluir dados existentes. |
| `01_ddl.sql` | Script de definição das estruturas do banco de dados. |
| `02_restricoes.sql` | Script relacionado às restrições de integridade do banco de dados. |
| `03_carga_inicial.sql` | Script de carga inicial de dados. |
| `04_dados_teste.sql` | Script com dados destinados a testes. |
| `banco_questoes.json` | Arquivo JSON relacionado ao banco de questões do sistema. |

## Tecnologia

- **SGBD:** Microsoft SQL Server.

## Orientações

1. Consulte os scripts antes de executá-los.
2. Verifique a ordem de execução e as dependências entre os arquivos.
3. Utilize um ambiente de desenvolvimento ou testes para validar os scripts.
4. Não execute o script de reinicialização em um banco com dados importantes sem realizar um backup.

A ordem de execução deve ser confirmada com base no conteúdo dos scripts e nas dependências definidas no projeto.
