namespace ObraFacil.Application.Interfaces;
public interface IPdfService
{
    Task<byte[]> GerarOrcamentoPdfAsync(int orcamentoId, CancellationToken ct = default);
}
