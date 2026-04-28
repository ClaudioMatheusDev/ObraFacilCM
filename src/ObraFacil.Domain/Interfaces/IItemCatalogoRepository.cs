using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Enums;
namespace ObraFacil.Domain.Interfaces;
public interface IItemCatalogoRepository : IRepository<ItemCatalogo>
{
    Task<IList<ItemCatalogo>> BuscarAsync(string termo, TipoItem? tipo = null, CancellationToken ct = default);
}
