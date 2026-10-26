namespace SGCAE.Domain.Exceptions;

/// <summary>
/// Se lanza cuando se intenta crear un Value Object Email con un valor inválido o nulo.
/// </summary>
public sealed class InvalidEmailException : DomainException
{
    public string AttemptedValue { get; }

    public InvalidEmailException(string attemptedValue)
        : base($"El valor '{attemptedValue}' no es una dirección de correo electrónico válida.")
    {
        AttemptedValue = attemptedValue;
    }
}
