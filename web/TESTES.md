# Avalia+ | Roteiro de testes manuais da Web

Antes de começar: API rodando (`npm start` na pasta `api`), banco com os scripts 01 a 04 e o navegador em <http://localhost:3000>.
Senha inicial de cada usuário = CPF (somente números). Cole um print em cada linha de **Evidência**.

| Perfil | E-mail | Senha |
|---|---|---|
| Administrador | admin@avaliamais.test | 15881399803 |
| Professor (disciplinas 1, 2 e 3) | ricardo.monteiro@avaliamais.test | 69879730917 |
| Aluno (turma A) | ana.souza@aluno.avaliamais.test | 88447294072 |
| Aluno (turma B) | henrique.barbosa@aluno.avaliamais.test | 97875347718 |
| Aluna desativada | gabriela.carvalho@aluno.avaliamais.test | 76634400210 |

## 1. Acesso (RF01, RN02, RN03, RN26)

| # | Passos | Resultado esperado | OK? | Evidência |
|---|---|---|---|---|
| 1.1 | Entrar como Ana | Abre o dashboard do aluno com o nome dela e o aviso recomendando trocar a senha inicial | | |
| 1.2 | Entrar com senha errada; depois com e-mail inexistente; depois como Gabriela (desativada) | As três tentativas mostram a mesma mensagem: "E-mail ou senha inválidos." (na tela, sem `alert`) | | |
| 1.3 | Sem login, digitar o endereço `/pages/dashboardadmin.html` | Volta para o login | | |
| 1.4 | Logado como Ana, digitar `/pages/adminusuarios.html` | Volta para o dashboard do aluno | | |
| 1.5 | "Esqueceu a sua senha?" → informar o e-mail do Henrique → "Enviar código" | Mensagem de envio. O código aparece no **console da API** (modo log) | | |
| 1.6 | Abrir `/pages/novasenha.html` sem código | Os campos de nova senha só aparecem depois de conferir um código válido | | |
| 1.7 | Usar o código, criar a nova senha e entrar com ela | Login funciona com a nova senha. Tentar usar o mesmo código de novo é recusado | | |
| 1.8 | Perfil → Alterar senha, com a senha atual errada | Erro no campo "Senha atual" | | |

## 2. Administrador (RF02, RF03, RN05 a RN12, RN27)

| # | Passos | Resultado esperado | OK? | Evidência |
|---|---|---|---|---|
| 2.1 | Dashboard | Quatro contadores (turmas, usuários, disciplinas e questões ativos) e nenhum dado de desempenho | | |
| 2.2 | Usuários → Criar Usuário → perfil Professor | O campo Turma some; não há campo de senha | | |
| 2.3 | Criar um aluno com CPF inválido e e-mail de outro usuário | Erros destacados nos campos CPF e E-mail | | |
| 2.4 | Criar o aluno corretamente | Volta para a lista com mensagem de sucesso; a lista não mostra CPF nem ID | | |
| 2.5 | Turmas → desativar "ADS 3º Semestre B" | A confirmação avisa os alunos ativos; depois de desativar, os alunos continuam ativos. Reativar em seguida | | |
| 2.6 | Turmas → filtro "Inativas" | Aparece "ADS 2º Semestre A (2025)" com o botão Reativar | | |
| 2.7 | Disciplinas → editar a disciplina 8 | O select de responsável começa com "Sem responsável" | | |
| 2.8 | Questões → Cadastrar Questão | A disciplina "Gestão de Projetos..." (sem responsável) aparece bloqueada | | |
| 2.9 | Questões → editar a questão nº 1 | Aviso RN11: alternativa correta bloqueada; enunciado e explicação editáveis | | |
| 2.10 | Cadastrar uma questão com imagem (PNG) | Prévia da imagem; a questão aparece na lista com "Cadastrada por" | | |
| 2.11 | Medalhas → Nova Faixa com pontuação já usada (250) | Erro: já existe medalha com esta pontuação mínima | | |

## 3. Professor (RF03, RF06, RN08, RN24, RN25)

| # | Passos | Resultado esperado | OK? | Evidência |
|---|---|---|---|---|
| 3.1 | Entrar como Ricardo → Questões | Só questões das disciplinas 1, 2 e 3; filtro de disciplina só com elas | | |
| 3.2 | Desativar uma questão própria | Desativa; não aparece o botão Reativar (só o Administrador reativa) | | |
| 3.3 | Turmas | Só as turmas A e B; indicador de questões com maior índice de erro | | |
| 3.4 | Clicar na turma A → Ver todos os estudantes → clicar na Ana | Painel consolidado (média por disciplina e quantidade de simulados), sem respostas questão a questão | | |
| 3.5 | Lista de alunos da turma A | Gabriela (desativada) não aparece | | |

## 4. Aluno: ciclo do simulado (RF04, RF05, RN13 a RN23)

| # | Passos | Resultado esperado | OK? | Evidência |
|---|---|---|---|---|
| 4.1 | Criar Simulado com uma disciplina | Botão "Próximo" desabilitado e aviso de mínimo de duas | | |
| 4.2 | Duas disciplinas e 10 questões → Próximo | Instruções com o aviso de tentativa única | | |
| 4.3 | Iniciar; responder algumas, avançar outras em branco; usar "Anterior" | Navegação numerada marca as respondidas; o cronômetro corre | | |
| 4.4 | Na última questão, Finalizar | Confirmação informa quantas ficaram em branco | | |
| 4.5 | Resultado | Taxa, acertos, em branco, pontos obtidos e desempenho por disciplina; sem card de tempo | | |
| 4.6 | Revisar questões | Questões em branco aparecem como "Questão deixada em branco"; explicação quando existir | | |
| 4.7 | Durante outro simulado, clicar no logo | Confirmação de que sair encerra a tentativa; ao sair, o simulado aparece finalizado no histórico | | |
| 4.8 | Perfil do Henrique (110 pontos) | Bronze acinzentada e "Faltam 140 pontos para a medalha Bronze" | | |

## 5. Acessibilidade (RNF06) e responsividade (RNF05)

| # | Passos | Resultado esperado | OK? | Evidência |
|---|---|---|---|---|
| 5.1 | Navegar só com Tab e Enter no login e num formulário | O foco fica visível; "Pular para o conteúdo" aparece no primeiro Tab | | |
| 5.2 | Ícone do VLibras (canto da tela) | Abre o tradutor de Libras (precisa de internet) | | |
| 5.3 | Abrir no celular ou reduzir a janela | Menu vira faixa no topo; sem rolagem horizontal | | |
