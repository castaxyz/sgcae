using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.Credentials.DTOs;

namespace SGCAE.Application.Features.Credentials.Commands.AssignCredential;

/// <summary>
/// Comando para crear y asignar una nueva credencial RFID a un empleado.
/// Si EmployeeId es null, la credencial queda en inventario sin asignar.
/// </summary>
public sealed record AssignCredentialCommand(
    string Code,
    Guid EmployeeId
) : IRequest<Result<CredentialDto>>;
