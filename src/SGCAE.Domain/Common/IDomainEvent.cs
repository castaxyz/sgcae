namespace SGCAE.Domain.Common;

/// <summary>
/// Interfaz marcadora para todos los eventos de dominio.
/// Permite a la capa de Aplicación despacharlos mediante MediatR (INotification).
/// </summary>
public interface IDomainEvent
{
    /// <summary>Momento exacto (UTC) en que ocurrió el evento.</summary>
    DateTime OccurredOnUtc { get; }
}
