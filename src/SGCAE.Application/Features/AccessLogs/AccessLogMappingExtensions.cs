using SGCAE.Application.Features.AccessLogs.DTOs;
using SGCAE.Domain.Entities;

namespace SGCAE.Application.Features.AccessLogs;

internal static class AccessLogMappingExtensions
{
    internal static AccessLogDto ToDto(this AccessLog accessLog) => new(
        accessLog.Id,
        accessLog.CredentialCode.Value,
        accessLog.EmployeeId,
        accessLog.AccessPointId,
        accessLog.MovementType.ToString(),
        accessLog.TimestampUtc,
        accessLog.AccessResult.ToString(),
        accessLog.RejectionReason
    );
}
