using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.AccessPoints.DTOs;
using SGCAE.Domain.Repositories;

namespace SGCAE.Application.Features.AccessPoints.Queries.GetAllAccessPoints;

public sealed class GetAllAccessPointsQueryHandler
    : IRequestHandler<GetAllAccessPointsQuery, Result<IReadOnlyList<AccessPointDto>>>
{
    private readonly IAccessPointRepository _accessPointRepository;

    public GetAllAccessPointsQueryHandler(IAccessPointRepository accessPointRepository)
        => _accessPointRepository = accessPointRepository;

    public async Task<Result<IReadOnlyList<AccessPointDto>>> Handle(
        GetAllAccessPointsQuery request,
        CancellationToken cancellationToken)
    {
        var accessPoints = await _accessPointRepository.GetAllAsync(cancellationToken);
        var dtos = accessPoints.Select(ap => ap.ToDto()).ToList().AsReadOnly();
        return Result<IReadOnlyList<AccessPointDto>>.Success(dtos);
    }
}
