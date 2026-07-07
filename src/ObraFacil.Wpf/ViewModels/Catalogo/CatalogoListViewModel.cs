using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ObraFacil.Application.DTOs;
using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Enums;
using ObraFacil.Wpf.Services;
using System.Collections.ObjectModel;

namespace ObraFacil.Wpf.ViewModels.Catalogo;

/// <summary>
/// ViewModel da tela de listagem do catálogo de materiais e serviços.
/// Suporta busca reativa, filtro por tipo e operações de criação, edição e exclusão de itens.
/// </summary>
public partial class CatalogoListViewModel : ViewModelBase
{
    private readonly IItemCatalogoService _service;
    private readonly IWindowFactory _windows;

    /// <summary>Texto de busca digitado pelo usuário. Ao ser alterado, dispara novo carregamento.</summary>
    [ObservableProperty] string?          _busca;

    /// <summary>Item do catálogo selecionado na grade.</summary>
    [ObservableProperty] ItemCatalogoDto? _selecionado;

    private TipoOpcao? _tipoFiltro;

    /// <summary>
    /// Opção de tipo selecionada no filtro. Ao mudar, recarrega a lista automaticamente.
    /// </summary>
    public TipoOpcao? TipoFiltro
    {
        get => _tipoFiltro;
        set { if (SetProperty(ref _tipoFiltro, value)) CarregarCommand.Execute(null); }
    }

    /// <summary>Opções de tipo disponíveis no ComboBox de filtro, incluindo "Todos".</summary>
    public static IList<TipoOpcao> TipoOpcoes { get; } =
    [
        new(null,              "Todos"),
        new(TipoItem.Material, "Material"),
        new(TipoItem.Servico,  "Serviço"),
    ];

    /// <summary>Lista observável de itens do catálogo exibida na grade.</summary>
    public ObservableCollection<ItemCatalogoDto> Itens { get; } = [];

    /// <summary>Inicializa o ViewModel com o serviço de catálogo e a fábrica de loggers.</summary>
    public CatalogoListViewModel(IItemCatalogoService service, IWindowFactory windows,
        IDialogService dialogs, ILoggerFactory loggerFactory)
        : base(loggerFactory, dialogs)
    {
        _service = service;
        _windows = windows;
        Title    = "Catálogo";
    }

    /// <summary>Carrega os itens do catálogo aplicando os filtros de busca e tipo ativos.</summary>
    [RelayCommand]
    public async Task CarregarAsync()
        => await ExecuteSafeAsync(async () =>
        {
            var lista = await _service.ListarAsync(Busca, TipoFiltro?.Valor);
            Itens.Clear();
            foreach (var i in lista) Itens.Add(i);
        });

    /// <summary>Abre o formulário para criação de um novo item no catálogo e recarrega a lista se confirmado.</summary>
    [RelayCommand]
    void NovoItem()
    {
        var win = _windows.CreateItemCatalogoFormWindow();
        if (win.ShowDialog() == true) CarregarCommand.Execute(null);
    }

    /// <summary>Abre o formulário de edição do <paramref name="item"/> selecionado e recarrega a lista se confirmado.</summary>
    [RelayCommand]
    void EditarItem(ItemCatalogoDto? item)
    {
        if (item is null) return;
        var win = _windows.CreateItemCatalogoFormWindow(item);
        if (win.ShowDialog() == true) CarregarCommand.Execute(null);
    }

    /// <summary>Solicita confirmação e exclui o item informado do catálogo.</summary>
    [RelayCommand]
    async Task ExcluirItemAsync(ItemCatalogoDto? item)
    {
        if (item is null) return;
        if (!Dialogs.Confirm($"Excluir '{item.Nome}'?")) return;
        await ExecuteSafeAsync(() => _service.ExcluirAsync(item.Id));
        await CarregarAsync();
    }

    partial void OnBuscaChanged(string? value) => CarregarCommand.Execute(null);
}

/// <summary>Wrapper de <see cref="TipoItem"/> para exibição em ComboBox com opção "Todos" (valor nulo).</summary>
/// <param name="Valor">Valor do enum, ou <c>null</c> para representar "Todos".</param>
/// <param name="Label">Rótulo exibido ao usuário.</param>
public record TipoOpcao(TipoItem? Valor, string Label);
