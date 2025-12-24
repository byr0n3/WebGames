using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebGames.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCoinsAndXpToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Coins",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Xp",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coins",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Xp",
                table: "Users");
        }
    }
}
