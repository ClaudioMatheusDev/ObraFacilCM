using Microsoft.EntityFrameworkCore.Storage;
using ObraFacil.Application.Interfaces;
using ObraFacil.Infrastructure.Data;

namespace ObraFacil.Infrastructure.Data;

/// <summary>
/// Implementação de <see cref="IUnitOfWork"/> usando o <see cref="AppDbContext"/> do EF Core.
/// Gerencia o ciclo de vida de uma <see cref="IDbContextTransaction"/> por instância.
/// </summary>
internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    private IDbContextTransaction? _tx;

    /// <summary>Inicializa a unidade de trabalho com o contexto de banco de dados fornecido.</summary>
    public UnitOfWork(AppDbContext db) => _db = db;

    /// <inheritdoc/>
    public async Task BeginAsync(CancellationToken ct = default)
        => _tx = await _db.Database.BeginTransactionAsync(ct);

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Lançado se nenhuma transação estiver ativa.</exception>
    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_tx is null) throw new InvalidOperationException("Nenhuma transação ativa.");
        await _tx.CommitAsync(ct);
        await _tx.DisposeAsync();
        _tx = null;
    }

    /// <inheritdoc/>
    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_tx is null) return;
        await _tx.RollbackAsync(ct);
        await _tx.DisposeAsync();
        _tx = null;
    }
}
