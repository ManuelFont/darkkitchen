using DarkKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DarkKitchen.Infrastructure.EntityConfigurations;

public sealed class OrderItemSqlDbContext : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        builder.Property(oi => oi.ProductId)
            .IsRequired();

        builder.Ignore(oi => oi.Subtotal);
        builder.Ignore(oi => oi.Total);

        builder.HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new { Id = Guid.Parse("40000000-0000-0000-0000-000000000001"), ProductId = Guid.Parse("20000000-0000-0000-0000-000000000001"), Quantity = 2, OrderId = Guid.Parse("30000000-0000-0000-0000-000000000001") },
            new { Id = Guid.Parse("40000000-0000-0000-0000-000000000002"), ProductId = Guid.Parse("20000000-0000-0000-0000-000000000004"), Quantity = 1, OrderId = Guid.Parse("30000000-0000-0000-0000-000000000002") },
            new { Id = Guid.Parse("40000000-0000-0000-0000-000000000003"), ProductId = Guid.Parse("20000000-0000-0000-0000-000000000007"), Quantity = 3, OrderId = Guid.Parse("30000000-0000-0000-0000-000000000003") },
            new { Id = Guid.Parse("40000000-0000-0000-0000-000000000004"), ProductId = Guid.Parse("20000000-0000-0000-0000-000000000010"), Quantity = 2, OrderId = Guid.Parse("30000000-0000-0000-0000-000000000004") },
            new { Id = Guid.Parse("40000000-0000-0000-0000-000000000005"), ProductId = Guid.Parse("20000000-0000-0000-0000-000000000013"), Quantity = 4, OrderId = Guid.Parse("30000000-0000-0000-0000-000000000005") },
            new { Id = Guid.Parse("40000000-0000-0000-0000-000000000006"), ProductId = Guid.Parse("20000000-0000-0000-0000-000000000003"), Quantity = 1, OrderId = Guid.Parse("30000000-0000-0000-0000-000000000006") });
    }
}
