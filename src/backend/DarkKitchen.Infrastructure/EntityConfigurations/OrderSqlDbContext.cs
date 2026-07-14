using DarkKitchen.Domain.Constants;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DarkKitchen.Infrastructure.EntityConfigurations;

public sealed class OrderSqlDbContext : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.ClientId)
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.DeliveryTypeId)
            .IsRequired();

        builder.Property(o => o.DeliveryCost)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(o => o.StatusChangedAt)
            .IsRequired();

        builder.Ignore(o => o.Subtotal);
        builder.Ignore(o => o.Total);

        var customer = Guid.Parse("e5e5e5e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5");
        var t1 = new DateTime(2026, 4, 25, 10, 0, 0, DateTimeKind.Utc);
        var t2 = new DateTime(2026, 4, 26, 11, 0, 0, DateTimeKind.Utc);
        var t3 = new DateTime(2026, 4, 26, 14, 0, 0, DateTimeKind.Utc);
        var t4 = new DateTime(2026, 4, 27, 9, 0, 0, DateTimeKind.Utc);
        var t5 = new DateTime(2026, 4, 27, 16, 0, 0, DateTimeKind.Utc);
        var t6 = new DateTime(2026, 4, 28, 8, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            new { Id = Guid.Parse("30000000-0000-0000-0000-000000000001"), ClientId = customer, CreatedAt = t1, DeliveryTypeId = SeedDeliveryTypeIds.Express, DeliveryCost = 250m, Status = OrderStatus.Pending, StatusChangedAt = t1 },
            new { Id = Guid.Parse("30000000-0000-0000-0000-000000000002"), ClientId = customer, CreatedAt = t2, DeliveryTypeId = SeedDeliveryTypeIds.Express, DeliveryCost = 250m, Status = OrderStatus.Ready, StatusChangedAt = t2 },
            new { Id = Guid.Parse("30000000-0000-0000-0000-000000000003"), ClientId = customer, CreatedAt = t3, DeliveryTypeId = SeedDeliveryTypeIds.NextDay, DeliveryCost = 180m, Status = OrderStatus.Cancelled, StatusChangedAt = t3 },
            new { Id = Guid.Parse("30000000-0000-0000-0000-000000000004"), ClientId = customer, CreatedAt = t4, DeliveryTypeId = SeedDeliveryTypeIds.Express, DeliveryCost = 250m, Status = OrderStatus.OnTheWay, StatusChangedAt = t4 },
            new { Id = Guid.Parse("30000000-0000-0000-0000-000000000005"), ClientId = customer, CreatedAt = t5, DeliveryTypeId = SeedDeliveryTypeIds.NextDay, DeliveryCost = 180m, Status = OrderStatus.Delivered, StatusChangedAt = t5 },
            new { Id = Guid.Parse("30000000-0000-0000-0000-000000000006"), ClientId = customer, CreatedAt = t6, DeliveryTypeId = SeedDeliveryTypeIds.Express, DeliveryCost = 250m, Status = OrderStatus.NotDelivered, StatusChangedAt = t6 });

        builder.OwnsOne(o => o.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Street");

            addressBuilder.Property(a => a.DoorNumber)
                .IsRequired()
                .HasColumnName("DoorNumber");

            addressBuilder.Property(a => a.Apartment)
                .HasMaxLength(20)
                .HasColumnName("Apartment");

            addressBuilder.HasData(
                new { OrderId = Guid.Parse("30000000-0000-0000-0000-000000000001"), Street = "Av Libertad", DoorNumber = 100, Apartment = (string?)null },
                new { OrderId = Guid.Parse("30000000-0000-0000-0000-000000000002"), Street = "Calle Rivera", DoorNumber = 220, Apartment = (string?)"A" },
                new { OrderId = Guid.Parse("30000000-0000-0000-0000-000000000003"), Street = "Rambla Sur", DoorNumber = 345, Apartment = (string?)null },
                new { OrderId = Guid.Parse("30000000-0000-0000-0000-000000000004"), Street = "Av Italia", DoorNumber = 450, Apartment = (string?)"B" },
                new { OrderId = Guid.Parse("30000000-0000-0000-0000-000000000005"), Street = "Bulevar Artigas", DoorNumber = 560, Apartment = (string?)null },
                new { OrderId = Guid.Parse("30000000-0000-0000-0000-000000000006"), Street = "Calle Colonia", DoorNumber = 672, Apartment = (string?)null });
        });

        builder.HasOne(o => o.Client)
            .WithMany()
            .HasForeignKey(o => o.ClientId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.DeliveryType)
            .WithMany()
            .HasForeignKey(o => o.DeliveryTypeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey("OrderId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(o => o.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
