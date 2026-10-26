using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGCAE.Domain.Entities;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para el agregado AccessLog.
/// Contiene índices de consulta para soportar el requisito de auditoría e historial RF-04.
/// </summary>
public sealed class AccessLogConfiguration : IEntityTypeConfiguration<AccessLog>
{
    public void Configure(EntityTypeBuilder<AccessLog> builder)
    {
        builder.ToTable("AccessLogs");

        builder.HasKey(al => al.Id);

        builder.Property(al => al.CredentialCode)
            .HasConversion(
                code => code.Value,
                value => CredentialCode.Create(value))
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(al => al.AccessPointId)
            .IsRequired();

        builder.HasOne<AccessPoint>()
            .WithMany()
            .HasForeignKey(al => al.AccessPointId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(al => al.EmployeeId)
            .IsRequired(false);

        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(al => al.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.Property(al => al.MovementType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(al => al.TimestampUtc)
            .IsRequired();

        builder.Property(al => al.AccessResult)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(al => al.RejectionReason)
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(al => al.CreatedAtUtc)
            .IsRequired();

        builder.Property(al => al.UpdatedAtUtc)
            .IsRequired();

        // Índices para optimizar las consultas del historial (RF-04)
        builder.HasIndex(al => al.TimestampUtc);
        builder.HasIndex(al => al.AccessPointId);
        builder.HasIndex(al => al.EmployeeId);
        builder.HasIndex(al => al.AccessResult);

        // Ignorar eventos de dominio
        builder.Ignore(al => al.DomainEvents);
    }
}
