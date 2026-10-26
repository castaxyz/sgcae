using FluentValidation;

namespace SGCAE.Application.Features.Credentials.Commands.BlockCredential;

public sealed class BlockCredentialCommandValidator : AbstractValidator<BlockCredentialCommand>
{
    public BlockCredentialCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código de credencial es obligatorio.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("El motivo del bloqueo es obligatorio.")
            .MaximumLength(500).WithMessage("El motivo no puede exceder 500 caracteres.");
    }
}
