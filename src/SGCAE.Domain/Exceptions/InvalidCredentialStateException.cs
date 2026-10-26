namespace SGCAE.Domain.Exceptions;

/// <summary>
/// Se lanza cuando se intenta ejecutar una operación sobre una credencial
/// que se encuentra en un estado que no lo permite (ej. revocar una ya revocada).
/// </summary>
public sealed class InvalidCredentialStateException : DomainException
{
    public InvalidCredentialStateException(string operation, string currentState)
        : base($"No se puede ejecutar '{operation}' porque la credencial está en estado '{currentState}'.") { }
}
