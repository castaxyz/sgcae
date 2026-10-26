using FluentAssertions;
using Moq;
using SGCAE.Application.Features.AccessLogs.Commands.RegisterAccess;
using SGCAE.Domain.Entities;
using SGCAE.Domain.Enums;
using SGCAE.Domain.Repositories;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.UnitTests.Application;

/// <summary>
/// Pruebas unitarias para RegisterAccessCommandHandler (RF-03).
/// Verifica los cuatro escenarios principales de control de acceso:
///   1. Acceso autorizado (condiciones nominales).
///   2. Rechazo por punto de acceso deshabilitado.
///   3. Rechazo por credencial bloqueada.
///   4. Rechazo por empleado inactivo.
/// </summary>
public class RegisterAccessMovementCommandHandlerTests
{
    // ─── Infraestructura de mocks ────────────────────────────────────────────────

    private readonly Mock<IAccessLogRepository> _accessLogRepo;
    private readonly Mock<ICredentialRepository> _credentialRepo;
    private readonly Mock<IAccessPointRepository> _accessPointRepo;
    private readonly Mock<IEmployeeRepository> _employeeRepo;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    private readonly RegisterAccessCommandHandler _handler;

    // Constantes de semilla usadas en los tests (coinciden con el seed de datos)
    private const string ActiveCredentialCode    = "CARD-ACT-001";
    private const string BlockedCredentialCode   = "CARD-BLK-002";
    private const string InactiveEmpCredCode     = "CARD-INA-004";

    public RegisterAccessMovementCommandHandlerTests()
    {
        _accessLogRepo   = new Mock<IAccessLogRepository>(MockBehavior.Strict);
        _credentialRepo  = new Mock<ICredentialRepository>(MockBehavior.Strict);
        _accessPointRepo = new Mock<IAccessPointRepository>(MockBehavior.Strict);
        _employeeRepo    = new Mock<IEmployeeRepository>(MockBehavior.Strict);
        _unitOfWork      = new Mock<IUnitOfWork>(MockBehavior.Strict);

        _handler = new RegisterAccessCommandHandler(
            _accessLogRepo.Object,
            _credentialRepo.Object,
            _accessPointRepo.Object,
            _employeeRepo.Object,
            _unitOfWork.Object);
    }

    // ─── Helpers ────────────────────────────────────────────────────────────────

    /// <summary>Crea un empleado activo con datos por defecto.</summary>
    private static Employee CreateActiveEmployee()
    {
        return Employee.Create(
            DocumentNumber.Create("12345678"),
            "Juan",
            "Pérez",
            Email.Create("juan.perez@empresa.com"),
            "Operador",
            "Operaciones");
    }

    /// <summary>Crea un empleado inactivo.</summary>
    private static Employee CreateInactiveEmployee()
    {
        var employee = Employee.Create(
            DocumentNumber.Create("20202020"),
            "María",
            "López",
            Email.Create("maria.lopez@empresa.com"),
            "Analista",
            "Administración");
        employee.Deactivate();
        return employee;
    }

    /// <summary>Crea un punto de acceso habilitado.</summary>
    private static AccessPoint CreateEnabledAccessPoint()
        => AccessPoint.Create("REC-01", "Recepción Principal", "Entrada edificio A");

    /// <summary>Crea un punto de acceso deshabilitado.</summary>
    private static AccessPoint CreateDisabledAccessPoint()
    {
        var ap = AccessPoint.Create("BOD-01", "Bodega Central", "Nave logística 1");
        ap.Disable();
        return ap;
    }

    /// <summary>Crea una credencial activa asignada a un empleado.</summary>
    private static Credential CreateActiveCredential(Guid employeeId)
        => Credential.Create(CredentialCode.Create(ActiveCredentialCode), employeeId);

    /// <summary>Crea una credencial bloqueada.</summary>
    private static Credential CreateBlockedCredential(Guid employeeId)
    {
        var credential = Credential.Create(CredentialCode.Create(BlockedCredentialCode), employeeId);
        credential.Block("Extravío reportado");
        return credential;
    }

