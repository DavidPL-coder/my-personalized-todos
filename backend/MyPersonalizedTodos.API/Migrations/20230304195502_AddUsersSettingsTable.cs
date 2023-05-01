using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPersonalizedTodos.API.Migrations
{
    public partial class AddUsersSettingsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SettingsId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UsersSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Italic = table.Column<bool>(type: "bit", nullable: false),
                    Bold = table.Column<bool>(type: "bit", nullable: false),
                    Uppercase = table.Column<bool>(type: "bit", nullable: false),
                    TextColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackgroundColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeaderColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FontSize = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersSettings", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UsersSettings_SettingsId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UsersSettings");

            migrationBuilder.DropIndex(
                name: "IX_Users_SettingsId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SettingsId",
                table: "Users");
        }
    }
}
