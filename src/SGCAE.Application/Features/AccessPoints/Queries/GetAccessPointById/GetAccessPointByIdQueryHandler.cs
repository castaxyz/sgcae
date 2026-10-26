using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.AccessPoints.DTOs;
using SGCAE.Domain.Repositories;

namespace SGCAE.Application.Features.AccessPoints.Queries.GetAccessPointById;

public sealed class GetAccessPointByIdQueryHandler
    : IRequestHandler<GetAccessPointByIdQuery, Result<AccessPointDto>>
{
    private readonly IAccessPointRepository _accessPointRepository;

    public GetAccessPointByIdQueryHandler(IAccessPointRepository accessPointRepository)
        => _accessPointRepository = accessPointRepository;

    public async Task<Result<AccessPointDto>> Handle(
        GetAccessPointByIdQuery request,
        CancellationToken cancellationToken)
    {
        var accessPoint = await _accessPointRepository.GetByIdAsync(request.Id, cancellationToken);
        if (accessPoint is null)
            return Result<AccessPointDto>.NotFound(
                $"No se encontró el punto de acceso con Id '{request.Id}'.");

        return Result<AccessPointDto>.Success(accessPoint.ToDto());
    }
}
