using Avalia.Desktop.Core.Interfaces;
using Avalia.Desktop.Core.Modelos;
using Avalia.Desktop.Core.Servicos;
using Avalia.Desktop.Core.Validacoes;

namespace Avalia.Desktop.Core.Controles;

/// <summary>
/// Controle Visualizar_perfil (caso de uso Visualizar perfil): dados do
/// usuário conectado, alteração da própria senha (RN04) e saída da conta.
/// O nome segue o diagrama de classes.
/// </summary>
public class Visualizar_perfil(IUsuarioRepository usuarios, IHashSenha hashSenha, SessaoUsuario sessao)
{
    /// <summary>visualizarPerfil(id) → Usuario.consultar(id). Devolve os dados sem a senha.</summary>
    public async Task<Usuario?> VisualizarPerfil(int id)
    {
        var usuario = await usuarios.Consultar(id);
        if (usuario is not null)
            usuario.SenhaHash = string.Empty;
        return usuario;
    }

    /// <summary>
    /// alterarSenha(id, senhaAtual, novaSenha, confirmacao) → Usuario.alterarSenha(id, hash).
    /// RN04: o usuário altera a própria senha.
    /// </summary>
    public async Task<ResultadoOperacao> AlterarSenha(int id, string? senhaAtual, string? novaSenha, string? confirmacao)
    {
        senhaAtual ??= string.Empty;
        novaSenha ??= string.Empty;
        confirmacao ??= string.Empty;

        if (sessao.Usuario?.Id != id)
            return ResultadoOperacao.Falha("Cada usuário altera somente a própria senha.");

        var erros = new ListaErros();
        if (senhaAtual.Length == 0)
            erros.Adicionar(CamposSenha.SenhaAtual, "Informe a senha atual.");

        foreach (var erro in ValidadorSenha.Validar(novaSenha))
            erros.Adicionar(CamposSenha.NovaSenha, erro);

        if (novaSenha != confirmacao)
            erros.Adicionar(CamposSenha.Confirmacao, "A confirmação não é igual à nova senha.");

        if (erros.PossuiErros)
            return ResultadoOperacao.Falha(erros);

        var usuario = await usuarios.Consultar(id);
        if (usuario is null || !usuario.Ativo)
            return ResultadoOperacao.Falha("Usuário não encontrado ou desativado.");

        if (!hashSenha.Verificar(senhaAtual, usuario.SenhaHash))
            return ResultadoOperacao.Falha("A senha atual não confere.", CamposSenha.SenhaAtual);

        // Como a nova senha precisa ter letra (ValidadorSenha), ela nunca será o CPF
        if (novaSenha == senhaAtual)
            return ResultadoOperacao.Falha("A nova senha deve ser diferente da senha atual.", CamposSenha.NovaSenha);

        await usuarios.AlterarSenha(id, hashSenha.Gerar(novaSenha));
        sessao.RecomendarTrocaSenha = false;
        return ResultadoOperacao.Ok("Senha alterada com sucesso.");
    }

    /// <summary>sairConta(): encerra a sessão e volta para a tela de autenticação.</summary>
    public void SairConta() => sessao.Encerrar();
}

/// <summary>Nomes dos campos de senha, para destacar erros na tela.</summary>
public static class CamposSenha
{
    public const string SenhaAtual = nameof(SenhaAtual);
    public const string NovaSenha = nameof(NovaSenha);
    public const string Confirmacao = nameof(Confirmacao);
}
