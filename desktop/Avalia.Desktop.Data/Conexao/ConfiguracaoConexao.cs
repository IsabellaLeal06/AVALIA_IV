using Microsoft.Data.SqlClient;

namespace Avalia.Desktop.Data.Conexao;

/// <summary>
/// Localiza a string de conexão sem que ela seja versionada:
/// 1º variável de ambiente AVALIA_DB_CONNECTION;
/// 2º ConnectionStrings:Avalia do appsettings.Local.json (fora do Git).
/// </summary>
public static class ConfiguracaoConexao
{
    public const string VariavelAmbiente = "AVALIA_DB_CONNECTION";

    public static string? ObterStringConexao(string? valorDoArquivo)
    {
        var valor = Environment.GetEnvironmentVariable(VariavelAmbiente);
        if (string.IsNullOrWhiteSpace(valor))
            valor = valorDoArquivo;

        if (string.IsNullOrWhiteSpace(valor))
            return null;

        // O appsettings.example.json traz marcadores como <servidor> e <senha>
        return valor.Contains('<') && valor.Contains('>') ? null : valor;
    }

    /// <summary>Servidor e banco, para exibir na barra de status (sem usuário e senha).</summary>
    public static string DescreverDestino(string stringConexao)
    {
        try
        {
            var dados = new SqlConnectionStringBuilder(stringConexao);
            var servidor = dados.DataSource.Replace("tcp:", string.Empty).Split(',')[0];
            return $"{dados.InitialCatalog} @ {servidor}";
        }
        catch (ArgumentException)
        {
            return "string de conexão inválida";
        }
    }
}
