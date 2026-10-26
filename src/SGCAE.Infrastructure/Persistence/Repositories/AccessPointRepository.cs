using Microsoft.EntityFrameworkCore;
using SGCAE.Domain.Entities;
using SGCAE.Domain.Repositories;
using SGCAE.Infrastructure.Persistence;

namespace SGCAE.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación de EF Core para IAccessPointRepository.
/// </summary>
public sealed class AccessPointRepository : IAccessPointRepository
{
    private readonly ApplicationDbContext _context;

    public AccessPointRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AccessPoint?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AccessPoints
            .FirstOrDefaultAsync(ap => ap.Id == id, cancellationToken);
    }

    public async Task<AccessPoint?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return await _context.AccessPoints
            .FirstOrDefaultAsync(ap => ap.Code == normalizedCode, cancellationToken);
    }

    public async Task<IReadOnlyList<AccessPoint>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AccessPoints
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(AccessPoint accessPoint, CancellationToken cancellationToken = default)
    {
        await _context.AccessPoints.AddAsync(accessPoint, cancellationToken);
    }

    public void Update(AccessPoint accessPoint)
    {
        _context.AccessPoints.Update(accessPoint);
    }

    public Task UpdateAsync(AccessPoint accessPoint, CancellationToken cancellationToken = default)
    {
        Update(accessPoint);
        return Task.CompletedTask;
    }
}
