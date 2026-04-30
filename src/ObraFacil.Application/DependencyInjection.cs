using Microsoft.Extensions.DependencyInjection;
using ObraFacil.Application.Interfaces;
using ObraFacil.Application.Services;

namespace ObraFacil.Application;

/// <summary>
/// Métodos de extensão para registro dos serviços da camada Application no container de injeção de dependência.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra os serviços de aplicação (<see cref="IClienteService"/>,
    /// <see cref="IItemCatalogoService"/> e <see cref="IOrcamentoService"/>) com ciclo de vida Scoped.
    /// </summary>
    /// <param name="services">Coleção de descritores de serviço.</param>
    /// <returns>A própria <paramref name="services"/> para chamadas encadeadas.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IClienteService,      ClienteService>();
        services.AddScoped<IItemCatalogoService, ItemCatalogoService>();
        services.AddScoped<IOrcamentoService,    OrcamentoService>();
        return services;
    }
}
