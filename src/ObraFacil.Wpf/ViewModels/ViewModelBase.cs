using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace ObraFacil.Wpf.ViewModels;

/// <summary>
/// Classe base para todos os ViewModels da aplicação, fornecendo controle de estado
/// de carregamento (<see cref="IsBusy"/>), título da tela, logger e execução segura
/// de operações assíncronas com tratamento centralizado de erros.
/// </summary>
public abstract partial class ViewModelBase : ObservableObject
{
    /// <summary>Indica se uma operação assíncrona está em andamento. Controla o estado de carregamento da UI.</summary>
    [ObservableProperty] bool   _isBusy;

    /// <summary>Título exibido na tela ou janela associada a este ViewModel.</summary>
    [ObservableProperty] string _title = string.Empty;

    /// <summary>Logger da categoria do tipo concreto do ViewModel.</summary>
    protected readonly ILogger Logger;

    /// <summary>Inicializa o ViewModel criando o logger para o tipo concreto.</summary>
    /// <param name="loggerFactory">Fábrica de loggers injetada pelo container DI.</param>
    protected ViewModelBase(ILoggerFactory loggerFactory)
    {
        Logger = loggerFactory.CreateLogger(GetType());
    }

    /// <summary>
    /// Executa a <paramref name="action"/> de forma segura: ativa <see cref="IsBusy"/>,
    /// captura exceções, exibe <see cref="MessageBox"/> com a mensagem de erro e
    /// desativa <see cref="IsBusy"/> ao final.
    /// Ignora a chamada se já houver uma operação em andamento.
    /// </summary>
    /// <param name="action">Ação assíncrona a ser executada.</param>
    /// <param name="mensagemErro">Mensagem exibida ao usuário em caso de erro. Usa mensagem genérica se nulo.</param>
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
