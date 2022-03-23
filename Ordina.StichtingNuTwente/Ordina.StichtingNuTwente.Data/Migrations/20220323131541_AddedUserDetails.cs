using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class AddedUserDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vrijwilligers");

            migrationBuilder.AddColumn<int>(
                name: "fkGastgezinId",
                table: "Persoon",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BegeleiderId",
                table: "Gastgezinnen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ContactId",
                table: "Gastgezinnen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Gastgezinnen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AADId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Roles = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Persoon_fkGastgezinId",
                table: "Persoon",
                column: "fkGastgezinId");

            migrationBuilder.CreateIndex(
                name: "IX_Gastgezinnen_BegeleiderId",
                table: "Gastgezinnen",
                column: "BegeleiderId");

            migrationBuilder.CreateIndex(
                name: "IX_Gastgezinnen_ContactId",
                table: "Gastgezinnen",
                column: "ContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gastgezinnen_Persoon_ContactId",
                table: "Gastgezinnen",
                column: "ContactId",
                principalTable: "Persoon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Gastgezinnen_Users_BegeleiderId",
                table: "Gastgezinnen",
                column: "BegeleiderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Persoon_Gastgezinnen_fkGastgezinId",
                table: "Persoon",
                column: "fkGastgezinId",
                principalTable: "Gastgezinnen",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gastgezinnen_Persoon_ContactId",
                table: "Gastgezinnen");

            migrationBuilder.DropForeignKey(
                name: "FK_Gastgezinnen_Users_BegeleiderId",
                table: "Gastgezinnen");

            migrationBuilder.DropForeignKey(
                name: "FK_Persoon_Gastgezinnen_fkGastgezinId",
                table: "Persoon");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Persoon_fkGastgezinId",
                table: "Persoon");

            migrationBuilder.DropIndex(
                name: "IX_Gastgezinnen_BegeleiderId",
                table: "Gastgezinnen");

            migrationBuilder.DropIndex(
                name: "IX_Gastgezinnen_ContactId",
                table: "Gastgezinnen");

            migrationBuilder.DropColumn(
                name: "fkGastgezinId",
                table: "Persoon");

            migrationBuilder.DropColumn(
                name: "BegeleiderId",
                table: "Gastgezinnen");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "Gastgezinnen");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Gastgezinnen");

            migrationBuilder.CreateTable(
                name: "Vrijwilligers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GastgezinId = table.Column<int>(type: "int", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Vrijwilligers_GastgezinId",
                table: "Vrijwilligers",
                column: "GastgezinId");
        }
    }
}
