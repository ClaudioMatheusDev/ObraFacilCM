using Microsoft.Extensions.DependencyInjection;
using ObraFacil.Application.DTOs;
using ObraFacil.Application.Interfaces;
using ObraFacil.Wpf.Views.Catalogo;
using ObraFacil.Wpf.Views.Clientes;
using ObraFacil.Wpf.Views.Orcamentos;

namespace ObraFacil.Wpf.Services;

internal sealed class WindowFactory : IWindowFactory
{
    private readonly IServiceProvider _services;

    public WindowFactory(IServiceProvider services) => _services = services;

    public ClienteFormWindow CreateClienteFormWindow()
        => _services.GetRequiredService<ClienteFormWindow>();

    public OrcamentoFormWindow CreateOrcamentoFormWindow()
        => _services.GetRequiredService<OrcamentoFormWindow>();

    public ItemCatalogoFormWindow CreateItemCatalogoFormWindow(ItemCatalogoDto? item = null)
        => new(
            _services.GetRequiredService<IItemCatalogoService>(),
            _services.GetRequiredService<IDialogService>(),
            item);

    public SelecionarCatalogoWindow CreateSelecionarCatalogoWindow(IList<ItemCatalogoDto> itens)
        => new(itens);
}
