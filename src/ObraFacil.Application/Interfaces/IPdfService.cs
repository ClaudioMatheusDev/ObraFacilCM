namespace ObraFacil.Application.Interfaces;

/// <summary>
/// Serviço de geração de documentos PDF para orçamentos.
/// </summary>
public interface IPdfService
{
    /// <summary>
    /// Gera o PDF do orçamento especificado, incluindo cabeçalho da empresa, dados do cliente,
    /// tabela de itens e totais.
    /// </summary>
    /// <param name="orcamentoId">Identificador do orçamento.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Conteúdo binário do arquivo PDF gerado.</returns>
    /// <exception cref="System.Exception">Lançado se o orçamento não for encontrado.</exception>
    Task<byte[]> GerarOrcamentoPdfAsync(int orcamentoId, CancellationToken ct = default);
}
