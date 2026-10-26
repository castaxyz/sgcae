using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.AccessPoints.DTOs;
using SGCAE.Domain.Entities;
using SGCAE.Domain.Repositories;

namespace SGCAE.Application.Features.AccessPoints.Commands.CreateAccessPoint;

public sealed class CreateAccessPointCommandHandler
    : IRequestHandler<CreateAccessPointCommand, Result<AccessPointDto>>
{
    private readonly IAccessPointRepository _accessPointRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAccessPointCommandHandler(
        IAccessPointRepository accessPointRepository,
        IUnitOfWork unitOfWork)
    {
        _accessPointRepository = accessPointRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AccessPointDto>> Handle(
        CreateAccessPointCommand request,
        CancellationToken cancellationToken)
    {
        // Verificar que no exista ya un punto con el mismo código
        var normalizedCode = request.Code.Trim().ToUpperInvariant();
        var existing = await _accessPointRepository.GetByCodeAsync(normalizedCode, cancellationToken);
        if (existing is not null)
            return Result<AccessPointDto>.Conflict(
                $"Ya existe un punto de acceso con el código '{normalizedCode}'.");

        var accessPoint = AccessPoint.Create(
            request.Code,
            request.Name,
            request.LocationDescription);

        await _accessPointRepository.AddAsync(accessPoint, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AccessPointDto>.Success(accessPoint.ToDto(), 201);
    }
}
