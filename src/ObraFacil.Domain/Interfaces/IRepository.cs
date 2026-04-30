namespace ObraFacil.Domain.Interfaces;

/// <summary>
/// Contrato genérico de repositório para operações básicas de persistência (CRUD).
/// </summary>
/// <typeparam name="T">Tipo da entidade gerenciada pelo repositório.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>Retorna a entidade pelo seu identificador, ou <c>null</c> se não encontrada.</summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="ct">Token de cancelamento.</param>
    Task<T?>       GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Retorna todas as entidades persistidas.</summary>
    /// <param name="ct">Token de cancelamento.</param>
    Task<IList<T>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Persiste uma nova entidade e a retorna com o <c>Id</c> gerado.</summary>
    /// <param name="entity">Entidade a ser inserida.</param>
    /// <param name="ct">Token de cancelamento.</param>
    Task<T>        AddAsync(T entity, CancellationToken ct = default);

    /// <summary>Atualiza os dados de uma entidade já existente.</summary>
    /// <param name="entity">Entidade com os novos valores.</param>
    /// <param name="ct">Token de cancelamento.</param>
    Task           UpdateAsync(T entity, CancellationToken ct = default);

    /// <summary>Remove a entidade pelo seu identificador.</summary>
    /// <param name="id">Identificador da entidade a ser excluída.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <exception cref="ObraFacil.Domain.Exceptions.NotFoundException">Lançado se a entidade não existir.</exception>
    Task           DeleteAsync(int id, CancellationToken ct = default);
}
