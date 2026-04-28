namespace ObraFacil.Application.DTOs;
public record ClienteDto(int Id, string Nome, string? Telefone, string? Email,
    string? Documento, string? Endereco, string? Observacoes);
public record ClienteInputDto(string Nome, string? Telefone, string? Email,
    string? Documento, string? Endereco, string? Observacoes);
