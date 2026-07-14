using DarkKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DarkKitchen.Infrastructure.EntityConfigurations;

public sealed class PromotionSqlDbContext : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired();

        builder.Property(p => p.DiscountPercentage)
            .IsRequired()
            .HasColumnType("decimal(5,2)");

        builder.Property(p => p.StartDate).IsRequired();
        builder.Property(p => p.EndDate).IsRequired();

        builder.HasData(
            new
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                Name = "Summer Burger Deal",
                DiscountPercentage = 0.20m,
                StartDate = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc),
            },
            new
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000002"),
                Name = "Pizza Fiesta",
                DiscountPercentage = 0.15m,
                StartDate = new DateTime(2026, 4, 20, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc),
            },
            new
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000003"),
                Name = "Sushi Special",
                DiscountPercentage = 0.25m,
                StartDate = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc),
            });
    }
}
