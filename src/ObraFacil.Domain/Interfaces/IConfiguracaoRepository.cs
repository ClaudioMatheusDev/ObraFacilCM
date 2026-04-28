using ObraFacil.Domain.Entities;
namespace ObraFacil.Domain.Interfaces;
public interface IConfiguracaoRepository
{
    Task<Configuracao> GetAsync(CancellationToken ct = default);
    Task SalvarAsync(Configuracao config, CancellationToken ct = default);
}
