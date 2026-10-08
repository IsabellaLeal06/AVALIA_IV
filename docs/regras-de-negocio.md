# Regras de Negócio – Avalia+

As regras abaixo correspondem à versão 2.2 do documento de Requisitos e Regras de Negócio do PIM IV.

## RN01 – Criação de contas

Somente o Administrador cria contas de usuário, individualmente. Não há autocadastro nem importação de listas.

## RN02 – Senha inicial e primeiro acesso

A senha inicial de todo usuário corresponde ao seu CPF. O sistema recomenda a troca no primeiro acesso.

## RN03 – Recuperação de senha

A recuperação ocorre por e-mail, utilizando um código único com prazo de validade.

## RN04 – Alteração de dados de perfil

O usuário pode alterar a própria senha. Os demais dados cadastrais são mantidos pelo Administrador.

## RN05 – Vínculo de turma

Somente contas de Aluno possuem turma. Professor e Administrador não pertencem a uma turma.

## RN06 – Identificação da turma

A turma é identificada por nome e ano letivo. Não podem existir duas turmas com o mesmo nome no mesmo ano.

## RN07 – Responsabilidade pela disciplina

Cada disciplina possui no máximo um professor responsável. Uma disciplina pode permanecer temporariamente sem responsável.

## RN08 – Permissão sobre o banco de questões

O Administrador pode atuar sobre questões de qualquer disciplina. O Professor pode atuar somente nas disciplinas sob sua responsabilidade.

## RN09 – Composição da questão

Toda questão possui exatamente cinco alternativas, de A a E, com apenas uma correta. Enunciado e disciplina são obrigatórios; imagem e explicação são opcionais.

## RN10 – Permanência das questões no acervo

A questão pertence à disciplina, e não ao professor que a cadastrou. A troca do professor responsável não remove as questões existentes.

## RN11 – Edição de questão já respondida

Não é permitido alterar a alternativa correta de uma questão que já tenha sido utilizada em um simulado. Nesse caso, deve-se desativar a questão e cadastrar uma nova.

## RN12 – Desativação de registros

Usuários, turmas, disciplinas e questões não são excluídos fisicamente. São desativados mediante confirmação e podem ser reativados pelo Administrador.

## RN13 – Seleção de disciplinas

Todo simulado deve abranger duas ou mais disciplinas selecionadas pelo Aluno.

## RN14 – Quantidade de questões

O simulado pode possuir 10, 15 ou 20 questões.

## RN15 – Sorteio das questões

As questões são sorteadas aleatoriamente entre as questões ativas das disciplinas selecionadas, sem repetição no mesmo simulado.

## RN16 – Tentativa única e abandono

Cada simulado possui uma única tentativa. Sair da tela encerra a tentativa e não é possível retomá-la.

## RN17 – Cronômetro

O simulado apresenta o tempo decorrido, sem limite de duração. O tempo não é armazenado.

## RN18 – Registro das respostas

As respostas utilizam as letras A, B, C, D ou E. Questões podem ser deixadas em branco.

## RN19 – Correção automática

A correção ocorre automaticamente após a finalização do simulado e apresenta o resultado imediatamente.

## RN20 – Registro de histórico

Todos os simulados realizados são armazenados no histórico do Aluno.

## RN21 – Atribuição de pontos

O sistema atribui pontos ao Aluno conforme seu desempenho nos simulados.

## RN22 – Faixas de medalhas

A pontuação acumulada do Aluno determina a faixa de medalha correspondente.

## RN23 – Atualização da pontuação

A pontuação acumulada é atualizada após a correção do simulado.

## RN24 – Visibilidade por perfil

Cada perfil acessa somente os dados e funcionalidades permitidos para sua função no sistema.

## RN25 – Alunos desativados nos indicadores

Alunos com conta desativada permanecem com seu histórico armazenado, mas ficam fora dos cálculos de médias correntes das turmas.

## RN26 – Distribuição das plataformas

A disponibilidade das telas segue o escopo definido para Web, Desktop e Mobile. Essa distribuição não representa bloqueio de autenticação por plataforma.

## RN27 – Restrições de unicidade

O banco deve impedir duplicidades nos dados definidos pelo projeto, incluindo e-mail e CPF de usuário, nome de disciplina, combinação nome/ano de turma, nome e pontuação mínima de medalha e letra de alternativa dentro da mesma questão.

## Observação

As regras de negócio devem ser aplicadas de forma consistente nas aplicações e, quando relacionado à integridade dos dados, também no banco de dados e na API.
