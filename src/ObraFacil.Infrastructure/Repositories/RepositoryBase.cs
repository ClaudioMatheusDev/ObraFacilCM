using Microsoft.EntityFrameworkCore;
using ObraFacil.Domain.Exceptions;
using ObraFacil.Domain.Interfaces;
using ObraFacil.Infrastructure.Data;

namespace ObraFacil.Infrastructure.Repositories;

public abstract class RepositoryBase<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _db;
    protected RepositoryBase(AppDbContext db) => _db = db;

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Set<T>().FindAsync([id], ct);

    public virtual async Task<IList<T>> GetAllAsync(CancellationToken ct = default)
        => await _db.Set<T>().ToListAsync(ct);

    public virtual async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        _db.Set<T>().Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _db.Set<T>().Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var e = await GetByIdAsync(id, ct) ?? throw new NotFoundException(typeof(T).Name, id);
        _db.Set<T>().Remove(e);
        await _db.SaveChangesAsync(ct);
    }
}
