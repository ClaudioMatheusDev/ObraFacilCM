using ObraFacil.Application.DTOs;
using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Enums;
using ObraFacil.Domain.Exceptions;
using ObraFacil.Domain.Interfaces;

namespace ObraFacil.Application.Services;

public class OrcamentoService : IOrcamentoService
{
    private readonly IOrcamentoRepository    _orcamentos;
    private readonly IClienteRepository     _clientes;
    private readonly IConfiguracaoRepository _config;

    public OrcamentoService(IOrcamentoRepository orcamentos,
        IClienteRepository clientes, IConfiguracaoRepository config)
    {
        _orcamentos = orcamentos; _clientes = clientes; _config = config;
    }

    public async Task<IList<OrcamentoDto>> ListarAsync(string? busca = null,
        StatusOrcamento? status = null, DateTime? de = null, DateTime? ate = null,
        CancellationToken ct = default)
        => (await _orcamentos.FiltrarAsync(busca, status, de, ate, ct)).Select(ToDto).ToList();

    public async Task<OrcamentoDto> ObterAsync(int id, CancellationToken ct = default)
        => ToDto(await _orcamentos.GetComItensAsync(id, ct) ?? throw new NotFoundException("Orçamento", id));

    public async Task<OrcamentoDto> CriarAsync(OrcamentoInputDto dto, CancellationToken ct = default)
    {
        _ = await _clientes.GetByIdAsync(dto.ClienteId, ct)
            ?? throw new NotFoundException("Cliente", dto.ClienteId);
        var cfg    = await _config.GetAsync(ct);
        var numero = await _orcamentos.GerarProximoNumeroAsync(ct);
        var orc = new Orcamento
        {
            Numero = numero, ClienteId = dto.ClienteId, DataEmissao = DateTime.Today,
            DataValidade = dto.DataValidade ?? DateTime.Today.AddDays(cfg.ValidadePadraoEmDias),
            Desconto = dto.Desconto, Observacoes = dto.Observacoes,
            CondicoesPagamento = dto.CondicoesPagamento,
            Itens = dto.Itens.Select(MapItem).ToList()
        };
        await _orcamentos.AddAsync(orc, ct);
        return await ObterAsync(orc.Id, ct);
    }

    public async Task<OrcamentoDto> AtualizarAsync(int id, OrcamentoInputDto dto, CancellationToken ct = default)
    {
        var orc = await _orcamentos.GetComItensAsync(id, ct)
            ?? throw new NotFoundException("Orçamento", id);
        if (orc.Status != StatusOrcamento.Rascunho)
            throw new ObraFacilException("Somente rascunhos podem ser editados.");
        orc.ClienteId = dto.ClienteId; orc.DataValidade = dto.DataValidade;
        orc.Desconto = dto.Desconto; orc.Observacoes = dto.Observacoes;
        orc.CondicoesPagamento = dto.CondicoesPagamento; orc.AlteradoEm = DateTime.UtcNow;
        orc.Itens.Clear();
        foreach (var i in dto.Itens) orc.Itens.Add(MapItem(i));
        await _orcamentos.UpdateAsync(orc, ct);
        return await ObterAsync(id, ct);
    }

    public async Task AlterarStatusAsync(int id, StatusOrcamento novoStatus, CancellationToken ct = default)
    {
        var orc = await _orcamentos.GetByIdAsync(id, ct) ?? throw new NotFoundException("Orçamento", id);
        orc.Status = novoStatus; orc.AlteradoEm = DateTime.UtcNow;
        await _orcamentos.UpdateAsync(orc, ct);
    }

    public Task ExcluirAsync(int id, CancellationToken ct = default) => _orcamentos.DeleteAsync(id, ct);

    // ── snapshot: preço/descrição congelados no momento da adição ─────────
    private static ItemOrcamento MapItem(ItemOrcamentoInputDto d) => new()
    {
        ItemCatalogoId = d.ItemCatalogoId,
        DescricaoSnapshot = d.Descricao, UnidadeSnapshot = d.Unidade,
        PrecoUnitarioSnapshot = d.PrecoUnitario, CategoriaSnapshot = d.Categoria,
        Quantidade = d.Quantidade, DescontoItem = d.DescontoItem
    };

    private static OrcamentoDto ToDto(Orcamento o) => new(
        o.Id, o.Numero, o.ClienteId, o.Cliente?.Nome ?? string.Empty,
        o.Status, o.DataEmissao, o.DataValidade, o.Subtotal, o.Desconto, o.TotalFinal,
        o.Observacoes, o.CondicoesPagamento,
        o.Itens.Select(i => new ItemOrcamentoDto(i.Id, i.ItemCatalogoId,
            i.DescricaoSnapshot, i.UnidadeSnapshot, i.PrecoUnitarioSnapshot,
            i.CategoriaSnapshot, i.Quantidade, i.DescontoItem, i.Subtotal)).ToList());
}
