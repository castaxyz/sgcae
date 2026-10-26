namespace SGCAE.Application.Features.Employees.DTOs;

/// <summary>DTO de presentación para un empleado.</summary>
public sealed record EmployeeDto(
    Guid Id,
    string DocumentNumber,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string JobTitle,
    string Department,
    string EmploymentStatus,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);
