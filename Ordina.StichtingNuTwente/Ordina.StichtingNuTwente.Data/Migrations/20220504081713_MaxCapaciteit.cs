using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class MaxCapaciteit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxAdults",
                table: "Gastgezinnen",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxChildren",
                table: "Gastgezinnen",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxAdults",
                table: "Gastgezinnen");

            migrationBuilder.DropColumn(
                name: "MaxChildren",
                table: "Gastgezinnen");
        }
    }
}
