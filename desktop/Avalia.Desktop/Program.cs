using System.Diagnostics;
using System.Globalization;
using Avalia.Desktop.Componentes;
using Avalia.Desktop.Core.Controles;
using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Api;
using Avalia.Desktop.Api.Arquivos;
using Avalia.Desktop.Api.Repositorios;
using Avalia.Desktop.Core.Servicos;
using Avalia.Desktop.Telas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Avalia.Desktop;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = new CultureInfo("pt-BR");

        var configuracao = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Local.json", optional: true)
            .Build();

        // O Desktop só conhece o endereço da API: não há banco, senha nem string de conexão aqui
        var urlApi = ConfiguracaoApi.ObterUrl(configuracao["Api:Url"]);

        using var provedor = ConfigurarServicos(urlApi);
        var fabrica = provedor.GetRequiredService<IFabricaFormularios>();
        var navegador = provedor.GetRequiredService<Navegador>();

        // Erros não tratados também aparecem na barra de mensagens, sem janelas pop-up
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) =>
        {
            Debug.WriteLine(e.Exception);
            navegador.ExibirMensagem(TipoMensagem.Erro, "Ocorreu um erro inesperado: " + e.Exception.Message);
        };

        // Form_Autenticar_Usuario → Form_Administrador. "Sair da conta" volta para o login.
        while (true)
        {
            using (var login = fabrica.Criar<Form_Autenticar_Usuario>())
            {
                Application.Run(login);
                if (!login.LoginRealizado)
                    return;
            }

            using var principal = fabrica.Criar<Form_Administrador>();
            Application.Run(principal);
            if (!principal.SaiuDaConta)
                return;
        }
    }

    /// <summary>
    /// Ponto único da camada de dados: todos os repositórios falam com a API (projeto Avalia.Desktop.Api).
    /// A API é quem acessa o banco, valida as regras e confere o token a cada requisição (INF14).
    /// </summary>
    private static ServiceProvider ConfigurarServicos(string urlApi)
    {
        var servicos = new ServiceCollection();

        // Dados, pela API
        servicos.AddSingleton<SessaoUsuario>();
        servicos.AddSingleton(p => new ClienteApi(urlApi, p.GetRequiredService<SessaoUsuario>()));
        servicos.AddSingleton<IAutenticacao, AutenticacaoApi>();
        servicos.AddSingleton<IUsuarioRepository, UsuarioApiRepository>();
        servicos.AddSingleton<ITurmaRepository, TurmaApiRepository>();
        servicos.AddSingleton<IDisciplinaRepository, DisciplinaApiRepository>();
        servicos.AddSingleton<IQuestaoRepository, QuestaoApiRepository>();
        servicos.AddSingleton<IMedalhaRepository, MedalhaApiRepository>();
        servicos.AddSingleton<IArmazenamentoImagem>(p => new ArmazenamentoImagemApi(p.GetRequiredService<ClienteApi>(), "questoes"));
        servicos.AddSingleton<IArmazenamentoImagemMedalha>(p => new ArmazenamentoImagemMedalhaApi(p.GetRequiredService<ClienteApi>()));

        // Classes de controle (regras de negócio), com os nomes do diagrama de classes
        servicos.AddSingleton<ControleAutenticarUsuario>();
        servicos.AddSingleton<Visualizar_perfil>();
        servicos.AddSingleton<ControlePainel>();
        servicos.AddSingleton<ControleUsuario>();
        servicos.AddSingleton<ControleTurma>();
        servicos.AddSingleton<ControleDisciplina>();
        servicos.AddSingleton<ControleQuestao>();
        servicos.AddSingleton<ControleMedalha>();
        servicos.AddTransient<ConfirmarDesativacao>();

        // Telas
        var versao = typeof(Program).Assembly.GetName().Version?.ToString(3) ?? "1.0.0";
        servicos.AddSingleton(new InformacoesAmbiente(ConfiguracaoApi.DescreverDestino(urlApi), versao));
        servicos.AddSingleton<IFabricaFormularios, FabricaFormularios>();
        servicos.AddSingleton<Navegador>();
        servicos.AddSingleton<INavegacao>(p => p.GetRequiredService<Navegador>());
        servicos.AddTransient<Form_Autenticar_Usuario>();
        servicos.AddTransient<Form_Administrador>();
        servicos.AddTransient<Form_Usuario>();
        servicos.AddTransient<Form_Turma>();
        servicos.AddTransient<Form_Disciplina>();
        servicos.AddTransient<Form_Questao>();
        servicos.AddTransient<Form_Medalha>();
        servicos.AddTransient<Form_Visualizar_perfil>();
        servicos.AddTransient<Form_Conf_Desativação>();

        return servicos.BuildServiceProvider();
    }
}
