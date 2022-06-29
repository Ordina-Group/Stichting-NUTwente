using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class ExtendVerwijderdePlaatsing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Plaatsingen",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartureDestination",
                table: "Plaatsingen",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartureReason",
                table: "Plaatsingen",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Plaatsingen");

            migrationBuilder.DropColumn(
                name: "DepartureDestination",
                table: "Plaatsingen");

            migrationBuilder.DropColumn(
                name: "DepartureReason",
                table: "Plaatsingen");
        }
    }
}
