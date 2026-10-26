using SGCAE.Application.Features.Credentials.DTOs;
using SGCAE.Domain.Entities;

namespace SGCAE.Application.Features.Credentials;

internal static class CredentialMappingExtensions
{
    internal static CredentialDto ToDto(this Credential credential) => new(
        credential.Id,
        credential.Code.Value,
        credential.EmployeeId,
        credential.Status.ToString(),
        credential.IssueDateUtc,
        credential.BlockReason,
        credential.RevocationReason,
        credential.CreatedAtUtc,
        credential.UpdatedAtUtc
    );
}
