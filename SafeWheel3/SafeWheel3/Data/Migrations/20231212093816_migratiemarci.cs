using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeWheel3.Data.Migrations
{
    /// <inheritdoc />
    public partial class migratiemarci : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_AspNetUsers_VanzatorId",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Dealers_DealerId",
                table: "Cars");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dealers",
                table: "Dealers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cars",
                table: "Cars");

            migrationBuilder.RenameTable(
                name: "Dealers",
                newName: "Marci");

            migrationBuilder.RenameTable(
                name: "Cars",
                newName: "Anunturi");

            migrationBuilder.RenameColumn(
                name: "VanzatorId",
                table: "Anunturi",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Cars_VanzatorId",
                table: "Anunturi",
                newName: "IX_Anunturi_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Cars_DealerId",
                table: "Anunturi",
                newName: "IX_Anunturi_DealerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Marci",
                table: "Marci",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Anunturi",
                table: "Anunturi",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Anunturi_AspNetUsers_UserId",
                table: "Anunturi",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Anunturi_Marci_DealerId",
                table: "Anunturi",
                column: "DealerId",
                principalTable: "Marci",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Anunturi_AspNetUsers_UserId",
                table: "Anunturi");

            migrationBuilder.DropForeignKey(
                name: "FK_Anunturi_Marci_DealerId",
                table: "Anunturi");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Marci",
                table: "Marci");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Anunturi",
                table: "Anunturi");

            migrationBuilder.RenameTable(
                name: "Marci",
                newName: "Dealers");

            migrationBuilder.RenameTable(
                name: "Anunturi",
                newName: "Cars");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Cars",
                newName: "VanzatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Anunturi_UserId",
                table: "Cars",
                newName: "IX_Cars_VanzatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Anunturi_DealerId",
                table: "Cars",
                newName: "IX_Cars_DealerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dealers",
                table: "Dealers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cars",
                table: "Cars",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_AspNetUsers_VanzatorId",
                table: "Cars",
                column: "VanzatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Dealers_DealerId",
                table: "Cars",
                column: "DealerId",
                principalTable: "Dealers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
