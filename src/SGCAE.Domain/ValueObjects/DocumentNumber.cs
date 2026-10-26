using System.Text.RegularExpressions;
using SGCAE.Domain.Common;
using SGCAE.Domain.Exceptions;

namespace SGCAE.Domain.ValueObjects;

/// <summary>
/// Value Object inmutable que representa el número de identificación de un empleado
/// (cédula, pasaporte, NIT, etc.).
/// Solo acepta dígitos y longitud entre 5 y 20 caracteres.
/// </summary>
public sealed class DocumentNumber : ValueObject
{
    private const int MinLength = 5;
    private const int MaxLength = 20;

    private static readonly Regex DocumentRegex = new(
        @"^\d{5,20}$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100));

    /// <summary>Valor del número de documento.</summary>
    public string Value { get; }

    private DocumentNumber(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Crea un DocumentNumber validado.
    /// </summary>
    /// <exception cref="InvalidDocumentNumberException">
    /// Si el valor es nulo, vacío, contiene caracteres no numéricos,
    /// o no cumple con el rango de longitud permitido.
    /// </exception>
    public static DocumentNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidDocumentNumberException(value ?? string.Empty);

        var trimmed = value.Trim();

        if (!DocumentRegex.IsMatch(trimmed))
            throw new InvalidDocumentNumberException(trimmed);

        return new DocumentNumber(trimmed);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
