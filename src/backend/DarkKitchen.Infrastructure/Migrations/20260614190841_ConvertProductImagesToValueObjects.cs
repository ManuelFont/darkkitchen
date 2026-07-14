using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DarkKitchen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertProductImagesToValueObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductImages",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductId_Position",
                table: "ProductImages");

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("30000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("30000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("30000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("30000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("30000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("30000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("30000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("30000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("30000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("30000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("30000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("30000000-0000-0000-0000-000000000015"));

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductImages");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "ProductImages",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductImages",
                table: "ProductImages",
                columns: new[] { "ProductId", "Position" });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "Position", "ProductId", "Url" },
                values: new object[,]
                {
                    { 0, new Guid("20000000-0000-0000-0000-000000000001"), "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=900&q=85" },
                    { 0, new Guid("20000000-0000-0000-0000-000000000002"), "https://images.unsplash.com/photo-1571091718767-18b5b1457add?auto=format&fit=crop&w=900&q=85" },
                    { 0, new Guid("20000000-0000-0000-0000-000000000003"), "https://images.unsplash.com/photo-1550547660-d9450f859349?auto=format&fit=crop&w=900&q=85" },
                    { 0, new Guid("20000000-0000-0000-0000-000000000004"), "https://images.unsplash.com/photo-1574071318508-1cdbab80d002?auto=format&fit=crop&w=900&q=85" },
                    { 0, new Guid("20000000-0000-0000-0000-000000000005"), "https://images.unsplash.com/photo-1628840042765-356cda07504e?auto=format&fit=crop&w=900&q=85" },
                    { 0, new Guid("20000000-0000-0000-0000-000000000006"), "https://images.unsplash.com/photo-1579751626657-72bc17010498?auto=format&fit=crop&w=900&q=85" },
                    { 0, new Guid("20000000-0000-0000-0000-000000000007"), "https://images.unsplash.com/photo-1579871494447-9811cf80d66c?auto=format&fit=crop&w=900&q=85" },
                    { 0, new Guid("20000000-0000-0000-0000-000000000008"), "https://images.unsplash.com/photo-1617196034796-73dfa7b1fd56?auto=format&fit=crop&w=900&q=85" },
                    { 0, new Guid("20000000-0000-0000-0000-000000000009"), "https://images.unsplash.com/photo-1553621042-f6e147245754?auto=format&fit=crop&w=900&q=85" },
                    { 0, new Guid("20000000-0000-0000-0000-000000000010"), "https://images.unsplash.com/photo-1546793665-c74683f339c1?auto=format&fit=crop&w=900&q=85" },
                    { 0, new Guid("20000000-0000-0000-0000-000000000011"), "https://images.unsplash.com/photo-1540420773420-3366772f4999?auto=format&fit=crop&w=900&q=85" },
                    { 0, new Guid("20000000-0000-0000-0000-000000000012"), "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=900&q=85" },
                    { 0, new Guid("20000000-0000-0000-0000-000000000013"), "https://images.unsplash.com/photo-1523677011781-c91d1bbe2f9?auto=format&fit=crop&w=900&q=85" },
                    { 0, new Guid("20000000-0000-0000-0000-000000000014"), "https://images.unsplash.com/photo-1461023058943-07fcbe16d735?auto=format&fit=crop&w=900&q=85" },
                    { 0, new Guid("20000000-0000-0000-0000-000000000015"), "https://images.unsplash.com/photo-1600271886742-f049cd451bba?auto=format&fit=crop&w=900&q=85" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductImages",
                table: "ProductImages");

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumns: new[] { "Position", "ProductId" },
                keyValues: new object[] { 0, new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumns: new[] { "Position", "ProductId" },
                keyValues: new object[] { 0, new Guid("20000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumns: new[] { "Position", "ProductId" },
                keyValues: new object[] { 0, new Guid("20000000-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumns: new[] { "Position", "ProductId" },
                keyValues: new object[] { 0, new Guid("20000000-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumns: new[] { "Position", "ProductId" },
                keyValues: new object[] { 0, new Guid("20000000-0000-0000-0000-000000000005") });

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumns: new[] { "Position", "ProductId" },
                keyValues: new object[] { 0, new Guid("20000000-0000-0000-0000-000000000006") });

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumns: new[] { "Position", "ProductId" },
                keyValues: new object[] { 0, new Guid("20000000-0000-0000-0000-000000000007") });

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumns: new[] { "Position", "ProductId" },
                keyValues: new object[] { 0, new Guid("20000000-0000-0000-0000-000000000008") });

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumns: new[] { "Position", "ProductId" },
                keyValues: new object[] { 0, new Guid("20000000-0000-0000-0000-000000000009") });

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumns: new[] { "Position", "ProductId" },
                keyValues: new object[] { 0, new Guid("20000000-0000-0000-0000-000000000010") });

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumns: new[] { "Position", "ProductId" },
                keyValues: new object[] { 0, new Guid("20000000-0000-0000-0000-000000000011") });

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumns: new[] { "Position", "ProductId" },
                keyValues: new object[] { 0, new Guid("20000000-0000-0000-0000-000000000012") });

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumns: new[] { "Position", "ProductId" },
                keyValues: new object[] { 0, new Guid("20000000-0000-0000-0000-000000000013") });

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumns: new[] { "Position", "ProductId" },
                keyValues: new object[] { 0, new Guid("20000000-0000-0000-0000-000000000014") });

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumns: new[] { "Position", "ProductId" },
                keyValues: new object[] { 0, new Guid("20000000-0000-0000-0000-000000000015") });

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "ProductImages",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ProductImages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductImages",
                table: "ProductImages",
                column: "Id");

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
    }
}
