using Microsoft.Extensions.Logging;
using ObraFacil.Application.DTOs;
using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Exceptions;
using ObraFacil.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace ObraFacil.Application.Services;

/// <summary>
/// Implementação de <see cref="IClienteService"/> com validação de dados de entrada,
/// mapeamento de entidades para DTOs e registro de log das operações.
/// </summary>
public class ClienteService : IClienteService
{
    private readonly IClienteRepository      _repo;
    private readonly ILogger<ClienteService> _logger;

    public ClienteService(IClienteRepository repo, ILogger<ClienteService> logger)
    {
        _repo   = repo;
        _logger = logger;
    }

    public async Task<IList<ClienteDto>> ListarAsync(string? busca = null, CancellationToken ct = default)
    {
        var lista = string.IsNullOrWhiteSpace(busca)
            ? await _repo.GetAllAsync(ct)
            : await _repo.BuscarAsync(busca, ct);
        return lista.Select(ToDto).ToList();
    }

    public async Task<ClienteDto> ObterAsync(int id, CancellationToken ct = default)
        => ToDto(await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException("Cliente", id));

    public async Task<ClienteDto> CriarAsync(ClienteInputDto dto, CancellationToken ct = default)
    {
        ValidarDto(dto);
        var c = new Cliente
        {
            Nome        = dto.Nome.Trim(),
            Telefone    = dto.Telefone?.Trim(),
            Email       = dto.Email?.Trim(),
            Documento   = dto.Documento?.Trim(),
            Endereco    = dto.Endereco?.Trim(),
            Observacoes = dto.Observacoes?.Trim()
        };
        await _repo.AddAsync(c, ct);
        _logger.LogInformation("Cliente {Id} '{Nome}' criado.", c.Id, c.Nome);
        return ToDto(c);
    }

    public async Task<ClienteDto> AtualizarAsync(int id, ClienteInputDto dto, CancellationToken ct = default)
    {
        ValidarDto(dto);
        var c = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException("Cliente", id);
        c.Nome        = dto.Nome.Trim();
        c.Telefone    = dto.Telefone?.Trim();
        c.Email       = dto.Email?.Trim();
        c.Documento   = dto.Documento?.Trim();
        c.Endereco    = dto.Endereco?.Trim();
        c.Observacoes = dto.Observacoes?.Trim();
        c.AlteradoEm  = DateTime.UtcNow;
        await _repo.UpdateAsync(c, ct);
        _logger.LogInformation("Cliente {Id} '{Nome}' atualizado.", c.Id, c.Nome);
        return ToDto(c);
    }

    public async Task ExcluirAsync(int id, CancellationToken ct = default)
    {
        await _repo.DeleteAsync(id, ct);
        _logger.LogInformation("Cliente {Id} excluído.", id);
    }

    private static void ValidarDto(ClienteInputDto dto)
    {
        DtoValidator.Validar(dto);
        // Email: reforça via MailAddress (mais estrita que EmailAddressAttribute)
        if (!string.IsNullOrEmpty(dto.Email) && !IsEmailValido(dto.Email.Trim()))
            throw new ObraFacilException("E-mail informado é inválido.");
        if (!string.IsNullOrEmpty(dto.Telefone) && !IsTelefoneValido(dto.Telefone))
            throw new ObraFacilException("Telefone inválido (mínimo 8, máximo 15 dígitos).");
    }

    private static bool IsEmailValido(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            // Exige que o domínio contenha pelo menos um ponto (ex.: rejeita "user@s")
            return addr.Address == email && addr.Host.Contains('.');
        }
        catch
        {
            return false;
        }
    }

    private static bool IsTelefoneValido(string telefone)
    {
        var digitos = Regex.Replace(telefone, @"\D", "");
        return digitos.Length >= 8 && digitos.Length <= 15;
    }

    private static ClienteDto ToDto(Cliente c) =>
        new(c.Id, c.Nome, c.Telefone, c.Email, c.Documento, c.Endereco, c.Observacoes);
}
