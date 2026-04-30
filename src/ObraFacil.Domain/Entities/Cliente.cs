namespace ObraFacil.Domain.Entities;

/// <summary>
/// Representa um cliente cadastrado no sistema.
/// </summary>
public class Cliente : EntityBase
{
    /// <summary>Nome completo do cliente. Obrigatório.</summary>
    public string  Nome         { get; set; } = string.Empty;

    /// <summary>Número de telefone de contato. Opcional.</summary>
    public string? Telefone     { get; set; }

    /// <summary>Endereço de e-mail do cliente. Opcional.</summary>
    public string? Email        { get; set; }

    /// <summary>CPF ou CNPJ do cliente. Opcional.</summary>
    public string? Documento    { get; set; }

    /// <summary>Endereço completo do cliente. Opcional.</summary>
    public string? Endereco     { get; set; }

    /// <summary>Observações gerais sobre o cliente. Opcional.</summary>
    public string? Observacoes  { get; set; }

    /// <summary>Coleção de orçamentos associados a este cliente.</summary>
    public ICollection<Orcamento> Orcamentos { get; set; } = [];
}
