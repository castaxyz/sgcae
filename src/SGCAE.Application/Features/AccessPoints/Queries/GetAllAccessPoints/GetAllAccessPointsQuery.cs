using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.AccessPoints.DTOs;

namespace SGCAE.Application.Features.AccessPoints.Queries.GetAllAccessPoints;

/// <summary>Query para obtener todos los puntos de acceso del sistema.</summary>
public sealed record GetAllAccessPointsQuery : IRequest<Result<IReadOnlyList<AccessPointDto>>>;
