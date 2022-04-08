using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class AddedUserToReactie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserDetailsId",
                table: "Reacties",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reacties_UserDetailsId",
                table: "Reacties",
                column: "UserDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reacties_Users_UserDetailsId",
                table: "Reacties",
                column: "UserDetailsId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reacties_Users_UserDetailsId",
                table: "Reacties");

            migrationBuilder.DropIndex(
                name: "IX_Reacties_UserDetailsId",
                table: "Reacties");

            migrationBuilder.DropColumn(
                name: "UserDetailsId",
                table: "Reacties");
        }
    }
}
