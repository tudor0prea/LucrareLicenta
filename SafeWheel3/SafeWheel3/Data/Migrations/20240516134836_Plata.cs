using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeWheel3.Data.Migrations
{
    /// <inheritdoc />
    public partial class Plata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plata",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    CommentID = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plata_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Plata_Comments_CommentID",
                        column: x => x.CommentID,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Plata_ApplicationUserId",
                table: "Plata",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Plata_CommentID",
                table: "Plata",
                column: "CommentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Plata");
        }
    }
}
