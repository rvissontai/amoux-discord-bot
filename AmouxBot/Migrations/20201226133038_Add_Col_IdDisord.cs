using Microsoft.EntityFrameworkCore.Migrations;

namespace AmouxBot.Migrations
{
    public partial class Add_Col_IdDisord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdDiscord",
                table: "Usuarios",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdDiscord",
                table: "Usuarios");
        }
    }
}
