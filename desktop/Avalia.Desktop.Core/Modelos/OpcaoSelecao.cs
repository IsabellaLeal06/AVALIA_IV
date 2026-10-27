namespace Avalia.Desktop.Core.Modelos;

/// <summary>Item simples para listas de seleção (turmas, professores, disciplinas).</summary>
public sealed record OpcaoSelecao(int Id, string Nome, bool Ativo = true, string SufixoInativo = "(inativo)")
{
    /// <summary>Texto exibido: registros desativados recebem um sufixo, como "(inativa)".</summary>
    public string Descricao => Ativo ? Nome : $"{Nome} {SufixoInativo}";

    public override string ToString() => Descricao;
}
