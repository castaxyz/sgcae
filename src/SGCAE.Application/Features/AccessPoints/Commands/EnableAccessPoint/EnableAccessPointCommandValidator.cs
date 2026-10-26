using FluentValidation;

namespace SGCAE.Application.Features.AccessPoints.Commands.EnableAccessPoint;

public sealed class EnableAccessPointCommandValidator : AbstractValidator<EnableAccessPointCommand>
{
    public EnableAccessPointCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador del punto de acceso es obligatorio.");
    }
}
