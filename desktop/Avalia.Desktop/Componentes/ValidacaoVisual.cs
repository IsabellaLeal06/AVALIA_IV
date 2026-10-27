using Avalia.Desktop.Core.Servicos;

namespace Avalia.Desktop.Componentes;

/// <summary>
/// Liga os erros devolvidos pelos serviços aos campos da tela usando o
/// ErrorProvider (ícone ao lado do campo), resume os erros na barra de
/// mensagens e leva o foco ao primeiro campo inválido.
/// </summary>
public sealed class ValidacaoVisual(ErrorProvider provedor)
{
    private readonly Dictionary<string, Control> _campos = new(StringComparer.Ordinal);

    public ValidacaoVisual Campo(string nome, Control controle)
    {
        _campos[nome] = controle;
        return this;
    }

    public void Limpar()
    {
        foreach (var controle in _campos.Values)
        {
            provedor.SetError(controle, string.Empty);
            controle.AccessibleDescription = null;
        }
    }

    /// <summary>Destaca os campos com erro e envia o resumo para <paramref name="exibirAviso"/>.</summary>
    public void Mostrar(ResultadoOperacao resultado, Action<string> exibirAviso)
    {
        Limpar();
        Control? primeiro = null;

        foreach (var grupo in resultado.Erros.GroupBy(e => e.Campo))
        {
            if (grupo.Key is null || !_campos.TryGetValue(grupo.Key, out var controle))
                continue;

            var texto = string.Join(Environment.NewLine, grupo.Select(e => e.Mensagem).Distinct());
            provedor.SetError(controle, texto);
            controle.AccessibleDescription = texto; // leitores de tela também recebem o erro
            primeiro ??= controle;
        }

        var mensagens = resultado.Erros.Select(e => e.Mensagem).Distinct().ToList();
        exibirAviso(mensagens.Count switch
        {
            1 => mensagens[0],
            <= 3 => "Corrija os campos destacados:" + Environment.NewLine + "• " + string.Join(Environment.NewLine + "• ", mensagens),
            _ => $"Corrija os {mensagens.Count} itens destacados em vermelho. " +
                 "Passe o mouse sobre o ícone ao lado de cada campo para ver o que falta."
        });

        primeiro?.Focus();
    }
}
