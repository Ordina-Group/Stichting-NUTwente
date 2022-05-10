using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordina.StichtingNuTwente.Data.Migrations
{
    public partial class AddedCommentEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommenterId = table.Column<int>(type: "int", nullable: false),
                    CommentType = table.Column<int>(type: "int", nullable: false),
                    GastgezinId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Gastgezinnen_GastgezinId",
                        column: x => x.GastgezinId,
                        principalTable: "Gastgezinnen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Users_CommenterId",
                        column: x => x.CommenterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommenterId",
                table: "Comments",
                column: "CommenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_GastgezinId",
                table: "Comments",
                column: "GastgezinId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");
        }
    }
}
