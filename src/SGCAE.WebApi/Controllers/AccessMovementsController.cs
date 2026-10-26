using Microsoft.AspNetCore.Mvc;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.AccessLogs.Commands.RegisterAccess;
using SGCAE.Application.Features.AccessLogs.DTOs;
using SGCAE.Application.Features.AccessLogs.Queries.GetAccessHistory;

namespace SGCAE.WebApi.Controllers;

/// <summary>
/// Controlador para el registro de movimientos y consulta del historial de accesos (RF-03 &amp; RF-04).
/// </summary>
[Route("api/access")]
public sealed class AccessMovementsController : ApiControllerBase
{
    /// <summary>
    /// Registra un intento de movimiento de acceso (entrada o salida) validando credencial, punto de acceso y empleado.
    /// Devuelve 200 OK tanto si el acceso fue Autorizado como si fue Rechazado por reglas de negocio, con el motivo detallado.
    /// </summary>
    /// <param name="request">Código de credencial, punto de acceso y tipo de movimiento (Entry/Exit).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Registro de acceso con el resultado obtenido (Authorized / Rejected) y el motivo en caso de rechazo.</returns>
    [HttpPost("movements")]
    [ProducesResponseType(typeof(AccessLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegisterMovement(
        [FromBody] RegisterAccessMovementRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterAccessCommand(
            request.CredentialCode,
            request.AccessPointId,
            request.MovementType);

        var result = await Mediator.Send(command, cancellationToken);
        if (result.IsSuccess)
        {
            // Según requerimiento: HTTP 200 OK con el resultado de la operación
            return Ok(result.Value);
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Consulta el historial de accesos aplicando filtros multicriterio y paginación (RF-04).
    /// </summary>
    /// <param name="employeeId">Filtro opcional por identificador de empleado.</param>
    /// <param name="accessPointId">Filtro opcional por punto de acceso.</param>
    /// <param name="fromDate">Filtro opcional fecha inicial UTC (inclusive).</param>
    /// <param name="toDate">Filtro opcional fecha final UTC (inclusive).</param>
    /// <param name="accessResult">Filtro opcional por resultado (Authorized / Rejected).</param>
    /// <param name="pageNumber">Número de página (por defecto 1).</param>
    /// <param name="pageSize">Tamaño de página (por defecto 10).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Página con registros de acceso e información de paginación.</returns>
    [HttpGet("history")]
    [ProducesResponseType(typeof(PagedResult<AccessLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAccessHistory(
        [FromQuery] Guid? employeeId,
        [FromQuery] Guid? accessPointId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? accessResult,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAccessHistoryQuery(
            employeeId,
            accessPointId,
            fromDate,
            toDate,
            accessResult,
            pageNumber,
            pageSize);

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}

/// <summary>
/// Modelo de solicitud para registrar un intento de acceso.
/// </summary>
public sealed record RegisterAccessMovementRequest(
    string CredentialCode,
    Guid AccessPointId,
    string MovementType
);
