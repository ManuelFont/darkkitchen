using DarkKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DarkKitchen.Infrastructure.EntityConfigurations;

public sealed class RoleSqlDbContext : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.RoleId);

        builder.Property(r => r.RoleName)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasMany(r => r.RolePermissions)
            .WithMany()
            .UsingEntity("RolePermissions",
                l => l.HasOne(typeof(Permission)).WithMany().HasForeignKey("PermissionId"),
                r => r.HasOne(typeof(Role)).WithMany().HasForeignKey("RoleId"))
            .HasData(
                new { RoleId = 1, PermissionId = 1 },
                new { RoleId = 3, PermissionId = 1 },
                new { RoleId = 3, PermissionId = 2 },
                new { RoleId = 3, PermissionId = 3 },
                new { RoleId = 3, PermissionId = 4 },
                new { RoleId = 1, PermissionId = 5 },
                new { RoleId = 3, PermissionId = 5 },
                new { RoleId = 3, PermissionId = 6 },
                new { RoleId = 3, PermissionId = 7 },
                new { RoleId = 3, PermissionId = 8 },
                new { RoleId = 1, PermissionId = 9 },
                new { RoleId = 1, PermissionId = 10 },
                new { RoleId = 3, PermissionId = 10 },
                new { RoleId = 2, PermissionId = 11 },
                new { RoleId = 3, PermissionId = 11 },
                new { RoleId = 1, PermissionId = 12 },
                new { RoleId = 2, PermissionId = 12 },
                new { RoleId = 3, PermissionId = 12 },
                new { RoleId = 3, PermissionId = 13 },
                new { RoleId = 3, PermissionId = 14 },
                new { RoleId = 3, PermissionId = 15 },
                new { RoleId = 3, PermissionId = 16 },
                new { RoleId = 3, PermissionId = 17 },
                new { RoleId = 3, PermissionId = 18 },
                new { RoleId = 3, PermissionId = 19 },
                new { RoleId = 3, PermissionId = 20 },
                new { RoleId = 2, PermissionId = 21 },
                new { RoleId = 3, PermissionId = 21 },
                new { RoleId = 3, PermissionId = 22 },
                new { RoleId = 2, PermissionId = 23 },
                new { RoleId = 2, PermissionId = 24 },
                new { RoleId = 2, PermissionId = 25 },
                new { RoleId = 2, PermissionId = 26 },
                new { RoleId = 3, PermissionId = 27 },
                new { RoleId = 3, PermissionId = 28 },
                new { RoleId = 3, PermissionId = 29 },
                new { RoleId = 1, PermissionId = 30 },
                new { RoleId = 3, PermissionId = 30 },
                new { RoleId = 3, PermissionId = 31 },
                new { RoleId = 3, PermissionId = 32 });

        builder.Navigation(r => r.RolePermissions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasData(
            new { RoleId = 1, RoleName = "Customer" },
            new { RoleId = 2, RoleName = "Dispatcher" },
            new { RoleId = 3, RoleName = "Administrator" });
    }
}
