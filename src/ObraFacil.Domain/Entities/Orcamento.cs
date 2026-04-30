using ObraFacil.Domain.Enums;
namespace ObraFacil.Domain.Entities;

/// <summary>
/// Representa um orçamento emitido para um cliente, composto por itens com preços congelados no momento da inclusão.
/// </summary>
public class Orcamento : EntityBase
{
    /// <summary>Número único do orçamento no formato <c>AAAA-NNNN</c> (ex.: 2026-0001).</summary>
    public string          Numero             { get; set; } = string.Empty;

    /// <summary>Identificador do cliente ao qual o orçamento pertence.</summary>
    public int             ClienteId          { get; set; }

    /// <summary>Navegação para a entidade <see cref="Cliente"/> associada.</summary>
    public Cliente         Cliente            { get; set; } = null!;

    /// <summary>Status atual do orçamento no fluxo de trabalho.</summary>
    public StatusOrcamento Status             { get; set; } = StatusOrcamento.Rascunho;

    /// <summary>Data de emissão do orçamento.</summary>
    public DateTime        DataEmissao        { get; set; } = DateTime.Today;

    /// <summary>Data de validade do orçamento. Nulo indica validade indefinida.</summary>
    public DateTime?       DataValidade       { get; set; }

    /// <summary>Desconto global aplicado sobre o subtotal dos itens.</summary>
    public decimal         Desconto           { get; set; } = 0;

    /// <summary>Observações adicionais para o cliente. Opcional.</summary>
    public string?         Observacoes        { get; set; }

    /// <summary>Condições e formas de pagamento acordadas. Opcional.</summary>
    public string?         CondicoesPagamento { get; set; }

    /// <summary>Itens que compõem o orçamento, com preços e descrições congelados.</summary>
    public ICollection<ItemOrcamento> Itens   { get; set; } = [];

    /// <summary>Soma dos subtotais de todos os itens, sem descontos globais.</summary>
    public decimal Subtotal   => Itens.Sum(i => i.Subtotal);

    /// <summary>Valor final do orçamento após aplicação do desconto global (mínimo R$ 0,00).</summary>
    public decimal TotalFinal => Math.Max(0, Subtotal - Desconto);
}
