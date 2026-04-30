namespace ObraFacil.Application.Interfaces;

/// <summary>
/// Serviço para exportação e restauração do banco de dados SQLite.
/// </summary>
public interface IBackupService
{
    /// <summary>
    /// Exporta o banco de dados atual para a pasta Documentos do usuário.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Caminho completo do arquivo de backup gerado.</returns>
    Task<string> ExportarAsync(CancellationToken ct = default);

    /// <summary>
    /// Restaura o banco de dados a partir de um arquivo de backup previamente exportado.
    /// Valida a integridade do arquivo antes de qualquer operação destrutiva.
    /// </summary>
    /// <param name="caminhoArquivo">Caminho completo do arquivo <c>.db</c> a ser restaurado.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <exception cref="System.IO.FileNotFoundException">Lançado se o arquivo não existir.</exception>
    /// <exception cref="ObraFacil.Domain.Exceptions.ObraFacilException">Lançado se o arquivo estiver corrompido ou não for um SQLite válido.</exception>
    Task         RestaurarAsync(string caminhoArquivo, CancellationToken ct = default);
}
