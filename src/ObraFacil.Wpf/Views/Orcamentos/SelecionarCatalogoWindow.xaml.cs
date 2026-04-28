using ObraFacil.Application.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ObraFacil.Wpf.Views.Orcamentos;

public partial class SelecionarCatalogoWindow : Window
{
    private readonly IList<ItemCatalogoDto> _todos;
    public ItemCatalogoDto? Selecionado { get; private set; }

    public SelecionarCatalogoWindow(IList<ItemCatalogoDto> itens)
    {
        InitializeComponent();
        _todos       = itens;
        Grid.ItemsSource = _todos;
    }

    private void TxtBusca_TextChanged(object sender, TextChangedEventArgs e)
    {
        var termo = TxtBusca.Text.Trim().ToLowerInvariant();
        Grid.ItemsSource = string.IsNullOrEmpty(termo)
            ? _todos
            : _todos.Where(i => i.Nome.Contains(termo, System.StringComparison.OrdinalIgnoreCase)
                             || (i.Categoria?.Contains(termo, System.StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();
    }

    private void BtnSelecionar_Click(object sender, RoutedEventArgs e) => Confirmar();
    private void Grid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) => Confirmar();

    private void Confirmar()
    {
        Selecionado  = Grid.SelectedItem as ItemCatalogoDto;
        if (Selecionado is null) return;
        DialogResult = true;
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
