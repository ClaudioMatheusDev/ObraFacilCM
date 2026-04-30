using Microsoft.Extensions.DependencyInjection;
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

        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ObraFacil", "obrafacil.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

        _services = AppBootstrap.ConfigureServices(dbPath);
        await AppBootstrap.InitializeDatabaseAsync(_services);

        GetService<MainWindow>().Show();
    }
}
