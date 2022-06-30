using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class RenamedBezwaarTegenHuisdieren : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HuisdierenMogelijk",
                table: "PlaatsingsInfos",
                newName: "BezwaarTegenHuisdieren");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BezwaarTegenHuisdieren",
                table: "PlaatsingsInfos",
                newName: "HuisdierenMogelijk");
        }
    }
}
