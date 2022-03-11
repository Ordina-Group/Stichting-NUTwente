using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gastgezinnen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gastgezinnen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reacties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatumIngevuld = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FormulierId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reacties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vrijwilligers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    GastgezinId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vrijwilligers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vrijwilligers_Gastgezinnen_GastgezinId",
                        column: x => x.GastgezinId,
                        principalTable: "Gastgezinnen",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Antwoorden",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nummer = table.Column<int>(type: "int", nullable: false),
                    ReactieId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Antwoorden", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Antwoorden_Reacties_ReactieId",
                        column: x => x.ReactieId,
                        principalTable: "Reacties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Antwoorden_ReactieId",
                table: "Antwoorden",
                column: "ReactieId");

            migrationBuilder.CreateIndex(
                name: "IX_Vrijwilligers_GastgezinId",
                table: "Vrijwilligers",
                column: "GastgezinId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Antwoorden");

            migrationBuilder.DropTable(
                name: "Vrijwilligers");

            migrationBuilder.DropTable(
                name: "Reacties");

            migrationBuilder.DropTable(
                name: "Gastgezinnen");
        }
    }
}
