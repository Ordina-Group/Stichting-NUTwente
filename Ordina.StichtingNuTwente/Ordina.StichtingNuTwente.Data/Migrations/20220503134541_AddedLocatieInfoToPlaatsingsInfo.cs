using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class AddedLocatieInfoToPlaatsingsInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdresVanLocatie",
                table: "PlaatsingsInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlaatsnaamVanLocatie",
                table: "PlaatsingsInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostcodeVanLocatie",
                table: "PlaatsingsInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefoonnummerVanLocatie",
                table: "PlaatsingsInfos",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdresVanLocatie",
                table: "PlaatsingsInfos");

            migrationBuilder.DropColumn(
                name: "PlaatsnaamVanLocatie",
                table: "PlaatsingsInfos");

            migrationBuilder.DropColumn(
                name: "PostcodeVanLocatie",
                table: "PlaatsingsInfos");

            migrationBuilder.DropColumn(
                name: "TelefoonnummerVanLocatie",
                table: "PlaatsingsInfos");
        }
    }
}
