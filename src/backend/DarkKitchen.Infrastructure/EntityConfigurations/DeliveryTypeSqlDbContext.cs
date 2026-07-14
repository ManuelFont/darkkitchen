using DarkKitchen.Domain.Constants;
using DarkKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DarkKitchen.Infrastructure.EntityConfigurations;

public sealed class DeliveryTypeSqlDbContext : IEntityTypeConfiguration<DeliveryType>
{
    public void Configure(EntityTypeBuilder<DeliveryType> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Cost)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.HasIndex(d => d.Name)
            .IsUnique();

        builder.HasData(
            new { Id = SeedDeliveryTypeIds.Express, Name = "Envio express", Cost = 250m },
            new { Id = SeedDeliveryTypeIds.SameDay, Name = "Envio en el dia", Cost = 200m },
            new { Id = SeedDeliveryTypeIds.NextDay, Name = "Envio dia siguiente", Cost = 180m });
    }
}
