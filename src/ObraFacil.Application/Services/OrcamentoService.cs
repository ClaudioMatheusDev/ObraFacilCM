using Microsoft.Extensions.Logging;
using ObraFacil.Application.DTOs;
using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Enums;
using ObraFacil.Domain.Exceptions;
using ObraFacil.Domain.Interfaces;

namespace ObraFacil.Application.Services;

/// <summary>
/// Implementação de <see cref="IOrcamentoService"/> com geração transacional de número sequencial,
/// congelamento de preços dos itens (snapshot) e validação de desconto contra subtotal.
/// </summary>
public class OrcamentoService : IOrcamentoService
{
    private readonly IOrcamentoRepository      _orcamentos;
    private readonly IClienteRepository        _clientes;
    private readonly IConfiguracaoRepository   _config;
    private readonly IUnitOfWork               _uow;
    private readonly ILogger<OrcamentoService> _logger;

    public OrcamentoService(IOrcamentoRepository orcamentos,
        IClienteRepository clientes, IConfiguracaoRepository config,
        IUnitOfWork uow, ILogger<OrcamentoService> logger)
    {
        _orcamentos = orcamentos; _clientes = clientes;
        _config = config; _uow = uow; _logger = logger;
    }

    public async Task<IList<OrcamentoDto>> ListarAsync(string? busca = null,
        StatusOrcamento? status = null, DateTime? de = null, DateTime? ate = null,
        CancellationToken ct = default)
        => (await _orcamentos.FiltrarAsync(busca, status, de, ate, ct)).Select(ToDto).ToList();

    public async Task<OrcamentoDto> ObterAsync(int id, CancellationToken ct = default)
        => ToDto(await _orcamentos.GetComItensAsync(id, ct) ?? throw new NotFoundException("Orcamento", id));

    public async Task<OrcamentoDto> CriarAsync(OrcamentoInputDto dto, CancellationToken ct = default)
    {
        ValidarOrcamento(dto);

        _ = await _clientes.GetByIdAsync(dto.ClienteId, ct)
            ?? throw new NotFoundException("Cliente", dto.ClienteId);

        await _uow.BeginAsync(ct);
        try
        {
            var cfg    = await _config.GetAsync(ct);
            var numero = await _orcamentos.GerarProximoNumeroAsync(ct);
            var orc = new Orcamento
            {
                Numero = numero, ClienteId = dto.ClienteId, DataEmissao = DateTime.Today,
                DataValidade = dto.DataValidade ?? DateTime.Today.AddDays(cfg.ValidadePadraoEmDias),
                Desconto = dto.Desconto, Observacoes = dto.Observacoes?.Trim(),
                CondicoesPagamento = dto.CondicoesPagamento?.Trim(),
                Itens = dto.Itens.Select(MapItem).ToList()
            };
            await _orcamentos.AddAsync(orc, ct);
            await _uow.CommitAsync(ct);
            _logger.LogInformation("Orcamento {Numero} criado (id={Id}).", numero, orc.Id);
            return await ObterAsync(orc.Id, ct);
        }
        catch
        {
            await _uow.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<OrcamentoDto> AtualizarAsync(int id, OrcamentoInputDto dto, CancellationToken ct = default)
    {
        ValidarOrcamento(dto);

        var orc = await _orcamentos.GetComItensAsync(id, ct)
            ?? throw new NotFoundException("Orcamento", id);
        if (orc.Status != StatusOrcamento.Rascunho)
            throw new ObraFacilException("Somente rascunhos podem ser editados.");
        orc.ClienteId = dto.ClienteId; orc.DataValidade = dto.DataValidade;
        orc.Desconto = dto.Desconto;
        orc.Observacoes = dto.Observacoes?.Trim();
        orc.CondicoesPagamento = dto.CondicoesPagamento?.Trim();
        orc.AlteradoEm = DateTime.UtcNow;
        orc.Itens.Clear();
        foreach (var i in dto.Itens) orc.Itens.Add(MapItem(i));
        await _orcamentos.UpdateAsync(orc, ct);
        _logger.LogInformation("Orcamento {Id} atualizado.", id);
        return await ObterAsync(id, ct);
    }

    public async Task AlterarStatusAsync(int id, StatusOrcamento novoStatus, CancellationToken ct = default)
    {
        var orc = await _orcamentos.GetByIdAsync(id, ct) ?? throw new NotFoundException("Orcamento", id);
        var anterior = orc.Status;
        orc.Status = novoStatus; orc.AlteradoEm = DateTime.UtcNow;
        await _orcamentos.UpdateAsync(orc, ct);
        _logger.LogInformation("Orcamento {Id}: status {Anterior} -> {Novo}.", id, anterior, novoStatus);
    }

    public async Task ExcluirAsync(int id, CancellationToken ct = default)
    {
        await _orcamentos.DeleteAsync(id, ct);
        _logger.LogInformation("Orcamento {Id} excluido.", id);
    }

    // snapshot: preco/descricao congelados no momento da adicao
    private static ItemOrcamento MapItem(ItemOrcamentoInputDto d) => new()
    {
        ItemCatalogoId = d.ItemCatalogoId,
        DescricaoSnapshot = d.Descricao, UnidadeSnapshot = d.Unidade,
        PrecoUnitarioSnapshot = d.PrecoUnitario, CategoriaSnapshot = d.Categoria,
        Quantidade = d.Quantidade, DescontoItem = d.DescontoItem
    };

    private static void ValidarOrcamento(OrcamentoInputDto dto)
    {
        DtoValidator.Validar(dto);
        foreach (var item in dto.Itens) DtoValidator.Validar(item);
        var subtotal = dto.Itens.Sum(i => Math.Max(0, i.PrecoUnitario * i.Quantidade - i.DescontoItem));
        if (dto.Desconto > subtotal)
            throw new ObraFacilException("Desconto nao pode ser maior que o subtotal dos itens.");
    }

    private static OrcamentoDto ToDto(Orcamento o) => new(
        o.Id, o.Numero, o.ClienteId, o.Cliente?.Nome ?? string.Empty,
        o.Status, o.DataEmissao, o.DataValidade, o.Subtotal, o.Desconto, o.TotalFinal,
        o.Observacoes, o.CondicoesPagamento,
        o.Itens.Select(i => new ItemOrcamentoDto(i.Id, i.ItemCatalogoId,
            i.DescricaoSnapshot, i.UnidadeSnapshot, i.PrecoUnitarioSnapshot,
            i.CategoriaSnapshot, i.Quantidade, i.DescontoItem, i.Subtotal)).ToList());
}
