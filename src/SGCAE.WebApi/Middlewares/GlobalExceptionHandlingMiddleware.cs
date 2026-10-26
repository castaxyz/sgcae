using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SGCAE.Domain.Exceptions;

namespace SGCAE.WebApi.Middlewares;

/// <summary>
/// Middleware global para captura de excepciones y generación de respuestas estandarizadas RFC 7807 (ProblemDetails).
/// </summary>
public sealed class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        ProblemDetails problemDetails;

        switch (exception)
        {
            case SGCAE.Application.Common.Exceptions.ValidationException appValEx:
                _logger.LogWarning(appValEx, "Error de validación en la ruta {Path}", path);
                problemDetails = new HttpValidationProblemDetails(appValEx.Errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Error de validación",
                    Detail = appValEx.Message,
                    Instance = path
                };
                break;

            case FluentValidation.ValidationException fvEx:
                _logger.LogWarning(fvEx, "Error de validación de FluentValidation en la ruta {Path}", path);
                var errors = fvEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());

                problemDetails = new HttpValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Error de validación",
                    Detail = "Se produjeron uno o más errores de validación.",
                    Instance = path
                };
                break;

            case DomainException domainEx:
                _logger.LogWarning(domainEx, "Violación de regla de dominio en la ruta {Path}", path);
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Title = "Regla de negocio no satisfecha",
                    Detail = domainEx.Message,
                    Instance = path
                };
                break;

            case InvalidOperationException invOpEx:
                _logger.LogWarning(invOpEx, "Operación inválida en la ruta {Path}", path);
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Title = "Operación inválida",
                    Detail = invOpEx.Message,
                    Instance = path
                };
                break;

            case KeyNotFoundException notFoundEx:
                _logger.LogWarning(notFoundEx, "Recurso no encontrado en la ruta {Path}", path);
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Recurso no encontrado",
                    Detail = notFoundEx.Message,
                    Instance = path
                };
                break;

            default:
                _logger.LogError(exception, "Error no controlado al procesar la solicitud en {Path}", path);
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Error interno del servidor",
                    Detail = "Ha ocurrido un error inesperado al procesar la solicitud.",
                    Instance = path
                };
                break;
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        await JsonSerializer.SerializeAsync(context.Response.Body, problemDetails, problemDetails.GetType(), JsonOptions);
    }
}

public static class GlobalExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}
