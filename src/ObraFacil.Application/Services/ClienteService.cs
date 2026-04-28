using ObraFacil.Application.DTOs;
using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Exceptions;
using ObraFacil.Domain.Interfaces;

namespace ObraFacil.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repo;
    public ClienteService(IClienteRepository repo) => _repo = repo;

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
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ObraFacilException("Nome do cliente é obrigatório.");
        var c = new Cliente { Nome = dto.Nome, Telefone = dto.Telefone, Email = dto.Email,
            Documento = dto.Documento, Endereco = dto.Endereco, Observacoes = dto.Observacoes };
        await _repo.AddAsync(c, ct);
        return ToDto(c);
    }

    public async Task<ClienteDto> AtualizarAsync(int id, ClienteInputDto dto, CancellationToken ct = default)
    {
        var c = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException("Cliente", id);
        c.Nome = dto.Nome; c.Telefone = dto.Telefone; c.Email = dto.Email;
        c.Documento = dto.Documento; c.Endereco = dto.Endereco;
        c.Observacoes = dto.Observacoes; c.AlteradoEm = DateTime.UtcNow;
        await _repo.UpdateAsync(c, ct);
        return ToDto(c);
    }

    public Task ExcluirAsync(int id, CancellationToken ct = default) => _repo.DeleteAsync(id, ct);

    private static ClienteDto ToDto(Cliente c) =>
        new(c.Id, c.Nome, c.Telefone, c.Email, c.Documento, c.Endereco, c.Observacoes);
}
