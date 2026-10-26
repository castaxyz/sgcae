namespace SGCAE.Domain.Enums;

/// <summary>Estado laboral de un empleado en el sistema.</summary>
public enum EmploymentStatus
{
    /// <summary>El empleado se encuentra activo y puede operar en el sistema.</summary>
    Active = 1,

    /// <summary>El empleado ha sido desactivado y no puede utilizar sus credenciales.</summary>
    Inactive = 2
}
