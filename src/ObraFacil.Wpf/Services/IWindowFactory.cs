using ObraFacil.Application.DTOs;
using ObraFacil.Wpf.Views.Clientes;
using ObraFacil.Wpf.Views.Catalogo;
using ObraFacil.Wpf.Views.Orcamentos;

namespace ObraFacil.Wpf.Services;

public interface IWindowFactory
{
    ClienteFormWindow CreateClienteFormWindow();
    OrcamentoFormWindow CreateOrcamentoFormWindow();
    ItemCatalogoFormWindow CreateItemCatalogoFormWindow(ItemCatalogoDto? item = null);
    SelecionarCatalogoWindow CreateSelecionarCatalogoWindow(IList<ItemCatalogoDto> itens);
}
