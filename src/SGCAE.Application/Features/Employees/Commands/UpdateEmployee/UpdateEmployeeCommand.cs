using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.Employees.DTOs;

namespace SGCAE.Application.Features.Employees.Commands.UpdateEmployee;

/// <summary>Comando para actualizar los datos personales y laborales de un empleado.</summary>
public sealed record UpdateEmployeeCommand(
    Guid EmployeeId,
    string FirstName,
    string LastName,
    string Email,
    string JobTitle,
    string Department
) : IRequest<Result<EmployeeDto>>;
