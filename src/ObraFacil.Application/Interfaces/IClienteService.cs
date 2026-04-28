using ObraFacil.Application.DTOs;
namespace ObraFacil.Application.Interfaces;
public interface IClienteService
{
    Task<IList<ClienteDto>> ListarAsync(string? busca = null, CancellationToken ct = default);
    Task<ClienteDto>        ObterAsync(int id, CancellationToken ct = default);
    Task<ClienteDto>        CriarAsync(ClienteInputDto dto, CancellationToken ct = default);
    Task<ClienteDto>        AtualizarAsync(int id, ClienteInputDto dto, CancellationToken ct = default);
    Task                    ExcluirAsync(int id, CancellationToken ct = default);
}
