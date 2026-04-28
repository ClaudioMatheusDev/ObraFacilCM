using ObraFacil.Application.DTOs;
using ObraFacil.Domain.Enums;
namespace ObraFacil.Application.Interfaces;
public interface IOrcamentoService
{
    Task<IList<OrcamentoDto>> ListarAsync(string? busca = null, StatusOrcamento? status = null,
        DateTime? de = null, DateTime? ate = null, CancellationToken ct = default);
    Task<OrcamentoDto> ObterAsync(int id, CancellationToken ct = default);
    Task<OrcamentoDto> CriarAsync(OrcamentoInputDto dto, CancellationToken ct = default);
    Task<OrcamentoDto> AtualizarAsync(int id, OrcamentoInputDto dto, CancellationToken ct = default);
    Task               AlterarStatusAsync(int id, StatusOrcamento novoStatus, CancellationToken ct = default);
    Task               ExcluirAsync(int id, CancellationToken ct = default);
}
