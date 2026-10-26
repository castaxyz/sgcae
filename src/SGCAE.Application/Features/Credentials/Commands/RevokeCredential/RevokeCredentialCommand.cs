using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.Credentials.DTOs;

namespace SGCAE.Application.Features.Credentials.Commands.RevokeCredential;

/// <summary>
/// Comando para revocar permanentemente una credencial RFID (estado terminal).
/// Una credencial revocada no puede reactivarse.
/// </summary>
public sealed record RevokeCredentialCommand(
    string Code,
    string Reason
) : IRequest<Result<CredentialDto>>;
