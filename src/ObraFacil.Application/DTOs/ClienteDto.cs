using System.ComponentModel.DataAnnotations;

namespace ObraFacil.Application.DTOs;

/// <summary>Representa os dados de leitura de um cliente retornados pelos serviços.</summary>
/// <param name="Id">Identificador único do cliente.</param>
/// <param name="Nome">Nome completo do cliente.</param>
/// <param name="Telefone">Telefone de contato. Opcional.</param>
/// <param name="Email">E-mail do cliente. Opcional.</param>
/// <param name="Documento">CPF ou CNPJ. Opcional.</param>
/// <param name="Endereco">Endereço completo. Opcional.</param>
/// <param name="Observacoes">Observações gerais. Opcional.</param>
public record ClienteDto(int Id, string Nome, string? Telefone, string? Email,
    string? Documento, string? Endereco, string? Observacoes);

/// <summary>Dados de entrada para criação ou atualização de um cliente.</summary>
/// <param name="Nome">Nome completo do cliente. Obrigatório, máximo de 200 caracteres.</param>
/// <param name="Telefone">Telefone de contato. Opcional, máximo de 30 caracteres.</param>
/// <param name="Email">E-mail do cliente. Opcional, máximo de 200 caracteres.</param>
/// <param name="Documento">CPF ou CNPJ. Opcional, máximo de 20 caracteres.</param>
/// <param name="Endereco">Endereço completo. Opcional, máximo de 500 caracteres.</param>
/// <param name="Observacoes">Observações gerais. Opcional, máximo de 1000 caracteres.</param>
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
