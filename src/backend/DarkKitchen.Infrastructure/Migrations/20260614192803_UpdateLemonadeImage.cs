using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkKitchen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLemonadeImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumns: new[] { "Position", "ProductId" },
                keyValues: new object[] { 0, new Guid("20000000-0000-0000-0000-000000000013") },
                column: "Url",
                value: "https://images.unsplash.com/photo-1623084921164-4a8c5c37a912?auto=format&fit=crop&w=900&q=85");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumns: new[] { "Position", "ProductId" },
                keyValues: new object[] { 0, new Guid("20000000-0000-0000-0000-000000000013") },
                column: "Url",
                value: "https://images.unsplash.com/photo-1523677011781-c91d1bbe2f9?auto=format&fit=crop&w=900&q=85");
        }
    }
}
