using ObraFacil.Domain.Enums;
namespace ObraFacil.Domain.Entities;

/// <summary>
/// Item do catálogo de produtos e serviços utilizado na composição de orçamentos.
/// </summary>
public class ItemCatalogo : EntityBase
{
    /// <summary>Classificação do item: Material ou Serviço.</summary>
    public TipoItem      Tipo          { get; set; }

    /// <summary>Nome do item. Obrigatório.</summary>
    public string        Nome          { get; set; } = string.Empty;

    /// <summary>Descrição detalhada do item. Opcional.</summary>
    public string?       Descricao     { get; set; }

    /// <summary>Unidade de medida padrão do item (ex.: Unidade, Metro, Hora).</summary>
    public UnidadeMedida Unidade       { get; set; }

    /// <summary>Preço unitário de referência do item.</summary>
    public decimal       PrecoUnitario { get; set; }

    /// <summary>Categoria de agrupamento do item no catálogo. Opcional.</summary>
    public string?       Categoria     { get; set; }

    /// <summary>Indica se o item está ativo e disponível para uso em novos orçamentos.</summary>
    public bool          Ativo         { get; set; } = true;
}
