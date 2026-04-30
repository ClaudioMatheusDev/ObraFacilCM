using ObraFacil.Domain.Enums;
namespace ObraFacil.Domain.Entities;

/// <summary>
/// Linha de item em um orçamento com preço e descrição congelados (snapshot) no momento da inclusão,
/// garantindo imutabilidade histórica mesmo que o item do catálogo seja alterado posteriormente.
/// </summary>
public class ItemOrcamento : EntityBase
{
    /// <summary>Identificador do orçamento ao qual este item pertence.</summary>
    public int       OrcamentoId          { get; set; }

    /// <summary>Navegação para a entidade <see cref="Orcamento"/> pai.</summary>
    public Orcamento Orcamento            { get; set; } = null!;

    /// <summary>
    /// Referência histórica ao item de catálogo de origem. Pode ser <c>null</c>
    /// se o item foi informado manualmente ou o catálogo foi excluído.
    /// </summary>
    public int?      ItemCatalogoId       { get; set; }

    /// <summary>Descrição do item congelada no momento da inclusão no orçamento.</summary>
    public string        DescricaoSnapshot     { get; set; } = string.Empty;

    /// <summary>Unidade de medida congelada no momento da inclusão.</summary>
    public UnidadeMedida UnidadeSnapshot       { get; set; }

    /// <summary>Preço unitário congelado no momento da inclusão.</summary>
    public decimal       PrecoUnitarioSnapshot { get; set; }

    /// <summary>Categoria congelada no momento da inclusão. Opcional.</summary>
    public string?       CategoriaSnapshot     { get; set; }

    /// <summary>Quantidade do item neste orçamento.</summary>
    public decimal Quantidade   { get; set; } = 1;

    /// <summary>Desconto individual aplicado sobre este item.</summary>
    public decimal DescontoItem { get; set; } = 0;

    /// <summary>Subtotal do item: <c>PrecoUnitarioSnapshot × Quantidade − DescontoItem</c> (mínimo R$ 0,00).</summary>
    public decimal Subtotal => Math.Max(0, PrecoUnitarioSnapshot * Quantidade - DescontoItem);
}
