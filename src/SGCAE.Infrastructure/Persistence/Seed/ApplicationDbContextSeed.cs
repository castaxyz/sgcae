using Microsoft.EntityFrameworkCore;
using SGCAE.Domain.Entities;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Infrastructure.Persistence.Seed;

/// <summary>
/// Clase para inicializar datos de prueba en la base de datos si se encuentra vacía.
/// </summary>
public sealed class ApplicationDbContextSeed
{
    private readonly ApplicationDbContext? _context;

    public ApplicationDbContextSeed()
    {
    }

    public ApplicationDbContextSeed(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Ejecuta el sembrado utilizando el contexto inyectado por DI.
    /// </summary>
    public async Task SeedSampleDataAsync(CancellationToken cancellationToken = default)
    {
        if (_context is null)
            throw new InvalidOperationException("ApplicationDbContext no ha sido inyectado en ApplicationDbContextSeed.");

        await SeedSampleDataAsync(_context, cancellationToken);
    }

    /// <summary>
    /// Inserta datos iniciales de prueba si la base de datos no contiene registros.
    /// </summary>
    public async Task SeedSampleDataAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.AccessPoints.AnyAsync(cancellationToken) ||
            await context.Employees.AnyAsync(cancellationToken) ||
            await context.Credentials.AnyAsync(cancellationToken))
        {
            return;
        }

        // 1. Puntos de acceso
        var accessPoints = new[]
        {
            AccessPoint.Create("REC-01", "Recepción Principal", "Entrada principal edificio A"),
            AccessPoint.Create("BOD-01", "Bodega Central", "Nave logística 1"),
            AccessPoint.Create("PRK-01", "Parqueadero", "Acceso vehicular subterráneo"),
            AccessPoint.Create("RES-01", "Área Restringida Servidores", "Data Center piso 2")
        };

        await context.AccessPoints.AddRangeAsync(accessPoints, cancellationToken);

        // 2. Empleados
        var employee1 = Employee.Create(
            DocumentNumber.Create("10101001"),
            "Juan",
            "Pérez",
            Email.Create("juan.perez@empresa.com"),
            "Operador Logístico",
            "Operaciones");

        var employee2 = Employee.Create(
            DocumentNumber.Create("20202002"),
            "María",
            "López",
            Email.Create("maria.lopez@empresa.com"),
            "Analista",
            "Administración");
        employee2.Deactivate();

        await context.Employees.AddRangeAsync(new[] { employee1, employee2 }, cancellationToken);

        // 3. Credenciales
        var card1 = Credential.Create(CredentialCode.Create("CARD-ACT-001"), employee1.Id);

        var card2 = Credential.Create(CredentialCode.Create("CARD-BLK-002"), employee1.Id);
        card2.Block("Extravío de credencial reportado por el empleado");

        var card3 = Credential.Create(CredentialCode.Create("CARD-REV-003"));
        card3.Revoke("Revocada por desvinculación");

        var card4 = Credential.Create(CredentialCode.Create("CARD-INA-004"), employee2.Id);

        await context.Credentials.AddRangeAsync(new[] { card1, card2, card3, card4 }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}
