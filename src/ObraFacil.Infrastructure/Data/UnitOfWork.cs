using Microsoft.EntityFrameworkCore.Storage;
using ObraFacil.Application.Interfaces;
using ObraFacil.Infrastructure.Data;

namespace ObraFacil.Infrastructure.Data;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    private IDbContextTransaction? _tx;

    public UnitOfWork(AppDbContext db) => _db = db;

    public async Task BeginAsync(CancellationToken ct = default)
        => _tx = await _db.Database.BeginTransactionAsync(ct);

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_tx is null) throw new InvalidOperationException("Nenhuma transação ativa.");
        await _tx.CommitAsync(ct);
        await _tx.DisposeAsync();
        _tx = null;
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_tx is null) return;
        await _tx.RollbackAsync(ct);
        await _tx.DisposeAsync();
        _tx = null;
    }
}
