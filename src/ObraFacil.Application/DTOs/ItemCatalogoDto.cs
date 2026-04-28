using ObraFacil.Domain.Enums;
namespace ObraFacil.Application.DTOs;
public record ItemCatalogoDto(int Id, TipoItem Tipo, string Nome, string? Descricao,
    UnidadeMedida Unidade, decimal PrecoUnitario, string? Categoria, bool Ativo);
public record ItemCatalogoInputDto(TipoItem Tipo, string Nome, string? Descricao,
    UnidadeMedida Unidade, decimal PrecoUnitario, string? Categoria);
