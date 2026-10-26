using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.Credentials.DTOs;

namespace SGCAE.Application.Features.Credentials.Commands.BlockCredential;

/// <summary>Comando para bloquear temporalmente una credencial RFID.</summary>
public sealed record BlockCredentialCommand(
    string Code,
    string Reason
) : IRequest<Result<CredentialDto>>;
