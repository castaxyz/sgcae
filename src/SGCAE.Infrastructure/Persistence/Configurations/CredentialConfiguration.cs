using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGCAE.Domain.Entities;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para el agregado Credential.
/// </summary>
public sealed class CredentialConfiguration : IEntityTypeConfiguration<Credential>
{
    public void Configure(EntityTypeBuilder<Credential> builder)
    {
        builder.ToTable("Credentials");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code)
            .HasConversion(
                code => code.Value,
                value => CredentialCode.Create(value))
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(c => c.Code)
            .IsUnique();

        builder.Property(c => c.EmployeeId)
            .IsRequired(false);

        // Relación con Employee sin navegación directa, borrado en cascada restringido
        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(c => c.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.IssueDateUtc)
            .IsRequired();

        builder.Property(c => c.BlockReason)
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(c => c.RevocationReason)
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(c => c.CreatedAtUtc)
            .IsRequired();

        builder.Property(c => c.UpdatedAtUtc)
            .IsRequired();

        // Ignorar eventos de dominio
        builder.Ignore(c => c.DomainEvents);
    }
}
