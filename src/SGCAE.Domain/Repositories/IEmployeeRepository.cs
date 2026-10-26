using SGCAE.Domain.Entities;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Domain.Repositories;

/// <summary>
/// Contrato de persistencia para el agregado Employee.
/// Las implementaciones deben residir en SGCAE.Infrastructure.
/// </summary>
public interface IEmployeeRepository
{
    /// <summary>Obtiene un empleado por su identificador único.</summary>
    /// <returns>El empleado encontrado, o null si no existe.</returns>
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Obtiene un empleado por su número de documento.</summary>
    /// <returns>El empleado encontrado, o null si no existe.</returns>
    Task<Employee?> GetByDocumentAsync(DocumentNumber documentNumber, CancellationToken cancellationToken = default);

    /// <summary>Obtiene todos los empleados registrados en el sistema.</summary>
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Agrega un nuevo empleado al repositorio.</summary>
    Task AddAsync(Employee employee, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marca el empleado como modificado para que los cambios sean detectados.
    /// En implementaciones con EF Core, puede ser un no-op si el tracking está activo.
    /// </summary>
    void Update(Employee employee);
}
