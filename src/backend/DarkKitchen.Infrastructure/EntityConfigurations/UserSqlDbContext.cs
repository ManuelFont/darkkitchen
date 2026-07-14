using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DarkKitchen.Infrastructure.EntityConfigurations;

public sealed class UserSqlDbContext : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(25);

        builder.Property(u => u.Email)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Phone)
            .HasConversion(
                phone => phone.Value,
                value => PhoneNumber.FromStorage(value))
            .IsRequired()
            .HasColumnName("Phone");

        builder.Property(u => u.Password)
            .HasConversion(
                password => password.Value,
                value => Password.FromStorage(value))
            .IsRequired()
            .HasColumnName("Password");

        builder.HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        var password = Password.FromStorage("D@rkK!tchen#2026");

        builder.HasData(
            new
            {
                Id = Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"),
                FirstName = "Agustin",
                LastName = "Robaina",
                Email = "agustin.robaina@darkkitchen.com",
                Password = password,
                Phone = PhoneNumber.FromStorage("099000001"),
                RoleId = 3
            },
            new
            {
                Id = Guid.Parse("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"),
                FirstName = "Francisco",
                LastName = "Topolansky",
                Email = "francisco.topolansky@darkkitchen.com",
                Password = password,
                Phone = PhoneNumber.FromStorage("099000002"),
                RoleId = 3
            },
            new
            {
                Id = Guid.Parse("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3"),
                FirstName = "Manuel",
                LastName = "Font",
                Email = "manuel.font@darkkitchen.com",
                Password = password,
                Phone = PhoneNumber.FromStorage("099000003"),
                RoleId = 3
            },
            new
            {
                Id = Guid.Parse("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"),
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@darkkitchen.com",
                Password = password,
                Phone = PhoneNumber.FromStorage("099000004"),
                RoleId = 3
            },
            new
            {
                Id = Guid.Parse("e5e5e5e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5"),
                FirstName = "Customer",
                LastName = "User",
                Email = "customer@darkkitchen.com",
                Password = password,
                Phone = PhoneNumber.FromStorage("099000005"),
                RoleId = 1
            },
            new
            {
                Id = Guid.Parse("f6f6f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6"),
                FirstName = "Dispatcher",
                LastName = "User",
                Email = "dispatcher@darkkitchen.com",
                Password = password,
                Phone = PhoneNumber.FromStorage("099000006"),
                RoleId = 2
            });
    }
}
