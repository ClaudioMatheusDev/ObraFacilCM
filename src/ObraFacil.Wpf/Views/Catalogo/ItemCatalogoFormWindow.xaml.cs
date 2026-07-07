using ObraFacil.Application.DTOs;
using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Enums;
using ObraFacil.Wpf.Services;
using System.Windows;

namespace ObraFacil.Wpf.Views.Catalogo;

public partial class ItemCatalogoFormWindow : Window
{
    private readonly ItemCatalogoDto? _original;
    private readonly IItemCatalogoService _service;
    private readonly IDialogService _dialogs;

    public ItemCatalogoFormWindow(IItemCatalogoService service, IDialogService dialogs,
        ItemCatalogoDto? item = null)
    {
        InitializeComponent();
        _service  = service;
        _dialogs  = dialogs;
        _original = item;

        CboTipo.ItemsSource     = Enum.GetValues<TipoItem>().Cast<TipoItem>().ToList();
        CboUnidade.ItemsSource  = Enum.GetValues<UnidadeMedida>().Cast<UnidadeMedida>().ToList();

        if (item is not null)
        {
            TxtTitulo.Text      = "Editar Item";
            TxtNome.Text        = item.Nome;
            CboTipo.SelectedItem    = item.Tipo;
            CboUnidade.SelectedItem = item.Unidade;
            TxtPreco.Text       = item.PrecoUnitario.ToString("F2");
            TxtCategoria.Text   = item.Categoria ?? string.Empty;
            TxtDescricao.Text   = item.Descricao ?? string.Empty;
        }
        else
        {
            CboTipo.SelectedIndex    = 0;
            CboUnidade.SelectedIndex = 0;
        }
    }

    private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtNome.Text))
        {
            _dialogs.ShowWarning("Informe o nome do item.");
            return;
        }
        if (!decimal.TryParse(TxtPreco.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal preco) || preco < 0)
        {
            _dialogs.ShowWarning("Preço inválido.");
            return;
        }

        var dto = new ItemCatalogoInputDto(
            (TipoItem)CboTipo.SelectedItem!,
            TxtNome.Text.Trim(),
            string.IsNullOrWhiteSpace(TxtDescricao.Text) ? null : TxtDescricao.Text.Trim(),
            (UnidadeMedida)CboUnidade.SelectedItem!,
            preco,
            string.IsNullOrWhiteSpace(TxtCategoria.Text) ? null : TxtCategoria.Text.Trim());

        try
        {
            if (_original is null)
                await _service.CriarAsync(dto);
            else
                await _service.AtualizarAsync(_original.Id, dto);

            DialogResult = true;
        }
        catch (Exception ex)
        {
            _dialogs.ShowError($"Erro ao salvar: {ex.Message}");
        }
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
