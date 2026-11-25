
# Diagramas UML — Avalia+

Esta pasta reúne os diagramas UML do Avalia+, elaborados na ferramenta **Astah**.

## Arquivo

| Arquivo | Conteúdo |
|---|---|
| [`diagramasUML3.1.asta`](./diagramasUML3.1.asta) | Projeto Astah com os diagramas de casos de uso, de classes e de sequência |

| [`diagrama-de-implantacao.png`](./diagrama-de-implantacao.png) | Diagrama de implantação (MD12), também em [SVG](./diagrama-de-implantacao.svg) |
| [`diagrama-de-implementacao.png`](./diagrama-de-implementacao.png) | Diagrama de implementação, de componentes (MD13), também em [SVG](./diagrama-de-implementacao.svg) |

Para abrir o arquivo `.asta`, use o [Astah](https://astah.net/) (há licença gratuita para estudantes). Os diagramas de implantação e de implementação foram feitos no Graphviz; os arquivos `.dot` são o código-fonte editável de cada um.

## Diagramas

| Diagrama | O que representa |
|---|---|
| Casos de uso | Funcionalidades disponíveis para Aluno, Professor e Administrador, conforme os requisitos v2.2 |
| Classes | Estrutura das classes do sistema, revalidada contra o código entregue (MD11) |
| Sequência | Troca de mensagens entre telas (`Form_...`), controles (`Controle...`) e entidades em cada funcionalidade |
| Implantação | Onde cada parte roda: Desktop no Windows, Web no navegador, Mobile no Android, API em Node.js e banco no Azure SQL Database, com os protocolos entre eles (MD12, Etapa 8) |
| Implementação | Componentes de cada aplicação e suas dependências, com a interface REST que a API fornece e que Web, Desktop e Mobile usam (MD13) |

## Rastreabilidade com o código

Os nomes das telas e dos controles nos diagramas de sequência são os mesmos usados no código:

- Desktop: `desktop/Avalia.Desktop/Telas/` (telas) e `desktop/Avalia.Desktop.Core/Controles/` (controles);
- Mobile: `mobile/app/src/main/java/br/unip/avaliamais/telas/` e `.../controles/`.

## Responsável

Letícia Aparecida Santos Mota.

[Voltar aos diagramas](../README.md)
