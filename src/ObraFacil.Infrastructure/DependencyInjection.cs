using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Interfaces;
using ObraFacil.Infrastructure.Data;
using ObraFacil.Infrastructure.Repositories;
using ObraFacil.Infrastructure.Services;

namespace ObraFacil.Infrastructure;

/// <summary>
/// Métodos de extensão para registro dos serviços de infraestrutura no container de injeção de dependência.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra o <see cref="AppDbContext"/> com SQLite, todos os repositórios e serviços
    /// de infraestrutura (<see cref="IUnitOfWork"/>, <see cref="IBackupService"/>, <see cref="IPdfService"/>)
    /// com ciclo de vida Scoped.
    /// </summary>
    /// <param name="services">Coleção de descritores de serviço.</param>
    /// <param name="dbPath">Caminho completo do arquivo SQLite.</param>
    /// <returns>A própria <paramref name="services"/> para chamadas encadeadas.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string dbPath)
    {
        services.AddDbContext<AppDbContext>(opt => opt.UseSqlite($"Data Source={dbPath}"));

        services.AddScoped<IUnitOfWork,                UnitOfWork>();
        services.AddScoped<IClienteRepository,         ClienteRepository>();
        services.AddScoped<IItemCatalogoRepository,    ItemCatalogoRepository>();
        services.AddScoped<IOrcamentoRepository,       OrcamentoRepository>();
        services.AddScoped<IConfiguracaoRepository,    ConfiguracaoRepository>();
        services.AddScoped<IBackupService,             BackupService>();
        services.AddScoped<IPdfService,                PdfService>();

        return services;
    }
}
