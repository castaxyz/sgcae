using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.AccessLogs.DTOs;
using SGCAE.Domain.Entities;
using SGCAE.Domain.Enums;
using SGCAE.Domain.Repositories;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Application.Features.AccessLogs.Commands.RegisterAccess;

/// <summary>
/// Orquesta la validación y registro de un intento de acceso (RF-03).
/// Las reglas de negocio se evalúan en el siguiente orden:
///   1. El código de credencial debe existir en el sistema.
///   2. La credencial debe estar en estado Active (CanBeUsed).
///   3. El punto de acceso debe existir y estar habilitado.
///   4. El empleado asociado a la credencial debe estar activo.
/// Si alguna condición falla, se persiste un log rechazado con el motivo.
/// </summary>
public sealed class RegisterAccessCommandHandler
    : IRequestHandler<RegisterAccessCommand, Result<AccessLogDto>>
{
    private readonly IAccessLogRepository _accessLogRepository;
    private readonly ICredentialRepository _credentialRepository;
    private readonly IAccessPointRepository _accessPointRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterAccessCommandHandler(
        IAccessLogRepository accessLogRepository,
        ICredentialRepository credentialRepository,
        IAccessPointRepository accessPointRepository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _accessLogRepository = accessLogRepository;
        _credentialRepository = credentialRepository;
        _accessPointRepository = accessPointRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AccessLogDto>> Handle(
        RegisterAccessCommand request,
        CancellationToken cancellationToken)
    {
        var movementType = Enum.Parse<MovementType>(request.MovementType, ignoreCase: true);

        // 1. Resolver credencial
        CredentialCode credentialCode;
        try
        {
            credentialCode = CredentialCode.Create(request.CredentialCode);
        }
        catch (Exception)
        {
            return Result<AccessLogDto>.Failure(
                $"El código de credencial '{request.CredentialCode}' tiene un formato inválido.");
        }

        var credential = await _credentialRepository.GetByCodeAsync(credentialCode, cancellationToken);
        if (credential is null)
        {
            // No se puede persistir un log sin AccessPointId verificado; intentamos registrar igualmente
            // usando un AccessPoint temporal si el punto existe, para cumplir trazabilidad.
            var apForRejection = await _accessPointRepository.GetByIdAsync(request.AccessPointId, cancellationToken);
            if (apForRejection is not null)
            {
                var rejectedLog = AccessLog.CreateRejected(
                    credentialCode,
                    employeeId: null,
                    request.AccessPointId,
                    movementType,
                    rejectionReason: "Credencial no registrada en el sistema.");

                await _accessLogRepository.AddAsync(rejectedLog, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<AccessLogDto>.Success(rejectedLog.ToDto());
            }

            return Result<AccessLogDto>.NotFound(
                $"No se encontró la credencial '{request.CredentialCode}' ni el punto de acceso '{request.AccessPointId}'.");
        }

        // 2. Verificar punto de acceso
        var accessPoint = await _accessPointRepository.GetByIdAsync(request.AccessPointId, cancellationToken);
        if (accessPoint is null)
            return Result<AccessLogDto>.NotFound(
                $"No se encontró el punto de acceso con Id '{request.AccessPointId}'.");

        // 3. Evaluar reglas de negocio para determinar si el acceso es autorizado o rechazado
        string? rejectionReason = null;

        if (!credential.CanBeUsed())
        {
            rejectionReason = $"Credencial en estado '{credential.Status}': no puede utilizarse para el acceso.";
        }
        else if (!accessPoint.IsAvailable())
        {
            rejectionReason = $"El punto de acceso '{accessPoint.Code}' está deshabilitado.";
        }
        else if (credential.EmployeeId.HasValue)
        {
            var employee = await _employeeRepository.GetByIdAsync(credential.EmployeeId.Value, cancellationToken);
            if (employee is not null && !employee.IsActive())
                rejectionReason = $"El empleado asociado a la credencial está inactivo.";
        }

        AccessLog log;
        if (rejectionReason is null)
        {
            log = AccessLog.CreateAuthorized(
                credentialCode,
                credential.EmployeeId,
                accessPoint.Id,
                movementType);
        }
        else
        {
            log = AccessLog.CreateRejected(
                credentialCode,
                credential.EmployeeId,
                accessPoint.Id,
                movementType,
                rejectionReason);
        }

        await _accessLogRepository.AddAsync(log, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AccessLogDto>.Success(log.ToDto(), 201);
    }
}
