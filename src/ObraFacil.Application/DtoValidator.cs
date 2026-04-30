using ObraFacil.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace ObraFacil.Application;

/// <summary>
/// Valida qualquer objeto anotado com DataAnnotations e lança ObraFacilException com a primeira mensagem de erro.
/// </summary>
internal static class DtoValidator
{
    internal static void Validar(object dto)
    {
        var resultados = new List<ValidationResult>();
        var ctx = new ValidationContext(dto);
        if (!Validator.TryValidateObject(dto, ctx, resultados, validateAllProperties: true))
        {
            var primeira = resultados.FirstOrDefault()?.ErrorMessage ?? "Dados inválidos.";
            throw new ObraFacilException(primeira);
        }
    }
}
