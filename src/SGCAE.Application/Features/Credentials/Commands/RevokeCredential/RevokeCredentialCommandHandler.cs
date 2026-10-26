using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.Credentials.DTOs;
using SGCAE.Domain.Enums;
using SGCAE.Domain.Repositories;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Application.Features.Credentials.Commands.RevokeCredential;

public sealed class RevokeCredentialCommandHandler
    : IRequestHandler<RevokeCredentialCommand, Result<CredentialDto>>
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RevokeCredentialCommandHandler(
        ICredentialRepository credentialRepository,
        IUnitOfWork unitOfWork)
    {
        _credentialRepository = credentialRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CredentialDto>> Handle(
        RevokeCredentialCommand request,
        CancellationToken cancellationToken)
    {
        var code = CredentialCode.Create(request.Code);
        var credential = await _credentialRepository.GetByCodeAsync(code, cancellationToken);
        if (credential is null)
            return Result<CredentialDto>.NotFound($"No se encontró la credencial con código '{request.Code}'.");

        if (credential.Status == CredentialStatus.Revoked)
            return Result<CredentialDto>.Conflict(
                $"La credencial '{request.Code}' ya ha sido revocada.");

        credential.Revoke(request.Reason);
        _credentialRepository.Update(credential);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CredentialDto>.Success(credential.ToDto());
    }
}
