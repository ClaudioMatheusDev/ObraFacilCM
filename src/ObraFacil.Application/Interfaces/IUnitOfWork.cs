namespace ObraFacil.Application.Interfaces;

/// <summary>
/// Abstração de unidade de trabalho para coordenar transações entre repositórios.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Inicia uma transação serializável no banco de dados.</summary>
    Task BeginAsync(CancellationToken ct = default);

    /// <summary>Confirma todas as operações da transação corrente.</summary>
    Task CommitAsync(CancellationToken ct = default);

    /// <summary>Desfaz todas as operações da transação corrente.</summary>
    Task RollbackAsync(CancellationToken ct = default);
}
