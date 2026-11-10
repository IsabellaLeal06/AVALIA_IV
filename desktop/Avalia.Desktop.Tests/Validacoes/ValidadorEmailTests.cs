using Avalia.Desktop.Core.Validacoes;

namespace Avalia.Desktop.Tests.Validacoes;

/// <summary>Testes unitários da classe ValidadorEmail (RF01 e RF02: login e cadastro).</summary>
public class ValidadorEmailTests
{
    [Theory]
    [InlineData("admin@avaliamais.test")]
    [InlineData("ana.souza@aluno.avaliamais.test")]
    [InlineData("nome+etiqueta@dominio.com.br")]
    [InlineData("  MAIUSCULO@Dominio.COM  ")]  // espaços e maiúsculas são normalizados
    public void EhValido_EmailCorreto_RetornaVerdadeiro(string email)
    {
        Assert.True(ValidadorEmail.EhValido(email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("semarroba.com")]
    [InlineData("dois@@arrobas.com")]
    [InlineData("sem@ponto")]                 // domínio precisa ter ponto
    [InlineData("@semusuario.com")]
    [InlineData("com espaco@dominio.com")]
    [InlineData("usuario@-invalido.com")]     // domínio não começa com hífen
    public void EhValido_EmailIncorreto_RetornaFalso(string? email)
    {
        Assert.False(ValidadorEmail.EhValido(email));
    }

    [Fact]
    public void EhValido_AcimaDoTamanhoMaximo_RetornaFalso()
    {
        var longo = new string('a', ValidadorEmail.TamanhoMaximo) + "@dominio.com";

        Assert.False(ValidadorEmail.EhValido(longo));
    }

    [Fact]
    public void Normalizar_RemoveEspacosEUsaMinusculas()
    {
        Assert.Equal("ana@dominio.com", ValidadorEmail.Normalizar("  ANA@Dominio.com "));
        Assert.Equal(string.Empty, ValidadorEmail.Normalizar(null));
    }
}
