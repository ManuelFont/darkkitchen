using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DarkKitchen.Infrastructure.EntityConfigurations;

public sealed class ProductSqlDbContext : IEntityTypeConfiguration<Product>
{
    private static readonly IReadOnlyList<string> SeedImageUrls =
    [
        "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=900&q=85",
        "https://images.unsplash.com/photo-1571091718767-18b5b1457add?auto=format&fit=crop&w=900&q=85",
        "https://images.unsplash.com/photo-1550547660-d9450f859349?auto=format&fit=crop&w=900&q=85",
        "https://images.unsplash.com/photo-1574071318508-1cdbab80d002?auto=format&fit=crop&w=900&q=85",
        "https://images.unsplash.com/photo-1628840042765-356cda07504e?auto=format&fit=crop&w=900&q=85",
        "https://images.unsplash.com/photo-1579751626657-72bc17010498?auto=format&fit=crop&w=900&q=85",
        "https://images.unsplash.com/photo-1579871494447-9811cf80d66c?auto=format&fit=crop&w=900&q=85",
        "https://images.unsplash.com/photo-1617196034796-73dfa7b1fd56?auto=format&fit=crop&w=900&q=85",
        "https://images.unsplash.com/photo-1553621042-f6e147245754?auto=format&fit=crop&w=900&q=85",
        "https://images.unsplash.com/photo-1546793665-c74683f339c1?auto=format&fit=crop&w=900&q=85",
        "https://images.unsplash.com/photo-1540420773420-3366772f4999?auto=format&fit=crop&w=900&q=85",
        "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=900&q=85",
        "https://images.unsplash.com/photo-1623084921164-4a8c5c37a912?auto=format&fit=crop&w=900&q=85",
        "https://images.unsplash.com/photo-1461023058943-07fcbe16d735?auto=format&fit=crop&w=900&q=85",
        "https://images.unsplash.com/photo-1600271886742-f049cd451bba?auto=format&fit=crop&w=900&q=85"
    ];

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Ignore(p => p.ImagesUrls);

        builder.OwnsMany<Image>("_images", imageBuilder =>
        {
            imageBuilder.ToTable("ProductImages");

            imageBuilder.WithOwner()
                .HasForeignKey("ProductId");

            imageBuilder.HasKey("ProductId", nameof(Image.Position));

            imageBuilder.Property(i => i.Url)
                .IsRequired()
                .HasMaxLength(300)
                .HasColumnName("Url");

            imageBuilder.Property(image => image.Position)
                .IsRequired()
                .ValueGeneratedNever()
                .HasColumnName("Position");

            imageBuilder.HasData(
                CreateSeedImage(1),
                CreateSeedImage(2),
                CreateSeedImage(3),
                CreateSeedImage(4),
                CreateSeedImage(5),
                CreateSeedImage(6),
                CreateSeedImage(7),
                CreateSeedImage(8),
                CreateSeedImage(9),
                CreateSeedImage(10),
                CreateSeedImage(11),
                CreateSeedImage(12),
                CreateSeedImage(13),
                CreateSeedImage(14),
                CreateSeedImage(15));
        });

