using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeWheel3.Data.Migrations
{
    /// <inheritdoc />
    public partial class Bookmark2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnuntBookmarks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnuntId = table.Column<int>(type: "int", nullable: false),
                    BookmarkId = table.Column<int>(type: "int", nullable: false),
                    BookmarkDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnuntBookmarks", x => new { x.Id, x.AnuntId, x.BookmarkId });
                    table.ForeignKey(
                        name: "FK_AnuntBookmarks_Anunturi_AnuntId",
                        column: x => x.AnuntId,
                        principalTable: "Anunturi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnuntBookmarks_Bookmarks_BookmarkId",
                        column: x => x.BookmarkId,
                        principalTable: "Bookmarks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnuntBookmarks_AnuntId",
                table: "AnuntBookmarks",
                column: "AnuntId");

            migrationBuilder.CreateIndex(
                name: "IX_AnuntBookmarks_BookmarkId",
                table: "AnuntBookmarks",
                column: "BookmarkId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnuntBookmarks");
        }
    }
}
