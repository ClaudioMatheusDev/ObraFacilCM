using Microsoft.EntityFrameworkCore;
using ObraFacil.Domain.Exceptions;
using ObraFacil.Domain.Interfaces;
using ObraFacil.Infrastructure.Data;

namespace ObraFacil.Infrastructure.Repositories;

/// <summary>
/// Implementação genérica de <see cref="IRepository{T}"/> usando EF Core.
/// Repositórios concretos devem herdar desta classe e sobrescrever os métodos necessários.
/// </summary>
/// <typeparam name="T">Tipo da entidade gerenciada.</typeparam>
public abstract class RepositoryBase<T> : IRepository<T> where T : class
{
    /// <summary>Contexto de banco de dados compartilhado pelo repositório.</summary>
    protected readonly AppDbContext _db;

    /// <summary>Inicializa o repositório com o contexto fornecido.</summary>
    protected RepositoryBase(AppDbContext db) => _db = db;

    /// <inheritdoc/>
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Set<T>().FindAsync([id], ct);

    /// <inheritdoc/>
    public virtual async Task<IList<T>> GetAllAsync(CancellationToken ct = default)
        => await _db.Set<T>().ToListAsync(ct);

    /// <inheritdoc/>
    public virtual async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        _db.Set<T>().Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    /// <inheritdoc/>
    public virtual async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _db.Set<T>().Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    /// <inheritdoc/>
    public virtual async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var e = await GetByIdAsync(id, ct) ?? throw new NotFoundException(typeof(T).Name, id);
        _db.Set<T>().Remove(e);
        await _db.SaveChangesAsync(ct);
    }
}
