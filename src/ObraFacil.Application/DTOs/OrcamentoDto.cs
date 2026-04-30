using ObraFacil.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ObraFacil.Application.DTOs;

/// <summary>Representa os dados de leitura de um orçamento completo, incluindo seus itens.</summary>
/// <param name="Id">Identificador único do orçamento.</param>
/// <param name="Numero">Número sequencial do orçamento no formato <c>AAAA-NNNN</c>.</param>
/// <param name="ClienteId">Identificador do cliente.</param>
/// <param name="ClienteNome">Nome do cliente.</param>
/// <param name="Status">Status atual do orçamento.</param>
/// <param name="DataEmissao">Data de emissão do orçamento.</param>
/// <param name="DataValidade">Data de validade. Nulo indica validade indefinida.</param>
/// <param name="Subtotal">Soma dos subtotais dos itens sem desconto global.</param>
/// <param name="Desconto">Desconto global aplicado sobre o subtotal.</param>
/// <param name="TotalFinal">Valor final após aplicação do desconto.</param>
/// <param name="Observacoes">Observações para o cliente. Opcional.</param>
/// <param name="CondicoesPagamento">Condições de pagamento acordadas. Opcional.</param>
/// <param name="Itens">Lista de itens que compõem o orçamento.</param>
public record OrcamentoDto(
    int Id, string Numero, int ClienteId, string ClienteNome,
    StatusOrcamento Status, DateTime DataEmissao, DateTime? DataValidade,
    decimal Subtotal, decimal Desconto, decimal TotalFinal,
    string? Observacoes, string? CondicoesPagamento,
    IList<ItemOrcamentoDto> Itens);

/// <summary>Representa os dados de leitura de um item pertencente a um orçamento.</summary>
/// <param name="Id">Identificador do item no orçamento.</param>
/// <param name="ItemCatalogoId">Referência histórica ao item de catálogo de origem. Opcional.</param>
/// <param name="Descricao">Descrição congelada do item.</param>
/// <param name="Unidade">Unidade de medida congelada.</param>
/// <param name="PrecoUnitario">Preço unitário congelado.</param>
/// <param name="Categoria">Categoria congelada. Opcional.</param>
/// <param name="Quantidade">Quantidade do item no orçamento.</param>
/// <param name="DescontoItem">Desconto individual aplicado sobre este item.</param>
/// <param name="Subtotal">Subtotal calculado deste item.</param>
public record ItemOrcamentoDto(
    int Id, int? ItemCatalogoId, string Descricao,
    UnidadeMedida Unidade, decimal PrecoUnitario, string? Categoria,
    decimal Quantidade, decimal DescontoItem, decimal Subtotal);

/// <summary>Dados de entrada para criação ou atualização de um orçamento.</summary>
/// <param name="ClienteId">Identificador do cliente. Obrigatório e deve ser maior que zero.</param>
/// <param name="DataValidade">Data de validade do orçamento. Opcional.</param>
/// <param name="Desconto">Desconto global sobre o subtotal. Deve ser maior ou igual a zero.</param>
/// <param name="Observacoes">Observações para o cliente. Opcional, máximo de 1000 caracteres.</param>
/// <param name="CondicoesPagamento">Condições de pagamento. Opcional, máximo de 500 caracteres.</param>
/// <param name="Itens">Lista de itens do orçamento. Deve conter ao menos um item.</param>
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

/// <summary>Dados de entrada para um item de orçamento.</summary>
/// <param name="ItemCatalogoId">Referência ao item de catálogo de origem. Opcional.</param>
/// <param name="Descricao">Descrição do item. Obrigatória, máximo de 300 caracteres.</param>
/// <param name="Unidade">Unidade de medida do item.</param>
/// <param name="PrecoUnitario">Preço unitário. Deve ser maior que zero.</param>
/// <param name="Categoria">Categoria do item. Opcional, máximo de 100 caracteres.</param>
/// <param name="Quantidade">Quantidade do item. Deve ser maior que zero.</param>
/// <param name="DescontoItem">Desconto individual. Deve ser maior ou igual a zero.</param>
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
