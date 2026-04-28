namespace ObraFacil.Application.Interfaces;
public interface IBackupService
{
    Task<string> ExportarAsync(CancellationToken ct = default);
    Task         RestaurarAsync(string caminhoArquivo, CancellationToken ct = default);
}
