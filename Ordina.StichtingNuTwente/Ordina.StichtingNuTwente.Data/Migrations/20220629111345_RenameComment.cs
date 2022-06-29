using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class RenameComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "Plaatsingen",
                newName: "DepartureComment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DepartureComment",
                table: "Plaatsingen",
                newName: "Comment");
        }
    }
}
