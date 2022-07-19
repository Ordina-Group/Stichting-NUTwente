using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class renamedMaxChildren : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxChildren",
                table: "Gastgezinnen",
                newName: "MaxYoungerThanThree");

            migrationBuilder.RenameColumn(
                name: "MaxAdults",
                table: "Gastgezinnen",
                newName: "MaxOlderThanTwo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxYoungerThanThree",
                table: "Gastgezinnen",
                newName: "MaxChildren");

            migrationBuilder.RenameColumn(
                name: "MaxOlderThanTwo",
                table: "Gastgezinnen",
                newName: "MaxAdults");
        }
    }
}
