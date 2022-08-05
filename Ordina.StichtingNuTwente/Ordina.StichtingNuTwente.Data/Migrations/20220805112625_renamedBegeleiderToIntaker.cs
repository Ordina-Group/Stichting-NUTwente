using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class renamedBegeleiderToIntaker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gastgezinnen_Users_BegeleiderId",
                table: "Gastgezinnen");

            migrationBuilder.RenameColumn(
                name: "BegeleiderId",
                table: "Gastgezinnen",
                newName: "IntakerId");

            migrationBuilder.RenameIndex(
                name: "IX_Gastgezinnen_BegeleiderId",
                table: "Gastgezinnen",
                newName: "IX_Gastgezinnen_IntakerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gastgezinnen_Users_IntakerId",
                table: "Gastgezinnen",
                column: "IntakerId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gastgezinnen_Users_IntakerId",
                table: "Gastgezinnen");

            migrationBuilder.RenameColumn(
                name: "IntakerId",
                table: "Gastgezinnen",
                newName: "BegeleiderId");

            migrationBuilder.RenameIndex(
                name: "IX_Gastgezinnen_IntakerId",
                table: "Gastgezinnen",
                newName: "IX_Gastgezinnen_BegeleiderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gastgezinnen_Users_BegeleiderId",
                table: "Gastgezinnen",
                column: "BegeleiderId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
