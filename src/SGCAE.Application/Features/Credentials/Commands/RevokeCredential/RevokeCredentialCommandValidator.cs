using FluentValidation;

namespace SGCAE.Application.Features.Credentials.Commands.RevokeCredential;

public sealed class RevokeCredentialCommandValidator : AbstractValidator<RevokeCredentialCommand>
{
    public RevokeCredentialCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código de credencial es obligatorio.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("El motivo de la revocación es obligatorio.")
            .MaximumLength(500).WithMessage("El motivo no puede exceder 500 caracteres.");
    }
}
