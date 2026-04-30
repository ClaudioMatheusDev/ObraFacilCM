using ObraFacil.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace ObraFacil.Application;

/// <summary>
/// Valida qualquer objeto anotado com DataAnnotations e lança <see cref="ObraFacilException"/> com a primeira mensagem de erro.
/// </summary>
internal static class DtoValidator
{
    /// <summary>
    /// Executa a validação completa de <paramref name="dto"/> usando <see cref="Validator.TryValidateObject"/>.
    /// </summary>
    /// <param name="dto">Objeto a ser validado (geralmente um record de input).</param>
    /// <exception cref="ObraFacilException">Lançado com a primeira mensagem de validação falha encontrada.</exception>
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
