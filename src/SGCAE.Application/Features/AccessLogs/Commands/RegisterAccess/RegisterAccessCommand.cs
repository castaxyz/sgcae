using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.AccessLogs.DTOs;

namespace SGCAE.Application.Features.AccessLogs.Commands.RegisterAccess;

/// <summary>
/// Comando para registrar un intento de acceso (entrada o salida) en el sistema.
/// Valida la credencial, el empleado y el punto de acceso, y persiste el resultado.
/// Cumple con el requisito RF-03.
/// </summary>
public sealed record RegisterAccessCommand(
    string CredentialCode,
    Guid AccessPointId,
    string MovementType
) : IRequest<Result<AccessLogDto>>;
