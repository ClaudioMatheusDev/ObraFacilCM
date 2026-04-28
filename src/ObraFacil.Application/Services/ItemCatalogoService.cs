using ObraFacil.Application.DTOs;
using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Enums;
using ObraFacil.Domain.Exceptions;
using ObraFacil.Domain.Interfaces;

namespace ObraFacil.Application.Services;

public class ItemCatalogoService : IItemCatalogoService
{
    private readonly IItemCatalogoRepository _repo;
    public ItemCatalogoService(IItemCatalogoRepository repo) => _repo = repo;

    public async Task<IList<ItemCatalogoDto>> ListarAsync(string? busca = null, TipoItem? tipo = null, CancellationToken ct = default)
    {
        var lista = string.IsNullOrWhiteSpace(busca) && tipo is null
            ? await _repo.GetAllAsync(ct)
            : await _repo.BuscarAsync(busca ?? string.Empty, tipo, ct);
        return lista.Select(ToDto).ToList();
    }

    public async Task<ItemCatalogoDto> ObterAsync(int id, CancellationToken ct = default)
        => ToDto(await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException("Item", id));

    public async Task<ItemCatalogoDto> CriarAsync(ItemCatalogoInputDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ObraFacilException("Nome do item é obrigatório.");
        if (dto.PrecoUnitario < 0)
            throw new ObraFacilException("Preço não pode ser negativo.");
        var item = new ItemCatalogo { Tipo = dto.Tipo, Nome = dto.Nome, Descricao = dto.Descricao,
            Unidade = dto.Unidade, PrecoUnitario = dto.PrecoUnitario, Categoria = dto.Categoria };
        await _repo.AddAsync(item, ct);
        return ToDto(item);
    }

    public async Task<ItemCatalogoDto> AtualizarAsync(int id, ItemCatalogoInputDto dto, CancellationToken ct = default)
    {
        var item = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException("Item", id);
        item.Tipo = dto.Tipo; item.Nome = dto.Nome; item.Descricao = dto.Descricao;
        item.Unidade = dto.Unidade; item.PrecoUnitario = dto.PrecoUnitario;
        item.Categoria = dto.Categoria; item.AlteradoEm = DateTime.UtcNow;
        await _repo.UpdateAsync(item, ct);
        return ToDto(item);
    }

    public Task ExcluirAsync(int id, CancellationToken ct = default) => _repo.DeleteAsync(id, ct);

    private static ItemCatalogoDto ToDto(ItemCatalogo i) =>
        new(i.Id, i.Tipo, i.Nome, i.Descricao, i.Unidade, i.PrecoUnitario, i.Categoria, i.Ativo);
}
