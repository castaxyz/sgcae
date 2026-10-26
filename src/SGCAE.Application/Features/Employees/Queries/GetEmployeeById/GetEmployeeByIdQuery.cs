using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.Employees.DTOs;

namespace SGCAE.Application.Features.Employees.Queries.GetEmployeeById;

/// <summary>Query para obtener un empleado por su Id.</summary>
public sealed record GetEmployeeByIdQuery(Guid EmployeeId) : IRequest<Result<EmployeeDto>>;
