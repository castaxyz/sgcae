namespace SGCAE.Application.Features.AccessPoints.DTOs;

/// <summary>DTO de presentación para un punto de acceso físico.</summary>
public sealed record AccessPointDto(
    Guid Id,
    string Code,
    string Name,
    string LocationDescription,
    bool IsEnabled,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);
