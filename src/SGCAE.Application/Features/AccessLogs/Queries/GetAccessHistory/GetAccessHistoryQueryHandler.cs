using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.AccessLogs.DTOs;
using SGCAE.Domain.Enums;
using SGCAE.Domain.Repositories;

namespace SGCAE.Application.Features.AccessLogs.Queries.GetAccessHistory;

public sealed class GetAccessHistoryQueryHandler
    : IRequestHandler<GetAccessHistoryQuery, Result<PagedResult<AccessLogDto>>>
{
    private readonly IAccessLogRepository _accessLogRepository;

    public GetAccessHistoryQueryHandler(IAccessLogRepository accessLogRepository)
        => _accessLogRepository = accessLogRepository;

    public async Task<Result<PagedResult<AccessLogDto>>> Handle(
        GetAccessHistoryQuery request,
        CancellationToken cancellationToken)
    {
        // Parsear el filtro de resultado de acceso si se proporcionó
        AccessResult? accessResultFilter = null;
        if (!string.IsNullOrWhiteSpace(request.AccessResult))
            accessResultFilter = Enum.Parse<AccessResult>(request.AccessResult, ignoreCase: true);

        // Obtener el total de registros para la metadata de paginación
        var totalCount = await _accessLogRepository.GetTotalCountAsync(
            request.EmployeeId,
            request.AccessPointId,
            request.FromDate,
            request.ToDate,
            accessResultFilter,
            cancellationToken);

        if (totalCount == 0)
            return Result<PagedResult<AccessLogDto>>.Success(
                PagedResult<AccessLogDto>.Empty(request.PageNumber, request.PageSize));

        // Obtener los registros de la página solicitada
        var logs = await _accessLogRepository.GetFilteredAsync(
            request.EmployeeId,
            request.AccessPointId,
            request.FromDate,
            request.ToDate,
            accessResultFilter,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = logs.Select(l => l.ToDto()).ToList().AsReadOnly();
        var pagedResult = new PagedResult<AccessLogDto>(dtos, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<AccessLogDto>>.Success(pagedResult);
    }
}
