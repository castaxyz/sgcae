using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.Employees.DTOs;

namespace SGCAE.Application.Features.Employees.Queries.GetAllEmployees;

/// <summary>Query para obtener todos los empleados del sistema.</summary>
public sealed record GetAllEmployeesQuery : IRequest<Result<IReadOnlyList<EmployeeDto>>>;
