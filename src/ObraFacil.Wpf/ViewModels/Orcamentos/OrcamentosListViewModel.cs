using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ObraFacil.Application.DTOs;
using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Enums;
using ObraFacil.Wpf.Views.Orcamentos;
using System.Collections.ObjectModel;
using System.IO;

namespace ObraFacil.Wpf.ViewModels.Orcamentos;

public partial class OrcamentosListViewModel : ViewModelBase
{
    private readonly IOrcamentoService _service;
    private readonly IPdfService       _pdf;

    [ObservableProperty] string?          _busca;
    [ObservableProperty] OrcamentoDto?    _selecionado;

    // Filtro de status usando wrapper para o ComboBox
    private StatusOpcao? _statusOpcaoFiltro;
    public StatusOpcao? StatusFiltro
    {
        get => _statusOpcaoFiltro;
        set { if (SetProperty(ref _statusOpcaoFiltro, value)) CarregarCommand.Execute(null); }
    }

    public static IList<StatusOpcao> StatusOpcoes { get; } =
    [
        new(null, "Todos"),
        new(StatusOrcamento.Rascunho, "Rascunho"),
        new(StatusOrcamento.Enviado,  "Enviado"),
        new(StatusOrcamento.Aprovado, "Aprovado"),
        new(StatusOrcamento.Recusado, "Recusado"),
    ];

    public ObservableCollection<OrcamentoDto> Orcamentos { get; } = [];

    public OrcamentosListViewModel(IOrcamentoService service, IPdfService pdf)
    {
        _service = service; _pdf = pdf;
        Title    = "Orçamentos";
    }

    [RelayCommand]
    public async Task CarregarAsync()
        => await ExecuteSafeAsync(async () =>
        {
            var lista = await _service.ListarAsync(Busca, StatusFiltro?.Valor);
            Orcamentos.Clear();
            foreach (var o in lista) Orcamentos.Add(o);
        });

    [RelayCommand]
    void NovoOrcamento()
    {
        var win = App.GetService<OrcamentoFormWindow>();
        win.ShowDialog();
        CarregarCommand.Execute(null);
    }

    [RelayCommand]
    void EditarOrcamento(OrcamentoDto? orc)
    {
        if (orc is null) return;
        var win = App.GetService<OrcamentoFormWindow>();
        win.CarregarOrcamento(orc.Id);
        win.ShowDialog();
        CarregarCommand.Execute(null);
    }

    [RelayCommand]
    async Task GerarPdfAsync(OrcamentoDto? orc)
    {
        if (orc is null) return;
        await ExecuteSafeAsync(async () =>
        {
            var bytes = await _pdf.GerarOrcamentoPdfAsync(orc.Id);
            var path  = Path.Combine(Path.GetTempPath(), $"orcamento-{orc.Numero}.pdf");
            await File.WriteAllBytesAsync(path, bytes);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(path) { UseShellExecute = true });
        }, "Erro ao gerar PDF.");
    }

    [RelayCommand]
    async Task ExcluirOrcamentoAsync(OrcamentoDto? orc)
    {
        if (orc is null) return;
        var r = System.Windows.MessageBox.Show(
            $"Excluir orçamento {orc.Numero}?", "Confirmar",
            System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
        if (r != System.Windows.MessageBoxResult.Yes) return;
        await ExecuteSafeAsync(() => _service.ExcluirAsync(orc.Id));
        await CarregarAsync();
    }

    partial void OnBuscaChanged(string? value) => CarregarCommand.Execute(null);
}

/// <summary>Wrapper para exibir StatusOrcamento no ComboBox com opção "Todos" (null).</summary>
public record StatusOpcao(StatusOrcamento? Valor, string Label);
