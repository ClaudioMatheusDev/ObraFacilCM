using ObraFacil.Domain.Entities;
namespace ObraFacil.Domain.Interfaces;

/// <summary>
/// Repositório para leitura e persistência das configurações globais da aplicação.
/// Gerencia o registro único de <see cref="Configuracao"/> (Id = 1).
/// </summary>
public interface IConfiguracaoRepository
{
    /// <summary>
    /// Retorna as configurações atuais. Cria um registro padrão caso ainda não exista.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    Task<Configuracao> GetAsync(CancellationToken ct = default);

    /// <summary>Persiste as configurações fornecidas.</summary>
    /// <param name="config">Objeto de configuração com os novos valores.</param>
    /// <param name="ct">Token de cancelamento.</param>
    Task SalvarAsync(Configuracao config, CancellationToken ct = default);
}
