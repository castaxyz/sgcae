using FluentValidation;
using SGCAE.Domain.Enums;

namespace SGCAE.Application.Features.AccessLogs.Commands.RegisterAccess;

public sealed class RegisterAccessCommandValidator : AbstractValidator<RegisterAccessCommand>
{
    public RegisterAccessCommandValidator()
    {
        RuleFor(x => x.CredentialCode)
            .NotEmpty().WithMessage("El código de credencial es obligatorio.")
            .Matches(@"^[A-Za-z0-9\-]{4,64}$")
            .WithMessage("El código de credencial debe contener entre 4 y 64 caracteres alfanuméricos o guiones.");

        RuleFor(x => x.AccessPointId)
            .NotEmpty().WithMessage("El identificador del punto de acceso es obligatorio.");

        RuleFor(x => x.MovementType)
            .NotEmpty().WithMessage("El tipo de movimiento es obligatorio.")
            .Must(v => Enum.TryParse<MovementType>(v, ignoreCase: true, out _))
            .WithMessage("El tipo de movimiento debe ser 'Entry' o 'Exit'.");
    }
}
