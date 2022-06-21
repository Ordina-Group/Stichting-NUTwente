using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class ExtendPlaatsingsInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EigenToegangsdeur",
                table: "PlaatsingsInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GezinsLeeftijden",
                table: "PlaatsingsInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SlaapplaatsOpmerking",
                table: "PlaatsingsInfos",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EigenToegangsdeur",
                table: "PlaatsingsInfos");

            migrationBuilder.DropColumn(
                name: "GezinsLeeftijden",
                table: "PlaatsingsInfos");

            migrationBuilder.DropColumn(
                name: "SlaapplaatsOpmerking",
                table: "PlaatsingsInfos");
        }
    }
}
