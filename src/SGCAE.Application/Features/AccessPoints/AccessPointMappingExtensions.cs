using SGCAE.Application.Features.AccessPoints.DTOs;
using SGCAE.Domain.Entities;

namespace SGCAE.Application.Features.AccessPoints;

internal static class AccessPointMappingExtensions
{
    internal static AccessPointDto ToDto(this AccessPoint accessPoint) => new(
        accessPoint.Id,
        accessPoint.Code,
        accessPoint.Name,
        accessPoint.LocationDescription,
        accessPoint.IsEnabled,
        accessPoint.CreatedAtUtc,
        accessPoint.UpdatedAtUtc
    );
}
