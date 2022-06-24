using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class ExtendPlaatsingsInfoWithMissingFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DaglichtVerblijfsruimte",
                table: "PlaatsingsInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ElektraSpatwaterdicht",
                table: "PlaatsingsInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hobbys",
                table: "PlaatsingsInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RookmelderAanwezig",
                table: "PlaatsingsInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Talen",
                table: "PlaatsingsInfos",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaglichtVerblijfsruimte",
                table: "PlaatsingsInfos");

            migrationBuilder.DropColumn(
                name: "ElektraSpatwaterdicht",
                table: "PlaatsingsInfos");

            migrationBuilder.DropColumn(
                name: "Hobbys",
                table: "PlaatsingsInfos");

            migrationBuilder.DropColumn(
                name: "RookmelderAanwezig",
                table: "PlaatsingsInfos");

            migrationBuilder.DropColumn(
                name: "Talen",
                table: "PlaatsingsInfos");
        }
    }
}
