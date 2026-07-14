using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DarkKitchen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedPromotions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Promotions",
                columns: new[] { "Id", "DiscountPercentage", "EndDate", "Name", "ProductId", "StartDate" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000001"), 0.20m, new DateTime(2026, 4, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Summer Burger Deal", new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("50000000-0000-0000-0000-000000000002"), 0.15m, new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Pizza Fiesta", new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("50000000-0000-0000-0000-000000000003"), 0.25m, new DateTime(2026, 5, 31, 0, 0, 0, 0, DateTimeKind.Utc), "Sushi Special", new Guid("20000000-0000-0000-0000-000000000007"), new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Promotions",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Promotions",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Promotions",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"));
        }
    }
}
