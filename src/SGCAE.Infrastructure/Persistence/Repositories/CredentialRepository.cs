using Microsoft.EntityFrameworkCore;
using SGCAE.Domain.Entities;
using SGCAE.Domain.Repositories;
using SGCAE.Domain.ValueObjects;
using SGCAE.Infrastructure.Persistence;

namespace SGCAE.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación de EF Core para ICredentialRepository.
/// </summary>
public sealed class CredentialRepository : ICredentialRepository
{
    private readonly ApplicationDbContext _context;

    public CredentialRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Credential?> GetByCodeAsync(CredentialCode code, CancellationToken cancellationToken = default)
    {
        return await _context.Credentials
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
    }

    public async Task<IReadOnlyList<Credential>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.Credentials
            .Where(c => c.EmployeeId == employeeId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Credential credential, CancellationToken cancellationToken = default)
    {
        await _context.Credentials.AddAsync(credential, cancellationToken);
    }

    public void Update(Credential credential)
    {
        _context.Credentials.Update(credential);
    }

    public Task UpdateAsync(Credential credential, CancellationToken cancellationToken = default)
    {
        Update(credential);
        return Task.CompletedTask;
    }
}
