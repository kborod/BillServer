using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BilliardServer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserEntityExtended : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Chips",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Coins",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Exp",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PartiesCount",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalChipsPrize",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WinPartiesCount",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Chips",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Coins",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Exp",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PartiesCount",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TotalChipsPrize",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WinPartiesCount",
                table: "AspNetUsers");
        }
    }
}
