namespace Avalia.Desktop.Core.Interfaces;

/// <summary>Geração e verificação de hash de senha (RNF01).</summary>
public interface IHashSenha
{
    string Gerar(string senha);
    bool Verificar(string senha, string hash);
}
