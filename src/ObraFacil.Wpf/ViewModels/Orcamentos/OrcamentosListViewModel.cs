using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ObraFacil.Application.DTOs;
using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Enums;
using ObraFacil.Wpf.Services;
using ObraFacil.Wpf.Views.Orcamentos;
using System.Collections.ObjectModel;
using System.IO;

namespace ObraFacil.Wpf.ViewModels.Orcamentos;

/// <summary>
/// ViewModel da tela de listagem de orçamentos com suporte a filtragem por texto e status,
/// geração de PDF e exclusão com confirmação.
/// </summary>
public partial class OrcamentosListViewModel : ViewModelBase
{
    private readonly IOrcamentoService _service;
    private readonly IPdfService       _pdf;
    private readonly IWindowFactory    _windows;

    /// <summary>Texto de busca. Ao ser alterado, dispara novo carregamento da lista.</summary>
    [ObservableProperty] string?          _busca;

    /// <summary>Orçamento selecionado na grade.</summary>
    [ObservableProperty] OrcamentoDto?    _selecionado;

    private StatusOpcao? _statusOpcaoFiltro;

    /// <summary>
    /// Opção de status selecionada no filtro. Ao mudar, recarrega a lista automaticamente.
    /// </summary>
    public StatusOpcao? StatusFiltro
    {
        get => _statusOpcaoFiltro;
        set { if (SetProperty(ref _statusOpcaoFiltro, value)) CarregarCommand.Execute(null); }
    }

    /// <summary>Opções de status disponíveis no ComboBox de filtro, incluindo "Todos".</summary>
    public static IList<StatusOpcao> StatusOpcoes { get; } =
    [
        new(null, "Todos"),
        new(StatusOrcamento.Rascunho, "Rascunho"),
        new(StatusOrcamento.Enviado,  "Enviado"),
        new(StatusOrcamento.Aprovado, "Aprovado"),
        new(StatusOrcamento.Recusado, "Recusado"),
    ];

    /// <summary>Lista observável de orçamentos exibida na grade.</summary>
    public ObservableCollection<OrcamentoDto> Orcamentos { get; } = [];

    /// <summary>Inicializa o ViewModel com os serviços de orçamento, PDF e a fábrica de loggers.</summary>
    public OrcamentosListViewModel(IOrcamentoService service, IPdfService pdf,
        IWindowFactory windows, IDialogService dialogs, ILoggerFactory loggerFactory)
        : base(loggerFactory, dialogs)
    {
        _service = service; _pdf = pdf; _windows = windows;
        Title    = "Orçamentos";
    }

    /// <summary>Carrega a lista de orçamentos aplicando os filtros de busca e status ativos.</summary>
    [RelayCommand]
    public async Task CarregarAsync()
        => await ExecuteSafeAsync(async () =>
        {
            var lista = await _service.ListarAsync(Busca, StatusFiltro?.Valor);
            Orcamentos.Clear();
            foreach (var o in lista) Orcamentos.Add(o);
        });

    /// <summary>Abre o formulário para criação de um novo orçamento e recarrega a lista após fechar.</summary>
    [RelayCommand]
    void NovoOrcamento()
    {
        var win = _windows.CreateOrcamentoFormWindow();
        win.ShowDialog();
        CarregarCommand.Execute(null);
    }

    /// <summary>Abre o formulário de edição do orçamento selecionado e recarrega a lista após fechar.</summary>
    [RelayCommand]
    void EditarOrcamento(OrcamentoDto? orc)
    {
        if (orc is null) return;
        var win = _windows.CreateOrcamentoFormWindow();
        win.CarregarOrcamento(orc.Id);
        win.ShowDialog();
        CarregarCommand.Execute(null);
    }

    /// <summary>Gera o PDF do orçamento e o abre com o visualizador padrão do sistema.</summary>
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

    /// <summary>Cria um novo orçamento rascunho com base no orçamento informado.</summary>
    [RelayCommand]
    async Task DuplicarOrcamentoAsync(OrcamentoDto? orc)
    {
        if (orc is null) return;
        if (!Dialogs.Confirm($"Duplicar orçamento {orc.Numero}?")) return;

        await ExecuteSafeAsync(async () =>
        {
            var duplicado = await _service.DuplicarAsync(orc.Id);
            Dialogs.ShowInfo($"Orçamento {duplicado.Numero} criado como cópia.", "Orçamento duplicado");
        }, "Erro ao duplicar orçamento.");
        await CarregarAsync();
    }

    /// <summary>Solicita confirmação e exclui o orçamento informado.</summary>
    [RelayCommand]
    async Task ExcluirOrcamentoAsync(OrcamentoDto? orc)
    {
        if (orc is null) return;
        if (!Dialogs.Confirm($"Excluir orçamento {orc.Numero}?")) return;
        await ExecuteSafeAsync(() => _service.ExcluirAsync(orc.Id));
        await CarregarAsync();
    }

    partial void OnBuscaChanged(string? value) => CarregarCommand.Execute(null);
}

/// <summary>Wrapper para exibir StatusOrcamento no ComboBox com opção "Todos" (null).</summary>
/// <summary>Wrapper de <see cref="StatusOrcamento"/> para exibição em ComboBox com opção "Todos" (valor nulo).</summary>
/// <param name="Valor">Valor do enum, ou <c>null</c> para representar "Todos".</param>
/// <param name="Label">Rótulo exibido ao usuário.</param>
public record StatusOpcao(StatusOrcamento? Valor, string Label);
