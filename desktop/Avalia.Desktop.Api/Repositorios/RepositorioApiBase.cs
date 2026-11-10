using Avalia.Desktop.Core.Excecoes;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Core.Servicos;

namespace Avalia.Desktop.Api.Repositorios;

/// <summary>
/// Base dos repositórios que falam com a API. Três jeitos de chamar, conforme o que o erro significa:
///  - <see cref="Ler"/>: consulta; "não encontrado" (404) vira null;
///  - <see cref="Escrever"/>: gravação com campos; 400/409 vira <see cref="FalhaValidacaoApiException"/>,
///    já com os nomes dos campos das telas, para a tela destacar o campo certo;
///  - <see cref="Acao"/>: ação sem campos (desativar, reativar, excluir); qualquer recusa vira mensagem na barra.
/// </summary>
public abstract class RepositorioApiBase(ClienteApi api)
{
    internal ClienteApi Api { get; } = api;

    protected static string Situacao(FiltroSituacao filtro) => filtro switch
    {
        FiltroSituacao.Ativos => "ativo",
        FiltroSituacao.Inativos => "inativo",
        _ => "todos"
    };

    internal static async Task<T?> Ler<T>(Func<Task<T?>> chamada)
    {
        try
        {
            return await chamada();
        }
        catch (ErroApiException ex) when (ex.Status == 404)
        {
            return default;
        }
        catch (ErroApiException ex)
        {
            throw new FalhaAcessoDadosException(ex.Message, ex);
        }
    }

    internal static async Task<T?> Escrever<T>(Func<Task<T?>> chamada, IReadOnlyDictionary<string, string> nomesDosCampos)
    {
        try
        {
            return await chamada();
        }
        catch (ErroApiException ex) when (ex.EhValidacao)
        {
            var erros = ex.Campos.Count > 0
                ? ex.Campos.Select(c => new ErroValidacao(nomesDosCampos.GetValueOrDefault(c.Key), c.Value)).ToList()
                : [new ErroValidacao(null, ex.Message)];
            throw new FalhaValidacaoApiException(ex.Message, erros);
        }
        catch (ErroApiException ex)
        {
            throw new FalhaAcessoDadosException(ex.Message, ex);
        }
    }

    internal static async Task Acao(Func<Task> chamada)
    {
        try
        {
            await chamada();
        }
        catch (ErroApiException ex)
        {
            throw new FalhaAcessoDadosException(ex.Message, ex);
        }
    }

    internal async Task<PainelDto> Painel() =>
        await Ler(() => Api.Get<PainelDto>("painel")) ?? throw new FalhaAcessoDadosException("A API não devolveu o painel.");
}
