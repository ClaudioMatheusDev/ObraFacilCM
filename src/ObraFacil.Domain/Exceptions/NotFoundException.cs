namespace ObraFacil.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando uma entidade não é encontrada pelo seu identificador.
/// </summary>
public class NotFoundException : ObraFacilException
{
    /// <summary>
    /// Inicializa a exceção com o nome da entidade e o identificador que não foi localizado.
    /// </summary>
    /// <param name="entidade">Nome da entidade buscada (ex.: "Cliente", "Orçamento").</param>
    /// <param name="id">Identificador utilizado na busca.</param>
    public NotFoundException(string entidade, int id)
        : base($"{entidade} com id {id} não encontrado(a).") { }
}
