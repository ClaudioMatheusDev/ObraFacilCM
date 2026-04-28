namespace ObraFacil.Domain.Entities;
public class Cliente : EntityBase
{
    public string  Nome         { get; set; } = string.Empty;
    public string? Telefone     { get; set; }
    public string? Email        { get; set; }
    public string? Documento    { get; set; }
    public string? Endereco     { get; set; }
    public string? Observacoes  { get; set; }
    public ICollection<Orcamento> Orcamentos { get; set; } = [];
}
