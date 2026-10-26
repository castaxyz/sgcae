using FluentValidation;
using SGCAE.Domain.Enums;

namespace SGCAE.Application.Features.AccessLogs.Queries.GetAccessHistory;

public sealed class GetAccessHistoryQueryValidator : AbstractValidator<GetAccessHistoryQuery>
{
    public GetAccessHistoryQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("El número de página debe ser mayor o igual a 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("El tamaño de página debe estar entre 1 y 100.");

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("La fecha de inicio no puede ser posterior a la fecha de fin.");

        RuleFor(x => x.AccessResult)
            .Must(v => v is null || Enum.TryParse<AccessResult>(v, ignoreCase: true, out _))
            .WithMessage("El resultado de acceso debe ser 'Authorized' o 'Rejected'.");
    }
}
