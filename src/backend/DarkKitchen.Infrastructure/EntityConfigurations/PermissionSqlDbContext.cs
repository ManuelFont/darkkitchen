using DarkKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DarkKitchen.Infrastructure.EntityConfigurations;

public sealed class PermissionSqlDbContext : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(p => p.PermissionId);

        builder.Property(p => p.PermissionName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasData(
            new Permission(1, "CanGetProduct"),
            new Permission(2, "CanCreateProduct"),
            new Permission(3, "CanUpdateProduct"),
            new Permission(4, "CanDeleteProduct"),
            new Permission(5, "CanGetCategory"),
            new Permission(6, "CanCreateCategory"),
            new Permission(7, "CanUpdateCategory"),
            new Permission(8, "CanDeleteCategory"),
            new Permission(9, "CanCreateOrder"),
            new Permission(10, "CanGetClientOrders"),
            new Permission(11, "CanGetOrders"),
            new Permission(12, "CanGetOrderDetail"),
            new Permission(13, "CanGetPromotion"),
            new Permission(14, "CanCreatePromotion"),
            new Permission(15, "CanUpdatePromotion"),
            new Permission(16, "CanDeletePromotion"),
            new Permission(17, "CanGetUsers"),
            new Permission(18, "CanCreateUser"),
            new Permission(19, "CanUpdateUser"),
            new Permission(20, "CanDeleteUser"),
            new Permission(21, "CanMarkOrderReady"),
            new Permission(22, "CanCancelOrder"),
            new Permission(23, "CanMarkOrderOnTheWay"),
            new Permission(24, "CanDeliverOrder"),
            new Permission(25, "CanMarkOrderNotDelivered"),
            new Permission(26, "CanMarkOrderDelayed"),
            new Permission(27, "CanGetTopProducts"),
            new Permission(28, "CanGetSalesReport"),
            new Permission(29, "CanGetAuditLog"),
            new Permission(30, "CanGetDeliveryType"),
            new Permission(31, "CanCreateDeliveryType"),
            new Permission(32, "CanUpdateDeliveryType"));
    }
}
