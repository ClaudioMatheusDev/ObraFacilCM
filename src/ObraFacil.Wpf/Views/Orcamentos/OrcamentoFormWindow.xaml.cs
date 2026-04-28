using ObraFacil.Wpf.ViewModels.Orcamentos;
using System.Windows;

namespace ObraFacil.Wpf.Views.Orcamentos;

public partial class OrcamentoFormWindow : Window
{
    private readonly OrcamentoFormViewModel _vm;

    public OrcamentoFormWindow(OrcamentoFormViewModel vm)
    {
        InitializeComponent();
        _vm         = vm;
        DataContext = vm;
        vm.SalvoComSucesso += () => { DialogResult = true; };
    }

    public async void CarregarOrcamento(int id)
    {
        await _vm.CarregarOrcamentoAsync(id);
    }

    protected override async void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        if (_vm.Clientes.Count == 0)
            await _vm.InicializarAsync();
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
