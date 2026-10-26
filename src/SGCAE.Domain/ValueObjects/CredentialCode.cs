using System.Text.RegularExpressions;
using SGCAE.Domain.Common;
using SGCAE.Domain.Exceptions;

namespace SGCAE.Domain.ValueObjects;

/// <summary>
/// Value Object inmutable que representa el código alfanumérico único de una
/// tarjeta/credencial RFID (ej. "A1B2C3D4", "RFID-001-2024").
/// Acepta letras, dígitos y guiones, entre 4 y 64 caracteres.
/// Normaliza a mayúsculas para comparaciones consistentes.
/// </summary>
public sealed class CredentialCode : ValueObject
{
    private const int MinLength = 4;
    private const int MaxLength = 64;

    private static readonly Regex CodeRegex = new(
        @"^[A-Za-z0-9\-]{4,64}$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100));

    /// <summary>Valor del código de credencial normalizado en mayúsculas.</summary>
    public string Value { get; }

    private CredentialCode(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Crea un CredentialCode validado y normalizado a mayúsculas.
    /// </summary>
    /// <exception cref="InvalidCredentialCodeException">
    /// Si el valor es nulo, vacío, contiene caracteres no permitidos,
    /// o no cumple el rango de longitud.
    /// </exception>
    public static CredentialCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidCredentialCodeException(value ?? string.Empty);

        var trimmed = value.Trim();

        if (!CodeRegex.IsMatch(trimmed))
            throw new InvalidCredentialCodeException(trimmed);

        return new CredentialCode(trimmed.ToUpperInvariant());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
