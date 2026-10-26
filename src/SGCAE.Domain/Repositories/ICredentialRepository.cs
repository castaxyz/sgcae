using SGCAE.Domain.Entities;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Domain.Repositories;

/// <summary>
/// Contrato de persistencia para el agregado Credential.
/// Las implementaciones deben residir en SGCAE.Infrastructure.
/// </summary>
public interface ICredentialRepository
{
    /// <summary>Obtiene una credencial por su código RFID único.</summary>
    /// <returns>La credencial encontrada, o null si no existe.</returns>
    Task<Credential?> GetByCodeAsync(CredentialCode code, CancellationToken cancellationToken = default);

    /// <summary>Obtiene todas las credenciales asignadas a un empleado específico.</summary>
    Task<IReadOnlyList<Credential>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);

    /// <summary>Agrega una nueva credencial al repositorio.</summary>
    Task AddAsync(Credential credential, CancellationToken cancellationToken = default);

    /// <summary>Marca la credencial como modificada para que los cambios sean persistidos.</summary>
    void Update(Credential credential);
}
