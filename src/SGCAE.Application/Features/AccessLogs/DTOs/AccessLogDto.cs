namespace SGCAE.Application.Features.AccessLogs.DTOs;

/// <summary>DTO de presentación para un registro de acceso auditado.</summary>
public sealed record AccessLogDto(
    Guid Id,
    string CredentialCode,
    Guid? EmployeeId,
    Guid AccessPointId,
    string MovementType,
    DateTime TimestampUtc,
    string AccessResult,
    string? RejectionReason
);
