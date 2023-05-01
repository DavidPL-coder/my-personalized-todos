using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPersonalizedTodos.API.Migrations
{
    public partial class ChangeToDoContentIntoDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "ToDos",
                newName: "Description");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "ToDos",
                newName: "Content");
        }
    }
}
