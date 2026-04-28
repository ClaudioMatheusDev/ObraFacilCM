using ObraFacil.Domain.Enums;
namespace ObraFacil.Application.DTOs;

public record OrcamentoDto(
    int Id, string Numero, int ClienteId, string ClienteNome,
    StatusOrcamento Status, DateTime DataEmissao, DateTime? DataValidade,
    decimal Subtotal, decimal Desconto, decimal TotalFinal,
    string? Observacoes, string? CondicoesPagamento,
    IList<ItemOrcamentoDto> Itens);

public record ItemOrcamentoDto(
    int Id, int? ItemCatalogoId, string Descricao,
    UnidadeMedida Unidade, decimal PrecoUnitario, string? Categoria,
    decimal Quantidade, decimal DescontoItem, decimal Subtotal);

public record OrcamentoInputDto(
    int ClienteId, DateTime? DataValidade, decimal Desconto,
    string? Observacoes, string? CondicoesPagamento,
    IList<ItemOrcamentoInputDto> Itens);

public record ItemOrcamentoInputDto(
    int? ItemCatalogoId, string Descricao, UnidadeMedida Unidade,
    decimal PrecoUnitario, string? Categoria, decimal Quantidade, decimal DescontoItem);
