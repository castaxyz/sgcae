using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Domain.Repositories;

namespace SGCAE.Application.Features.Employees.Commands.DeactivateEmployee;

public sealed class DeactivateEmployeeCommandHandler
    : IRequestHandler<DeactivateEmployeeCommand, Result>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeactivateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
            return Result.NotFound($"No se encontró el empleado con Id '{request.EmployeeId}'.");

        if (!employee.IsActive())
            return Result.Conflict($"El empleado '{request.EmployeeId}' ya está inactivo.");

        employee.Deactivate();
        _employeeRepository.Update(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
