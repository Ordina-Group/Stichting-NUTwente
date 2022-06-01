using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class CommentForDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReactieId",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ReactieId",
                table: "Comments",
                column: "ReactieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Reacties_ReactieId",
                table: "Comments",
                column: "ReactieId",
                principalTable: "Reacties",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Reacties_ReactieId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ReactieId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ReactieId",
                table: "Comments");
        }
    }
}
