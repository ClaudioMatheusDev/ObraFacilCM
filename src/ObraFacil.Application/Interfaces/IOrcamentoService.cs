using ObraFacil.Application.DTOs;
using ObraFacil.Domain.Enums;
namespace ObraFacil.Application.Interfaces;

/// <summary>
/// Serviço de aplicação para gerenciamento completo de orçamentos, incluindo criação, edição,
/// mudança de status e exclusão.
/// </summary>
public interface IOrcamentoService
{
    /// <summary>
    /// Lista orçamentos com filtros opcionais de texto, status e período de emissão.
    /// </summary>
    /// <param name="busca">Texto para filtrar por número do orçamento ou nome do cliente.</param>
    /// <param name="status">Status desejado. Nulo retorna todos os status.</param>
    /// <param name="de">Início do período de emissão.</param>
    /// <param name="ate">Fim do período de emissão.</param>
    /// <param name="ct">Token de cancelamento.</param>
    Task<IList<OrcamentoDto>> ListarAsync(string? busca = null, StatusOrcamento? status = null,
        DateTime? de = null, DateTime? ate = null, CancellationToken ct = default);

    /// <summary>Retorna um orçamento completo (com itens) pelo identificador.</summary>
    /// <param name="id">Identificador do orçamento.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <exception cref="ObraFacil.Domain.Exceptions.NotFoundException">Lançado se o orçamento não existir.</exception>
    Task<OrcamentoDto> ObterAsync(int id, CancellationToken ct = default);

    /// <summary>Cria um novo orçamento gerando número sequencial e congelando os preços dos itens.</summary>
    /// <param name="dto">Dados do orçamento a ser criado.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <exception cref="ObraFacil.Domain.Exceptions.ObraFacilException">Lançado se a validação falhar.</exception>
    Task<OrcamentoDto> CriarAsync(OrcamentoInputDto dto, CancellationToken ct = default);

    /// <summary>Atualiza os dados do cabeçalho e itens de um orçamento existente.</summary>
    /// <param name="id">Identificador do orçamento a ser atualizado.</param>
    /// <param name="dto">Novos dados do orçamento.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <exception cref="ObraFacil.Domain.Exceptions.NotFoundException">Lançado se o orçamento não existir.</exception>
    Task<OrcamentoDto> AtualizarAsync(int id, OrcamentoInputDto dto, CancellationToken ct = default);

    /// <summary>Altera o status de um orçamento (ex.: de Rascunho para Enviado).</summary>
    /// <param name="id">Identificador do orçamento.</param>
    /// <param name="novoStatus">Novo status desejado.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <exception cref="ObraFacil.Domain.Exceptions.NotFoundException">Lançado se o orçamento não existir.</exception>
    Task               AlterarStatusAsync(int id, StatusOrcamento novoStatus, CancellationToken ct = default);

    /// <summary>Exclui permanentemente um orçamento e seus itens.</summary>
    /// <param name="id">Identificador do orçamento a ser excluído.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <exception cref="ObraFacil.Domain.Exceptions.NotFoundException">Lançado se o orçamento não existir.</exception>
    Task               ExcluirAsync(int id, CancellationToken ct = default);
}
