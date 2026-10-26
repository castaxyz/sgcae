namespace SGCAE.Domain.Repositories;

/// <summary>
/// Contrato del patrón Unit of Work.
/// Coordina la persistencia de todos los cambios de la unidad de trabajo
/// como una sola transacción atómica.
/// La implementación concreta vive en SGCAE.Infrastructure (EF Core).
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Persiste todos los cambios rastreados en el contexto actual como una transacción.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación de la operación.</param>
    /// <returns>Número de entidades afectadas en la base de datos.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
