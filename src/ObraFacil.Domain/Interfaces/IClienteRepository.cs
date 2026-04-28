using ObraFacil.Domain.Entities;
namespace ObraFacil.Domain.Interfaces;
public interface IClienteRepository : IRepository<Cliente>
{
    Task<IList<Cliente>> BuscarAsync(string termo, CancellationToken ct = default);
}
