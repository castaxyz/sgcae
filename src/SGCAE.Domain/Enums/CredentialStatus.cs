namespace SGCAE.Domain.Enums;

/// <summary>Estado de una credencial/tarjeta RFID.</summary>
public enum CredentialStatus
{
    /// <summary>La credencial está activa y puede usarse para acceder.</summary>
    Active = 1,

    /// <summary>
    /// La credencial está temporalmente bloqueada (pérdida, mal uso, etc.).
    /// Puede desbloquearse.
    /// </summary>
    Blocked = 2,

    /// <summary>
    /// La credencial ha sido revocada permanentemente (estado terminal).
    /// No puede reactivarse.
    /// </summary>
    Revoked = 3
}
