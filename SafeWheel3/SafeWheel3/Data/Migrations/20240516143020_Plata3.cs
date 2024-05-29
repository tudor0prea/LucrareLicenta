using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeWheel3.Data.Migrations
{
    /// <inheritdoc />
    public partial class Plata3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plata_AspNetUsers_ApplicationUserId",
                table: "Plata");

            migrationBuilder.DropForeignKey(
                name: "FK_Plata_Comments_CommentID",
                table: "Plata");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Plata",
                table: "Plata");

            migrationBuilder.RenameTable(
                name: "Plata",
                newName: "Plati");

            migrationBuilder.RenameIndex(
                name: "IX_Plata_CommentID",
                table: "Plati",
                newName: "IX_Plati_CommentID");

            migrationBuilder.RenameIndex(
                name: "IX_Plata_ApplicationUserId",
                table: "Plati",
                newName: "IX_Plati_ApplicationUserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Plati",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Plati",
                table: "Plati",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Plati_AspNetUsers_ApplicationUserId",
                table: "Plati",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Plati_Comments_CommentID",
                table: "Plati",
                column: "CommentID",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plati_AspNetUsers_ApplicationUserId",
                table: "Plati");

            migrationBuilder.DropForeignKey(
                name: "FK_Plati_Comments_CommentID",
                table: "Plati");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Plati",
                table: "Plati");

            migrationBuilder.RenameTable(
                name: "Plati",
                newName: "Plata");

            migrationBuilder.RenameIndex(
                name: "IX_Plati_CommentID",
                table: "Plata",
                newName: "IX_Plata_CommentID");

            migrationBuilder.RenameIndex(
                name: "IX_Plati_ApplicationUserId",
                table: "Plata",
                newName: "IX_Plata_ApplicationUserId");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "Plata",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Plata",
                table: "Plata",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Plata_AspNetUsers_ApplicationUserId",
                table: "Plata",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Plata_Comments_CommentID",
                table: "Plata",
                column: "CommentID",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
