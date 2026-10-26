using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGCAE.Domain.Entities;
using SGCAE.Domain.Repositories;
using SGCAE.Infrastructure;
using SGCAE.Infrastructure.Persistence;
using SGCAE.Infrastructure.Persistence.Seed;

namespace SGCAE.UnitTests.Infrastructure;

public class InfrastructureTests
{
    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=localhost;Database=SGCAE_Db;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public void ModelBuilder_ShouldConfigureEmployeesEntityCorrectly()
    {
        using var context = CreateContext();
        var entityType = context.Model.FindEntityType(typeof(Employee));

        entityType.Should().NotBeNull();
        entityType!.GetTableName().Should().Be("Employees");

        // Primary Key
        var pk = entityType.FindPrimaryKey();
        pk.Should().NotBeNull();
        pk!.Properties.Select(p => p.Name).Should().Contain("Id");

        // DocumentNumber
        var docProp = entityType.FindProperty(nameof(Employee.DocumentNumber));
        docProp.Should().NotBeNull();
        docProp!.GetMaxLength().Should().Be(20);
        docProp.IsNullable.Should().BeFalse();
        docProp.GetValueConverter().Should().NotBeNull();

        // Unique index on DocumentNumber
        var docIndex = entityType.GetIndexes().FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(Employee.DocumentNumber)));
        docIndex.Should().NotBeNull();
        docIndex!.IsUnique.Should().BeTrue();

        // Email
        var emailProp = entityType.FindProperty(nameof(Employee.Email));
        emailProp.Should().NotBeNull();
        emailProp!.GetMaxLength().Should().Be(150);
        emailProp.IsNullable.Should().BeFalse();
        emailProp.GetValueConverter().Should().NotBeNull();

        // Unique index on Email
        var emailIndex = entityType.GetIndexes().FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(Employee.Email)));
        emailIndex.Should().NotBeNull();
        emailIndex!.IsUnique.Should().BeTrue();

        // Names & Job & Department
        entityType.FindProperty(nameof(Employee.FirstName))!.GetMaxLength().Should().Be(100);
        entityType.FindProperty(nameof(Employee.LastName))!.GetMaxLength().Should().Be(100);
        entityType.FindProperty(nameof(Employee.JobTitle))!.GetMaxLength().Should().Be(100);
        entityType.FindProperty(nameof(Employee.Department))!.GetMaxLength().Should().Be(100);

        // EmploymentStatus conversion
        var statusProp = entityType.FindProperty(nameof(Employee.EmploymentStatus));
        statusProp.Should().NotBeNull();
        // Check provider clr type or converter
        statusProp!.GetProviderClrType().Should().Be(typeof(string));

        // FullName and DomainEvents should be ignored
        entityType.FindProperty("FullName").Should().BeNull();
        entityType.FindProperty("DomainEvents").Should().BeNull();
    }

    [Fact]
    public void ModelBuilder_ShouldConfigureCredentialsEntityCorrectly()
    {
        using var context = CreateContext();
        var entityType = context.Model.FindEntityType(typeof(Credential));

        entityType.Should().NotBeNull();
        entityType!.GetTableName().Should().Be("Credentials");

        // Primary key
        entityType.FindPrimaryKey()!.Properties.Select(p => p.Name).Should().Contain("Id");

        // Code
        var codeProp = entityType.FindProperty(nameof(Credential.Code));
        codeProp.Should().NotBeNull();
        codeProp!.GetMaxLength().Should().Be(50);
        codeProp.IsNullable.Should().BeFalse();
        codeProp.GetValueConverter().Should().NotBeNull();

        // Unique index on Code
        var codeIndex = entityType.GetIndexes().FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(Credential.Code)));
        codeIndex.Should().NotBeNull();
        codeIndex!.IsUnique.Should().BeTrue();

        // Status conversion
        var statusProp = entityType.FindProperty(nameof(Credential.Status));
        statusProp.Should().NotBeNull();
        statusProp!.GetProviderClrType().Should().Be(typeof(string));

        // BlockReason and RevocationReason
        var blockReason = entityType.FindProperty(nameof(Credential.BlockReason));
        blockReason!.GetMaxLength().Should().Be(250);
        blockReason.IsNullable.Should().BeTrue();

        var revocationReason = entityType.FindProperty(nameof(Credential.RevocationReason));
        revocationReason!.GetMaxLength().Should().Be(250);
        revocationReason.IsNullable.Should().BeTrue();

        // Foreign Key to Employee with Restrict delete behavior
        var fk = entityType.GetForeignKeys().FirstOrDefault(f => f.PrincipalEntityType.ClrType == typeof(Employee));
        fk.Should().NotBeNull();
        fk!.DeleteBehavior.Should().Be(DeleteBehavior.Restrict);
        fk.Properties.Select(p => p.Name).Should().Contain("EmployeeId");
    }

    [Fact]
    public void ModelBuilder_ShouldConfigureAccessPointsEntityCorrectly()
    {
        using var context = CreateContext();
        var entityType = context.Model.FindEntityType(typeof(AccessPoint));

        entityType.Should().NotBeNull();
        entityType!.GetTableName().Should().Be("AccessPoints");

        // Primary key
        entityType.FindPrimaryKey()!.Properties.Select(p => p.Name).Should().Contain("Id");

        // Code
        var codeProp = entityType.FindProperty(nameof(AccessPoint.Code));
        codeProp.Should().NotBeNull();
        codeProp!.GetMaxLength().Should().Be(20);
        codeProp.IsNullable.Should().BeFalse();

        // Unique index on Code
        var codeIndex = entityType.GetIndexes().FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(AccessPoint.Code)));
        codeIndex.Should().NotBeNull();
        codeIndex!.IsUnique.Should().BeTrue();

        // Name
        var nameProp = entityType.FindProperty(nameof(AccessPoint.Name));
        nameProp!.GetMaxLength().Should().Be(100);
        nameProp.IsNullable.Should().BeFalse();

        // LocationDescription
        var locProp = entityType.FindProperty(nameof(AccessPoint.LocationDescription));
        locProp!.GetMaxLength().Should().Be(250);

        // IsEnabled default value true
        var isEnabledProp = entityType.FindProperty(nameof(AccessPoint.IsEnabled));
        isEnabledProp!.GetDefaultValue().Should().Be(true);
    }

    [Fact]
    public void ModelBuilder_ShouldConfigureAccessLogsEntityCorrectly()
    {
        using var context = CreateContext();
        var entityType = context.Model.FindEntityType(typeof(AccessLog));

        entityType.Should().NotBeNull();
        entityType!.GetTableName().Should().Be("AccessLogs");

        // Primary key
        entityType.FindPrimaryKey()!.Properties.Select(p => p.Name).Should().Contain("Id");

        // MovementType and AccessResult conversions
        entityType.FindProperty(nameof(AccessLog.MovementType))!.GetProviderClrType().Should().Be(typeof(string));
        entityType.FindProperty(nameof(AccessLog.AccessResult))!.GetProviderClrType().Should().Be(typeof(string));

        // RejectionReason
        var rejProp = entityType.FindProperty(nameof(AccessLog.RejectionReason));
        rejProp!.GetMaxLength().Should().Be(250);
        rejProp.IsNullable.Should().BeTrue();

        // Indices: TimestampUtc, AccessPointId, EmployeeId, AccessResult
        var indexes = entityType.GetIndexes().ToList();
        indexes.Should().Contain(i => i.Properties.Any(p => p.Name == nameof(AccessLog.TimestampUtc)));
        indexes.Should().Contain(i => i.Properties.Any(p => p.Name == nameof(AccessLog.AccessPointId)));
        indexes.Should().Contain(i => i.Properties.Any(p => p.Name == nameof(AccessLog.EmployeeId)));
        indexes.Should().Contain(i => i.Properties.Any(p => p.Name == nameof(AccessLog.AccessResult)));
    }

    [Fact]
    public void AddInfrastructureServices_ShouldRegisterAllRequiredServices()
    {
        var services = new ServiceCollection();
        var configurationMock = new Moq.Mock<IConfiguration>();
        var connStringSection = new Moq.Mock<IConfigurationSection>();
        connStringSection.Setup(c => c.Value).Returns("Server=localhost;Database=SGCAE_Db;Trusted_Connection=True;");
        configurationMock
            .Setup(c => c.GetSection("ConnectionStrings"))
            .Returns(new Moq.Mock<IConfigurationSection>().Object);
        configurationMock
            .Setup(c => c.GetSection("ConnectionStrings:DefaultConnection"))
            .Returns(connStringSection.Object);

        services.AddInfrastructureServices(configurationMock.Object);

        // Verify descriptors
        services.Should().Contain(s => s.ServiceType == typeof(ApplicationDbContext) && s.Lifetime == ServiceLifetime.Scoped);
        services.Should().Contain(s => s.ServiceType == typeof(IUnitOfWork) && s.Lifetime == ServiceLifetime.Scoped);
        services.Should().Contain(s => s.ServiceType == typeof(IEmployeeRepository) && s.Lifetime == ServiceLifetime.Scoped);
        services.Should().Contain(s => s.ServiceType == typeof(ICredentialRepository) && s.Lifetime == ServiceLifetime.Scoped);
        services.Should().Contain(s => s.ServiceType == typeof(IAccessPointRepository) && s.Lifetime == ServiceLifetime.Scoped);
        services.Should().Contain(s => s.ServiceType == typeof(IAccessLogRepository) && s.Lifetime == ServiceLifetime.Scoped);
        services.Should().Contain(s => s.ServiceType == typeof(ApplicationDbContextSeed) && s.Lifetime == ServiceLifetime.Scoped);
    }
}
