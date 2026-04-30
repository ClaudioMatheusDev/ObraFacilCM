using ObraFacil.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ObraFacil.Application.DTOs;

public record ItemCatalogoDto(int Id, TipoItem Tipo, string Nome, string? Descricao,
    UnidadeMedida Unidade, decimal PrecoUnitario, string? Categoria, bool Ativo);

public record ItemCatalogoInputDto(
    TipoItem Tipo,

    [property: Required(ErrorMessage = "Nome é obrigatório.")]
    [property: StringLength(200, ErrorMessage = "Nome pode ter no máximo 200 caracteres.")]
    string Nome,

    [property: StringLength(500, ErrorMessage = "Descrição pode ter no máximo 500 caracteres.")]
    string? Descricao,

    UnidadeMedida Unidade,

    [property: Range(0, double.MaxValue, ErrorMessage = "Preço não pode ser negativo.")]
    decimal PrecoUnitario,

    [property: StringLength(100, ErrorMessage = "Categoria pode ter no máximo 100 caracteres.")]
    string? Categoria);
