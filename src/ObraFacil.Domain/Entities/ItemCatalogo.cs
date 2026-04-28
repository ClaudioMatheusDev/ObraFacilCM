using ObraFacil.Domain.Enums;
namespace ObraFacil.Domain.Entities;
public class ItemCatalogo : EntityBase
{
    public TipoItem      Tipo          { get; set; }
    public string        Nome          { get; set; } = string.Empty;
    public string?       Descricao     { get; set; }
    public UnidadeMedida Unidade       { get; set; }
    public decimal       PrecoUnitario { get; set; }
    public string?       Categoria     { get; set; }
    public bool          Ativo         { get; set; } = true;
}
