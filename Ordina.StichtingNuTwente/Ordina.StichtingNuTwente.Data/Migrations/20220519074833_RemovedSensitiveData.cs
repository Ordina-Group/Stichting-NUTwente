using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class RemovedSensitiveData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeboorteDatum",
                table: "Persoon");

            migrationBuilder.DropColumn(
                name: "Geboorteplaats",
                table: "Persoon");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GeboorteDatum",
                table: "Persoon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Geboorteplaats",
                table: "Persoon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
