using ObraFacil.Wpf.ViewModels.Orcamentos;
using System.Windows.Controls;

namespace ObraFacil.Wpf.Views.Orcamentos;

public partial class OrcamentosListPage : Page
{
    public OrcamentosListPage(OrcamentosListViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        Loaded += async (_, _) => await vm.CarregarAsync();
    }
}
