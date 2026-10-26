namespace SGCAE.Application.Features.Credentials.DTOs;

/// <summary>DTO de presentación para una credencial RFID.</summary>
public sealed record CredentialDto(
    Guid Id,
    string Code,
    Guid? EmployeeId,
    string Status,
    DateTime IssueDateUtc,
    string? BlockReason,
    string? RevocationReason,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);
