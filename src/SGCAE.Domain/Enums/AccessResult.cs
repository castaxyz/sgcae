namespace SGCAE.Domain.Enums;

/// <summary>Resultado de un intento de acceso en un punto de control.</summary>
public enum AccessResult
{
    /// <summary>El acceso fue concedido correctamente.</summary>
    Authorized = 1,

    /// <summary>El acceso fue denegado por alguna violación de regla de negocio.</summary>
    Rejected = 2
}
