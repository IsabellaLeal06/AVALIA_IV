
# Diagramas do Banco de Dados — Avalia+

Esta pasta reúne a modelagem do banco de dados do Avalia+, implementado em SQL Server e hospedado no Azure.

## Arquivos

| Arquivo | Conteúdo | Como abrir |
|---|---|---|
| [`MER_Avalia_v2 (1).docx.pdf`](./MER_Avalia_v2%20(1).docx.pdf) | Modelo Entidade-Relacionamento (MER) v2: entidades, atributos, relacionamentos, cardinalidades e decisões de modelagem | Qualquer leitor de PDF |
| [`DB_DER_Avalia+_v2.1 (2).brM3`](./DB_DER_Avalia+_v2.1%20(2).brM3) | Diagrama Entidade-Relacionamento (DER) v2.1 | [brModelo](http://www.sis4.com/brModelo/) |
| [`dicionario-de-dados.md`](./dicionario-de-dados.md) | Dicionário de dados: tabelas, colunas, tipos, restrições, procedures e triggers | Direto no GitHub |

## Resumo do modelo

| Item | Quantidade |
|---|---|
| Tabelas | 13 |
| Chaves estrangeiras | 18 |
| Restrições de unicidade | 8 |
| Restrições de domínio | 4 |
| Procedures | 4 |
| Triggers | 5 |

## Principais decisões

- **Medalha apurada, não gravada:** a medalha do aluno é calculada a partir da pontuação e das faixas da tabela `Medalha` (RN22), para não ficar desatualizada quando as faixas mudarem.
- **Simulado sem coluna de disciplina:** as disciplinas são derivadas das questões sorteadas (RN13).
- **Exclusão lógica** em `Usuario`, `Questao`, `Turma` e `Disciplina` (RN12), com chaves estrangeiras sem cascata.
- **Respostas e resultados imutáveis** após a correção, garantidos por triggers (RN19).

## Scripts

Os scripts SQL que criam este modelo estão na pasta [`database/`](../../database/).

## Responsável

Isabella Santos Leal.

[Voltar aos diagramas](../README.md)
