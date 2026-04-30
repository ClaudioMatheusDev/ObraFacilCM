using Microsoft.EntityFrameworkCore;
using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Enums;
using ObraFacil.Domain.Interfaces;
using ObraFacil.Infrastructure.Data;

namespace ObraFacil.Infrastructure.Repositories;

/// <summary>
/// Repositório concreto de <see cref="ItemCatalogo"/>, com busca filtrada por nome, categoria e tipo.
/// Somente itens ativos são retornados pela busca.
/// </summary>
public class ItemCatalogoRepository : RepositoryBase<ItemCatalogo>, IItemCatalogoRepository
{
    /// <summary>Inicializa o repositório com o contexto de banco de dados.</summary>
    public ItemCatalogoRepository(AppDbContext db) : base(db) { }

    /// <inheritdoc/>
    public async Task<IList<ItemCatalogo>> BuscarAsync(string termo, TipoItem? tipo = null, CancellationToken ct = default)
    {
        var q = _db.ItensCatalogo.Where(i => i.Ativo).AsQueryable();
        if (!string.IsNullOrWhiteSpace(termo))
            q = q.Where(i => i.Nome.Contains(termo) || (i.Categoria != null && i.Categoria.Contains(termo)));
        if (tipo.HasValue) q = q.Where(i => i.Tipo == tipo.Value);
        return await q.OrderBy(i => i.Nome).ToListAsync(ct);
    }
}
