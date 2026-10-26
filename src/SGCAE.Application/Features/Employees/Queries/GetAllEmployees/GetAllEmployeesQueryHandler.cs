using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.Employees.DTOs;
using SGCAE.Domain.Repositories;

namespace SGCAE.Application.Features.Employees.Queries.GetAllEmployees;

public sealed class GetAllEmployeesQueryHandler
    : IRequestHandler<GetAllEmployeesQuery, Result<IReadOnlyList<EmployeeDto>>>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetAllEmployeesQueryHandler(IEmployeeRepository employeeRepository)
        => _employeeRepository = employeeRepository;

    public async Task<Result<IReadOnlyList<EmployeeDto>>> Handle(
        GetAllEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetAllAsync(cancellationToken);
        var dtos = employees.Select(e => e.ToDto()).ToList().AsReadOnly();
        return Result<IReadOnlyList<EmployeeDto>>.Success(dtos);
    }
}
