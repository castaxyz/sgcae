using SGCAE.Domain.Common;
using SGCAE.Domain.Enums;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Domain.Entities;

/// <summary>
/// Aggregate Root que representa un registro de acceso auditado (inmutable).
/// Cada instancia es una entrada permanente en el historial del sistema.
/// No expone métodos de mutación: una vez creado, el log es inmutable.
/// Cumple con los requisitos RF-03 (registro de movimientos) y RF-04 (auditoría).
/// </summary>
public sealed class AccessLog : AggregateRoot
{
    /// <summary>
    /// Código de la credencial que intentó el acceso.
    /// Se almacena como valor para preservar el historial aunque la credencial sea eliminada.
    /// </summary>
    public CredentialCode CredentialCode { get; private set; }

    /// <summary>
    /// Identificador del empleado asociado al momento del acceso.
    /// Nullable: puede ser null si la credencial no tenía empleado asignado.
    /// </summary>
    public Guid? EmployeeId { get; private set; }

    /// <summary>Identificador del punto de acceso donde ocurrió el evento.</summary>
    public Guid AccessPointId { get; private set; }

    /// <summary>Tipo de movimiento: entrada o salida.</summary>
    public MovementType MovementType { get; private set; }

    /// <summary>Marca de tiempo exacta (UTC) del evento de acceso.</summary>
    public DateTime TimestampUtc { get; private set; }

    /// <summary>Resultado del intento de acceso: autorizado o rechazado.</summary>
    public AccessResult AccessResult { get; private set; }

    /// <summary>
    /// Motivo del rechazo (solo cuando AccessResult == Rejected).
    /// Ejemplos: "Empleado inactivo", "Credencial bloqueada", "Punto inhabilitado".
    /// </summary>
    public string? RejectionReason { get; private set; }

    // Constructor privado para ORM.
#pragma warning disable CS8618
    private AccessLog() { }
#pragma warning restore CS8618

    private AccessLog(
        CredentialCode credentialCode,
        Guid? employeeId,
        Guid accessPointId,
        MovementType movementType,
        AccessResult accessResult,
        string? rejectionReason) : base()
    {
        CredentialCode = credentialCode;
        EmployeeId = employeeId;
        AccessPointId = accessPointId;
        MovementType = movementType;
        TimestampUtc = DateTime.UtcNow;
        AccessResult = accessResult;
        RejectionReason = rejectionReason;
    }

    /// <summary>
    /// Crea un registro de acceso autorizado.
    /// </summary>
    /// <param name="credentialCode">Código de la credencial usada.</param>
    /// <param name="employeeId">Id del empleado (puede ser null).</param>
    /// <param name="accessPointId">Id del punto de acceso.</param>
    /// <param name="movementType">Entrada o salida.</param>
    public static AccessLog CreateAuthorized(
        CredentialCode credentialCode,
        Guid? employeeId,
        Guid accessPointId,
        MovementType movementType)
    {
        return new AccessLog(
            credentialCode,
            employeeId,
            accessPointId,
            movementType,
            AccessResult.Authorized,
            rejectionReason: null);
    }

    /// <summary>
    /// Crea un registro de acceso rechazado con el motivo correspondiente.
    /// </summary>
    /// <param name="credentialCode">Código de la credencial que intentó el acceso.</param>
    /// <param name="employeeId">Id del empleado (puede ser null si no hay asignación).</param>
    /// <param name="accessPointId">Id del punto de acceso.</param>
    /// <param name="movementType">Entrada o salida intentada.</param>
    /// <param name="rejectionReason">Motivo del rechazo (obligatorio para rechazos).</param>
    /// <exception cref="ArgumentException">Si el motivo de rechazo está vacío.</exception>
    public static AccessLog CreateRejected(
        CredentialCode credentialCode,
        Guid? employeeId,
        Guid accessPointId,
        MovementType movementType,
        string rejectionReason)
    {
        if (string.IsNullOrWhiteSpace(rejectionReason))
            throw new ArgumentException("El motivo del rechazo es obligatorio para un acceso rechazado.", nameof(rejectionReason));

        return new AccessLog(
            credentialCode,
            employeeId,
            accessPointId,
            movementType,
            AccessResult.Rejected,
            rejectionReason.Trim());
    }

    /// <summary>Indica si el acceso fue autorizado.</summary>
    public bool WasAuthorized() => AccessResult == AccessResult.Authorized;
}
