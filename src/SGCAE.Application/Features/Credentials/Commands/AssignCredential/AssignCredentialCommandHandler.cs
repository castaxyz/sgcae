using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.Credentials.DTOs;
using SGCAE.Domain.Entities;
using SGCAE.Domain.Repositories;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Application.Features.Credentials.Commands.AssignCredential;

public sealed class AssignCredentialCommandHandler
    : IRequestHandler<AssignCredentialCommand, Result<CredentialDto>>
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignCredentialCommandHandler(
        ICredentialRepository credentialRepository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _credentialRepository = credentialRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CredentialDto>> Handle(
        AssignCredentialCommand request,
        CancellationToken cancellationToken)
    {
        // Verificar que el empleado existe y está activo
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
            return Result<CredentialDto>.NotFound($"No se encontró el empleado con Id '{request.EmployeeId}'.");

        if (!employee.IsActive())
            return Result<CredentialDto>.Conflict($"El empleado '{request.EmployeeId}' está inactivo y no se le puede asignar una credencial.");

        // Verificar que el código de credencial no exista ya
        var code = CredentialCode.Create(request.Code);
        var existing = await _credentialRepository.GetByCodeAsync(code, cancellationToken);
        if (existing is not null)
            return Result<CredentialDto>.Conflict($"Ya existe una credencial con el código '{request.Code}'.");

        var credential = Credential.Create(code, request.EmployeeId);

        await _credentialRepository.AddAsync(credential, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CredentialDto>.Success(credential.ToDto(), 201);
    }
}
