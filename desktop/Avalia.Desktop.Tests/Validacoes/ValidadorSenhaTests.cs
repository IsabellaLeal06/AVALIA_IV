using Avalia.Desktop.Core.Validacoes;

namespace Avalia.Desktop.Tests.Validacoes;

/// <summary>Testes unitários da classe ValidadorSenha (RN04: troca da própria senha).</summary>
public class ValidadorSenhaTests
{
    [Theory]
    [InlineData("abcdefg1")]         // exatamente 8 caracteres
    [InlineData("Senha123")]
    [InlineData("minhaSenhaForte2026")]
    [InlineData("ação1234")]         // letra acentuada conta como letra
    public void Validar_SenhaBoa_NaoTemErros(string senha)
    {
        Assert.Empty(ValidadorSenha.Validar(senha));
    }

    [Fact]
    public void Validar_MenosDeOitoCaracteres_InformaTamanho()
    {
        var erros = ValidadorSenha.Validar("abc123");

        Assert.Contains(erros, e => e.Contains("pelo menos 8 caracteres"));
    }

    [Fact]
    public void Validar_SemNumero_InformaQueFaltaNumero()
    {
        var erros = ValidadorSenha.Validar("somenteletras");

        Assert.Contains(erros, e => e.Contains("número"));
        Assert.DoesNotContain(erros, e => e.Contains("letra"));
    }

    [Fact]
    public void Validar_SemLetra_InformaQueFaltaLetra()
    {
        var erros = ValidadorSenha.Validar("12345678");

        Assert.Contains(erros, e => e.Contains("letra"));
        Assert.DoesNotContain(erros, e => e.Contains("número"));
    }

    [Theory]
    [InlineData(" Senha1234")]       // espaço no começo
    [InlineData("Senha1234 ")]       // espaço no fim
    public void Validar_EspacoNasPontas_Recusa(string senha)
    {
        Assert.Contains(ValidadorSenha.Validar(senha), e => e.Contains("espaço"));
    }

    [Fact]
    public void Validar_MaisDe72Bytes_Recusa()
    {
        // O BCrypt só considera 72 bytes; acima disso a senha seria cortada em silêncio
        var longa = new string('a', ValidadorSenha.TamanhoMaximoBytes) + "1";

        Assert.Contains(ValidadorSenha.Validar(longa), e => e.Contains("longa demais"));
    }

    [Fact]
    public void Validar_Exatamente72Bytes_Aceita()
    {
        var limite = new string('a', ValidadorSenha.TamanhoMaximoBytes - 1) + "1";

        Assert.Empty(ValidadorSenha.Validar(limite));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validar_Vazia_ListaTodosOsErrosPossiveis(string? senha)
    {
        var erros = ValidadorSenha.Validar(senha);

        Assert.Equal(3, erros.Count);    // tamanho, letra e número
    }
}
