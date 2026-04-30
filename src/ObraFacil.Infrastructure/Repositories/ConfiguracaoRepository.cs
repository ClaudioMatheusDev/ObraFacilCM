using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Interfaces;
using ObraFacil.Infrastructure.Data;

namespace ObraFacil.Infrastructure.Repositories;

/// <summary>
/// Repositório para persistência das configurações globais da aplicação.
/// Garante a existência do registro único (Id = 1) criando-o automaticamente se necessário.
/// </summary>
public class ConfiguracaoRepository : IConfiguracaoRepository
{
    private readonly AppDbContext _db;

    /// <summary>Inicializa o repositório com o contexto de banco de dados.</summary>
    public ConfiguracaoRepository(AppDbContext db) => _db = db;

    /// <inheritdoc/>
    public async Task<Configuracao> GetAsync(CancellationToken ct = default)
    {
        var cfg = await _db.Configuracoes.FindAsync([1], ct);
        if (cfg is null)
        {
            cfg = new Configuracao { Id = 1, NomeEmpresa = "Minha Empresa" };
            _db.Configuracoes.Add(cfg);
            await _db.SaveChangesAsync(ct);
        }
        return cfg;
    }

    /// <inheritdoc/>
    public async Task SalvarAsync(Configuracao config, CancellationToken ct = default)
    {
        _db.Configuracoes.Update(config);
        await _db.SaveChangesAsync(ct);
    }
}
