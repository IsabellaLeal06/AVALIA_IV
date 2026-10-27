using Avalia.Desktop.Core.Interfaces;

namespace Avalia.Desktop.Core.Servicos;

/// <summary>
/// Hash BCrypt com custo 10. O formato ($2a$/$2b$) é o mesmo lido pelas
/// bibliotecas bcrypt/bcryptjs, que a API em JavaScript vai usar.
/// </summary>
public class BCryptHashSenha : IHashSenha
{
    public const int Custo = 10;

    public string Gerar(string senha) => BCrypt.Net.BCrypt.HashPassword(senha, Custo);

    public bool Verificar(string senha, string hash)
    {
        if (string.IsNullOrEmpty(hash))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(senha, hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // Hash inválido gravado no banco: trata como senha incorreta
            return false;
        }
    }
}
