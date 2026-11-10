# Avalia+ | Aplicativo Mobile (Android)

Aplicativo do **Aluno** (RN26), em **Kotlin + Jetpack Compose**, ligado à **mesma API** da versão web (pasta `../api`). O app não acessa o banco: ele só envia e exibe. Sorteio, correção, pontuação e medalha ficam na API (RNF07).

Itens do backlog: **PB10 / PB15** (Sprint 7).

## Telas

| Tela (nome do diagrama UML) | O que faz | Controle (UML) |
|---|---|---|
| `Form_Autenticar_Usuario` | Login. Mensagem genérica na falha; botão "Servidor da API" | `ControleAutenticarUsuario` (`autenticarUsuario`, `sairSistema`) |
| `Form_Recuperar_Senha` | E-mail → código → nova senha (RN03) | `ControleAutenticarUsuario` |
| `Form_Inicio` | Resumo: taxa média, simulados, pontos e desempenho por disciplina; aviso da senha inicial (RN02) | `ControleDesempenho`, `ControlePontuacao` |
| `Form_Realizar_Simulado` | Duas ou mais disciplinas (RN13), 10/15/20 questões (RN14), instruções e `iniciarSimulado` | `ControleSimulado` |
| `Form_Resposta` | `exibirQuestao`, `selecionarResposta`, `finalizarSimulado` → `calcularResultado` → resultado | `ControleSimulado` |
| `Form_Revisar_Simulados` | `revisarSimulado`: marcada, correta e explicação; em branco identificada | `ControleSimulado` |
| `Form_Consultar_Historico` | `consultarHistorico`: simulados finalizados, paginados (RN20) | `ControleSimulado` |
| `Form_Desempenho` | `visualizarDesempenho`: aproveitamento por disciplina (RF06) | `ControleDesempenho` |
| `Form_Visualizar_Pontuacao` | `visualizarPontuacao`: pontos, medalha e quanto falta (RN23) | `ControlePontuacao` |
| `Form_Visualizar_perfil` | `visualizarPerfil`, `alterarSenha` (RN04), `sairConta` | `Visualizar_perfil` |

Professor e Administrador conseguem entrar (não há bloqueio por plataforma, RN26), mas veem um aviso de que as telas deles estão na Web.

## Como rodar (passo a passo)

### 1. Ligue a API
Na pasta `api`, rode `npm start` (veja `api/README.md`). Confira no navegador: <http://localhost:3000>.

### 2. Abra o projeto no Android Studio
1. Instale o **Android Studio** (versão Koala ou mais nova): <https://developer.android.com/studio>.
2. **File → Open** e escolha a pasta **`mobile`** (não a raiz do repositório).
3. Espere o **Gradle Sync** terminar. Na primeira vez ele baixa as bibliotecas e pode demorar alguns minutos. Se o Android Studio sugerir atualizar o Android Gradle Plugin, pode aceitar.

### 3a. Rodar no emulador (padrão)
1. **Device Manager → Create Virtual Device** (ex.: Pixel 7, Android 14).
2. Clique em **Run ▶**.
3. O app já vem apontando para `http://10.0.2.2:3000/`. O `10.0.2.2` é o "localhost" do computador visto de dentro do emulador.

### 3b. Rodar no celular de verdade
1. Celular e computador na **mesma rede Wi-Fi**.
2. Descubra o IP do computador: no Windows, rode `ipconfig` e procure o **Endereço IPv4**, por exemplo `192.168.0.15`.
3. Libere o Node.js no **Firewall do Windows**, se ele perguntar, em **Redes privadas**.
4. No celular, ative as **Opções do desenvolvedor → Depuração USB**, ligue no cabo e clique em **Run ▶**.
5. Na tela de login, toque em **Servidor da API** e informe `http://192.168.0.15:3000/`, trocando pelo seu IP. O endereço fica gravado no celular.

