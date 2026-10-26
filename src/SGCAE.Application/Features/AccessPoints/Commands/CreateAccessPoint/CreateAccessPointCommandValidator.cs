using FluentValidation;

namespace SGCAE.Application.Features.AccessPoints.Commands.CreateAccessPoint;

public sealed class CreateAccessPointCommandValidator : AbstractValidator<CreateAccessPointCommand>
{
    public CreateAccessPointCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código del punto de acceso es obligatorio.")
            .MaximumLength(20).WithMessage("El código no puede exceder 20 caracteres.")
            .Matches(@"^[A-Za-z0-9\-]+$").WithMessage("El código solo puede contener letras, números y guiones.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del punto de acceso es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre no puede exceder 150 caracteres.");

        RuleFor(x => x.LocationDescription)
            .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres.");
    }
}
