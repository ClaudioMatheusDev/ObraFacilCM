using Microsoft.Extensions.DependencyInjection;
using ObraFacil.Application.Interfaces;
using ObraFacil.Application.Services;
namespace ObraFacil.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IClienteService,      ClienteService>();
        services.AddScoped<IItemCatalogoService, ItemCatalogoService>();
        services.AddScoped<IOrcamentoService,    OrcamentoService>();
        return services;
    }
}
