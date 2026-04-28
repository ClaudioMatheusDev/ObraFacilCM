using Microsoft.EntityFrameworkCore;
using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Interfaces;
using ObraFacil.Infrastructure.Data;

namespace ObraFacil.Infrastructure.Repositories;

public class ClienteRepository : RepositoryBase<Cliente>, IClienteRepository
{
    public ClienteRepository(AppDbContext db) : base(db) { }

    public async Task<IList<Cliente>> BuscarAsync(string termo, CancellationToken ct = default)
        => await _db.Clientes
            .Where(c => c.Nome.Contains(termo) || (c.Telefone != null && c.Telefone.Contains(termo)))
            .OrderBy(c => c.Nome).ToListAsync(ct);
}
