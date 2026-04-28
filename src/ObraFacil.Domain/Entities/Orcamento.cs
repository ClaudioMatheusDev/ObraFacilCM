using ObraFacil.Domain.Enums;
namespace ObraFacil.Domain.Entities;
public class Orcamento : EntityBase
{
    public string          Numero             { get; set; } = string.Empty;
    public int             ClienteId          { get; set; }
    public Cliente         Cliente            { get; set; } = null!;
    public StatusOrcamento Status             { get; set; } = StatusOrcamento.Rascunho;
    public DateTime        DataEmissao        { get; set; } = DateTime.Today;
    public DateTime?       DataValidade       { get; set; }
    public decimal         Desconto           { get; set; } = 0;
    public string?         Observacoes        { get; set; }
    public string?         CondicoesPagamento { get; set; }
    public ICollection<ItemOrcamento> Itens   { get; set; } = [];

    public decimal Subtotal   => Itens.Sum(i => i.Subtotal);
    public decimal TotalFinal => Math.Max(0, Subtotal - Desconto);
}
