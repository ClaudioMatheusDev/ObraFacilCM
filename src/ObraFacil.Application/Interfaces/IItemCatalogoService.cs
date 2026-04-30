using ObraFacil.Application.DTOs;
using ObraFacil.Domain.Enums;
namespace ObraFacil.Application.Interfaces;

/// <summary>
/// Serviço de aplicação para gerenciamento do catálogo de materiais e serviços.
/// </summary>
public interface IItemCatalogoService
{
    /// <summary>Lista itens ativos do catálogo com filtragem opcional por texto e tipo.</summary>
    /// <param name="busca">Texto para filtrar por nome ou categoria.</param>
    /// <param name="tipo">Tipo do item (<see cref="TipoItem.Material"/> ou <see cref="TipoItem.Servico"/>). Nulo retorna todos.</param>
    /// <param name="ct">Token de cancelamento.</param>
    Task<IList<ItemCatalogoDto>> ListarAsync(string? busca = null, TipoItem? tipo = null, CancellationToken ct = default);

    /// <summary>Retorna um item do catálogo pelo identificador.</summary>
    /// <param name="id">Identificador do item.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <exception cref="ObraFacil.Domain.Exceptions.NotFoundException">Lançado se o item não existir.</exception>
    Task<ItemCatalogoDto>        ObterAsync(int id, CancellationToken ct = default);

    /// <summary>Cria um novo item no catálogo após validar o DTO de entrada.</summary>
    /// <param name="dto">Dados do novo item.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <exception cref="ObraFacil.Domain.Exceptions.ObraFacilException">Lançado se a validação falhar.</exception>
    Task<ItemCatalogoDto>        CriarAsync(ItemCatalogoInputDto dto, CancellationToken ct = default);

    /// <summary>Atualiza um item do catálogo com os dados do DTO fornecido.</summary>
    /// <param name="id">Identificador do item a ser atualizado.</param>
    /// <param name="dto">Novos dados do item.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <exception cref="ObraFacil.Domain.Exceptions.NotFoundException">Lançado se o item não existir.</exception>
    Task<ItemCatalogoDto>        AtualizarAsync(int id, ItemCatalogoInputDto dto, CancellationToken ct = default);

    /// <summary>Exclui permanentemente um item do catálogo.</summary>
    /// <param name="id">Identificador do item a ser excluído.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <exception cref="ObraFacil.Domain.Exceptions.NotFoundException">Lançado se o item não existir.</exception>
    Task                         ExcluirAsync(int id, CancellationToken ct = default);
}
