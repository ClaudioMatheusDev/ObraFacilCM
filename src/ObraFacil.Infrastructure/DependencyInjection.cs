using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Interfaces;
using ObraFacil.Infrastructure.Data;
using ObraFacil.Infrastructure.Repositories;
using ObraFacil.Infrastructure.Services;

namespace ObraFacil.Infrastructure;

public static class DependencyInjection
{
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
