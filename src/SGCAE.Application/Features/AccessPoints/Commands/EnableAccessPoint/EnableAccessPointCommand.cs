using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.AccessPoints.DTOs;

namespace SGCAE.Application.Features.AccessPoints.Commands.EnableAccessPoint;

/// <summary>Comando para habilitar un punto de acceso previamente deshabilitado.</summary>
public sealed record EnableAccessPointCommand(Guid Id) : IRequest<Result<AccessPointDto>>;
