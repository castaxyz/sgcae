using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.Employees.DTOs;

namespace SGCAE.Application.Features.Employees.Commands.CreateEmployee;

/// <summary>Comando para registrar un nuevo empleado en el sistema.</summary>
public sealed record CreateEmployeeCommand(
    string DocumentNumber,
    string FirstName,
    string LastName,
    string Email,
    string JobTitle,
    string Department
) : IRequest<Result<EmployeeDto>>;
