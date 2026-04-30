namespace ObraFacil.Domain.Entities;

/// <summary>
/// Classe base para todas as entidades do domínio, fornecendo identidade e controle de auditoria de datas.
/// </summary>
public abstract class EntityBase
{
    /// <summary>Identificador único da entidade gerado pelo banco de dados.</summary>
    public int      Id         { get; set; }

    /// <summary>Data e hora de criação do registro (UTC).</summary>
    public DateTime CriadoEm  { get; set; } = DateTime.UtcNow;

    /// <summary>Data e hora da última alteração do registro (UTC).</summary>
    public DateTime AlteradoEm { get; set; } = DateTime.UtcNow;
}
