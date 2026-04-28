namespace ObraFacil.Domain.Exceptions;
public class NotFoundException : ObraFacilException
{
    public NotFoundException(string entidade, int id)
        : base($"{entidade} com id {id} não encontrado(a).") { }
}
