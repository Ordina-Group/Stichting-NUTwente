using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class moving_whatsapp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Whatsapp",
                table: "Persoon");

            migrationBuilder.AddColumn<string>(
                name: "Whatsapp",
                table: "PlaatsingsInfos",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Whatsapp",
                table: "PlaatsingsInfos");

            migrationBuilder.AddColumn<bool>(
                name: "Whatsapp",
                table: "Persoon",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
