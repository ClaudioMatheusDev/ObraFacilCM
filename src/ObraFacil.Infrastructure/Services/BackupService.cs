using Microsoft.EntityFrameworkCore;
using ObraFacil.Application.Interfaces;
using ObraFacil.Infrastructure.Data;

namespace ObraFacil.Infrastructure.Services;

public class BackupService : IBackupService
{
    private readonly AppDbContext _db;
    public BackupService(AppDbContext db) => _db = db;

    public Task<string> ExportarAsync(CancellationToken ct = default)
    {
        var origem  = _db.Database.GetDbConnection().DataSource;
        var destino = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            $"obra-facil-backup-{DateTime.Now:yyyyMMdd-HHmmss}.db");
        File.Copy(origem, destino, overwrite: true);
        return Task.FromResult(destino);
    }

    public async Task RestaurarAsync(string caminhoArquivo, CancellationToken ct = default)
    {
        var destino = _db.Database.GetDbConnection().DataSource;
        await _db.Database.CloseConnectionAsync();
        File.Copy(caminhoArquivo, destino, overwrite: true);
        await _db.Database.OpenConnectionAsync(ct);
    }
}
