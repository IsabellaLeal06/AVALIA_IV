using System.Globalization;

namespace Avalia.Desktop.Core.Modelos;

/// <summary>
/// Tabela Medalha: faixa de pontuação da gamificação (RN22). A medalha não é
/// gravada no aluno; é apurada comparando a pontuação com estas faixas.
/// </summary>
public class Medalha
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int PontosMinimos { get; set; }
    public string ImagemUrl { get; set; } = string.Empty;

    // Preenchidos na listagem
    /// <summary>Pontuação mínima da faixa seguinte (nula na última faixa).</summary>
    public int? PontosProximaFaixa { get; set; }

    /// <summary>Alunos ativos cuja pontuação está nesta faixa.</summary>
    public int QtdAlunos { get; set; }

    public string Faixa => PontosProximaFaixa is int proxima
        ? string.Format(PtBr, "{0:N0} a {1:N0} pontos", PontosMinimos, proxima - 1)
        : string.Format(PtBr, "{0:N0} pontos ou mais", PontosMinimos);

    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");
}
