using DarkKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DarkKitchen.Infrastructure.EntityConfigurations;

public sealed class ProductCategorySqlDbContext : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.HasKey(c => c.CategoryId);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasData(
            new { CategoryId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "Burgers", Description = "Fresh handcrafted burgers" },
            new { CategoryId = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "Pizzas", Description = "Stone oven baked pizzas" },
            new { CategoryId = Guid.Parse("10000000-0000-0000-0000-000000000003"), Name = "Sushi", Description = "Fresh Japanese rolls and nigiri" },
            new { CategoryId = Guid.Parse("10000000-0000-0000-0000-000000000004"), Name = "Salads", Description = "Light and fresh salads" },
            new { CategoryId = Guid.Parse("10000000-0000-0000-0000-000000000005"), Name = "Beverages", Description = "Cold and hot drinks" });
    }
}
