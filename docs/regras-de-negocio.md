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

Somente contas de Aluno possuem turma, e cada aluno pertence a no máximo uma turma por vez. Professor e Administrador não pertencem a uma turma.

## RN06 – Identificação da turma

A turma é identificada por nome e ano letivo. O período, quando aplicável, integra o próprio nome da turma. Não podem existir duas turmas com o mesmo nome no mesmo ano letivo.

## RN07 – Responsabilidade pela disciplina

Cada disciplina possui no máximo um professor responsável, e um professor pode responder por várias disciplinas. Uma disciplina pode permanecer temporariamente sem responsável; nesse caso continua disponível aos alunos, mas o cadastro de novas questões fica suspenso até a designação de um responsável.

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

Cada questão respondida corretamente vale 10 pontos. Erros e questões em branco não pontuam e não descontam. O total é cumulativo e nunca sofre redução. Professor e Administrador não acumulam pontos.

## RN22 – Faixas de medalha

A medalha é determinada pela pontuação acumulada: Bronze a partir de 250 pontos, Prata a partir de 1.000 e Ouro a partir de 2.500. A medalha não é armazenada no cadastro do aluno: é apurada no momento da consulta, comparando a pontuação com as faixas mantidas em tabela própria no banco de dados.

## RN23 – Exibição da medalha

O perfil do aluno exibe a medalha corrente e informa quantos pontos faltam para a próxima faixa. Quem ainda não alcançou a primeira faixa visualiza a medalha Bronze em versão acinzentada; quem alcançou o Ouro visualiza mensagem de nível máximo. Não há barra de progresso.

## RN24 – Visibilidade de dados por perfil

O Aluno acessa apenas os próprios dados. O Professor acessa apenas os dados das suas turmas, em formato consolidado (média de acertos por disciplina e quantidade de simulados), sem as respostas questão a questão. O Administrador não acessa dados de desempenho. A verificação ocorre no servidor, a cada consulta.

## RN25 – Alunos desativados nos indicadores

Alunos com conta desativada permanecem com seu histórico armazenado, mas ficam fora dos cálculos de médias correntes das turmas.

## RN26 – Escopo de telas por plataforma

A aplicação Web contempla as telas dos três perfis, a Desktop as telas de cadastro e edição do Administrador e o Mobile as telas do Aluno. Trata-se de delimitação de escopo de desenvolvimento, e não de restrição de segurança: não há bloqueio de autenticação por plataforma.

## RN27 – Unicidade dos cadastros

O banco deve impedir duplicidade de: e-mail do usuário, CPF do usuário, nome da disciplina, nome da turma no mesmo ano letivo, nome da medalha, pontuação mínima da medalha e letra da alternativa dentro da mesma questão.

## Observação

As regras de negócio devem ser aplicadas de forma consistente nas aplicações e, quando relacionado à integridade dos dados, também no banco de dados e na API.
