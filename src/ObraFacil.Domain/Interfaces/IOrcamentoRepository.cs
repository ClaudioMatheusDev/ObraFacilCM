using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Enums;
namespace ObraFacil.Domain.Interfaces;

/// <summary>
/// Repositório específico de <see cref="Orcamento"/>, com carregamento de itens,
/// filtragem avançada e geração de número sequencial.
/// </summary>
public interface IOrcamentoRepository : IRepository<Orcamento>
{
    /// <summary>
    /// Retorna o orçamento pelo id com seus itens e cliente carregados,
    /// ou <c>null</c> se não encontrado.
    /// </summary>
    /// <param name="id">Identificador do orçamento.</param>
    /// <param name="ct">Token de cancelamento.</param>
    Task<Orcamento?> GetComItensAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Filtra orçamentos combinando texto livre, status e período de emissão.
    /// Todos os parâmetros são opcionais; sem filtros retorna todos os orçamentos.
    /// </summary>
    /// <param name="termo">Texto para filtrar por número ou nome do cliente.</param>
    /// <param name="status">Status do orçamento. Nulo retorna todos os status.</param>
    /// <param name="de">Data de emissão inicial do período.</param>
    /// <param name="ate">Data de emissão final do período.</param>
    /// <param name="ct">Token de cancelamento.</param>
    Task<IList<Orcamento>> FiltrarAsync(
        string? termo = null, StatusOrcamento? status = null,
        DateTime? de = null, DateTime? ate = null, CancellationToken ct = default);

    /// <summary>
    /// Gera e reserva atomicamente o próximo número de orçamento no formato <c>AAAA-NNNN</c>,
    /// incrementando o contador na tabela de configurações.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    Task<string> GerarProximoNumeroAsync(CancellationToken ct = default);
}
