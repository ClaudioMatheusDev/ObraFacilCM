using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ObraFacil.Application.DTOs;
using ObraFacil.Application.Interfaces;
using ObraFacil.Wpf.Views.Clientes;
using System.Collections.ObjectModel;

namespace ObraFacil.Wpf.ViewModels.Clientes;

/// <summary>
/// ViewModel da tela de listagem de clientes. Exibe a grade de clientes com suporte
/// a busca reativa, abertura do formulário de criação/edição e exclusão com confirmação.
/// </summary>
public partial class ClientesListViewModel : ViewModelBase
{
    private readonly IClienteService _service;

    /// <summary>Texto de busca digitado pelo usuário. Ao ser alterado, dispara novo carregamento da lista.</summary>
    [ObservableProperty] string _busca = string.Empty;

    /// <summary>Cliente selecionado na grade.</summary>
    [ObservableProperty] ClienteDto? _selecionado;

    /// <summary>Lista observável de clientes exibida na grade.</summary>
    public ObservableCollection<ClienteDto> Clientes { get; } = [];

    /// <summary>Inicializa o ViewModel com o serviço de clientes e a fábrica de loggers.</summary>
    public ClientesListViewModel(IClienteService service, ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
        _service = service;
        Title    = "Clientes";
    }

    /// <summary>Carrega (ou recarrega) a lista de clientes aplicando o filtro de busca atual.</summary>
    [RelayCommand]
    public async Task CarregarAsync()
        => await ExecuteSafeAsync(async () =>
        {
            var lista = await _service.ListarAsync(string.IsNullOrWhiteSpace(Busca) ? null : Busca);
            Clientes.Clear();
            foreach (var c in lista) Clientes.Add(c);
        });

    /// <summary>Abre o formulário para criação de um novo cliente e recarrega a lista após fechar.</summary>
    [RelayCommand]
    void NovoCliente()
    {
        var win = App.GetService<ClienteFormWindow>();
        win.ShowDialog();
        CarregarCommand.Execute(null);
    }

    /// <summary>Abre o formulário de edição do <paramref name="cliente"/> selecionado e recarrega a lista após fechar.</summary>
    [RelayCommand]
    void EditarCliente(ClienteDto? cliente)
    {
        if (cliente is null) return;
        var win = App.GetService<ClienteFormWindow>();
        win.CarregarCliente(cliente.Id);
        win.ShowDialog();
        CarregarCommand.Execute(null);
    }

    /// <summary>Solicita confirmação e exclui o <paramref name="cliente"/> informado.</summary>
    [RelayCommand]
    async Task ExcluirClienteAsync(ClienteDto? cliente)
    {
        if (cliente is null) return;
        var r = System.Windows.MessageBox.Show(
            $"Excluir cliente \"{cliente.Nome}\"?", "Confirmar",
            System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
        if (r != System.Windows.MessageBoxResult.Yes) return;
        await ExecuteSafeAsync(() => _service.ExcluirAsync(cliente.Id));
        await CarregarAsync();
    }

    partial void OnBuscaChanged(string value) => CarregarCommand.Execute(null);
}
