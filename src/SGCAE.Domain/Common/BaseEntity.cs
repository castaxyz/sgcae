namespace SGCAE.Domain.Common;

/// <summary>
/// Clase base abstracta para todas las entidades del dominio.
/// Proporciona identidad (Guid), y marcas de tiempo de auditoría.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Constructor de rehidratación para ORM (EF Core vía reflection).
    /// No debe usarse directamente en código de dominio.
    /// </summary>
    protected BaseEntity(Guid id, DateTime createdAtUtc, DateTime updatedAtUtc)
    {
        Id = id;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    /// <summary>
    /// Actualiza la marca de tiempo de última modificación.
    /// Debe llamarse en cada método de negocio que mute el estado.
    /// </summary>
    protected void MarkAsUpdated()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
