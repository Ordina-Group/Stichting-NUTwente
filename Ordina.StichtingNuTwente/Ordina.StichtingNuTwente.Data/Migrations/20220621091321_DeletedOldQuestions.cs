using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class DeletedOldQuestions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlchoholEnDrugs",
                table: "PlaatsingsInfos");

            migrationBuilder.DropColumn(
                name: "Belemmering",
                table: "PlaatsingsInfos");

            migrationBuilder.DropColumn(
                name: "FaciliteitenVoorKinderen",
                table: "PlaatsingsInfos");

            migrationBuilder.DropColumn(
                name: "KinderenInDeBuurt",
                table: "PlaatsingsInfos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlchoholEnDrugs",
                table: "PlaatsingsInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Belemmering",
                table: "PlaatsingsInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FaciliteitenVoorKinderen",
                table: "PlaatsingsInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KinderenInDeBuurt",
                table: "PlaatsingsInfos",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
