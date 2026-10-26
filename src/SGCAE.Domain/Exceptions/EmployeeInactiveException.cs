namespace SGCAE.Domain.Exceptions;

/// <summary>
/// Se lanza cuando se intenta realizar una operación que requiere
/// que el empleado esté activo, pero se encuentra inactivo.
/// </summary>
public sealed class EmployeeInactiveException : DomainException
{
    public Guid EmployeeId { get; }

    public EmployeeInactiveException(Guid employeeId)
        : base($"El empleado con Id '{employeeId}' está inactivo y no puede realizar esta operación.")
    {
        EmployeeId = employeeId;
    }
}
