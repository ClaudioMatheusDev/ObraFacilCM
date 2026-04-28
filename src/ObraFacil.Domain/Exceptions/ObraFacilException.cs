namespace ObraFacil.Domain.Exceptions;
public class ObraFacilException : Exception
{
    public ObraFacilException(string msg) : base(msg) { }
    public ObraFacilException(string msg, Exception inner) : base(msg, inner) { }
}
