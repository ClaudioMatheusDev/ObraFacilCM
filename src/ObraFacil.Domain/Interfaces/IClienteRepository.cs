using ObraFacil.Domain.Entities;
namespace ObraFacil.Domain.Interfaces;

/// <summary>
/// Repositório específico de <see cref="Cliente"/>, estendendo as operações genéricas com busca textual.
/// </summary>
public interface IClienteRepository : IRepository<Cliente>
{
    /// <summary>
    /// Busca clientes cujo nome ou telefone contenha o <paramref name="termo"/> informado.
    /// </summary>
    /// <param name="termo">Texto para filtrar clientes.</param>
    /// <param name="ct">Token de cancelamento.</param>
    Task<IList<Cliente>> BuscarAsync(string termo, CancellationToken ct = default);
}
