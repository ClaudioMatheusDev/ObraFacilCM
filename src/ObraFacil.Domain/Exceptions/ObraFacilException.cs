namespace ObraFacil.Domain.Exceptions;

/// <summary>
/// Exceção base do domínio ObraFacil, lançada para representar erros de regra de negócio
/// e validações que devem ser exibidos ao usuário.
/// </summary>
public class ObraFacilException : Exception
{
    /// <summary>Inicializa a exceção com a mensagem de erro especificada.</summary>
    /// <param name="msg">Mensagem descritiva do erro de negócio.</param>
    public ObraFacilException(string msg) : base(msg) { }

    /// <summary>Inicializa a exceção com uma mensagem e a exceção interna que a causou.</summary>
    /// <param name="msg">Mensagem descritiva do erro de negócio.</param>
    /// <param name="inner">Exceção original que desencadeou este erro.</param>
    public ObraFacilException(string msg, Exception inner) : base(msg, inner) { }
}
