using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using WebGames.Database.Models.Options;

#nullable disable

namespace WebGames.Database.Migrations
{
    /// <inheritdoc />
    public partial class SetupSynchronization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"create publication {DatabaseSynchronizationOptions.PublicationName} for table \"Users\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"drop publication {DatabaseSynchronizationOptions.PublicationName};");
        }
    }
}
