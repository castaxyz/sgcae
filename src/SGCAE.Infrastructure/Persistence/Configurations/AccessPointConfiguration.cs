using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGCAE.Domain.Entities;

namespace SGCAE.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para el agregado AccessPoint.
/// </summary>
public sealed class AccessPointConfiguration : IEntityTypeConfiguration<AccessPoint>
{
    public void Configure(EntityTypeBuilder<AccessPoint> builder)
    {
        builder.ToTable("AccessPoints");

        builder.HasKey(ap => ap.Id);

        builder.Property(ap => ap.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(ap => ap.Code)
            .IsUnique();

        builder.Property(ap => ap.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ap => ap.LocationDescription)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(ap => ap.IsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(ap => ap.CreatedAtUtc)
            .IsRequired();

        builder.Property(ap => ap.UpdatedAtUtc)
            .IsRequired();

        // Ignorar eventos de dominio
        builder.Ignore(ap => ap.DomainEvents);
    }
}
