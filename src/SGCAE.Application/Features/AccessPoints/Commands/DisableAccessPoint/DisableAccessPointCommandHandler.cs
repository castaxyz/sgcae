using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.AccessPoints.DTOs;
using SGCAE.Domain.Exceptions;
using SGCAE.Domain.Repositories;

namespace SGCAE.Application.Features.AccessPoints.Commands.DisableAccessPoint;

public sealed class DisableAccessPointCommandHandler
    : IRequestHandler<DisableAccessPointCommand, Result<AccessPointDto>>
{
    private readonly IAccessPointRepository _accessPointRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DisableAccessPointCommandHandler(
        IAccessPointRepository accessPointRepository,
        IUnitOfWork unitOfWork)
    {
        _accessPointRepository = accessPointRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AccessPointDto>> Handle(
        DisableAccessPointCommand request,
        CancellationToken cancellationToken)
    {
        var accessPoint = await _accessPointRepository.GetByIdAsync(request.Id, cancellationToken);
        if (accessPoint is null)
            return Result<AccessPointDto>.NotFound(
                $"No se encontró el punto de acceso con Id '{request.Id}'.");

        try
        {
            accessPoint.Disable();
        }
        catch (DomainException ex)
        {
            return Result<AccessPointDto>.Conflict(ex.Message);
        }

        _accessPointRepository.Update(accessPoint);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AccessPointDto>.Success(accessPoint.ToDto());
    }
}
