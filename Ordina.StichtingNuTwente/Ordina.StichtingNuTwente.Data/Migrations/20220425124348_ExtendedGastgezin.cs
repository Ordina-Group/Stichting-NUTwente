using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class ExtendedGastgezin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Gastgezinnen",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "BuddyId",
                table: "Gastgezinnen",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasVOG",
                table: "Gastgezinnen",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Gastgezinnen",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gastgezinnen_BuddyId",
                table: "Gastgezinnen",
                column: "BuddyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gastgezinnen_Users_BuddyId",
                table: "Gastgezinnen",
                column: "BuddyId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gastgezinnen_Users_BuddyId",
                table: "Gastgezinnen");

            migrationBuilder.DropIndex(
                name: "IX_Gastgezinnen_BuddyId",
                table: "Gastgezinnen");

            migrationBuilder.DropColumn(
                name: "BuddyId",
                table: "Gastgezinnen");

            migrationBuilder.DropColumn(
                name: "HasVOG",
                table: "Gastgezinnen");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Gastgezinnen");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Gastgezinnen",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
