using MediatR;
using SGCAE.Application.Common.Models;
using SGCAE.Application.Features.Credentials.DTOs;

namespace SGCAE.Application.Features.Credentials.Queries.GetCredentialsByEmployee;

/// <summary>Query para obtener todas las credenciales de un empleado específico.</summary>
public sealed record GetCredentialsByEmployeeQuery(Guid EmployeeId)
    : IRequest<Result<IReadOnlyList<CredentialDto>>>;
