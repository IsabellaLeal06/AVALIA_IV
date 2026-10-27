using System.Text.RegularExpressions;
using Avalia.Desktop.Core.Excecoes;
using Microsoft.Data.SqlClient;

namespace Avalia.Desktop.Data.Conexao;

/// <summary>Converte erros do SQL Server em exceções com mensagens amigáveis.</summary>
public static partial class TradutorErrosSql
{
    public static Exception Traduzir(SqlException ex)
    {
        var numeros = ex.Errors.Cast<SqlError>().Select(e => e.Number).Append(ex.Number).ToHashSet();

        if (numeros.Overlaps([2627, 2601]))
            return new ViolacaoUnicidadeException(NomeRestricao().Match(ex.Message) is { Success: true } m ? m.Value : null, ex);

        string mensagem;
        if (numeros.Contains(547))
            mensagem = "A operação foi recusada por uma regra de integridade do banco" +
                       (NomeRestricaoIntegridade().Match(ex.Message) is { Success: true } r ? $" ({r.Value})." : ".");
        else if (numeros.Contains(18456))
            mensagem = "Falha de login no banco de dados. Confira o usuário e a senha da string de conexão.";
        else if (numeros.Overlaps([40615, 40914]))
            mensagem = "O IP deste computador não está liberado no firewall do servidor do Azure. " +
                       "Adicione o IP em Servidor SQL → Rede → Regras de firewall.\n\nDetalhe: " + ex.Message;
        else if (numeros.Overlaps([40613, 40197, 40501, 49918, 49919, 49920, 40540]))
            mensagem = "O banco de dados está temporariamente indisponível (pode estar saindo da pausa automática). " +
                       "Aguarde alguns segundos e tente novamente.";
        else if (numeros.Contains(4060))
            mensagem = "Banco de dados não encontrado. Confira o nome do banco (Initial Catalog) na string de conexão.";
        else if (numeros.Contains(208))
            mensagem = "As tabelas do Avalia+ não foram encontradas. Execute os scripts da pasta database (01, 02 e 03).";
        else if (numeros.Contains(-2))
            mensagem = "O banco de dados demorou demais para responder. Tente novamente.";
        else if (ex.Class >= 20 || numeros.Overlaps([-1, 2, 53, 11001, 10060, 10061]))
            mensagem = "Não foi possível conectar ao servidor do banco. Verifique a internet e o endereço do servidor.";
        else
            mensagem = "Erro ao acessar o banco de dados: " + ex.Message;

        return new FalhaAcessoDadosException(mensagem, ex);
    }

    [GeneratedRegex(@"UQ_\w+")]
    private static partial Regex NomeRestricao();

    [GeneratedRegex(@"(FK|CK)_\w+")]
    private static partial Regex NomeRestricaoIntegridade();
}
