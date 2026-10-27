using Avalia.Desktop.Core.Excecoes;
using Microsoft.Data.SqlClient;

namespace Avalia.Desktop.Data.Conexao;

public interface IFabricaConexao
{
    /// <summary>Abre uma conexão com o banco (o chamador é responsável por descartá-la).</summary>
    Task<SqlConnection> AbrirAsync();
}

/// <summary>
/// Cria conexões com o Azure SQL. Usa novas tentativas automáticas para
/// erros transitórios, comuns quando o banco serverless sai da pausa.
/// </summary>
public sealed class FabricaConexaoSql : IFabricaConexao
{
    private readonly string _stringConexao;
    private readonly SqlRetryLogicBaseProvider _novasTentativas;

    public FabricaConexaoSql(string stringConexao)
    {
        _stringConexao = stringConexao;
        _novasTentativas = SqlConfigurableRetryFactory.CreateExponentialRetryProvider(new SqlRetryLogicOption
        {
            NumberOfTries = 5,
            DeltaTime = TimeSpan.FromSeconds(2),
            MaxTimeInterval = TimeSpan.FromSeconds(20)
        });
    }

    public async Task<SqlConnection> AbrirAsync()
    {
        SqlConnection conexao;
        try
        {
            conexao = new SqlConnection(_stringConexao) { RetryLogicProvider = _novasTentativas };
        }
        catch (ArgumentException ex)
        {
            throw new FalhaAcessoDadosException(
                "A string de conexão do banco está em formato inválido. Confira o appsettings.Local.json " +
                "ou a variável de ambiente AVALIA_DB_CONNECTION.", ex);
        }

        try
        {
            await conexao.OpenAsync();
            return conexao;
        }
        catch
        {
            await conexao.DisposeAsync();
            throw;
        }
    }
}
