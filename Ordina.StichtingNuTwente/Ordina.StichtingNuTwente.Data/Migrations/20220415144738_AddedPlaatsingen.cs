using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class AddedPlaatsingen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserDetailsId",
                table: "Reacties",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Plaatsingen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fkGastgezinId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    AgeGroup = table.Column<int>(type: "int", nullable: false),
                    PlacementType = table.Column<int>(type: "int", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VrijwilligerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plaatsingen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plaatsingen_Gastgezinnen_fkGastgezinId",
                        column: x => x.fkGastgezinId,
                        principalTable: "Gastgezinnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Plaatsingen_Users_VrijwilligerId",
                        column: x => x.VrijwilligerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reacties_UserDetailsId",
                table: "Reacties",
                column: "UserDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_Plaatsingen_fkGastgezinId",
                table: "Plaatsingen",
                column: "fkGastgezinId");

            migrationBuilder.CreateIndex(
                name: "IX_Plaatsingen_VrijwilligerId",
                table: "Plaatsingen",
                column: "VrijwilligerId");

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

            migrationBuilder.DropTable(
                name: "Plaatsingen");

            migrationBuilder.DropIndex(
                name: "IX_Reacties_UserDetailsId",
                table: "Reacties");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserDetailsId",
                table: "Reacties");
        }
    }
}
