using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.Credentials.DTOs;
using SGCAE.Domain.Repositories;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Application.Features.Credentials.Queries.GetCredentialByCode;

public sealed class GetCredentialByCodeQueryHandler
    : IRequestHandler<GetCredentialByCodeQuery, Result<CredentialDto>>
{
    private readonly ICredentialRepository _credentialRepository;

    public GetCredentialByCodeQueryHandler(ICredentialRepository credentialRepository)
        => _credentialRepository = credentialRepository;

    public async Task<Result<CredentialDto>> Handle(
        GetCredentialByCodeQuery request,
        CancellationToken cancellationToken)
    {
        var code = CredentialCode.Create(request.Code);
        var credential = await _credentialRepository.GetByCodeAsync(code, cancellationToken);
        if (credential is null)
            return Result<CredentialDto>.NotFound($"No se encontró la credencial con código '{request.Code}'.");

        return Result<CredentialDto>.Success(credential.ToDto());
    }
}
