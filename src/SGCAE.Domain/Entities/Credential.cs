using SGCAE.Domain.Common;
using SGCAE.Domain.Enums;
using SGCAE.Domain.Exceptions;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Domain.Entities;

/// <summary>
/// Aggregate Root que representa una credencial/tarjeta RFID del sistema.
/// Gestiona el ciclo de vida de la tarjeta: emisión, bloqueo y revocación.
/// </summary>
public sealed class Credential : AggregateRoot
{
    /// <summary>Código alfanumérico único de la tarjeta RFID.</summary>
    public CredentialCode Code { get; private set; }

    /// <summary>
    /// Identificador del empleado al que está asignada la credencial.
    /// Puede ser null si aún no ha sido asignada.
    /// </summary>
    public Guid? EmployeeId { get; private set; }

    /// <summary>Estado actual de la credencial.</summary>
    public CredentialStatus Status { get; private set; }

    /// <summary>Fecha y hora UTC en que se emitió la credencial.</summary>
    public DateTime IssueDateUtc { get; private set; }

    /// <summary>Motivo del bloqueo (solo cuando Status == Blocked).</summary>
    public string? BlockReason { get; private set; }

    /// <summary>Motivo de la revocación (solo cuando Status == Revoked).</summary>
    public string? RevocationReason { get; private set; }

    // Constructor privado para ORM.
#pragma warning disable CS8618
    private Credential() { }
#pragma warning restore CS8618

    private Credential(CredentialCode code, Guid? employeeId) : base()
    {
        Code = code;
        EmployeeId = employeeId;
        Status = CredentialStatus.Active;
        IssueDateUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Crea una nueva credencial activa, opcionalmente asignada a un empleado.
    /// </summary>
    public static Credential Create(CredentialCode code, Guid? employeeId = null)
        => new(code, employeeId);

    /// <summary>
    /// Asigna la credencial a un empleado.
    /// </summary>
    /// <exception cref="InvalidCredentialStateException">Si la credencial está revocada.</exception>
    public void AssignToEmployee(Guid employeeId)
    {
        if (Status == CredentialStatus.Revoked)
            throw new InvalidCredentialStateException(nameof(AssignToEmployee), Status.ToString());

        EmployeeId = employeeId;
        MarkAsUpdated();
    }

    /// <summary>
    /// Bloquea temporalmente la credencial. Puede desbloquearse posteriormente.
    /// </summary>
    /// <param name="reason">Motivo obligatorio del bloqueo.</param>
    /// <exception cref="InvalidCredentialStateException">Si la credencial ya está bloqueada o revocada.</exception>
    /// <exception cref="ArgumentException">Si el motivo está vacío.</exception>
    public void Block(string reason)
    {
        if (Status == CredentialStatus.Blocked)
            throw new InvalidCredentialStateException(nameof(Block), Status.ToString());

        if (Status == CredentialStatus.Revoked)
            throw new InvalidCredentialStateException(nameof(Block), Status.ToString());

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("El motivo del bloqueo es obligatorio.", nameof(reason));

        Status = CredentialStatus.Blocked;
        BlockReason = reason.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Revoca permanentemente la credencial (estado terminal).
    /// Una credencial revocada no puede reactivarse ni bloquearse.
    /// </summary>
    /// <param name="reason">Motivo obligatorio de la revocación.</param>
    /// <exception cref="InvalidCredentialStateException">Si la credencial ya está revocada.</exception>
    /// <exception cref="ArgumentException">Si el motivo está vacío.</exception>
    public void Revoke(string reason)
    {
        if (Status == CredentialStatus.Revoked)
            throw new InvalidCredentialStateException(nameof(Revoke), Status.ToString());

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("El motivo de la revocación es obligatorio.", nameof(reason));

        Status = CredentialStatus.Revoked;
        RevocationReason = reason.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Desbloquea una credencial previamente bloqueada, volviéndola a estado Active.
    /// </summary>
    /// <exception cref="InvalidCredentialStateException">Si la credencial no está bloqueada.</exception>
    public void Unblock()
    {
        if (Status != CredentialStatus.Blocked)
            throw new InvalidCredentialStateException(nameof(Unblock), Status.ToString());

        Status = CredentialStatus.Active;
        BlockReason = null;
        MarkAsUpdated();
    }

    /// <summary>
    /// Indica si la credencial puede usarse para registrar un acceso.
    /// Solo las credenciales en estado Active son válidas.
    /// </summary>
    public bool CanBeUsed() => Status == CredentialStatus.Active;
}
