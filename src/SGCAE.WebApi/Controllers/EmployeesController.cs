using Microsoft.AspNetCore.Mvc;
using SGCAE.Application.Features.Employees.Commands.CreateEmployee;
using SGCAE.Application.Features.Employees.Commands.DeactivateEmployee;
using SGCAE.Application.Features.Employees.Commands.UpdateEmployee;
using SGCAE.Application.Features.Employees.DTOs;
using SGCAE.Application.Features.Employees.Queries.GetAllEmployees;
using SGCAE.Application.Features.Employees.Queries.GetEmployeeById;

namespace SGCAE.WebApi.Controllers;

/// <summary>
/// Controlador para la gestión integral de empleados (RF-01).
/// </summary>
[Route("api/employees")]
public sealed class EmployeesController : ApiControllerBase
{
    /// <summary>
    /// Registra un nuevo empleado en el sistema.
    /// </summary>
    /// <param name="command">Datos del nuevo empleado a registrar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Empleado creado con su identificador generado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateEmployee(
        [FromBody] CreateEmployeeCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetEmployeeById), new { id = result.Value!.Id }, result.Value);
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene el listado completo de todos los empleados.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Colección de empleados registrados.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllEmployees(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllEmployeesQuery(), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Consulta los datos de un empleado por su identificador único.
    /// </summary>
    /// <param name="id">Identificador único del empleado.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Datos del empleado si existe.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployeeById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetEmployeeByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Actualiza los datos personales y laborales de un empleado.
    /// </summary>
    /// <param name="id">Identificador único del empleado.</param>
    /// <param name="request">Datos actualizados del empleado.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Datos actualizados del empleado.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEmployee(
        Guid id,
        [FromBody] UpdateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateEmployeeCommand(
            id,
            request.FirstName,
            request.LastName,
            request.Email,
            request.JobTitle,
            request.Department);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Desactiva a un empleado impidiendo el uso de sus credenciales.
    /// </summary>
    /// <param name="id">Identificador del empleado a desactivar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DeactivateEmployee(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeactivateEmployeeCommand(id), cancellationToken);
        return HandleResult(result);
    }
}

/// <summary>
/// Modelo de solicitud para la actualización de un empleado.
/// </summary>
public sealed record UpdateEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    string JobTitle,
    string Department
);
