using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebGames.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<byte[]>(type: "bytea", nullable: false),
                    Email = table.Column<byte[]>(type: "bytea", nullable: false),
                    Password = table.Column<byte[]>(type: "bytea", nullable: false),
                    Flags = table.Column<int>(type: "integer", nullable: false),
                    AccountConfirmationToken = table.Column<Guid>(type: "uuid", nullable: true),
                    PasswordResetToken = table.Column<Guid>(type: "uuid", nullable: true),
                    PasswordResetExpiry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    LastSignInTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false),
                    AccessLockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    HasMfaEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
