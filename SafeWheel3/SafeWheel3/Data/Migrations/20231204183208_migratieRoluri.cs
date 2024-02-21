using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeWheel3.Data.Migrations
{
    /// <inheritdoc />
    public partial class migratieRoluri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VanzatorId",
                table: "Cars",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_VanzatorId",
                table: "Cars",
                column: "VanzatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_AspNetUsers_VanzatorId",
                table: "Cars",
                column: "VanzatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_AspNetUsers_VanzatorId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_VanzatorId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "VanzatorId",
                table: "Cars");
        }
    }
}
