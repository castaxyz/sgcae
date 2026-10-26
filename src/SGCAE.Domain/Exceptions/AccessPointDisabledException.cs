namespace SGCAE.Domain.Exceptions;

/// <summary>
/// Se lanza cuando se intenta registrar un acceso en un punto que se encuentra deshabilitado.
/// </summary>
public sealed class AccessPointDisabledException : DomainException
{
    public Guid AccessPointId { get; }

    public AccessPointDisabledException(Guid accessPointId)
        : base($"El punto de acceso con Id '{accessPointId}' está deshabilitado.")
    {
        AccessPointId = accessPointId;
    }
}
