using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DarkKitchen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "Id", "Position", "ProductId", "Url" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), 0, new Guid("20000000-0000-0000-0000-000000000001"), "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=900&q=85" },
                    { new Guid("30000000-0000-0000-0000-000000000002"), 0, new Guid("20000000-0000-0000-0000-000000000002"), "https://images.unsplash.com/photo-1571091718767-18b5b1457add?auto=format&fit=crop&w=900&q=85" },
                    { new Guid("30000000-0000-0000-0000-000000000003"), 0, new Guid("20000000-0000-0000-0000-000000000003"), "https://images.unsplash.com/photo-1550547660-d9450f859349?auto=format&fit=crop&w=900&q=85" },
                    { new Guid("30000000-0000-0000-0000-000000000004"), 0, new Guid("20000000-0000-0000-0000-000000000004"), "https://images.unsplash.com/photo-1574071318508-1cdbab80d002?auto=format&fit=crop&w=900&q=85" },
                    { new Guid("30000000-0000-0000-0000-000000000005"), 0, new Guid("20000000-0000-0000-0000-000000000005"), "https://images.unsplash.com/photo-1628840042765-356cda07504e?auto=format&fit=crop&w=900&q=85" },
                    { new Guid("30000000-0000-0000-0000-000000000006"), 0, new Guid("20000000-0000-0000-0000-000000000006"), "https://images.unsplash.com/photo-1579751626657-72bc17010498?auto=format&fit=crop&w=900&q=85" },
                    { new Guid("30000000-0000-0000-0000-000000000007"), 0, new Guid("20000000-0000-0000-0000-000000000007"), "https://images.unsplash.com/photo-1579871494447-9811cf80d66c?auto=format&fit=crop&w=900&q=85" },
                    { new Guid("30000000-0000-0000-0000-000000000008"), 0, new Guid("20000000-0000-0000-0000-000000000008"), "https://images.unsplash.com/photo-1617196034796-73dfa7b1fd56?auto=format&fit=crop&w=900&q=85" },
                    { new Guid("30000000-0000-0000-0000-000000000009"), 0, new Guid("20000000-0000-0000-0000-000000000009"), "https://images.unsplash.com/photo-1553621042-f6e147245754?auto=format&fit=crop&w=900&q=85" },
                    { new Guid("30000000-0000-0000-0000-000000000010"), 0, new Guid("20000000-0000-0000-0000-000000000010"), "https://images.unsplash.com/photo-1546793665-c74683f339c1?auto=format&fit=crop&w=900&q=85" },
                    { new Guid("30000000-0000-0000-0000-000000000011"), 0, new Guid("20000000-0000-0000-0000-000000000011"), "https://images.unsplash.com/photo-1540420773420-3366772f4999?auto=format&fit=crop&w=900&q=85" },
                    { new Guid("30000000-0000-0000-0000-000000000012"), 0, new Guid("20000000-0000-0000-0000-000000000012"), "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=900&q=85" },
                    { new Guid("30000000-0000-0000-0000-000000000013"), 0, new Guid("20000000-0000-0000-0000-000000000013"), "https://images.unsplash.com/photo-1523677011781-c91d1bbe2f9?auto=format&fit=crop&w=900&q=85" },
                    { new Guid("30000000-0000-0000-0000-000000000014"), 0, new Guid("20000000-0000-0000-0000-000000000014"), "https://images.unsplash.com/photo-1461023058943-07fcbe16d735?auto=format&fit=crop&w=900&q=85" },
                    { new Guid("30000000-0000-0000-0000-000000000015"), 0, new Guid("20000000-0000-0000-0000-000000000015"), "https://images.unsplash.com/photo-1600271886742-f049cd451bba?auto=format&fit=crop&w=900&q=85" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId_Position",
                table: "ProductImages",
                columns: new[] { "ProductId", "Position" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductImages");
        }
    }
}
