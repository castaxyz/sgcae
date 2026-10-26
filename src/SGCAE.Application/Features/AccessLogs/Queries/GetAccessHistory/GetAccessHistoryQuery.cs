using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.AccessLogs.DTOs;
using SGCAE.Domain.Enums;

namespace SGCAE.Application.Features.AccessLogs.Queries.GetAccessHistory;

/// <summary>
/// Query para consultar el historial de accesos con filtros opcionales y paginación.
/// Cumple con el requisito RF-04.
/// </summary>
public sealed record GetAccessHistoryQuery(
    Guid? EmployeeId,
    Guid? AccessPointId,
    DateTime? FromDate,
    DateTime? ToDate,
    string? AccessResult,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<Result<PagedResult<AccessLogDto>>>;
