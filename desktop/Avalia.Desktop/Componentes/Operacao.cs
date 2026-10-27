using Avalia.Desktop.Core.Excecoes;

namespace Avalia.Desktop.Componentes;

/// <summary>
/// Executa chamadas assíncronas aos serviços a partir das telas: mostra o
/// cursor de espera, bloqueia a tela durante a operação e trata os erros,
/// exibindo-os na própria tela (sem caixas de mensagem).
/// </summary>
public static class Operacao
{
    /// <returns>true se a operação terminou sem exceção.</returns>
    public static async Task<bool> ExecutarAsync(Control tela, Action<string> exibirErro, Func<Task> acao)
    {
        var form = tela.FindForm() ?? tela;
        form.UseWaitCursor = true;
        tela.Enabled = false;
        try
        {
            await acao();
            return true;
        }
        catch (FalhaAcessoDadosException ex)
        {
            exibirErro(ex.Message);
        }
        catch (Exception ex)
        {
            exibirErro("Ocorreu um erro inesperado: " + ex.Message);
        }
        finally
        {
            tela.Enabled = true;
            form.UseWaitCursor = false;
        }

        return false;
    }

    /// <summary>Versão para as telas da janela principal: o erro aparece na barra de mensagens.</summary>
    public static Task<bool> ExecutarAsync(Control tela, INavegacao navegacao, Func<Task> acao) =>
        ExecutarAsync(tela, erro => navegacao.ExibirMensagem(TipoMensagem.Erro, erro), acao);
}
