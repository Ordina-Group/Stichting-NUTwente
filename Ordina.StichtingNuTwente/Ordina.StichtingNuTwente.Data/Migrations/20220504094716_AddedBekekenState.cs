using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class AddedBekekenState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BekekenDoorBuddy",
                table: "Gastgezinnen",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BekekenDoorIntaker",
                table: "Gastgezinnen",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BekekenDoorBuddy",
                table: "Gastgezinnen");

            migrationBuilder.DropColumn(
                name: "BekekenDoorIntaker",
                table: "Gastgezinnen");
        }
    }
}
