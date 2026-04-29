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
        if (!File.Exists(caminhoArquivo))
            throw new FileNotFoundException("Arquivo de backup não encontrado.", caminhoArquivo);

        var destino = _db.Database.GetDbConnection().DataSource;
        var temp    = destino + ".bak";

        await _db.Database.CloseConnectionAsync();
        try
        {
            // Preserva cópia de segurança temporária antes de sobrescrever
            if (File.Exists(destino))
                File.Copy(destino, temp, overwrite: true);

            File.Copy(caminhoArquivo, destino, overwrite: true);
        }
        catch
        {
            // Rollback: restaura o banco original se a cópia falhou
            if (File.Exists(temp))
                File.Move(temp, destino, overwrite: true);
            throw;
        }
        finally
        {
            if (File.Exists(temp)) File.Delete(temp);
            await _db.Database.OpenConnectionAsync(ct);
        }
    }
}
