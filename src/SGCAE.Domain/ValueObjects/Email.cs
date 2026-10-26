using System.Text.RegularExpressions;
using SGCAE.Domain.Common;
using SGCAE.Domain.Exceptions;

namespace SGCAE.Domain.ValueObjects;

/// <summary>
/// Value Object inmutable que representa una dirección de correo electrónico válida.
/// Valida formato según RFC 5322 simplificado y normaliza a minúsculas.
/// </summary>
public sealed class Email : ValueObject
{
    // Expresión regular simplificada pero robusta para correos electrónicos.
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase,
        TimeSpan.FromMilliseconds(250));

    /// <summary>Valor normalizado del correo electrónico (siempre en minúsculas).</summary>
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Crea un Email validado y normalizado.
    /// </summary>
    /// <exception cref="InvalidEmailException">Si el valor es nulo, vacío o no tiene formato válido.</exception>
    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidEmailException(value ?? string.Empty);

        var normalized = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(normalized))
            throw new InvalidEmailException(value);

        return new Email(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
