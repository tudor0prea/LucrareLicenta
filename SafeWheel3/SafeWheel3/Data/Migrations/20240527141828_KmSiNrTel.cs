using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeWheel3.Data.Migrations
{
    /// <inheritdoc />
    public partial class KmSiNrTel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Km",
                table: "Anunturi",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NrTel",
                table: "Anunturi",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Km",
                table: "Anunturi");

            migrationBuilder.DropColumn(
                name: "NrTel",
                table: "Anunturi");
        }
    }
}
