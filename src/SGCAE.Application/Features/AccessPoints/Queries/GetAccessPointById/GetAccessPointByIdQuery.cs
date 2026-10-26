using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.AccessPoints.DTOs;

namespace SGCAE.Application.Features.AccessPoints.Queries.GetAccessPointById;

/// <summary>Query para obtener un punto de acceso por su identificador único.</summary>
public sealed record GetAccessPointByIdQuery(Guid Id) : IRequest<Result<AccessPointDto>>;
