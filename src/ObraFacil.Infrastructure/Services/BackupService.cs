using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Exceptions;
using ObraFacil.Infrastructure.Data;

namespace ObraFacil.Infrastructure.Services;

/// <summary>
/// Implementação de <see cref="IBackupService"/> para exportação e restauração do banco SQLite.
/// Valida a integridade do arquivo antes de qualquer substituição destrutiva e mantém
/// uma cópia temporária como rollback em caso de falha na restauração.
/// </summary>
public class BackupService : IBackupService
{
    private readonly AppDbContext _db;
    public BackupService(AppDbContext db) => _db = db;

    public async Task<string> ExportarAsync(CancellationToken ct = default)
    {
        var origem  = _db.Database.GetDbConnection().DataSource;
        var destino = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            $"obra-facil-backup-{DateTime.Now:yyyyMMdd-HHmmss}.db");

        // Fecha conexão antes de copiar para garantir flush do WAL
        await _db.Database.CloseConnectionAsync();
        try
        {
            File.Copy(origem, destino, overwrite: true);
        }
        finally
        {
            await _db.Database.OpenConnectionAsync(ct);
        }

        return destino;
    }

    public async Task RestaurarAsync(string caminhoArquivo, CancellationToken ct = default)
    {
        if (!File.Exists(caminhoArquivo))
            throw new FileNotFoundException("Arquivo de backup não encontrado.", caminhoArquivo);

        // Valida se o arquivo é um banco SQLite válido antes de qualquer operação destrutiva
        await ValidarSQLiteAsync(caminhoArquivo, ct);

        var destino = _db.Database.GetDbConnection().DataSource;
        var temp    = destino + ".bak";

        await _db.Database.CloseConnectionAsync();
        try
        {
            // Preserva cópia de segurança temporária antes de sobrescrever
            if (File.Exists(destino))
                File.Copy(destino, temp, overwrite: true);

            File.Copy(caminhoArquivo, destino, overwrite: true);

            // Limpa o temp somente após cópia bem-sucedida
            if (File.Exists(temp)) File.Delete(temp);
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
            await _db.Database.OpenConnectionAsync(ct);
        }
    }

    private static async Task ValidarSQLiteAsync(string caminho, CancellationToken ct)
    {
        try
        {
            var connStr = new SqliteConnectionStringBuilder { DataSource = caminho, Mode = SqliteOpenMode.ReadOnly }.ToString();
            await using var conn = new SqliteConnection(connStr);
            await conn.OpenAsync(ct);
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "PRAGMA integrity_check;";
            var resultado = (string?)await cmd.ExecuteScalarAsync(ct);
            if (resultado != "ok")
                throw new ObraFacilException("O arquivo de backup está corrompido (integrity_check falhou).");
        }
        catch (SqliteException ex)
        {
            throw new ObraFacilException("O arquivo selecionado não é um banco de dados SQLite válido.", ex);
        }
    }
}
