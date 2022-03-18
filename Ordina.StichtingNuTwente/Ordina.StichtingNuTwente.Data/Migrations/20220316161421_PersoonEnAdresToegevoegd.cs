using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class PersoonEnAdresToegevoegd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Adres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Straat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Postcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Woonplaats = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fkReactieId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adres_Reacties_fkReactieId",
                        column: x => x.fkReactieId,
                        principalTable: "Reacties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Persoon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeboorteDatum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Geboorteplaats = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefoonnummer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mobiel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nationaliteit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Talen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fkReactieId = table.Column<int>(type: "int", nullable: true),
                    fkAdresId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persoon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persoon_Adres_fkAdresId",
                        column: x => x.fkAdresId,
                        principalTable: "Adres",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Persoon_Reacties_fkReactieId",
                        column: x => x.fkReactieId,
                        principalTable: "Reacties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adres_fkReactieId",
                table: "Adres",
                column: "fkReactieId");

            migrationBuilder.CreateIndex(
                name: "IX_Persoon_fkAdresId",
                table: "Persoon",
                column: "fkAdresId");

            migrationBuilder.CreateIndex(
                name: "IX_Persoon_fkReactieId",
                table: "Persoon",
                column: "fkReactieId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Persoon");

            migrationBuilder.DropTable(
                name: "Adres");
        }
    }
}
