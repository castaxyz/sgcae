using FluentValidation;

namespace SGCAE.Application.Features.Credentials.Commands.AssignCredential;

public sealed class AssignCredentialCommandValidator : AbstractValidator<AssignCredentialCommand>
{
    public AssignCredentialCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código de credencial es obligatorio.")
            .Matches(@"^[A-Za-z0-9\-]{4,64}$")
            .WithMessage("El código debe ser alfanumérico (guiones permitidos), entre 4 y 64 caracteres.");

        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("El Id del empleado es obligatorio.");
    }
}
