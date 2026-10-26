namespace SGCAE.Domain.Common;

/// <summary>
/// Clase base para raíces de agregado (Aggregate Roots).
/// Extiende BaseEntity con soporte para eventos de dominio.
/// Los eventos se recopilan en memoria y se despachan desde la capa de Aplicación.
/// </summary>
public abstract class AggregateRoot : BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Eventos de dominio pendientes de despacho.
    /// Solo lectura desde el exterior.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot() : base() { }

    protected AggregateRoot(Guid id, DateTime createdAtUtc, DateTime updatedAtUtc)
        : base(id, createdAtUtc, updatedAtUtc) { }

    /// <summary>
    /// Registra un evento de dominio para ser despachado posteriormente.
    /// </summary>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Limpia los eventos de dominio después de ser despachados.
    /// Llamado por la infraestructura al persistir el agregado.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
