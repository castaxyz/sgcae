using Microsoft.AspNetCore.Mvc;
using SGCAE.Application.Features.Credentials.Commands.AssignCredential;
using SGCAE.Application.Features.Credentials.Commands.BlockCredential;
using SGCAE.Application.Features.Credentials.Commands.RevokeCredential;
using SGCAE.Application.Features.Credentials.DTOs;
using SGCAE.Application.Features.Credentials.Queries.GetCredentialByCode;
using SGCAE.Application.Features.Credentials.Queries.GetCredentialsByEmployee;

namespace SGCAE.WebApi.Controllers;

/// <summary>
/// Controlador para el control del ciclo de vida de las credenciales RFID (RF-02).
/// </summary>
[Route("api/credentials")]
public sealed class CredentialsController : ApiControllerBase
{
    /// <summary>
    /// Crea y asigna una credencial RFID a un empleado.
    /// </summary>
    /// <param name="command">Código RFID e Id del empleado asignado.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Datos de la credencial creada y asignada.</returns>
    [HttpPost("assign")]
    [ProducesResponseType(typeof(CredentialDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignCredential(
        [FromBody] AssignCredentialCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetCredentialByCode),
                new { code = result.Value!.Code },
                result.Value);
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Consulta la información y estado de una credencial por su código RFID único.
    /// </summary>
    /// <param name="code">Código de la credencial (ej. "CARD-ACT-001").</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Información de la credencial.</returns>
    [HttpGet("{code}")]
    [ProducesResponseType(typeof(CredentialDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCredentialByCode(string code, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetCredentialByCodeQuery(code), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Consulta todas las credenciales asociadas a un empleado específico.
    /// </summary>
    /// <param name="employeeId">Identificador único del empleado.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Listado de credenciales del empleado.</returns>
    [HttpGet("employee/{employeeId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<CredentialDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCredentialsByEmployee(Guid employeeId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetCredentialsByEmployeeQuery(employeeId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Bloquea temporalmente una credencial por pérdida o sospecha de mal uso.
    /// </summary>
    /// <param name="code">Código de la credencial a bloquear.</param>
    /// <param name="request">Motivo obligatorio del bloqueo.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Credencial actualizada en estado Blocked.</returns>
    [HttpPatch("{code}/block")]
    [ProducesResponseType(typeof(CredentialDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> BlockCredential(
        string code,
        [FromBody] BlockCredentialRequest request,
        CancellationToken cancellationToken)
    {
        var command = new BlockCredentialCommand(code, request.Reason);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Revoca permanentemente una credencial (estado terminal e irreversible).
    /// </summary>
    /// <param name="code">Código de la credencial a revocar.</param>
    /// <param name="request">Motivo obligatorio de la revocación.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Credencial actualizada en estado Revoked.</returns>
    [HttpPatch("{code}/revoke")]
    [ProducesResponseType(typeof(CredentialDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RevokeCredential(
        string code,
        [FromBody] RevokeCredentialRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RevokeCredentialCommand(code, request.Reason);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}

/// <summary>
/// Modelo de solicitud para bloquear una credencial.
/// </summary>
public sealed record BlockCredentialRequest(string Reason);

/// <summary>
/// Modelo de solicitud para revocar una credencial.
/// </summary>
public sealed record RevokeCredentialRequest(string Reason);
