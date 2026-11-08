using Avalia.Desktop.Core.Validacoes;

namespace Avalia.Desktop.Tests.Validacoes;

/// <summary>Testes unitários da classe ValidadorCpf (RF02: cadastro de usuário).</summary>
public class ValidadorCpfTests
{
    // CPFs fictícios com dígitos verificadores corretos (os mesmos dos dados de teste do banco)
    [Theory]
    [InlineData("15881399803")]
    [InlineData("158.813.998-03")]   // com pontos e traço
    [InlineData(" 69879730917 ")]    // com espaços nas pontas
    [InlineData("88447294072")]
    public void EhValido_CpfCorreto_RetornaVerdadeiro(string cpf)
    {
        Assert.True(ValidadorCpf.EhValido(cpf));
    }

    [Theory]
    [InlineData("15881399804")]      // último dígito errado
    [InlineData("12345678900")]      // dígitos verificadores errados
    [InlineData("1588139980")]       // 10 dígitos
    [InlineData("158813998033")]     // 12 dígitos
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void EhValido_CpfIncorreto_RetornaFalso(string? cpf)
    {
        Assert.False(ValidadorCpf.EhValido(cpf));
    }

    [Theory]
    [InlineData("00000000000")]
    [InlineData("11111111111")]
    [InlineData("99999999999")]
    public void EhValido_TodosOsDigitosIguais_RetornaFalso(string cpf)
    {
        // Passam no cálculo do módulo 11, mas não são CPFs válidos
        Assert.False(ValidadorCpf.EhValido(cpf));
    }

    [Fact]
    public void EhValido_ComLetras_RetornaFalso()
    {
        Assert.False(ValidadorCpf.EhValido("1588139980A"));
        Assert.False(ValidadorCpf.EhValido("158x813x998x03"));
    }

    [Fact]
    public void Normalizar_RemoveTudoQueNaoForDigito()
    {
        Assert.Equal("15881399803", ValidadorCpf.Normalizar(" 158.813.998-03 "));
        Assert.Equal(string.Empty, ValidadorCpf.Normalizar(null));
    }

    [Fact]
    public void Formatar_ElevenDigitos_AplicaMascara()
    {
        Assert.Equal("158.813.998-03", ValidadorCpf.Formatar("15881399803"));
    }

    [Fact]
    public void Formatar_QuantidadeErrada_DevolveOTextoOriginal()
    {
        Assert.Equal("123", ValidadorCpf.Formatar("123"));
    }
}
