namespace SGCAE.Domain.Exceptions;

/// <summary>
/// Se lanza cuando se intenta crear un DocumentNumber con un valor inválido o nulo.
/// </summary>
public sealed class InvalidDocumentNumberException : DomainException
{
    public string AttemptedValue { get; }

    public InvalidDocumentNumberException(string attemptedValue)
        : base($"El número de documento '{attemptedValue}' no es válido. " +
               "Debe contener solo dígitos y tener entre 5 y 20 caracteres.")
    {
        AttemptedValue = attemptedValue;
    }
}
