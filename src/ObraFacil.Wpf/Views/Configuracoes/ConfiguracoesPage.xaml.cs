using ObraFacil.Wpf.ViewModels.Configuracoes;
using System.Windows.Controls;

namespace ObraFacil.Wpf.Views.Configuracoes;

public partial class ConfiguracoesPage : Page
{
    public ConfiguracoesPage(ConfiguracoesViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        Loaded     += async (_, _) => await vm.CarregarAsync();
    }
}
