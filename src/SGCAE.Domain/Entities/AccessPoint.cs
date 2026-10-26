using SGCAE.Domain.Common;
using SGCAE.Domain.Exceptions;

namespace SGCAE.Domain.Entities;

/// <summary>
/// Aggregate Root que representa un punto de control de acceso físico
/// (ej. torniquete, puerta con lector, barrera).
/// Ejemplo: Code="REC-01", Name="Recepción Principal".
/// </summary>
public sealed class AccessPoint : AggregateRoot
{
    /// <summary>
    /// Código corto único del punto de acceso (ej. "REC-01", "BOD-01").
    /// Inmutable tras la creación.
    /// </summary>
    public string Code { get; private set; }

    /// <summary>Nombre descriptivo del punto de acceso (ej. "Recepción", "Bodega").</summary>
    public string Name { get; private set; }

    /// <summary>Descripción detallada de la ubicación física.</summary>
    public string LocationDescription { get; private set; }

    /// <summary>Indica si el punto de acceso está actualmente habilitado.</summary>
    public bool IsEnabled { get; private set; }

    // Constructor privado para ORM.
#pragma warning disable CS8618
    private AccessPoint() { }
#pragma warning restore CS8618

    private AccessPoint(string code, string name, string locationDescription) : base()
    {
        Code = code;
        Name = name;
        LocationDescription = locationDescription;
        IsEnabled = true;
    }

    /// <summary>
    /// Crea un nuevo punto de acceso habilitado por defecto.
    /// </summary>
    /// <param name="code">Código único (ej. "REC-01"). No puede estar vacío.</param>
    /// <param name="name">Nombre descriptivo. No puede estar vacío.</param>
    /// <param name="locationDescription">Descripción de la ubicación física.</param>
    /// <exception cref="ArgumentException">Si code o name están vacíos.</exception>
    public static AccessPoint Create(string code, string name, string locationDescription = "")
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("El código del punto de acceso no puede estar vacío.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del punto de acceso no puede estar vacío.", nameof(name));

        return new AccessPoint(code.Trim().ToUpperInvariant(), name.Trim(), locationDescription.Trim());
    }

    /// <summary>Habilita el punto de acceso para que pueda procesar entradas/salidas.</summary>
    /// <exception cref="DomainException">Si el punto ya está habilitado.</exception>
    public void Enable()
    {
        if (IsEnabled)
            throw new DomainException($"El punto de acceso '{Code}' ya está habilitado.");

        IsEnabled = true;
        MarkAsUpdated();
    }

    /// <summary>Deshabilita el punto de acceso bloqueando nuevos registros de acceso.</summary>
    /// <exception cref="DomainException">Si el punto ya está deshabilitado.</exception>
    public void Disable()
    {
        if (!IsEnabled)
            throw new DomainException($"El punto de acceso '{Code}' ya está deshabilitado.");

        IsEnabled = false;
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza el nombre y descripción del punto de acceso.
    /// El código es inmutable para preservar la integridad de los registros históricos.
    /// </summary>
    public void UpdateDetails(string name, string locationDescription)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del punto de acceso no puede estar vacío.", nameof(name));

        Name = name.Trim();
        LocationDescription = locationDescription.Trim();
        MarkAsUpdated();
    }

    /// <summary>Indica si el punto de acceso está disponible para procesar accesos.</summary>
    public bool IsAvailable() => IsEnabled;
}
