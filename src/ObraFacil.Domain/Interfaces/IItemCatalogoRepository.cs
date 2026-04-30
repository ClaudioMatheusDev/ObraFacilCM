using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Enums;
namespace ObraFacil.Domain.Interfaces;

/// <summary>
/// Repositório específico de <see cref="ItemCatalogo"/>, com suporte a busca filtrada.
/// </summary>
public interface IItemCatalogoRepository : IRepository<ItemCatalogo>
{
    /// <summary>
    /// Busca itens ativos do catálogo pelo nome ou categoria, com filtro opcional por tipo.
    /// </summary>
    /// <param name="termo">Texto para filtrar por nome ou categoria.</param>
    /// <param name="tipo">Tipo do item para filtrar (<see cref="TipoItem.Material"/> ou <see cref="TipoItem.Servico"/>). Nulo retorna todos.</param>
    /// <param name="ct">Token de cancelamento.</param>
    Task<IList<ItemCatalogo>> BuscarAsync(string termo, TipoItem? tipo = null, CancellationToken ct = default);
}
