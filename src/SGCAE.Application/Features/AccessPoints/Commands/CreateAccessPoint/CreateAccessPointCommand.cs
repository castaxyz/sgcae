using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.AccessPoints.DTOs;

namespace SGCAE.Application.Features.AccessPoints.Commands.CreateAccessPoint;

/// <summary>Comando para registrar un nuevo punto de acceso físico en el sistema.</summary>
public sealed record CreateAccessPointCommand(
    string Code,
    string Name,
    string LocationDescription
) : IRequest<Result<AccessPointDto>>;
