using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.Credentials.DTOs;

namespace SGCAE.Application.Features.Credentials.Queries.GetCredentialByCode;

/// <summary>Query para obtener una credencial por su código RFID.</summary>
public sealed record GetCredentialByCodeQuery(string Code) : IRequest<Result<CredentialDto>>;
