using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPersonalizedTodos.API.Migrations
{
    public partial class AddCascadeDeletingForEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // TODO: Remove test data on production.

            migrationBuilder.DropForeignKey(
                name: "FK_Users_UsersSettings_SettingsId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SettingsId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SettingsId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UsersSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UsersSettings_UserId",
                table: "UsersSettings",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersSettings_Users_UserId",
                table: "UsersSettings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersSettings_Users_UserId",
                table: "UsersSettings");

            migrationBuilder.DropIndex(
                name: "IX_UsersSettings_UserId",
                table: "UsersSettings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UsersSettings");

            migrationBuilder.AddColumn<int>(
                name: "SettingsId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SettingsId",
                table: "Users",
                column: "SettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UsersSettings_SettingsId",
                table: "Users",
                column: "SettingsId",
                principalTable: "UsersSettings",
                principalColumn: "Id");
        }
    }
}
