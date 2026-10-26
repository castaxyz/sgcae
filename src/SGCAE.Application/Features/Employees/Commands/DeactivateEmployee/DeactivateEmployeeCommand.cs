using MediatR;
using SGCAE.Application.Common.Models;

namespace SGCAE.Application.Features.Employees.Commands.DeactivateEmployee;

/// <summary>Comando para desactivar a un empleado del sistema.</summary>
public sealed record DeactivateEmployeeCommand(Guid EmployeeId) : IRequest<Result>;
