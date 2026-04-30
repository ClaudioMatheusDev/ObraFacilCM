using Microsoft.EntityFrameworkCore;
using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Interfaces;
using ObraFacil.Infrastructure.Data;

namespace ObraFacil.Infrastructure.Repositories;

/// <summary>
/// Repositório concreto de <see cref="Cliente"/>, adicionando busca filtrada por nome ou telefone.
/// </summary>
public class ClienteRepository : RepositoryBase<Cliente>, IClienteRepository
{
    /// <summary>Inicializa o repositório com o contexto de banco de dados.</summary>
    public ClienteRepository(AppDbContext db) : base(db) { }

    /// <inheritdoc/>
    public async Task<IList<Cliente>> BuscarAsync(string termo, CancellationToken ct = default)
        => await _db.Clientes
            .Where(c => c.Nome.Contains(termo) || (c.Telefone != null && c.Telefone.Contains(termo)))
            .OrderBy(c => c.Nome).ToListAsync(ct);
}
