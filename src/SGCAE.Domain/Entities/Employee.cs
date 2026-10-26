using SGCAE.Domain.Common;
using SGCAE.Domain.Enums;
using SGCAE.Domain.Exceptions;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Domain.Entities;

/// <summary>
/// Aggregate Root que representa a un empleado de la organización.
/// Contiene las reglas de negocio relacionadas con el ciclo de vida del empleado
/// y es el punto de entrada para todas las mutaciones de estado.
/// </summary>
public sealed class Employee : AggregateRoot
{
    /// <summary>Número de documento de identidad del empleado.</summary>
    public DocumentNumber DocumentNumber { get; private set; }

    /// <summary>Nombre(s) del empleado.</summary>
    public string FirstName { get; private set; }

    /// <summary>Apellido(s) del empleado.</summary>
    public string LastName { get; private set; }

    /// <summary>Correo electrónico corporativo.</summary>
    public Email Email { get; private set; }

    /// <summary>Cargo o puesto de trabajo.</summary>
    public string JobTitle { get; private set; }

    /// <summary>Departamento o área a la que pertenece.</summary>
    public string Department { get; private set; }

    /// <summary>Estado laboral actual del empleado.</summary>
    public EmploymentStatus EmploymentStatus { get; private set; }

    // Constructor privado para ORM (EF Core vía reflection).
    // EF Core necesita un constructor sin parámetros o con parámetros que coincidan con los nombres de columna.
#pragma warning disable CS8618
    private Employee() { }
#pragma warning restore CS8618

    private Employee(
        DocumentNumber documentNumber,
        string firstName,
        string lastName,
        Email email,
        string jobTitle,
        string department) : base()
    {
        DocumentNumber = documentNumber;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        JobTitle = jobTitle;
        Department = department;
        EmploymentStatus = EmploymentStatus.Active;
    }

    /// <summary>
    /// Método de fábrica para crear un nuevo empleado.
    /// Garantiza que todos los invariantes se cumplan desde el momento de la creación.
    /// </summary>
    /// <exception cref="ArgumentException">Si nombre, apellido, cargo o departamento están vacíos.</exception>
    public static Employee Create(
        DocumentNumber documentNumber,
        string firstName,
        string lastName,
        Email email,
        string jobTitle,
        string department)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("El apellido no puede estar vacío.", nameof(lastName));

        if (string.IsNullOrWhiteSpace(jobTitle))
            throw new ArgumentException("El cargo no puede estar vacío.", nameof(jobTitle));

        if (string.IsNullOrWhiteSpace(department))
            throw new ArgumentException("El departamento no puede estar vacío.", nameof(department));

        return new Employee(documentNumber, firstName.Trim(), lastName.Trim(), email, jobTitle.Trim(), department.Trim());
    }

    /// <summary>
    /// Actualiza los datos personales y laborales del empleado.
    /// </summary>
    public void UpdateDetails(
        string firstName,
        string lastName,
        Email email,
        string jobTitle,
        string department)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("El apellido no puede estar vacío.", nameof(lastName));

        if (string.IsNullOrWhiteSpace(jobTitle))
            throw new ArgumentException("El cargo no puede estar vacío.", nameof(jobTitle));

        if (string.IsNullOrWhiteSpace(department))
            throw new ArgumentException("El departamento no puede estar vacío.", nameof(department));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email;
        JobTitle = jobTitle.Trim();
        Department = department.Trim();

        MarkAsUpdated();
    }

    /// <summary>Desactiva al empleado impidiendo el uso de sus credenciales.</summary>
    /// <exception cref="DomainException">Si el empleado ya está inactivo.</exception>
    public void Deactivate()
    {
        if (EmploymentStatus == EmploymentStatus.Inactive)
            throw new DomainException($"El empleado '{Id}' ya se encuentra inactivo.");

        EmploymentStatus = EmploymentStatus.Inactive;
        MarkAsUpdated();
    }

    /// <summary>Reactiva al empleado permitiendo el uso de sus credenciales.</summary>
    /// <exception cref="DomainException">Si el empleado ya está activo.</exception>
    public void Activate()
    {
        if (EmploymentStatus == EmploymentStatus.Active)
            throw new DomainException($"El empleado '{Id}' ya se encuentra activo.");

        EmploymentStatus = EmploymentStatus.Active;
        MarkAsUpdated();
    }

    /// <summary>Indica si el empleado está activo.</summary>
    public bool IsActive() => EmploymentStatus == EmploymentStatus.Active;

    /// <summary>Nombre completo del empleado (firstName + lastName).</summary>
    public string FullName => $"{FirstName} {LastName}";
}
