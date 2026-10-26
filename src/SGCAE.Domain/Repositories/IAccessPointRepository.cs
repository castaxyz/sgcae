using SGCAE.Domain.Entities;

namespace SGCAE.Domain.Repositories;

/// <summary>
/// Contrato de persistencia para el agregado AccessPoint.
/// Las implementaciones deben residir en SGCAE.Infrastructure.
/// </summary>
public interface IAccessPointRepository
{
    /// <summary>Obtiene un punto de acceso por su identificador único.</summary>
    /// <returns>El punto de acceso encontrado, o null si no existe.</returns>
    Task<AccessPoint?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un punto de acceso por su código corto (ej. "REC-01").
    /// </summary>
    /// <returns>El punto de acceso encontrado, o null si no existe.</returns>
    Task<AccessPoint?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>Obtiene todos los puntos de acceso del sistema.</summary>
    Task<IReadOnlyList<AccessPoint>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Agrega un nuevo punto de acceso al repositorio.</summary>
    Task AddAsync(AccessPoint accessPoint, CancellationToken cancellationToken = default);

    /// <summary>Marca el punto de acceso como modificado para que los cambios sean persistidos.</summary>
    void Update(AccessPoint accessPoint);
}
