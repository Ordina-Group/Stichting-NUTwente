using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class AddedAanmeldFormulierToGastgezin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AanmeldFormulierId",
                table: "Gastgezinnen",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gastgezinnen_AanmeldFormulierId",
                table: "Gastgezinnen",
                column: "AanmeldFormulierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gastgezinnen_Reacties_AanmeldFormulierId",
                table: "Gastgezinnen",
                column: "AanmeldFormulierId",
                principalTable: "Reacties",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gastgezinnen_Reacties_AanmeldFormulierId",
                table: "Gastgezinnen");

            migrationBuilder.DropIndex(
                name: "IX_Gastgezinnen_AanmeldFormulierId",
                table: "Gastgezinnen");

            migrationBuilder.DropColumn(
                name: "AanmeldFormulierId",
                table: "Gastgezinnen");
        }
    }
}
