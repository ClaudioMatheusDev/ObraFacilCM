using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace ObraFacil.Wpf.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty] bool   _isBusy;
    [ObservableProperty] string _title = string.Empty;

    protected readonly ILogger Logger;

    protected ViewModelBase(ILoggerFactory loggerFactory)
    {
        Logger = loggerFactory.CreateLogger(GetType());
    }

    protected async Task ExecuteSafeAsync(Func<Task> action, string? mensagemErro = null)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            await action();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro em {ViewModel}: {Message}", GetType().Name, ex.Message);
            var msg = mensagemErro ?? "Ocorreu um erro inesperado.";
            MessageBox.Show($"{msg}\n\n{ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally { IsBusy = false; }
    }
}
