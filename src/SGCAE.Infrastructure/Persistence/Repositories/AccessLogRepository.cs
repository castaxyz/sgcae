using Microsoft.EntityFrameworkCore;
using SGCAE.Domain.Entities;
using SGCAE.Domain.Enums;
using SGCAE.Domain.Repositories;
using SGCAE.Infrastructure.Persistence;

namespace SGCAE.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación de EF Core para IAccessLogRepository.
/// Maneja el registro append-only y consultas paginadas con filtros (RF-03, RF-04).
/// </summary>
public sealed class AccessLogRepository : IAccessLogRepository
{
    private readonly ApplicationDbContext _context;

    public AccessLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AccessLog accessLog, CancellationToken cancellationToken = default)
    {
        await _context.AccessLogs.AddAsync(accessLog, cancellationToken);
    }

    public async Task<IReadOnlyList<AccessLog>> GetFilteredAsync(
        Guid? employeeId,
        Guid? accessPointId,
        DateTime? fromDate,
        DateTime? toDate,
        AccessResult? accessResult,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AccessLogs.AsNoTracking();

        query = ApplyFilters(query, employeeId, accessPointId, fromDate, toDate, accessResult);

        var skip = (pageNumber - 1) * pageSize;

        return await query
            .OrderByDescending(l => l.TimestampUtc)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(
        Guid? employeeId,
        Guid? accessPointId,
        DateTime? fromDate,
        DateTime? toDate,
        AccessResult? accessResult,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AccessLogs.AsNoTracking();

        query = ApplyFilters(query, employeeId, accessPointId, fromDate, toDate, accessResult);

        return await query.CountAsync(cancellationToken);
    }

    private static IQueryable<AccessLog> ApplyFilters(
        IQueryable<AccessLog> query,
        Guid? employeeId,
        Guid? accessPointId,
        DateTime? fromDate,
        DateTime? toDate,
        AccessResult? accessResult)
    {
        if (employeeId.HasValue)
        {
            query = query.Where(l => l.EmployeeId == employeeId.Value);
        }

        if (accessPointId.HasValue)
        {
            query = query.Where(l => l.AccessPointId == accessPointId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(l => l.TimestampUtc >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(l => l.TimestampUtc <= toDate.Value);
        }

        if (accessResult.HasValue)
        {
            query = query.Where(l => l.AccessResult == accessResult.Value);
        }

        return query;
    }
}
