using ObraFacil.Domain.Enums;
namespace ObraFacil.Domain.Entities;
/// <summary>Snapshot do item — preço/descrição congelados no momento da adição.</summary>
public class ItemOrcamento : EntityBase
{
    public int       OrcamentoId          { get; set; }
    public Orcamento Orcamento            { get; set; } = null!;
    public int?      ItemCatalogoId       { get; set; }   // referência histórica, nullable

    // ── SNAPSHOT ──────────────────────────────────────────────────────────
    public string        DescricaoSnapshot     { get; set; } = string.Empty;
    public UnidadeMedida UnidadeSnapshot       { get; set; }
    public decimal       PrecoUnitarioSnapshot { get; set; }
    public string?       CategoriaSnapshot     { get; set; }

    // ── Quantidades e desconto por item ───────────────────────────────────
    public decimal Quantidade   { get; set; } = 1;
    public decimal DescontoItem { get; set; } = 0;

    public decimal Subtotal => Math.Max(0, PrecoUnitarioSnapshot * Quantidade - DescontoItem);
}
