using Microsoft.Extensions.DependencyInjection;

namespace Avalia.Desktop.Componentes;

/// <summary>Cria as telas pela injeção de dependência (cada tela recebe os serviços de que precisa).</summary>
public interface IFabricaFormularios
{
    T Criar<T>() where T : Form;
}

public sealed class FabricaFormularios(IServiceProvider provedor) : IFabricaFormularios
{
    public T Criar<T>() where T : Form => provedor.GetRequiredService<T>();
}
