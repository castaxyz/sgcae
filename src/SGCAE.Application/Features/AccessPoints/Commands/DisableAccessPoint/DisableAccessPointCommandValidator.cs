using FluentValidation;

namespace SGCAE.Application.Features.AccessPoints.Commands.DisableAccessPoint;

public sealed class DisableAccessPointCommandValidator : AbstractValidator<DisableAccessPointCommand>
{
    public DisableAccessPointCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador del punto de acceso es obligatorio.");
    }
}