### Usuários de teste
Os mesmos do `04_dados_teste.sql` (senha inicial = CPF), por exemplo:
- `ana.souza@aluno.avaliamais.test` / `88447294072`
- `henrique.barbosa@aluno.avaliamais.test` / `97875347718`

## Regras de negócio no app

- **RN16, tentativa única:** durante a prova, o botão Voltar do celular e o botão "Sair" pedem confirmação, e sair **finaliza** o simulado com o que foi marcado. Se a rede falhar na finalização, as marcações continuam na tela e o aluno toca de novo: a API devolve o resultado já apurado caso a primeira tentativa tenha chegado. Minimizar o app (por exemplo, para atender uma ligação) não encerra a prova.
- **RN17:** o cronômetro só aparece na tela; o tempo não é enviado.
- **RN18:** dá para avançar com a questão em branco e voltar com "Anterior"; "Deixar em branco" limpa a marcação. Na última questão aparece "Finalizar", com a contagem das questões em branco.
- **RN22 e RN23:** a medalha vem apurada da API (faixas da tabela Medalha). Sem medalha, a Bronze aparece acinzentada com quantos pontos faltam. Sem barra de progresso.
- **Sessão:** o token da API vale **7 dias**, então o aluno não precisa entrar a cada abertura. Token vencido ou conta desativada levam de volta ao login.

## Acessibilidade (RNF06)

- Imagens com descrição (`contentDescription`).
- Títulos marcados como cabeçalho.
- Alternativas como grupo de botões de opção, para o **TalkBack** ler "selecionado / não selecionado".
- Botões numerados das questões com descrição ("Questão 3, marcada B").
- Mensagens de erro e de sucesso anunciadas pelo leitor de tela.
- Alvos de toque de pelo menos 44 dp.
- O tamanho da fonte acompanha a configuração do celular.

## Estrutura

```text
mobile/
├── settings.gradle.kts, build.gradle.kts, gradle/   # projeto Gradle (wrapper 8.9)
└── app/
    ├── build.gradle.kts                             # dependências; endereço padrão da API (API_URL_PADRAO)
    └── src/main/
        ├── AndroidManifest.xml
        ├── res/                                     # logo e ícone (vetores do logo da web), tema, rede
        └── java/br/unip/avaliamais/
            ├── MainActivity.kt, AvaliaApp.kt, Navegacao.kt
            ├── dados/      # ApiAvalia (rotas), Modelos (JSON), ClienteApi (token), Repositorio (erros), Sessao (DataStore)
            ├── controles/  # ControleAutenticarUsuario, ControleSimulado, ControleDesempenho, ControlePontuacao, Visualizar_perfil
            ├── telas/      # Form_* (uma por tela do diagrama)
            └── ui/         # cores da web, botões em degradê, cards e barras
```

## Observações

- **HTTP sem "S":** liberado só para os testes locais (`res/xml/network_security_config.xml`). Com a API publicada em HTTPS, troque `cleartextTrafficPermitted` para `false`.
- **Imagens:** a API devolve o endereço completo das imagens com o endereço dela (`localhost`). O app troca o começo pelo endereço configurado, para funcionar no emulador e no celular.
- **VLibras:** é um recurso da versão web. No app, a acessibilidade usa o leitor de tela do próprio Android (TalkBack), como prevê o RNF06.

### Como o código foi conferido
- A camada de dados (`dados/`) foi compilada e executada contra a API, com o banco dos scripts de teste: login, perfil, sorteio, finalização, reenvio, revisão, histórico, desempenho, erros de campo, token inválido e servidor fora do ar.
- As telas, componentes e controles foram compilados (0 erros) e desenhados com o Compose, com os dados reais da API.
- O build completo (`assembleDebug`) **não** foi feito aqui, porque o ambiente não tinha acesso ao Android SDK. Faça o primeiro build no Android Studio; se aparecer algum erro, ele indica o arquivo e a linha.
