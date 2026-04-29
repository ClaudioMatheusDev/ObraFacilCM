using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ObraFacil.Infrastructure;
using ObraFacil.Infrastructure.Data;
using ObraFacil.Infrastructure.Seed;
using ObraFacil.Wpf.ViewModels.Catalogo;
using ObraFacil.Wpf.ViewModels.Clientes;
using ObraFacil.Wpf.ViewModels.Configuracoes;
using ObraFacil.Wpf.ViewModels.Orcamentos;
using ObraFacil.Wpf.Views;
using ObraFacil.Wpf.Views.Catalogo;
using ObraFacil.Wpf.Views.Clientes;
using ObraFacil.Wpf.Views.Configuracoes;
using ObraFacil.Wpf.Views.Orcamentos;
using AppDI = ObraFacil.Application.DependencyInjection;

namespace ObraFacil.Wpf.Startup;

internal static class AppBootstrap
{
    internal static IServiceProvider ConfigureServices(string dbPath)
    {
        var services = new ServiceCollection();

        services.AddLogging(logging =>
        {
            logging.AddDebug();
            logging.SetMinimumLevel(LogLevel.Debug);
        });

        services.AddInfrastructure(dbPath);
        AppDI.AddApplication(services);

        // ViewModels
        services.AddTransient<ClientesListViewModel>();
        services.AddTransient<OrcamentosListViewModel>();
        services.AddTransient<OrcamentoFormViewModel>();
        services.AddTransient<CatalogoListViewModel>();
        services.AddTransient<ConfiguracoesViewModel>();

        // Pages
        services.AddTransient<OrcamentosListPage>();
        services.AddTransient<ClientesListPage>();
        services.AddTransient<CatalogoListPage>();
        services.AddTransient<ConfiguracoesPage>();

        // Windows (Transient = nova instância a cada abertura)
        services.AddTransient<MainWindow>();
        services.AddTransient<ClienteFormWindow>();
        services.AddTransient<OrcamentoFormWindow>();

        return services.BuildServiceProvider();
    }

    internal static async Task InitializeDatabaseAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await BaselineMigrateAsync(db);
        await DatabaseSeeder.SeedAsync(db);
    }

    /// <summary>
    /// Lida com bancos criados antes das migrations (via EnsureCreated).
    /// Se o banco já existe mas não tem histórico de migrations, registra a
    /// InitialCreate como aplicada sem tentar recriar as tabelas.
    /// </summary>
    private static async Task BaselineMigrateAsync(AppDbContext db)
    {
        var canConnect = await db.Database.CanConnectAsync();

        if (canConnect)
        {
            var applied = await db.Database.GetAppliedMigrationsAsync();
            if (!applied.Any())
            {
                await db.Database.ExecuteSqlRawAsync(
                    """
                    CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                        "MigrationId"    TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
                        "ProductVersion" TEXT NOT NULL
                    )
                    """);
                await db.Database.ExecuteSqlRawAsync(
                    """
                    INSERT OR IGNORE INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
                    VALUES ('20260429120925_InitialCreate', '8.0.14')
                    """);
            }
        }

        await db.Database.MigrateAsync();
    }
}
