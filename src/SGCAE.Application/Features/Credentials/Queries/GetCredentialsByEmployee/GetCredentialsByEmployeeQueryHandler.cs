using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.Credentials.DTOs;
using SGCAE.Domain.Repositories;

namespace SGCAE.Application.Features.Credentials.Queries.GetCredentialsByEmployee;

public sealed class GetCredentialsByEmployeeQueryHandler
    : IRequestHandler<GetCredentialsByEmployeeQuery, Result<IReadOnlyList<CredentialDto>>>
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public GetCredentialsByEmployeeQueryHandler(
        ICredentialRepository credentialRepository,
        IEmployeeRepository employeeRepository)
    {
        _credentialRepository = credentialRepository;
        _employeeRepository = employeeRepository;
    }

    public async Task<Result<IReadOnlyList<CredentialDto>>> Handle(
        GetCredentialsByEmployeeQuery request,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
            return Result<IReadOnlyList<CredentialDto>>.NotFound(
                $"No se encontró el empleado con Id '{request.EmployeeId}'.");

        var credentials = await _credentialRepository.GetByEmployeeIdAsync(request.EmployeeId, cancellationToken);
        var dtos = credentials.Select(c => c.ToDto()).ToList().AsReadOnly();

        return Result<IReadOnlyList<CredentialDto>>.Success(dtos);
    }
}
