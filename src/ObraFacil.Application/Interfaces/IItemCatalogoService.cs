using ObraFacil.Application.DTOs;
using ObraFacil.Domain.Enums;
namespace ObraFacil.Application.Interfaces;
public interface IItemCatalogoService
{
    Task<IList<ItemCatalogoDto>> ListarAsync(string? busca = null, TipoItem? tipo = null, CancellationToken ct = default);
    Task<ItemCatalogoDto>        ObterAsync(int id, CancellationToken ct = default);
    Task<ItemCatalogoDto>        CriarAsync(ItemCatalogoInputDto dto, CancellationToken ct = default);
    Task<ItemCatalogoDto>        AtualizarAsync(int id, ItemCatalogoInputDto dto, CancellationToken ct = default);
    Task                         ExcluirAsync(int id, CancellationToken ct = default);
}
