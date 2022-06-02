using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class AddedContactLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContacterId = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GastgezinId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactLog_Gastgezinnen_GastgezinId",
                        column: x => x.GastgezinId,
                        principalTable: "Gastgezinnen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContactLog_Users_ContacterId",
                        column: x => x.ContacterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactLog_ContacterId",
                table: "ContactLog",
                column: "ContacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactLog_GastgezinId",
                table: "ContactLog",
                column: "GastgezinId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactLog");
        }
    }
}
