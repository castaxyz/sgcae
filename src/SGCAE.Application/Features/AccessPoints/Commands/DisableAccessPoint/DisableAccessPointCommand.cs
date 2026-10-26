using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.AccessPoints.DTOs;

namespace SGCAE.Application.Features.AccessPoints.Commands.DisableAccessPoint;

/// <summary>Comando para deshabilitar un punto de acceso, bloqueando nuevos registros.</summary>
public sealed record DisableAccessPointCommand(Guid Id) : IRequest<Result<AccessPointDto>>;
