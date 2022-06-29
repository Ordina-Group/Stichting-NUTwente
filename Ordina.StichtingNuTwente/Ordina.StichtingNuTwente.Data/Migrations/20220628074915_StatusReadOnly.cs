using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class StatusReadOnly : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Gastgezinnen");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Gastgezinnen",
                type: "int",
                nullable: true);
        }
    }
}
