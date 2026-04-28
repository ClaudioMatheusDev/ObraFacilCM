using ObraFacil.Wpf.ViewModels.Clientes;
using System.Windows.Controls;

namespace ObraFacil.Wpf.Views.Clientes;

public partial class ClientesListPage : Page
{
    private readonly ClientesListViewModel _vm;
    public ClientesListPage(ClientesListViewModel vm)
    {
        InitializeComponent();
        _vm        = vm;
        DataContext = vm;
        Loaded     += async (_, _) => await vm.CarregarAsync();
    }
}
