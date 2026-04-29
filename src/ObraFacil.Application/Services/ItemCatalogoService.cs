using Microsoft.Extensions.Logging;
using ObraFacil.Application.DTOs;
using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Enums;
using ObraFacil.Domain.Exceptions;
using ObraFacil.Domain.Interfaces;

namespace ObraFacil.Application.Services;

public class ItemCatalogoService : IItemCatalogoService
{
    private readonly IItemCatalogoRepository      _repo;
    private readonly ILogger<ItemCatalogoService> _logger;

    public ItemCatalogoService(IItemCatalogoRepository repo, ILogger<ItemCatalogoService> logger)
    {
        _repo   = repo;
        _logger = logger;
    }

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
        ValidarDto(dto);
        var item = new ItemCatalogo
        {
            Tipo           = dto.Tipo,
            Nome           = dto.Nome.Trim(),
            Descricao      = dto.Descricao?.Trim(),
            Unidade        = dto.Unidade,
            PrecoUnitario  = dto.PrecoUnitario,
            Categoria      = dto.Categoria?.Trim()
        };
        await _repo.AddAsync(item, ct);
        _logger.LogInformation("Item catálogo {Id} '{Nome}' criado.", item.Id, item.Nome);
        return ToDto(item);
    }

    public async Task<ItemCatalogoDto> AtualizarAsync(int id, ItemCatalogoInputDto dto, CancellationToken ct = default)
    {
        ValidarDto(dto);
        var item = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException("Item", id);
        item.Tipo          = dto.Tipo;
        item.Nome          = dto.Nome.Trim();
        item.Descricao     = dto.Descricao?.Trim();
        item.Unidade       = dto.Unidade;
        item.PrecoUnitario = dto.PrecoUnitario;
        item.Categoria     = dto.Categoria?.Trim();
        item.AlteradoEm    = DateTime.UtcNow;
        await _repo.UpdateAsync(item, ct);
        _logger.LogInformation("Item catálogo {Id} '{Nome}' atualizado.", item.Id, item.Nome);
        return ToDto(item);
    }

    public async Task ExcluirAsync(int id, CancellationToken ct = default)
    {
        await _repo.DeleteAsync(id, ct);
        _logger.LogInformation("Item catálogo {Id} excluído.", id);
    }

    private static void ValidarDto(ItemCatalogoInputDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ObraFacilException("Nome do item é obrigatório.");
        if (dto.PrecoUnitario < 0)
            throw new ObraFacilException("Preço não pode ser negativo.");
    }

    private static ItemCatalogoDto ToDto(ItemCatalogo i) =>
        new(i.Id, i.Tipo, i.Nome, i.Descricao, i.Unidade, i.PrecoUnitario, i.Categoria, i.Ativo);
}
