using ObraFacil.Domain.Enums;
using System.ComponentModel.DataAnnotations;

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
    [property: Range(1, int.MaxValue, ErrorMessage = "Cliente inválido.")]
    int ClienteId,

    DateTime? DataValidade,

    [property: Range(0, double.MaxValue, ErrorMessage = "Desconto não pode ser negativo.")]
    decimal Desconto,

    [property: StringLength(1000, ErrorMessage = "Observações podem ter no máximo 1000 caracteres.")]
    string? Observacoes,

    [property: StringLength(500, ErrorMessage = "Condições de pagamento podem ter no máximo 500 caracteres.")]
    string? CondicoesPagamento,

    [property: Required(ErrorMessage = "O orçamento deve ter ao menos um item.")]
    [property: MinLength(1, ErrorMessage = "O orçamento deve ter ao menos um item.")]
    IList<ItemOrcamentoInputDto> Itens);

public record ItemOrcamentoInputDto(
    int? ItemCatalogoId,

    [property: Required(ErrorMessage = "Descrição do item é obrigatória.")]
    [property: StringLength(300, ErrorMessage = "Descrição pode ter no máximo 300 caracteres.")]
    string Descricao,

    UnidadeMedida Unidade,

    [property: Range(0.01, double.MaxValue, ErrorMessage = "Preço unitário deve ser maior que zero.")]
    decimal PrecoUnitario,

    [property: StringLength(100, ErrorMessage = "Categoria pode ter no máximo 100 caracteres.")]
    string? Categoria,

    [property: Range(0.001, double.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero.")]
    decimal Quantidade,

    [property: Range(0, double.MaxValue, ErrorMessage = "Desconto do item não pode ser negativo.")]
    decimal DescontoItem);
