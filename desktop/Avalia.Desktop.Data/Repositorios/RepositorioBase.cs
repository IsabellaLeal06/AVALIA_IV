using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Data.Conexao;
using Microsoft.Data.SqlClient;

namespace Avalia.Desktop.Data.Repositorios;

/// <summary>
/// Base dos repositórios: abre a conexão, controla transações e traduz
/// os erros do SQL Server. Todas as consultas usam parâmetros (RNF01).
/// </summary>
public abstract class RepositorioBase(IFabricaConexao fabrica)
{
    protected async Task<T> ExecutarAsync<T>(Func<SqlConnection, Task<T>> acao)
    {
        try
        {
            await using var conexao = await fabrica.AbrirAsync();
            return await acao(conexao);
        }
        catch (SqlException ex)
        {
            throw TradutorErrosSql.Traduzir(ex);
        }
    }

    protected Task ExecutarAsync(Func<SqlConnection, Task> acao) =>
        ExecutarAsync(async conexao =>
        {
            await acao(conexao);
            return true;
        });

    /// <summary>Executa as gravações em uma única transação (tudo ou nada).</summary>
    protected Task<T> ExecutarEmTransacaoAsync<T>(Func<SqlConnection, SqlTransaction, Task<T>> acao) =>
        ExecutarAsync(async conexao =>
        {
            await using var transacao = (SqlTransaction)await conexao.BeginTransactionAsync();
            try
            {
                var resultado = await acao(conexao, transacao);
                await transacao.CommitAsync();
                return resultado;
            }
            catch
            {
                await transacao.RollbackAsync();
                throw;
            }
        });

    /// <summary>Converte o filtro de situação no parâmetro @Ativo (null = todos).</summary>
    protected static bool? ParametroAtivo(FiltroSituacao situacao) => situacao switch
    {
        FiltroSituacao.Ativos => true,
        FiltroSituacao.Inativos => false,
        _ => null
    };

    /// <summary>Monta o parâmetro do LIKE (%texto%), escapando os curingas digitados.</summary>
    protected static string? ParametroBusca(string? busca)
    {
        if (string.IsNullOrWhiteSpace(busca))
            return null;

        var escapado = busca.Trim()
            .Replace(@"\", @"\\")
            .Replace("%", @"\%")
            .Replace("_", @"\_")
            .Replace("[", @"\[");
        return $"%{escapado}%";
    }
}
