using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkKitchen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameRolePermissionsColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Permissions_RolePermissionsPermissionId",
                table: "RolePermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_RolePermissionsPermissionId",
                table: "RolePermissions");

            migrationBuilder.RenameColumn(
                name: "RolePermissionsPermissionId",
                table: "RolePermissions",
                newName: "PermissionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Permissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "PermissionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Permissions_PermissionId",
                table: "RolePermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_RoleId",
                table: "RolePermissions");

            migrationBuilder.RenameColumn(
                name: "PermissionId",
                table: "RolePermissions",
                newName: "RolePermissionsPermissionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                columns: new[] { "RoleId", "RolePermissionsPermissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RolePermissionsPermissionId",
                table: "RolePermissions",
                column: "RolePermissionsPermissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Permissions_RolePermissionsPermissionId",
                table: "RolePermissions",
                column: "RolePermissionsPermissionId",
                principalTable: "Permissions",
                principalColumn: "PermissionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