    /// <summary>Crea una credencial asignada a un empleado inactivo.</summary>
    private static Credential CreateCredentialForInactiveEmployee(Guid employeeId)
        => Credential.Create(CredentialCode.Create(InactiveEmpCredCode), employeeId);

    // ─── Test 1: Acceso AUTORIZADO ────────────────────────────────────────────

    /// <summary>
    /// RF-03 - Caso nominal: punto habilitado + credencial activa + empleado activo → Authorized.
    /// </summary>
    [Fact]
    public async Task Handle_WhenAccessPointEnabledAndCredentialActiveAndEmployeeActive_ShouldReturnAuthorized()
    {
        // Arrange
        var employee    = CreateActiveEmployee();
        var accessPoint = CreateEnabledAccessPoint();
        var credential  = CreateActiveCredential(employee.Id);
        var accessPointId = accessPoint.Id;

        var command = new RegisterAccessCommand(
            CredentialCode:  ActiveCredentialCode,
            AccessPointId:   accessPointId,
            MovementType:    "Entry");

        _credentialRepo
            .Setup(r => r.GetByCodeAsync(It.Is<CredentialCode>(c => c.Value == ActiveCredentialCode), It.IsAny<CancellationToken>()))
            .ReturnsAsync(credential);

        _accessPointRepo
            .Setup(r => r.GetByIdAsync(accessPointId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessPoint);

        _employeeRepo
            .Setup(r => r.GetByIdAsync(employee.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        _accessLogRepo
            .Setup(r => r.AddAsync(It.IsAny<AccessLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.AccessResult.Should().Be(AccessResult.Authorized.ToString());
        result.Value.RejectionReason.Should().BeNull();

        _accessLogRepo.Verify(r => r.AddAsync(
            It.Is<AccessLog>(log => log.AccessResult == AccessResult.Authorized),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Test 2: Rechazo por PUNTO DE ACCESO DESHABILITADO ──────────────────────

    /// <summary>
    /// RF-03 - Rechazo inmediato cuando el punto de acceso está deshabilitado.
    /// La credencial existe y está activa, pero el punto está fuera de servicio.
    /// </summary>
    [Fact]
    public async Task Handle_WhenAccessPointDisabled_ShouldReturnRejectedWithDisabledReason()
    {
        // Arrange
        var employee    = CreateActiveEmployee();
        var accessPoint = CreateDisabledAccessPoint(); // deshabilitado
        var credential  = CreateActiveCredential(employee.Id);
        var accessPointId = accessPoint.Id;

        var command = new RegisterAccessCommand(
            CredentialCode:  ActiveCredentialCode,
            AccessPointId:   accessPointId,
            MovementType:    "Entry");

        _credentialRepo
            .Setup(r => r.GetByCodeAsync(It.Is<CredentialCode>(c => c.Value == ActiveCredentialCode), It.IsAny<CancellationToken>()))
            .ReturnsAsync(credential);

        _accessPointRepo
            .Setup(r => r.GetByIdAsync(accessPointId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessPoint);

        _accessLogRepo
            .Setup(r => r.AddAsync(It.IsAny<AccessLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue("el acceso rechazado también es un resultado de negocio válido y se persiste");
        result.Value.Should().NotBeNull();
        result.Value!.AccessResult.Should().Be(AccessResult.Rejected.ToString());
        result.Value.RejectionReason.Should().NotBeNullOrWhiteSpace();
        result.Value.RejectionReason.Should().Contain("deshabilitado", Exactly.Once(),
            because: "el motivo debe indicar que el punto está deshabilitado");

        _accessLogRepo.Verify(r => r.AddAsync(
            It.Is<AccessLog>(log => log.AccessResult == AccessResult.Rejected),
            It.IsAny<CancellationToken>()), Times.Once);

        // El empleado NO debe consultarse porque el rechazo se da por credencial/punto antes
        _employeeRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── Test 3: Rechazo por CREDENCIAL BLOQUEADA ────────────────────────────────

    /// <summary>
    /// RF-03 - Rechazo cuando la credencial está en estado Blocked (CanBeUsed() == false).
    /// El punto puede estar habilitado, pero la credencial inválida impide el acceso.
    /// </summary>
    [Fact]
    public async Task Handle_WhenCredentialIsBlocked_ShouldReturnRejectedWithCredentialReason()
    {
        // Arrange
        var employee    = CreateActiveEmployee();
        var accessPoint = CreateEnabledAccessPoint();
        var credential  = CreateBlockedCredential(employee.Id); // bloqueada
        var accessPointId = accessPoint.Id;

        var command = new RegisterAccessCommand(
            CredentialCode:  BlockedCredentialCode,
            AccessPointId:   accessPointId,
            MovementType:    "Entry");

        _credentialRepo
            .Setup(r => r.GetByCodeAsync(It.Is<CredentialCode>(c => c.Value == BlockedCredentialCode), It.IsAny<CancellationToken>()))
            .ReturnsAsync(credential);

        _accessPointRepo
            .Setup(r => r.GetByIdAsync(accessPointId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessPoint);

        _accessLogRepo
            .Setup(r => r.AddAsync(It.IsAny<AccessLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.AccessResult.Should().Be(AccessResult.Rejected.ToString());
        result.Value.RejectionReason.Should().NotBeNullOrWhiteSpace();
        result.Value.RejectionReason.Should().Contain("Credencial",
            because: "el motivo debe referenciar el estado de la credencial");

        _accessLogRepo.Verify(r => r.AddAsync(
            It.Is<AccessLog>(log =>
                log.AccessResult == AccessResult.Rejected &&
                log.RejectionReason != null),
            It.IsAny<CancellationToken>()), Times.Once);

        // El empleado NO debe consultarse porque el rechazo se da antes de evaluar el empleado
        _employeeRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── Test 4: Rechazo por EMPLEADO INACTIVO ───────────────────────────────────

    /// <summary>
    /// RF-03 - Rechazo cuando el empleado asociado a la credencial se encuentra inactivo.
    /// La credencial está activa y el punto habilitado, pero el vínculo laboral impide el acceso.
    /// </summary>
    [Fact]
    public async Task Handle_WhenEmployeeIsInactive_ShouldReturnRejectedWithEmployeeInactiveReason()
    {
        // Arrange
        var inactiveEmployee = CreateInactiveEmployee();
        var accessPoint      = CreateEnabledAccessPoint();
        var credential       = CreateCredentialForInactiveEmployee(inactiveEmployee.Id); // activa pero dueño inactivo
        var accessPointId    = accessPoint.Id;

        var command = new RegisterAccessCommand(
            CredentialCode:  InactiveEmpCredCode,
            AccessPointId:   accessPointId,
            MovementType:    "Exit");

        _credentialRepo
            .Setup(r => r.GetByCodeAsync(It.Is<CredentialCode>(c => c.Value == InactiveEmpCredCode), It.IsAny<CancellationToken>()))
            .ReturnsAsync(credential);

        _accessPointRepo
            .Setup(r => r.GetByIdAsync(accessPointId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessPoint);

        _employeeRepo
            .Setup(r => r.GetByIdAsync(inactiveEmployee.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inactiveEmployee);

        _accessLogRepo
            .Setup(r => r.AddAsync(It.IsAny<AccessLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.AccessResult.Should().Be(AccessResult.Rejected.ToString());
        result.Value.RejectionReason.Should().NotBeNullOrWhiteSpace();
        result.Value.RejectionReason.Should().Contain("inactivo",
            because: "el motivo debe indicar que el empleado está inactivo");

        _accessLogRepo.Verify(r => r.AddAsync(
            It.Is<AccessLog>(log =>
                log.AccessResult == AccessResult.Rejected &&
                log.EmployeeId == inactiveEmployee.Id),
            It.IsAny<CancellationToken>()), Times.Once);

        _employeeRepo.Verify(r =>
            r.GetByIdAsync(inactiveEmployee.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
