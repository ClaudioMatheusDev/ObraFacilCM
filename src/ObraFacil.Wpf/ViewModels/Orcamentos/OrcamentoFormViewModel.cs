using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ObraFacil.Application.DTOs;
using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Enums;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace ObraFacil.Wpf.ViewModels.Orcamentos;

/// <summary>
/// ViewModel do formulário de criação e edição de orçamentos.
/// Gerencia o cabeçalho (cliente, validade, desconto geral, condições, observações),
/// a coleção editável de itens (<see cref="ItemFormDto"/>) e os totais calculados
/// reativamente a partir das mudanças nos itens.
/// </summary>
public partial class OrcamentoFormViewModel : ViewModelBase
{
    private readonly IOrcamentoService    _orcamentos;
    private readonly IClienteService      _clientes;
    private readonly IItemCatalogoService _catalogo;

    /// <summary>Id do orçamento em edição, ou <c>null</c> quando se trata de um novo orçamento.</summary>
    private int? _orcamentoId;

    // ── Cabeçalho ────────────────────────────────────────────────────────
    /// <summary>Cliente vinculado ao orçamento.</summary>
    [ObservableProperty] ClienteDto?  _clienteSelecionado;
    /// <summary>Data de validade da proposta.</summary>
    [ObservableProperty] DateTime?    _dataValidade = DateTime.Today.AddDays(30);
    /// <summary>Desconto global aplicado sobre o subtotal (em reais).</summary>
    [ObservableProperty] decimal      _desconto;
    /// <summary>Condições de pagamento descritas em texto livre (opcional).</summary>
    [ObservableProperty] string?      _condicoesPagamento;
    /// <summary>Observações adicionais do orçamento (opcional).</summary>
    [ObservableProperty] string?      _observacoes;

    /// <summary>Lista de clientes disponíveis para seleção no ComboBox.</summary>
    public ObservableCollection<ClienteDto>      Clientes { get; } = [];
    /// <summary>Itens editáveis do orçamento exibidos na grade.</summary>
    public ObservableCollection<ItemFormDto>     Itens    { get; }

    /// <summary>Inicializa o ViewModel com os serviços necessários e registra os handlers de notificação dos itens.</summary>
    public OrcamentoFormViewModel(IOrcamentoService orcamentos,
        IClienteService clientes, IItemCatalogoService catalogo,
        ILoggerFactory loggerFactory) : base(loggerFactory)
    {
        _orcamentos = orcamentos;
        _clientes   = clientes;
        _catalogo   = catalogo;
        Title       = "Novo Orçamento";

        Itens = [];
        Itens.CollectionChanged += OnItensCollectionChanged;
    }

    /// <summary>
    /// Registra/desregistra o handler de propriedades em cada item adicionado/removido
    /// e recalcula os totais após qualquer mudança na coleção.
    /// </summary>
    private void OnItensCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
            foreach (ItemFormDto item in e.NewItems)
                item.PropertyChanged += OnItemPropertyChanged;

        if (e.OldItems is not null)
            foreach (ItemFormDto item in e.OldItems)
                item.PropertyChanged -= OnItemPropertyChanged;

