using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ObraFacil.Application.DTOs;
using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Enums;
using System.Collections.ObjectModel;
using System.Windows;

namespace ObraFacil.Wpf.ViewModels.Catalogo;

public partial class CatalogoListViewModel : ViewModelBase
{
    private readonly IItemCatalogoService _service;

    [ObservableProperty] string?          _busca;
    [ObservableProperty] ItemCatalogoDto? _selecionado;

    private TipoOpcao? _tipoFiltro;
    public TipoOpcao? TipoFiltro
    {
        get => _tipoFiltro;
        set { if (SetProperty(ref _tipoFiltro, value)) CarregarCommand.Execute(null); }
    }

    public static IList<TipoOpcao> TipoOpcoes { get; } =
    [
        new(null,              "Todos"),
        new(TipoItem.Material, "Material"),
        new(TipoItem.Servico,  "Serviço"),
    ];

    public ObservableCollection<ItemCatalogoDto> Itens { get; } = [];

    public CatalogoListViewModel(IItemCatalogoService service, ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
        _service = service;
        Title    = "Catálogo";
    }

    [RelayCommand]
    public async Task CarregarAsync()
        => await ExecuteSafeAsync(async () =>
        {
            var lista = await _service.ListarAsync(Busca, TipoFiltro?.Valor);
            Itens.Clear();
            foreach (var i in lista) Itens.Add(i);
        });

    [RelayCommand]
    void NovoItem()
    {
        var win = new Views.Catalogo.ItemCatalogoFormWindow();
        if (win.ShowDialog() == true) CarregarCommand.Execute(null);
    }

    [RelayCommand]
    void EditarItem(ItemCatalogoDto? item)
    {
        if (item is null) return;
        var win = new Views.Catalogo.ItemCatalogoFormWindow(item);
        if (win.ShowDialog() == true) CarregarCommand.Execute(null);
    }

    [RelayCommand]
    async Task ExcluirItemAsync(ItemCatalogoDto? item)
    {
        if (item is null) return;
        var r = MessageBox.Show($"Excluir '{item.Nome}'?", "Confirmar",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (r != MessageBoxResult.Yes) return;
        await ExecuteSafeAsync(() => _service.ExcluirAsync(item.Id));
        await CarregarAsync();
    }

    partial void OnBuscaChanged(string? value) => CarregarCommand.Execute(null);
}

public record TipoOpcao(TipoItem? Valor, string Label);
