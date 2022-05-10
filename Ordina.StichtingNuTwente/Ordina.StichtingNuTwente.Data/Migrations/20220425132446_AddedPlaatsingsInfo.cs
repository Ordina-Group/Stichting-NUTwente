using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class AddedPlaatsingsInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "fkPlaatsingsId",
                table: "Gastgezinnen",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PlaatsingsInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Belemmering = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KleineKinderen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VolwassenenGrotereKinderen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SlaapkamerRuimte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Privacy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opbergruimte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Faciliteiten = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZelfKoken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KoelkastRuimte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DaglichtSlaapkamer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Roken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlchoholEnDrugs = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VeiligeOpbergruimte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HuisdierenAanwezig = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HuisdierenMogelijk = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Allergieen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VluchtelingOphalen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BasisscholenAanwezig = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KinderenInDeBuurt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FaciliteitenVoorKinderen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fkReactieId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaatsingsInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaatsingsInfos_Reacties_fkReactieId",
                        column: x => x.fkReactieId,
                        principalTable: "Reacties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gastgezinnen_fkPlaatsingsId",
                table: "Gastgezinnen",
                column: "fkPlaatsingsId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaatsingsInfos_fkReactieId",
                table: "PlaatsingsInfos",
                column: "fkReactieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gastgezinnen_PlaatsingsInfos_fkPlaatsingsId",
                table: "Gastgezinnen",
                column: "fkPlaatsingsId",
                principalTable: "PlaatsingsInfos",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gastgezinnen_PlaatsingsInfos_fkPlaatsingsId",
                table: "Gastgezinnen");

            migrationBuilder.DropTable(
                name: "PlaatsingsInfos");

            migrationBuilder.DropIndex(
                name: "IX_Gastgezinnen_fkPlaatsingsId",
                table: "Gastgezinnen");

            migrationBuilder.DropColumn(
                name: "fkPlaatsingsId",
                table: "Gastgezinnen");
        }
    }
}
