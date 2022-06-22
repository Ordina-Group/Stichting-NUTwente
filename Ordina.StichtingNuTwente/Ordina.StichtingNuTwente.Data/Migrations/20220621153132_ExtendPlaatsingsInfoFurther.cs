using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class ExtendPlaatsingsInfoFurther : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sanitair",
                table: "PlaatsingsInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Toegankelijkheid",
                table: "PlaatsingsInfos",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sanitair",
                table: "PlaatsingsInfos");

            migrationBuilder.DropColumn(
                name: "Toegankelijkheid",
                table: "PlaatsingsInfos");
        }
    }
}
