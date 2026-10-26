namespace SGCAE.Application.Common.Exceptions;

/// <summary>
/// Excepción lanzada por el ValidationBehavior cuando los validadores de FluentValidation
/// detectan uno o más errores en el request.
/// Agrupa los mensajes de error por nombre de campo.
/// </summary>
public sealed class ValidationException : Exception
{
    /// <summary>
    /// Diccionario de errores de validación: clave = nombre del campo, valor = mensajes de error.
    /// </summary>
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("Se produjeron uno o más errores de validación.")
    {
        Errors = errors;
    }

    public ValidationException(string field, string message)
        : base("Se produjeron uno o más errores de validación.")
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { message } }
        };
    }
}
