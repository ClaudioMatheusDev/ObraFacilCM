namespace ObraFacil.Domain.Entities;

/// <summary>
/// Configurações globais da empresa. Existe sempre uma única linha com <c>Id = 1</c>.
/// </summary>
public class Configuracao
{
    /// <summary>Identificador fixo da configuração. Valor sempre igual a <c>1</c>.</summary>
    public int     Id                      { get; set; } = 1;

    /// <summary>Razão social ou nome fantasia da empresa. Exibido no cabeçalho dos PDFs.</summary>
    public string  NomeEmpresa             { get; set; } = string.Empty;

    /// <summary>Telefone de contato da empresa. Opcional.</summary>
    public string? TelefoneEmpresa         { get; set; }

    /// <summary>E-mail de contato da empresa. Opcional.</summary>
    public string? EmailEmpresa            { get; set; }

    /// <summary>Endereço físico da empresa. Opcional.</summary>
    public string? EnderecoEmpresa         { get; set; }

    /// <summary>Logo da empresa codificada em Base64 para uso nos PDFs. Opcional.</summary>
    public string? LogoBase64              { get; set; }

    /// <summary>Sequencial utilizado na geração do próximo número de orçamento.</summary>
    public int     ProximoNumeroOrcamento  { get; set; } = 1;

    /// <summary>Quantidade de dias de validade padrão aplicada ao criar novos orçamentos.</summary>
    public int     ValidadePadraoEmDias    { get; set; } = 15;
}
