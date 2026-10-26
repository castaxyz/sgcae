using SGCAE.Domain.Entities;
using SGCAE.Domain.Enums;

namespace SGCAE.Domain.Repositories;

/// <summary>
/// Contrato de persistencia para el agregado AccessLog.
/// El historial de accesos es append-only: nunca se actualiza ni elimina.
/// Las implementaciones deben residir en SGCAE.Infrastructure.
/// </summary>
public interface IAccessLogRepository
{
    /// <summary>
    /// Agrega un nuevo registro de acceso al historial.
    /// </summary>
    Task AddAsync(AccessLog accessLog, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una página de registros de acceso aplicando los filtros especificados.
    /// Todos los parámetros de filtro son opcionales (null = sin filtro).
    /// </summary>
    /// <param name="employeeId">Filtra por empleado específico.</param>
    /// <param name="accessPointId">Filtra por punto de acceso específico.</param>
    /// <param name="fromDate">Filtra registros desde esta fecha (inclusive).</param>
    /// <param name="toDate">Filtra registros hasta esta fecha (inclusive).</param>
    /// <param name="accessResult">Filtra por resultado: Authorized o Rejected.</param>
    /// <param name="pageNumber">Número de página (base 1).</param>
    /// <param name="pageSize">Cantidad de registros por página.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Página de registros ordenados por TimestampUtc descendente.</returns>
    Task<IReadOnlyList<AccessLog>> GetFilteredAsync(
        Guid? employeeId,
        Guid? accessPointId,
        DateTime? fromDate,
        DateTime? toDate,
        AccessResult? accessResult,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el total de registros que coinciden con los filtros especificados.
    /// Se usa en conjunto con <see cref="GetFilteredAsync"/> para paginación.
    /// </summary>
    Task<int> GetTotalCountAsync(
        Guid? employeeId,
        Guid? accessPointId,
        DateTime? fromDate,
        DateTime? toDate,
        AccessResult? accessResult,
        CancellationToken cancellationToken = default);
}