        AtualizarTotais();
    }

    /// <summary>Recalcula os totais sempre que quantidade, preço unitário ou desconto de um item mudar.</summary>
    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ItemFormDto.Subtotal) or
            nameof(ItemFormDto.Quantidade) or
            nameof(ItemFormDto.PrecoUnitario) or
            nameof(ItemFormDto.DescontoItem))
            AtualizarTotais();
    }

    // ── Totais calculados ─────────────────────────────────────────────────
    /// <summary>Soma dos subtotais de todos os itens.</summary>
    public decimal Subtotal   => Itens.Sum(i => i.Subtotal);
    /// <summary>Total final após aplicar o desconto global. Nunca negativo.</summary>
    public decimal TotalFinal => Math.Max(0, Subtotal - Desconto);

    partial void OnDescontoChanged(decimal value) => AtualizarTotais();

    // ── Resultado para fechar a janela ────────────────────────────────────
    /// <summary>Disparado quando o orçamento é salvo com sucesso para que a View possa fechar a janela.</summary>
    public event Action? SalvoComSucesso;

    /// <summary>Carrega a lista de clientes disponíveis para o ComboBox.</summary>
    public async Task InicializarAsync()
    {
        await ExecuteSafeAsync(async () =>
        {
            var lista = await _clientes.ListarAsync();
            Clientes.Clear();
            foreach (var c in lista) Clientes.Add(c);
        });
    }

    /// <summary>
    /// Inicializa o formulário no modo de edição: carrega o orçamento pelo
    /// <paramref name="id"/>, populando todas as propriedades e a coleção de itens.
    /// </summary>
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

    /// <summary>Abre a janela de seleção do catálogo e adiciona o item escolhido à lista de itens.</summary>
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

    /// <summary>Adiciona um item avulso (sem vínculo com o catálogo) à lista de itens.</summary>
    [RelayCommand]
    void AdicionarAvulso()
    {
        Itens.Add(new ItemFormDto(null, "Novo item", UnidadeMedida.Unidade, 0, null, 1, 0));
        AtualizarTotais();
    }

    /// <summary>Remove o <paramref name="item"/> informado da lista de itens.</summary>
    [RelayCommand]
    void RemoverItem(ItemFormDto? item)
    {
        if (item is null) return;
        Itens.Remove(item);
        AtualizarTotais();
    }

    /// <summary>Valida e persiste o orçamento (criação ou atualização) e dispara <see cref="SalvoComSucesso"/>.</summary>
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

    /// <summary>Notifica a UI para reler <see cref="Subtotal"/> e <see cref="TotalFinal"/>.</summary>
    public void AtualizarTotais()
    {
        OnPropertyChanged(nameof(Subtotal));
        OnPropertyChanged(nameof(TotalFinal));
    }
}

// ── DTO mutável para edição na grade ─────────────────────────────────────────
/// <summary>
/// DTO observável que representa um item do orçamento dentro do formulário de edição.
/// Cada propriedade editável notifica a UI e o ViewModel pai recalcula os totais.
/// </summary>
public partial class ItemFormDto : ObservableObject
{
    /// <summary>Id do item do catálogo de origem, ou <c>null</c> para itens avulsos.</summary>
    public int?          ItemCatalogoId { get; }
    /// <summary>Descrição do item exibida na grade e impressa no PDF.</summary>
    [ObservableProperty] string        _descricao;
    /// <summary>Unidade de medida do item.</summary>
    [ObservableProperty] UnidadeMedida _unidade;
    /// <summary>Preço unitário do item em reais.</summary>
    [ObservableProperty] decimal       _precoUnitario;
    /// <summary>Categoria do item (opcional), para agrupamento no PDF.</summary>
    [ObservableProperty] string?       _categoria;
    /// <summary>Quantidade do item no orçamento.</summary>
    [ObservableProperty] decimal       _quantidade;
    /// <summary>Desconto aplicado apenas a este item (em reais).</summary>
    [ObservableProperty] decimal       _descontoItem;

    /// <summary>Subtotal calculado: <c>Quantidade × PrecoUnitario − DescontoItem</c>. Nunca negativo.</summary>
    public decimal Subtotal => Math.Max(0, Quantidade * PrecoUnitario - DescontoItem);

    // Notifica o item para recalcular Subtotal e propaga para os totais do VM
    partial void OnQuantidadeChanged(decimal value)    => OnPropertyChanged(nameof(Subtotal));
    partial void OnPrecoUnitarioChanged(decimal value)  => OnPropertyChanged(nameof(Subtotal));
    partial void OnDescontoItemChanged(decimal value)   => OnPropertyChanged(nameof(Subtotal));

    /// <summary>Inicializa o DTO com todos os campos necessários para a grade de edição.</summary>
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
