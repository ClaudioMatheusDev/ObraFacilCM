using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ObraFacil.Application.DTOs;
using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Enums;
using System.Collections.ObjectModel;
using System.Windows;

namespace ObraFacil.Wpf.ViewModels.Orcamentos;

public partial class OrcamentoFormViewModel : ViewModelBase
{
    private readonly IOrcamentoService    _orcamentos;
    private readonly IClienteService      _clientes;
    private readonly IItemCatalogoService _catalogo;

    private int? _orcamentoId;

    // ── Cabeçalho ────────────────────────────────────────────────────────
    [ObservableProperty] ClienteDto?  _clienteSelecionado;
    [ObservableProperty] DateTime?    _dataValidade = DateTime.Today.AddDays(30);
    [ObservableProperty] decimal      _desconto;
    [ObservableProperty] string?      _condicoesPagamento;
    [ObservableProperty] string?      _observacoes;

    public ObservableCollection<ClienteDto>      Clientes { get; } = [];
    public ObservableCollection<ItemFormDto>     Itens    { get; } = [];

    // ── Totais calculados ─────────────────────────────────────────────────
    public decimal Subtotal   => Itens.Sum(i => i.Subtotal);
    public decimal TotalFinal => Math.Max(0, Subtotal - Desconto);

    partial void OnDescontoChanged(decimal value) { OnPropertyChanged(nameof(TotalFinal)); }

    // ── Resultado para fechar a janela ────────────────────────────────────
    public event Action? SalvoComSucesso;

    public OrcamentoFormViewModel(IOrcamentoService orcamentos,
        IClienteService clientes, IItemCatalogoService catalogo)
    {
        _orcamentos = orcamentos;
        _clientes   = clientes;
        _catalogo   = catalogo;
        Title       = "Novo Orçamento";
    }

    public async Task InicializarAsync()
    {
        await ExecuteSafeAsync(async () =>
        {
            var lista = await _clientes.ListarAsync();
            Clientes.Clear();
            foreach (var c in lista) Clientes.Add(c);
        });
    }

    public async Task CarregarOrcamentoAsync(int id)
    {
        await InicializarAsync();
        await ExecuteSafeAsync(async () =>
        {
            _orcamentoId = id;
            Title        = $"Editar Orçamento";
            var orc      = await _orcamentos.ObterAsync(id);
            ClienteSelecionado  = Clientes.FirstOrDefault(c => c.Id == orc.ClienteId);
            DataValidade        = orc.DataValidade;
            Desconto            = orc.Desconto;
            CondicoesPagamento  = orc.CondicoesPagamento;
            Observacoes         = orc.Observacoes;
            Itens.Clear();
            foreach (var i in orc.Itens)
                Itens.Add(new ItemFormDto(i.ItemCatalogoId, i.Descricao, i.Unidade,
                    i.PrecoUnitario, i.Categoria, i.Quantidade, i.DescontoItem));
            AtualizarTotais();
        });
    }

    [RelayCommand]
    async Task AdicionarDoCatalogoAsync()
    {
        await ExecuteSafeAsync(async () =>
        {
            var itens = await _catalogo.ListarAsync();
            if (!itens.Any()) { MessageBox.Show("Catálogo vazio.", "Aviso"); return; }

            // Janela simples de seleção
            var win = new Views.Orcamentos.SelecionarCatalogoWindow(itens);
            if (win.ShowDialog() != true || win.Selecionado is null) return;
            var s = win.Selecionado;
            Itens.Add(new ItemFormDto(s.Id, s.Nome, s.Unidade, s.PrecoUnitario, s.Categoria, 1, 0));
            AtualizarTotais();
        });
    }

    [RelayCommand]
    void AdicionarAvulso()
    {
        Itens.Add(new ItemFormDto(null, "Novo item", UnidadeMedida.Unidade, 0, null, 1, 0));
        AtualizarTotais();
    }

    [RelayCommand]
    void RemoverItem(ItemFormDto? item)
    {
        if (item is null) return;
        Itens.Remove(item);
        AtualizarTotais();
    }

    [RelayCommand]
    async Task SalvarAsync()
    {
        if (ClienteSelecionado is null) { MessageBox.Show("Selecione um cliente.", "Atenção"); return; }
        if (!Itens.Any()) { MessageBox.Show("Adicione pelo menos um item.", "Atenção"); return; }

        await ExecuteSafeAsync(async () =>
        {
            var dto = new OrcamentoInputDto(
                ClienteSelecionado.Id, DataValidade, Desconto,
                Observacoes, CondicoesPagamento,
                Itens.Select(i => new ItemOrcamentoInputDto(
                    i.ItemCatalogoId, i.Descricao, i.Unidade,
                    i.PrecoUnitario, i.Categoria, i.Quantidade, i.DescontoItem)).ToList());

            if (_orcamentoId.HasValue)
                await _orcamentos.AtualizarAsync(_orcamentoId.Value, dto);
            else
                await _orcamentos.CriarAsync(dto);

            SalvoComSucesso?.Invoke();
        });
    }

    public void AtualizarTotais()
    {
        OnPropertyChanged(nameof(Subtotal));
        OnPropertyChanged(nameof(TotalFinal));
    }
}

// ── DTO mutável para edição na grade ─────────────────────────────────────────
public partial class ItemFormDto : ObservableObject
{
    public int?          ItemCatalogoId { get; }
    [ObservableProperty] string        _descricao;
    [ObservableProperty] UnidadeMedida _unidade;
    [ObservableProperty] decimal       _precoUnitario;
    [ObservableProperty] string?       _categoria;
    [ObservableProperty] decimal       _quantidade;
    [ObservableProperty] decimal       _descontoItem;

    public decimal Subtotal => Math.Max(0, Quantidade * PrecoUnitario - DescontoItem);

    public ItemFormDto(int? itemCatalogoId, string descricao, UnidadeMedida unidade,
        decimal precoUnitario, string? categoria, decimal quantidade, decimal descontoItem)
    {
        ItemCatalogoId = itemCatalogoId;
        _descricao     = descricao;
        _unidade       = unidade;
        _precoUnitario = precoUnitario;
        _categoria     = categoria;
        _quantidade    = quantidade;
        _descontoItem  = descontoItem;
    }
}
