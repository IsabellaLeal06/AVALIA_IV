using System.Diagnostics;
using System.Globalization;
using Avalia.Desktop.Componentes;
using Avalia.Desktop.Core.Controles;
using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Servicos;
using Avalia.Desktop.Data.Arquivos;
using Avalia.Desktop.Data.Conexao;
using Avalia.Desktop.Data.Repositorios;
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

        var stringConexao = ConfiguracaoConexao.ObterStringConexao(configuracao.GetConnectionString("Avalia"));

        using var provedor = ConfigurarServicos(configuracao, stringConexao ?? string.Empty);
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
                if (stringConexao is null)
                    login.ExibirErroConfiguracao(
                        "A conexão com o banco não está configurada. Crie o arquivo appsettings.Local.json " +
                        $"(a partir do appsettings.example.json) ou a variável {ConfiguracaoConexao.VariavelAmbiente}. " +
                        "Veja o passo a passo no documento de instalação.");

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
    /// Ponto único de troca da camada de dados: hoje os repositórios acessam o
    /// Azure SQL diretamente; com a API em JavaScript, basta registrar aqui as
    /// implementações do projeto Avalia.Desktop.Api (HttpClient).
    /// </summary>
    private static ServiceProvider ConfigurarServicos(IConfiguration configuracao, string stringConexao)
    {
        var servicos = new ServiceCollection();

        // Dados (acesso direto ao banco enquanto a API não existe)
        servicos.AddSingleton<IFabricaConexao>(new FabricaConexaoSql(stringConexao));
        servicos.AddSingleton<IUsuarioRepository, UsuarioRepository>();
        servicos.AddSingleton<ITurmaRepository, TurmaRepository>();
        servicos.AddSingleton<IDisciplinaRepository, DisciplinaRepository>();
        servicos.AddSingleton<IQuestaoRepository, QuestaoRepository>();
        servicos.AddSingleton<IMedalhaRepository, MedalhaRepository>();
        servicos.AddSingleton<IArmazenamentoImagem>(CriarArmazenamentoImagem(configuracao,
            "Armazenamento:PastaImagensQuestoes", "Armazenamento:PrefixoUrlImagensQuestoes", "questoes",
            (pasta, prefixo) => new ArmazenamentoImagemLocal(pasta, prefixo)));
        servicos.AddSingleton<IArmazenamentoImagemMedalha>(CriarArmazenamentoImagem(configuracao,
            "Armazenamento:PastaImagensMedalhas", "Armazenamento:PrefixoUrlImagensMedalhas", "medalhas",
            (pasta, prefixo) => new ArmazenamentoImagemMedalhaLocal(pasta, prefixo)));

        // Classes de controle (regras de negócio), com os nomes do diagrama de classes
        servicos.AddSingleton<SessaoUsuario>();
        servicos.AddSingleton<IHashSenha, BCryptHashSenha>();
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
        servicos.AddSingleton(new InformacoesAmbiente(
            stringConexao.Length == 0 ? "não configurado" : ConfiguracaoConexao.DescreverDestino(stringConexao), versao));
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

    private static T CriarArmazenamentoImagem<T>(IConfiguration configuracao, string chavePasta, string chavePrefixo,
        string padrao, Func<string, string, T> criar)
    {
        var pasta = configuracao[chavePasta];
        if (string.IsNullOrWhiteSpace(pasta))
            pasta = Path.Combine("uploads", padrao);
        if (!Path.IsPathRooted(pasta))
            pasta = Path.Combine(AppContext.BaseDirectory, pasta);

        var prefixo = configuracao[chavePrefixo];
        return criar(pasta, string.IsNullOrWhiteSpace(prefixo) ? $"uploads/{padrao}/" : prefixo);
    }
}
