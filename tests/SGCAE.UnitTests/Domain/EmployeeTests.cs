using FluentAssertions;
using SGCAE.Domain.Entities;
using SGCAE.Domain.Enums;
using SGCAE.Domain.Exceptions;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.UnitTests.Domain;

/// <summary>
/// Pruebas unitarias para el Aggregate Root Employee.
/// Verifica los invariantes y reglas de negocio del dominio (RF-01).
/// </summary>
public class EmployeeTests
{
    // ─── Helpers ────────────────────────────────────────────────────────────────

    private static Employee CreateActiveEmployee(
        string docNumber = "12345678",
        string firstName = "Juan",
        string lastName = "Pérez",
        string email = "juan.perez@empresa.com",
        string jobTitle = "Desarrollador",
        string department = "TI")
    {
        return Employee.Create(
            DocumentNumber.Create(docNumber),
            firstName,
            lastName,
            Email.Create(email),
            jobTitle,
            department);
    }

    // ─── Creación ────────────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithValidData_ShouldSetActiveStatus()
    {
        // Act
        var employee = CreateActiveEmployee();

        // Assert
        employee.EmploymentStatus.Should().Be(EmploymentStatus.Active);
    }

    [Fact]
    public void Create_WithValidData_ShouldBeActive()
    {
        // Act
        var employee = CreateActiveEmployee();

        // Assert
        employee.IsActive().Should().BeTrue();
    }

    [Fact]
    public void Create_WithValidData_ShouldGenerateNonEmptyId()
    {
        // Act
        var employee = CreateActiveEmployee();

        // Assert
        employee.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValidData_ShouldSetFullName()
    {
        // Act
        var employee = CreateActiveEmployee(firstName: "María", lastName: "López");

        // Assert
        employee.FullName.Should().Be("María López");
    }

    [Fact]
    public void Create_WithEmptyFirstName_ShouldThrowArgumentException()
    {
        // Act
        var act = () => Employee.Create(
            DocumentNumber.Create("12345678"),
            string.Empty,
            "Pérez",
            Email.Create("juan@empresa.com"),
            "Desarrollador",
            "TI");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("firstName");
    }

    [Fact]
    public void Create_WithEmptyJobTitle_ShouldThrowArgumentException()
    {
        // Act
        var act = () => Employee.Create(
            DocumentNumber.Create("12345678"),
            "Juan",
            "Pérez",
            Email.Create("juan@empresa.com"),
            string.Empty,
            "TI");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("jobTitle");
    }

    // ─── Desactivación ──────────────────────────────────────────────────────────

    [Fact]
    public void Deactivate_WhenActive_ShouldChangeStatusToInactive()
    {
        // Arrange
        var employee = CreateActiveEmployee();

        // Act
        employee.Deactivate();

        // Assert
        employee.EmploymentStatus.Should().Be(EmploymentStatus.Inactive);
    }

    [Fact]
    public void Deactivate_WhenActive_ShouldMakeIsActiveReturnFalse()
    {
        // Arrange
        var employee = CreateActiveEmployee();

        // Act
        employee.Deactivate();

        // Assert
        employee.IsActive().Should().BeFalse();
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ShouldThrowDomainException()
    {
        // Arrange
        var employee = CreateActiveEmployee();
        employee.Deactivate();

        // Act
        var act = () => employee.Deactivate();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*inactivo*");
    }

    // ─── Reactivación ────────────────────────────────────────────────────────────

    [Fact]
    public void Activate_WhenInactive_ShouldChangeStatusToActive()
    {
        // Arrange
        var employee = CreateActiveEmployee();
        employee.Deactivate();

        // Act
        employee.Activate();

        // Assert
        employee.EmploymentStatus.Should().Be(EmploymentStatus.Active);
        employee.IsActive().Should().BeTrue();
    }

    [Fact]
    public void Activate_WhenAlreadyActive_ShouldThrowDomainException()
    {
        // Arrange
        var employee = CreateActiveEmployee();

        // Act
        var act = () => employee.Activate();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*activo*");
    }

    // ─── Value Object Email ──────────────────────────────────────────────────────

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-an-email")]
    [InlineData("@domain.com")]
    [InlineData("user@")]
    [InlineData("user@domain")]
    [InlineData("user@.com")]
    public void Email_WithInvalidFormat_ShouldThrowInvalidEmailException(string invalidEmail)
    {
        // Act
        var act = () => Email.Create(invalidEmail);

        // Assert
        act.Should().Throw<InvalidEmailException>();
    }

    [Theory]
    [InlineData("user@domain.com")]
    [InlineData("User.Name+tag@sub.domain.org")]
    [InlineData("UPPERCASE@DOMAIN.COM")]
    public void Email_WithValidFormat_ShouldCreateSuccessfully(string validEmail)
    {
        // Act
        var email = Email.Create(validEmail);

        // Assert
        email.Should().NotBeNull();
        email.Value.Should().Be(validEmail.Trim().ToLowerInvariant());
    }

    [Fact]
    public void Email_WithValidEmail_ShouldNormalizeToLowerCase()
    {
        // Act
        var email = Email.Create("JUAN.PEREZ@EMPRESA.COM");

        // Assert
        email.Value.Should().Be("juan.perez@empresa.com");
    }
}
