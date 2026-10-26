using MediatR;
using Microsoft.AspNetCore.Mvc;
using SGCAE.Application.Common.Models;

namespace SGCAE.WebApi.Controllers;

/// <summary>
/// Controlador base que provee acceso centralizado a MediatR y estandarización de respuestas Result.
/// </summary>
[ApiController]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.StatusCode switch
            {
                StatusCodes.Status201Created => StatusCode(StatusCodes.Status201Created, result.Value),
                StatusCodes.Status204NoContent => NoContent(),
                _ => Ok(result.Value)
            };
        }

        return Problem(
            statusCode: result.StatusCode,
            title: GetTitleForStatusCode(result.StatusCode),
            detail: result.ErrorMessage,
            instance: HttpContext.Request.Path);
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return result.StatusCode switch
            {
                StatusCodes.Status204NoContent => NoContent(),
                _ => Ok()
            };
        }

        return Problem(
            statusCode: result.StatusCode,
            title: GetTitleForStatusCode(result.StatusCode),
            detail: result.ErrorMessage,
            instance: HttpContext.Request.Path);
    }

    private static string GetTitleForStatusCode(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => "Solicitud incorrecta",
        StatusCodes.Status404NotFound => "Recurso no encontrado",
        StatusCodes.Status409Conflict => "Conflicto con el estado actual del recurso",
        StatusCodes.Status422UnprocessableEntity => "Entidad no procesable",
        _ => "Error en la operación"
    };
}
