using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class GastgezinChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gastgezinnen_Users_BegeleiderId",
                table: "Gastgezinnen");

            migrationBuilder.AlterColumn<int>(
                name: "BegeleiderId",
                table: "Gastgezinnen",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "IntakeFormulierId",
                table: "Gastgezinnen",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gastgezinnen_IntakeFormulierId",
                table: "Gastgezinnen",
                column: "IntakeFormulierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gastgezinnen_Reacties_IntakeFormulierId",
                table: "Gastgezinnen",
                column: "IntakeFormulierId",
                principalTable: "Reacties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Gastgezinnen_Users_BegeleiderId",
                table: "Gastgezinnen",
                column: "BegeleiderId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gastgezinnen_Reacties_IntakeFormulierId",
                table: "Gastgezinnen");

            migrationBuilder.DropForeignKey(
                name: "FK_Gastgezinnen_Users_BegeleiderId",
                table: "Gastgezinnen");

            migrationBuilder.DropIndex(
                name: "IX_Gastgezinnen_IntakeFormulierId",
                table: "Gastgezinnen");

            migrationBuilder.DropColumn(
                name: "IntakeFormulierId",
                table: "Gastgezinnen");

            migrationBuilder.AlterColumn<int>(
                name: "BegeleiderId",
                table: "Gastgezinnen",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Gastgezinnen_Users_BegeleiderId",
                table: "Gastgezinnen",
                column: "BegeleiderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
