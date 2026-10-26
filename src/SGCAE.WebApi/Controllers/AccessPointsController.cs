using Microsoft.AspNetCore.Mvc;
using SGCAE.Application.Features.AccessPoints.Commands.CreateAccessPoint;
using SGCAE.Application.Features.AccessPoints.Commands.DisableAccessPoint;
using SGCAE.Application.Features.AccessPoints.Commands.EnableAccessPoint;
using SGCAE.Application.Features.AccessPoints.DTOs;
using SGCAE.Application.Features.AccessPoints.Queries.GetAllAccessPoints;
using SGCAE.Application.Features.AccessPoints.Queries.GetAccessPointById;

namespace SGCAE.WebApi.Controllers;

/// <summary>
/// Controlador para la administración de puntos de acceso físico (RF-06).
/// </summary>
[Route("api/access-points")]
public sealed class AccessPointsController : ApiControllerBase
{
    /// <summary>
    /// Registra un nuevo punto de acceso físico en el sistema.
    /// </summary>
    /// <param name="command">Datos del punto de acceso (código, nombre, descripción de ubicación).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Punto de acceso creado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AccessPointDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAccessPoint(
        [FromBody] CreateAccessPointCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetAccessPointById),
                new { id = result.Value!.Id },
                result.Value);
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene el listado de todos los puntos de acceso físico existentes.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Colección de puntos de acceso.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AccessPointDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAccessPoints(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllAccessPointsQuery(), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Consulta los datos de un punto de acceso por su identificador único.
    /// </summary>
    /// <param name="id">Identificador único del punto de acceso.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Datos del punto de acceso.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AccessPointDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccessPointById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAccessPointByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Alterna, habilita o inhabilita el estado de un punto de acceso.
    /// Si no se especifica el cuerpo de la solicitud, conmuta automáticamente el estado actual.
    /// </summary>
    /// <param name="id">Identificador del punto de acceso.</param>
    /// <param name="request">Opcional: indicador explícito de habilitar (true) o inhabilitar (false).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Punto de acceso con su nuevo estado.</returns>
    [HttpPatch("{id:guid}/toggle-status")]
    [ProducesResponseType(typeof(AccessPointDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ToggleAccessPointStatus(
        Guid id,
        [FromBody] ToggleAccessPointStatusRequest? request,
        CancellationToken cancellationToken)
    {
        if (request?.Enable.HasValue == true)
        {
            if (request.Enable.Value)
            {
                var enableResult = await Mediator.Send(new EnableAccessPointCommand(id), cancellationToken);
                return HandleResult(enableResult);
            }
            else
            {
                var disableResult = await Mediator.Send(new DisableAccessPointCommand(id), cancellationToken);
                return HandleResult(disableResult);
            }
        }

        // Si no se especifica explícitamente, alternamos según el estado actual
        var current = await Mediator.Send(new GetAccessPointByIdQuery(id), cancellationToken);
        if (current.IsFailure || current.Value is null)
        {
            return HandleResult(current);
        }

        if (current.Value.IsEnabled)
        {
            var disableResult = await Mediator.Send(new DisableAccessPointCommand(id), cancellationToken);
            return HandleResult(disableResult);
        }
        else
        {
            var enableResult = await Mediator.Send(new EnableAccessPointCommand(id), cancellationToken);
            return HandleResult(enableResult);
        }
    }
}

/// <summary>
/// Modelo de solicitud opcional para definir explícitamente el nuevo estado del punto de acceso.
/// </summary>
public sealed record ToggleAccessPointStatusRequest(bool? Enable = null);
