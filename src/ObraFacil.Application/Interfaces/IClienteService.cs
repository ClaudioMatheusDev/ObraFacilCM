using ObraFacil.Application.DTOs;
namespace ObraFacil.Application.Interfaces;

/// <summary>
/// Serviço de aplicação para gerenciamento de clientes, expondo operações de CRUD com validação e mapeamento.
/// </summary>
public interface IClienteService
{
    /// <summary>Lista todos os clientes, com filtragem opcional por nome ou telefone.</summary>
    /// <param name="busca">Termo de busca. Nulo ou vazio retorna todos os clientes.</param>
    /// <param name="ct">Token de cancelamento.</param>
    Task<IList<ClienteDto>> ListarAsync(string? busca = null, CancellationToken ct = default);

    /// <summary>Retorna um cliente pelo identificador.</summary>
    /// <param name="id">Identificador do cliente.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <exception cref="ObraFacil.Domain.Exceptions.NotFoundException">Lançado se o cliente não existir.</exception>
    Task<ClienteDto>        ObterAsync(int id, CancellationToken ct = default);

    /// <summary>Cria um novo cliente após validar o DTO de entrada.</summary>
    /// <param name="dto">Dados do novo cliente.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <exception cref="ObraFacil.Domain.Exceptions.ObraFacilException">Lançado se a validação falhar.</exception>
    Task<ClienteDto>        CriarAsync(ClienteInputDto dto, CancellationToken ct = default);

    /// <summary>Atualiza um cliente existente com os dados do DTO fornecido.</summary>
    /// <param name="id">Identificador do cliente a ser atualizado.</param>
    /// <param name="dto">Novos dados do cliente.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <exception cref="ObraFacil.Domain.Exceptions.NotFoundException">Lançado se o cliente não existir.</exception>
    /// <exception cref="ObraFacil.Domain.Exceptions.ObraFacilException">Lançado se a validação falhar.</exception>
    Task<ClienteDto>        AtualizarAsync(int id, ClienteInputDto dto, CancellationToken ct = default);

    /// <summary>Exclui permanentemente um cliente.</summary>
    /// <param name="id">Identificador do cliente a ser excluído.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <exception cref="ObraFacil.Domain.Exceptions.NotFoundException">Lançado se o cliente não existir.</exception>
    Task                    ExcluirAsync(int id, CancellationToken ct = default);
}
