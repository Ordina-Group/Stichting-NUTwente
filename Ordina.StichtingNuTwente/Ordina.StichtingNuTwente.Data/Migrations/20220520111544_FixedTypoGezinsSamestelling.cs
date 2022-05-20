using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class FixedTypoGezinsSamestelling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GezinsSamestelling",
                table: "PlaatsingsInfos",
                newName: "GezinsSamenstelling");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GezinsSamenstelling",
                table: "PlaatsingsInfos",
                newName: "GezinsSamestelling");
        }
    }
}
