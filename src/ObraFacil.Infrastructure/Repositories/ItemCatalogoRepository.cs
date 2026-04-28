using Microsoft.EntityFrameworkCore;
using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Enums;
using ObraFacil.Domain.Interfaces;
using ObraFacil.Infrastructure.Data;

namespace ObraFacil.Infrastructure.Repositories;

public class ItemCatalogoRepository : RepositoryBase<ItemCatalogo>, IItemCatalogoRepository
{
    public ItemCatalogoRepository(AppDbContext db) : base(db) { }

    public async Task<IList<ItemCatalogo>> BuscarAsync(string termo, TipoItem? tipo = null, CancellationToken ct = default)
    {
        var q = _db.ItensCatalogo.Where(i => i.Ativo).AsQueryable();
        if (!string.IsNullOrWhiteSpace(termo))
            q = q.Where(i => i.Nome.Contains(termo) || (i.Categoria != null && i.Categoria.Contains(termo)));
        if (tipo.HasValue) q = q.Where(i => i.Tipo == tipo.Value);
        return await q.OrderBy(i => i.Nome).ToListAsync(ct);
    }
}