        builder.Navigation("_images")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey("CategoryId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Promotions)
            .WithMany()
            .UsingEntity("ProductPromotions",
                l => l.HasOne(typeof(Promotion)).WithMany().HasForeignKey("PromotionId"),
                r => r.HasOne(typeof(Product)).WithMany().HasForeignKey("ProductId"))
            .HasData(
                new { ProductId = Guid.Parse("20000000-0000-0000-0000-000000000001"), PromotionId = Guid.Parse("50000000-0000-0000-0000-000000000001") },
                new { ProductId = Guid.Parse("20000000-0000-0000-0000-000000000004"), PromotionId = Guid.Parse("50000000-0000-0000-0000-000000000002") },
                new { ProductId = Guid.Parse("20000000-0000-0000-0000-000000000007"), PromotionId = Guid.Parse("50000000-0000-0000-0000-000000000003") });

        builder.Navigation(p => p.Promotions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        var burgers = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var pizzas = Guid.Parse("10000000-0000-0000-0000-000000000002");
        var sushi = Guid.Parse("10000000-0000-0000-0000-000000000003");
        var salads = Guid.Parse("10000000-0000-0000-0000-000000000004");
        var beverages = Guid.Parse("10000000-0000-0000-0000-000000000005");

        builder.HasData(
            new { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), Name = "Classic Burger", Description = "Juicy beef patty with lettuce tomato and cheese", Price = 8.99m, CategoryId = burgers },
            new { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), Name = "Veggie Burger", Description = "Plant based patty with fresh vegetables and avocado", Price = 7.99m, CategoryId = burgers },
            new { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), Name = "Bacon Burger", Description = "Crispy bacon with beef patty and cheddar", Price = 9.99m, CategoryId = burgers },
            new { Id = Guid.Parse("20000000-0000-0000-0000-000000000004"), Name = "Margherita", Description = "Classic tomato sauce with fresh mozzarella and basil", Price = 10.99m, CategoryId = pizzas },
            new { Id = Guid.Parse("20000000-0000-0000-0000-000000000005"), Name = "Pepperoni", Description = "Spicy pepperoni with tomato sauce and mozzarella", Price = 12.99m, CategoryId = pizzas },
            new { Id = Guid.Parse("20000000-0000-0000-0000-000000000006"), Name = "Four Cheese", Description = "Blend of mozzarella cheddar parmesan and gorgonzola", Price = 13.99m, CategoryId = pizzas },
            new { Id = Guid.Parse("20000000-0000-0000-0000-000000000007"), Name = "Salmon Roll", Description = "Fresh salmon with rice and seaweed", Price = 11.99m, CategoryId = sushi },
            new { Id = Guid.Parse("20000000-0000-0000-0000-000000000008"), Name = "Tuna Nigiri", Description = "Sliced fresh tuna over seasoned rice", Price = 13.99m, CategoryId = sushi },
            new { Id = Guid.Parse("20000000-0000-0000-0000-000000000009"), Name = "California Roll", Description = "Crab avocado and cucumber wrapped in rice", Price = 10.99m, CategoryId = sushi },
            new { Id = Guid.Parse("20000000-0000-0000-0000-000000000010"), Name = "Caesar Salad", Description = "Romaine lettuce with parmesan and caesar dressing", Price = 8.99m, CategoryId = salads },
            new { Id = Guid.Parse("20000000-0000-0000-0000-000000000011"), Name = "Greek Salad", Description = "Tomatoes cucumbers olives feta cheese and oregano", Price = 7.99m, CategoryId = salads },
            new { Id = Guid.Parse("20000000-0000-0000-0000-000000000012"), Name = "Garden Salad", Description = "Mixed greens with tomatoes carrots and vinaigrette", Price = 6.99m, CategoryId = salads },
            new { Id = Guid.Parse("20000000-0000-0000-0000-000000000013"), Name = "Lemonade", Description = "Freshly squeezed lemon juice with sparkling water", Price = 3.99m, CategoryId = beverages },
            new { Id = Guid.Parse("20000000-0000-0000-0000-000000000014"), Name = "Iced Coffee", Description = "Cold brew coffee with milk and ice", Price = 4.99m, CategoryId = beverages },
            new { Id = Guid.Parse("20000000-0000-0000-0000-000000000015"), Name = "Orange Juice", Description = "Freshly squeezed natural orange juice", Price = 3.99m, CategoryId = beverages });
    }

    private static object CreateSeedImage(int productNumber)
    {
        return new
        {
            ProductId = Guid.Parse($"20000000-0000-0000-0000-{productNumber:D12}"),
            Position = 0,
            Url = SeedImageUrls[productNumber - 1]
        };
    }
}
