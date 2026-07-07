using ObraFacil.Wpf.Views.Clientes;
using ObraFacil.Wpf.Views.Orcamentos;
using ObraFacil.Wpf.Views.Catalogo;
using ObraFacil.Wpf.Views.Configuracoes;
using System.Windows;
using System.Windows.Controls;

namespace ObraFacil.Wpf.Views;

public partial class MainWindow : Window
{
    private readonly OrcamentosListPage _orcamentos;
    private readonly ClientesListPage _clientes;
    private readonly CatalogoListPage _catalogo;
    private readonly ConfiguracoesPage _configuracoes;

    public MainWindow(OrcamentosListPage orcamentos, ClientesListPage clientes,
        CatalogoListPage catalogo, ConfiguracoesPage configuracoes)
    {
        InitializeComponent();
        _orcamentos     = orcamentos;
        _clientes        = clientes;
        _catalogo        = catalogo;
        _configuracoes   = configuracoes;
        Loaded += (_, _) => Navegar("Orcamentos");
    }

    private void NavClick(object sender, RoutedEventArgs e)
        => Navegar(((Button)sender).Tag.ToString()!);

    private void Navegar(string destino)
    {
        Page page = destino switch
        {
            "Orcamentos" => _orcamentos,
            "Clientes"   => _clientes,
            "Catalogo"   => _catalogo,
            "Config"     => _configuracoes,
            _            => _orcamentos
        };
        MainFrame.Navigate(page);
    }
}
