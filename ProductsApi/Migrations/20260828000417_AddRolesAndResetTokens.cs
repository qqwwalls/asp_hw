using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductsApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesAndResetTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "reset_token",
                table: "users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "reset_token_expires",
                table: "users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reset_token",
                table: "users");

            migrationBuilder.DropColumn(
                name: "reset_token_expires",
                table: "users");

            migrationBuilder.DropColumn(
                name: "role",
                table: "users");
        }
    }
}
