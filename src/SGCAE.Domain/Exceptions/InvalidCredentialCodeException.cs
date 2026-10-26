namespace SGCAE.Domain.Exceptions;

/// <summary>
/// Se lanza cuando se intenta crear un CredentialCode con un valor inválido o nulo.
/// </summary>
public sealed class InvalidCredentialCodeException : DomainException
{
    public string AttemptedValue { get; }

    public InvalidCredentialCodeException(string attemptedValue)
        : base($"El código de credencial '{attemptedValue}' no es válido. " +
               "Debe ser alfanumérico y tener entre 4 y 64 caracteres.")
    {
        AttemptedValue = attemptedValue;
    }
}
