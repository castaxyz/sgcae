using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.Credentials.DTOs;
using SGCAE.Domain.Repositories;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Application.Features.Credentials.Commands.BlockCredential;

public sealed class BlockCredentialCommandHandler
    : IRequestHandler<BlockCredentialCommand, Result<CredentialDto>>
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BlockCredentialCommandHandler(
        ICredentialRepository credentialRepository,
        IUnitOfWork unitOfWork)
    {
        _credentialRepository = credentialRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CredentialDto>> Handle(
        BlockCredentialCommand request,
        CancellationToken cancellationToken)
    {
        var code = CredentialCode.Create(request.Code);
        var credential = await _credentialRepository.GetByCodeAsync(code, cancellationToken);
        if (credential is null)
            return Result<CredentialDto>.NotFound($"No se encontró la credencial con código '{request.Code}'.");

        if (!credential.CanBeUsed())
            return Result<CredentialDto>.Conflict(
                $"La credencial '{request.Code}' no puede bloquearse porque está en estado '{credential.Status}'.");

        credential.Block(request.Reason);
        _credentialRepository.Update(credential);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CredentialDto>.Success(credential.ToDto());
    }
}
