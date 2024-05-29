using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeWheel3.Data.Migrations
{
    /// <inheritdoc />
    public partial class Plata4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Tokens",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tokens",
                table: "AspNetUsers");
        }
    }
}
