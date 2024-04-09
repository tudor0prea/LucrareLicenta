using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeWheel3.Data.Migrations
{
    /// <inheritdoc />
    public partial class ComentariiSiDinastea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Anunturi_AspNetUsers_UserId",
                table: "Anunturi");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Anunturi",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Anunturi_UserId",
                table: "Anunturi",
                newName: "IX_Anunturi_UserID");

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AnuntId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Anunturi_AnuntId",
                        column: x => x.AnuntId,
                        principalTable: "Anunturi",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AnuntId",
                table: "Comments",
                column: "AnuntId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Anunturi_AspNetUsers_UserID",
                table: "Anunturi",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Anunturi_AspNetUsers_UserID",
                table: "Anunturi");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Anunturi",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Anunturi_UserID",
                table: "Anunturi",
                newName: "IX_Anunturi_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Anunturi_AspNetUsers_UserId",
                table: "Anunturi",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
