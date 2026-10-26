using FluentAssertions;
using SGCAE.Domain.Entities;
using SGCAE.Domain.Enums;
using SGCAE.Domain.Exceptions;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.UnitTests.Domain;

/// <summary>
/// Pruebas unitarias para el Aggregate Root Credential.
/// Verifica el ciclo de vida de las credenciales RFID (RF-02).
/// </summary>
public class CredentialTests
{
    // ─── Helpers ────────────────────────────────────────────────────────────────

    private static Credential CreateActiveCredential(string code = "CARD-ACT-001", Guid? employeeId = null)
    {
        return Credential.Create(CredentialCode.Create(code), employeeId);
    }

    private static Credential CreateBlockedCredential(string code = "CARD-BLK-002", string reason = "Pérdida reportada")
    {
        var credential = CreateActiveCredential(code);
        credential.Block(reason);
        return credential;
    }

    private static Credential CreateRevokedCredential(string code = "CARD-REV-003", string reason = "Revocada por desvinculación")
    {
        var credential = CreateActiveCredential(code);
        credential.Revoke(reason);
        return credential;
    }

    // ─── Creación ────────────────────────────────────────────────────────────────

    [Fact]
    public void Create_NewCredential_ShouldBeInActiveStatus()
    {
        // Act
        var credential = CreateActiveCredential();

        // Assert
        credential.Status.Should().Be(CredentialStatus.Active);
    }

    [Fact]
    public void Create_NewCredential_ShouldBeUsable()
    {
        // Act
        var credential = CreateActiveCredential();

        // Assert
        credential.CanBeUsed().Should().BeTrue();
    }

    [Fact]
    public void Create_NewCredential_ShouldHaveNonEmptyId()
    {
        // Act
        var credential = CreateActiveCredential();

        // Assert
        credential.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_NewCredential_ShouldHaveIssueDateUtc()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var credential = CreateActiveCredential();

        // Assert
        credential.IssueDateUtc.Should().BeAfter(before);
    }

    [Fact]
    public void Create_WithEmployeeId_ShouldAssignEmployee()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        // Act
        var credential = CreateActiveCredential(employeeId: employeeId);

        // Assert
        credential.EmployeeId.Should().Be(employeeId);
    }

    // ─── Bloqueo ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Block_ActiveCredential_ShouldChangeStatusToBlocked()
    {
        // Arrange
        var credential = CreateActiveCredential();

        // Act
        credential.Block("Extravío reportado");

        // Assert
        credential.Status.Should().Be(CredentialStatus.Blocked);
    }

    [Fact]
    public void Block_ActiveCredential_ShouldMakeCanBeUsedReturnFalse()
    {
        // Arrange
        var credential = CreateActiveCredential();

        // Act
        credential.Block("Extravío reportado");

        // Assert
        credential.CanBeUsed().Should().BeFalse();
    }

    [Fact]
    public void Block_ActiveCredential_ShouldSaveBlockReason()
    {
        // Arrange
        var credential = CreateActiveCredential();
        const string reason = "Sospecha de uso indebido";

        // Act
        credential.Block(reason);

        // Assert
        credential.BlockReason.Should().Be(reason);
    }

    [Fact]
    public void Block_AlreadyBlockedCredential_ShouldThrowInvalidCredentialStateException()
    {
        // Arrange
        var credential = CreateBlockedCredential();

        // Act
        var act = () => credential.Block("Otro motivo");

        // Assert
        act.Should().Throw<InvalidCredentialStateException>();
    }

    [Fact]
    public void Block_RevokedCredential_ShouldThrowInvalidCredentialStateException()
    {
        // Arrange
        var credential = CreateRevokedCredential();

        // Act
        var act = () => credential.Block("Motivo");

        // Assert
        act.Should().Throw<InvalidCredentialStateException>();
    }

    [Fact]
    public void Block_WithEmptyReason_ShouldThrowArgumentException()
    {
        // Arrange
        var credential = CreateActiveCredential();

        // Act
        var act = () => credential.Block(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("reason");
    }

    // ─── Revocación ──────────────────────────────────────────────────────────────

    [Fact]
    public void Revoke_ActiveCredential_ShouldChangeStatusToRevoked()
    {
        // Arrange
        var credential = CreateActiveCredential();

        // Act
        credential.Revoke("Baja definitiva del empleado");

        // Assert
        credential.Status.Should().Be(CredentialStatus.Revoked);
    }

    [Fact]
    public void Revoke_ActiveCredential_ShouldNotBeUsable()
    {
        // Arrange
        var credential = CreateActiveCredential();

        // Act
        credential.Revoke("Baja definitiva");

        // Assert
        credential.CanBeUsed().Should().BeFalse();
    }

    [Fact]
    public void Revoke_ActiveCredential_ShouldSaveRevocationReason()
    {
        // Arrange
        var credential = CreateActiveCredential();
        const string reason = "Terminación de contrato";

        // Act
        credential.Revoke(reason);

        // Assert
        credential.RevocationReason.Should().Be(reason);
    }

    [Fact]
    public void Revoke_AlreadyRevokedCredential_ShouldThrowInvalidCredentialStateException()
    {
        // Arrange
        var credential = CreateRevokedCredential();

        // Act
        var act = () => credential.Revoke("Otro motivo");

        // Assert
        act.Should().Throw<InvalidCredentialStateException>();
    }

    [Fact]
    public void Revoke_BlockedCredential_ShouldSucceedAndMarkAsRevoked()
    {
        // Arrange - credencial bloqueada que también puede ser revocada
        var credential = CreateBlockedCredential();

        // Act
        credential.Revoke("Pérdida permanente confirmada");

        // Assert
        credential.Status.Should().Be(CredentialStatus.Revoked);
        credential.CanBeUsed().Should().BeFalse();
    }

    // ─── Revocada no puede reactivarse ──────────────────────────────────────────

    [Fact]
    public void RevokedCredential_CannotBeUnblocked()
    {
        // Arrange - una credencial revocada no puede desbloquearse
        var credential = CreateRevokedCredential();

        // Act
        var act = () => credential.Unblock();

        // Assert — Unblock solo aplica a bloqueadas; la revocada lanza excepción
        act.Should().Throw<InvalidCredentialStateException>();
    }

    [Fact]
    public void RevokedCredential_CannotBeAssignedToEmployee()
    {
        // Arrange
        var credential = CreateRevokedCredential();

        // Act
        var act = () => credential.AssignToEmployee(Guid.NewGuid());

        // Assert
        act.Should().Throw<InvalidCredentialStateException>();
    }

    // ─── Desbloqueo ──────────────────────────────────────────────────────────────

    [Fact]
    public void Unblock_BlockedCredential_ShouldRestoreActiveStatus()
    {
        // Arrange
        var credential = CreateBlockedCredential();

        // Act
        credential.Unblock();

        // Assert
        credential.Status.Should().Be(CredentialStatus.Active);
        credential.CanBeUsed().Should().BeTrue();
        credential.BlockReason.Should().BeNull();
    }

    [Fact]
    public void Unblock_ActiveCredential_ShouldThrowInvalidCredentialStateException()
    {
        // Arrange
        var credential = CreateActiveCredential();

        // Act
        var act = () => credential.Unblock();

        // Assert
        act.Should().Throw<InvalidCredentialStateException>();
    }
}
