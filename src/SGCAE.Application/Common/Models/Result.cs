namespace SGCAE.Application.Common.Models;

/// <summary>
/// Patrón Result genérico para estandarizar respuestas de los handlers.
/// Evita el uso de excepciones para flujos de negocio esperados.
/// </summary>
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public string ErrorMessage { get; }
    public int StatusCode { get; }

    private Result(bool isSuccess, T? value, string errorMessage, int statusCode)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
    }

    /// <summary>Crea un resultado exitoso con el valor retornado.</summary>
    public static Result<T> Success(T value, int statusCode = 200)
        => new(true, value, string.Empty, statusCode);

    /// <summary>Crea un resultado de fallo con mensaje de error.</summary>
    public static Result<T> Failure(string errorMessage, int statusCode = 400)
        => new(false, default, errorMessage, statusCode);

    /// <summary>Fallo por recurso no encontrado (404).</summary>
    public static Result<T> NotFound(string errorMessage)
        => new(false, default, errorMessage, 404);

    /// <summary>Fallo por conflicto de negocio (409).</summary>
    public static Result<T> Conflict(string errorMessage)
        => new(false, default, errorMessage, 409);
}

/// <summary>Result no genérico para comandos que no retornan valor.</summary>
public sealed class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string ErrorMessage { get; }
    public int StatusCode { get; }

    private Result(bool isSuccess, string errorMessage, int statusCode)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
    }

    public static Result Success(int statusCode = 204)
        => new(true, string.Empty, statusCode);

    public static Result Failure(string errorMessage, int statusCode = 400)
        => new(false, errorMessage, statusCode);

    public static Result NotFound(string errorMessage)
        => new(false, errorMessage, 404);

    public static Result Conflict(string errorMessage)
        => new(false, errorMessage, 409);
}
