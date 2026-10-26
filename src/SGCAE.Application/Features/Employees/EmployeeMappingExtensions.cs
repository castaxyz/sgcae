using SGCAE.Application.Features.Employees.DTOs;
using SGCAE.Domain.Entities;

namespace SGCAE.Application.Features.Employees;

internal static class EmployeeMappingExtensions
{
    internal static EmployeeDto ToDto(this Employee employee) => new(
        employee.Id,
        employee.DocumentNumber.Value,
        employee.FirstName,
        employee.LastName,
        employee.FullName,
        employee.Email.Value,
        employee.JobTitle,
        employee.Department,
        employee.EmploymentStatus.ToString(),
        employee.CreatedAtUtc,
        employee.UpdatedAtUtc
    );
}
