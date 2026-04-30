using System.ComponentModel.DataAnnotations;

namespace ObraFacil.Application.DTOs;

public record ClienteDto(int Id, string Nome, string? Telefone, string? Email,
    string? Documento, string? Endereco, string? Observacoes);

public record ClienteInputDto(
    [property: Required(ErrorMessage = "Nome é obrigatório.")]
    [property: StringLength(200, ErrorMessage = "Nome pode ter no máximo 200 caracteres.")]
    string Nome,

    [property: StringLength(30, ErrorMessage = "Telefone pode ter no máximo 30 caracteres.")]
    string? Telefone,

    [property: StringLength(200, ErrorMessage = "E-mail pode ter no máximo 200 caracteres.")]
    string? Email,

    [property: StringLength(20, ErrorMessage = "CPF/CNPJ pode ter no máximo 20 caracteres.")]
    string? Documento,

    [property: StringLength(500, ErrorMessage = "Endereço pode ter no máximo 500 caracteres.")]
    string? Endereco,

    [property: StringLength(1000, ErrorMessage = "Observações podem ter no máximo 1000 caracteres.")]
    string? Observacoes);
