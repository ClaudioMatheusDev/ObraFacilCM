using ObraFacil.Wpf.Views.Clientes;
using ObraFacil.Wpf.Views.Orcamentos;
using ObraFacil.Wpf.Views.Catalogo;
using ObraFacil.Wpf.Views.Configuracoes;
using System.Windows;
using System.Windows.Controls;

namespace ObraFacil.Wpf.Views;

public partial class MainWindow : Window
{
    public MainWindow() { InitializeComponent(); Loaded += (_, _) => Navegar("Orcamentos"); }

    private void NavClick(object sender, RoutedEventArgs e)
        => Navegar(((Button)sender).Tag.ToString()!);

    private void Navegar(string destino)
    {
        Page page = destino switch
        {
            "Orcamentos" => App.GetService<OrcamentosListPage>(),
            "Clientes"   => App.GetService<ClientesListPage>(),
            "Catalogo"   => App.GetService<CatalogoListPage>(),
            "Config"     => App.GetService<ConfiguracoesPage>(),
            _            => App.GetService<OrcamentosListPage>()
        };
        MainFrame.Navigate(page);
    }
}
