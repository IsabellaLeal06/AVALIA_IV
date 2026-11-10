# Aplicação Desktop — Avalia+

Solução da aplicação desktop do Avalia+, em C# com Windows Forms. A Desktop **não acessa mais o banco de dados diretamente**: ela conversa com a API REST da pasta `../api` (INF14, RNF07), a mesma usada pelas versões Web e Mobile. As regras de negócio e a senha do banco ficam só na API.

## Estrutura

| Projeto | Responsabilidade |
|---|---|
| `Avalia.Desktop` | Telas (Windows Forms), `Program.cs` e configuração. |
| `Avalia.Desktop.Core` | Modelos, interfaces (`I...Repository`, `IAutenticacao`), controles, validações e sessão. |
| `Avalia.Desktop.Tests` | Testes unitários (xUnit) da camada Core: `ValidadorCpfTests`, `ValidadorSenhaTests` e `ValidadorEmailTests` (TST07). Não usam banco nem API. |
| `Avalia.Desktop.Api` | Implementações HTTP das interfaces do Core (`ClienteApi`, `*ApiRepository`, `AutenticacaoApi`, envio de imagens). Substitui o antigo `Avalia.Desktop.Data`. |

`Avalia.Desktop.sln` abre os projetos no Visual Studio.

## Como executar

1. Deixe a API rodando (veja `../api/README.md`): `npm start` na pasta `api`. Ela precisa de acesso ao banco (IP liberado no firewall do Azure).
2. Informe o endereço da API. A ordem de busca é:
   - variável de ambiente `AVALIA_API_URL`;
   - `Api:Url` em `Avalia.Desktop/appsettings.Local.json` (arquivo ignorado pelo Git);
   - padrão: `http://localhost:3000/`.

   Modelo (`appsettings.example.json`):
   ```json
   { "Api": { "Url": "http://localhost:3000/" } }
   ```
   Copie para `appsettings.Local.json` apenas se a API estiver em outro endereço.
3. Abra `Avalia.Desktop.sln` no Visual Studio, defina `Avalia.Desktop` como projeto de inicialização e execute.
4. Entre com um usuário Administrador (ex.: `admin@avaliamais.test`, senha inicial = CPF; veja `../api/README.md`).

> A Desktop não tem mais string de conexão nem senha de banco. Não existe mais `ConnectionStrings` na configuração.

## O que mudou na migração

- Login, troca de senha, usuários, turmas, disciplinas, questões e medalhas passam pela API. O token JWT fica em `SessaoUsuario`.
- Erros de validação da API (400/409) voltam para os mesmos campos das telas; falhas de rede ou do servidor viram `FalhaAcessoDadosException`.
- Imagens (questões e medalhas) são enviadas por `POST /upload/imagem`. Formatos aceitos: PNG, JPG, GIF e WEBP, até 2 MB (BMP deixou de ser aceito).
- A API também aplica as regras que antes só existiam na Desktop, como RN07 (disciplina sem responsável não recebe novas questões) e CPF duplicado.
- A tela do Administrador mostra o endereço da API em uso.

## Testes unitários

Teste unitário pega uma parte pequena do código (uma classe) e confere, sozinha, se ela responde certo para entradas boas e ruins. Aqui são três classes de teste, 46 casos no total:

| Classe de teste | Classe testada | O que confere |
|---|---|---|
| `ValidadorCpfTests` | `ValidadorCpf` | dígitos verificadores, CPF com máscara, tamanho errado, todos os dígitos iguais, letras, formatação |
| `ValidadorSenhaTests` | `ValidadorSenha` | mínimo de 8 caracteres, letra e número obrigatórios, espaço nas pontas, limite de 72 bytes do BCrypt |
| `ValidadorEmailTests` | `ValidadorEmail` | formato, domínio com ponto, normalização, tamanho máximo |

Como rodar:

- **Visual Studio:** menu **Testes → Gerenciador de Testes → Executar Todos os Testes**.
- **Terminal:** na pasta `desktop`, `dotnet test Avalia.Desktop.Tests`.
