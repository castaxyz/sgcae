using FluentValidation;

namespace SGCAE.Application.Features.Employees.Commands.CreateEmployee;

public sealed class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.DocumentNumber)
            .NotEmpty().WithMessage("El número de documento es obligatorio.")
            .Matches(@"^\d{5,20}$").WithMessage("El documento debe contener solo dígitos (5-20 caracteres).");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es obligatorio.")
            .MaximumLength(100).WithMessage("El apellido no puede exceder 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El correo electrónico no tiene formato válido.");

        RuleFor(x => x.JobTitle)
            .NotEmpty().WithMessage("El cargo es obligatorio.")
            .MaximumLength(150).WithMessage("El cargo no puede exceder 150 caracteres.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("El departamento es obligatorio.")
            .MaximumLength(150).WithMessage("El departamento no puede exceder 150 caracteres.");
    }
}
