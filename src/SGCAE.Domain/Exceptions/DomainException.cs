namespace SGCAE.Domain.Exceptions;

/// <summary>
/// Excepción base para violaciones de reglas e invariantes del dominio.
/// No debe capturarse de forma silenciosa; representa un estado inválido del modelo.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message)
        : base(message) { }

    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
