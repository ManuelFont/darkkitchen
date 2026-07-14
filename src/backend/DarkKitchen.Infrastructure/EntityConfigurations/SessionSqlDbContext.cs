using DarkKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DarkKitchen.Infrastructure.EntityConfigurations;

public sealed class SessionSqlDbContext : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasKey(s => s.Token);

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.ExpiresAt)
            .IsRequired();
    }
}
