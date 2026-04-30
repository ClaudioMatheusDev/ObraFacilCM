using ObraFacil.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ObraFacil.Application.DTOs;

/// <summary>Representa os dados de leitura de um item do catálogo.</summary>
/// <param name="Id">Identificador único do item.</param>
/// <param name="Tipo">Classificação do item (Material ou Serviço).</param>
/// <param name="Nome">Nome do item.</param>
/// <param name="Descricao">Descrição detalhada. Opcional.</param>
/// <param name="Unidade">Unidade de medida padrão.</param>
/// <param name="PrecoUnitario">Preço unitário de referência.</param>
/// <param name="Categoria">Categoria de agrupamento. Opcional.</param>
/// <param name="Ativo">Indica se o item está ativo e disponível para uso em orçamentos.</param>
public record ItemCatalogoDto(int Id, TipoItem Tipo, string Nome, string? Descricao,
    UnidadeMedida Unidade, decimal PrecoUnitario, string? Categoria, bool Ativo);

/// <summary>Dados de entrada para criação ou atualização de um item do catálogo.</summary>
/// <param name="Tipo">Classificação do item (Material ou Serviço).</param>
/// <param name="Nome">Nome do item. Obrigatório, máximo de 200 caracteres.</param>
/// <param name="Descricao">Descrição detalhada. Opcional, máximo de 500 caracteres.</param>
/// <param name="Unidade">Unidade de medida padrão.</param>
/// <param name="PrecoUnitario">Preço unitário de referência. Deve ser maior ou igual a zero.</param>
/// <param name="Categoria">Categoria de agrupamento. Opcional, máximo de 100 caracteres.</param>
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
