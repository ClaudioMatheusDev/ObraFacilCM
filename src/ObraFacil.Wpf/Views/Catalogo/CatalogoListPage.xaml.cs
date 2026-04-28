using ObraFacil.Wpf.ViewModels.Catalogo;
using System.Windows.Controls;

namespace ObraFacil.Wpf.Views.Catalogo;

public partial class CatalogoListPage : Page
{
    private readonly CatalogoListViewModel _vm;

    public CatalogoListPage(CatalogoListViewModel vm)
    {
        InitializeComponent();
        _vm         = vm;
        DataContext = vm;
        Loaded     += async (_, _) => await vm.CarregarAsync();
    }
}
