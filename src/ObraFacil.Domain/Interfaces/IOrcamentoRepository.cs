using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Enums;
namespace ObraFacil.Domain.Interfaces;
public interface IOrcamentoRepository : IRepository<Orcamento>
{
    Task<Orcamento?> GetComItensAsync(int id, CancellationToken ct = default);
    Task<IList<Orcamento>> FiltrarAsync(
        string? termo = null, StatusOrcamento? status = null,
        DateTime? de = null, DateTime? ate = null, CancellationToken ct = default);
    Task<string> GerarProximoNumeroAsync(CancellationToken ct = default);
}
