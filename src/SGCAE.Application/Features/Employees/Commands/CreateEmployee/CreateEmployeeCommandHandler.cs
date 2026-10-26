using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.Employees.DTOs;
using SGCAE.Domain.Entities;
using SGCAE.Domain.Repositories;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Application.Features.Employees.Commands.CreateEmployee;

public sealed class CreateEmployeeCommandHandler
    : IRequestHandler<CreateEmployeeCommand, Result<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<EmployeeDto>> Handle(
        CreateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        // Verificar duplicado por número de documento
        var documentNumber = DocumentNumber.Create(request.DocumentNumber);
        var existing = await _employeeRepository.GetByDocumentAsync(documentNumber, cancellationToken);
        if (existing is not null)
            return Result<EmployeeDto>.Conflict(
                $"Ya existe un empleado con el documento '{request.DocumentNumber}'.");

        var email = Email.Create(request.Email);
        var employee = Employee.Create(
            documentNumber,
            request.FirstName,
            request.LastName,
            email,
            request.JobTitle,
            request.Department);

        await _employeeRepository.AddAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<EmployeeDto>.Success(employee.ToDto(), 201);
    }
}
