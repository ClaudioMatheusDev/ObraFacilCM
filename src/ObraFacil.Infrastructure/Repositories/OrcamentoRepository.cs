using Microsoft.EntityFrameworkCore;
using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Enums;
using ObraFacil.Domain.Interfaces;
using ObraFacil.Infrastructure.Data;

namespace ObraFacil.Infrastructure.Repositories;

/// <summary>
/// Repositório concreto de <see cref="Orcamento"/> com suporte a eager-loading de itens e cliente,
/// filtragem avançada e geração de número sequencial.
/// </summary>
public class OrcamentoRepository : RepositoryBase<Orcamento>, IOrcamentoRepository
{
    /// <summary>Inicializa o repositório com o contexto de banco de dados.</summary>
    public OrcamentoRepository(AppDbContext db) : base(db) { }

    /// <inheritdoc/>
    public async Task<Orcamento?> GetComItensAsync(int id, CancellationToken ct = default)
        => await _db.Orcamentos.Include(o => o.Cliente).Include(o => o.Itens)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    /// <inheritdoc/>
    public async Task<IList<Orcamento>> FiltrarAsync(string? termo = null,
        StatusOrcamento? status = null, DateTime? de = null, DateTime? ate = null,
        CancellationToken ct = default)
    {
        var q = _db.Orcamentos.Include(o => o.Cliente).Include(o => o.Itens).AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(termo))
            q = q.Where(o => o.Numero.Contains(termo) || o.Cliente.Nome.Contains(termo));
        if (status.HasValue) q = q.Where(o => o.Status == status.Value);
        if (de.HasValue)  q = q.Where(o => o.DataEmissao >= de.Value);
        if (ate.HasValue) q = q.Where(o => o.DataEmissao <= ate.Value);
        return await q.OrderByDescending(o => o.DataEmissao).ToListAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<string> GerarProximoNumeroAsync(CancellationToken ct = default)
    {
        var cfg = await _db.Configuracoes.FindAsync([1], ct) ?? new Domain.Entities.Configuracao();
        var num = $"{DateTime.Today.Year}-{cfg.ProximoNumeroOrcamento:D4}";
        cfg.ProximoNumeroOrcamento++;
        _db.Configuracoes.Update(cfg);
        await _db.SaveChangesAsync(ct);
        return num;
    }
}
