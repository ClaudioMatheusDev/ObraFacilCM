using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ObraFacil.Wpf.Startup;
using ObraFacil.Wpf.Views;
using System.IO;
using System.Windows;

namespace ObraFacil.Wpf;

public partial class App : System.Windows.Application
{
    private static IServiceProvider _services = null!;

    public static T GetService<T>() where T : notnull => _services.GetRequiredService<T>();

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ObraFacil", "obrafacil.db");
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

            _services = AppBootstrap.ConfigureServices(dbPath);
            await AppBootstrap.InitializeDatabaseAsync(_services);

            GetService<MainWindow>().Show();
        }
        catch (Exception ex)
        {
            var logger = _services?.GetService<ILogger<App>>();
            logger?.LogCritical(ex, "Falha ao inicializar o aplicativo ObraFácil.");

            MessageBox.Show(
                $"Não foi possível iniciar o ObraFácil.\n\nDetalhes: {ex.Message}",
                "Erro de inicialização",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown(-1);
        }
    }
}
