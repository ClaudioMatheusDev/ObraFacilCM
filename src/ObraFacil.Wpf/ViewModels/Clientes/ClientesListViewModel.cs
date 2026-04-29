using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ObraFacil.Application.DTOs;
using ObraFacil.Application.Interfaces;
using ObraFacil.Wpf.Views.Clientes;
using System.Collections.ObjectModel;

namespace ObraFacil.Wpf.ViewModels.Clientes;

public partial class ClientesListViewModel : ViewModelBase
{
    private readonly IClienteService _service;

    [ObservableProperty] string _busca = string.Empty;
    [ObservableProperty] ClienteDto? _selecionado;

    public ObservableCollection<ClienteDto> Clientes { get; } = [];

    public ClientesListViewModel(IClienteService service, ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
        _service = service;
        Title    = "Clientes";
    }

    [RelayCommand]
    public async Task CarregarAsync()
        => await ExecuteSafeAsync(async () =>
        {
            var lista = await _service.ListarAsync(string.IsNullOrWhiteSpace(Busca) ? null : Busca);
            Clientes.Clear();
            foreach (var c in lista) Clientes.Add(c);
        });

    [RelayCommand]
    void NovoCliente()
    {
        var win = App.GetService<ClienteFormWindow>();
        win.ShowDialog();
        CarregarCommand.Execute(null);
    }

    [RelayCommand]
    void EditarCliente(ClienteDto? cliente)
    {
        if (cliente is null) return;
        var win = App.GetService<ClienteFormWindow>();
        win.CarregarCliente(cliente.Id);
        win.ShowDialog();
        CarregarCommand.Execute(null);
    }

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
