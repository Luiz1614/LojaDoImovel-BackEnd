using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LojaDoImovel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnterpriseAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "enterprises",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "enterprises",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "enterprises",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "enterprises",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "enterprises");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "enterprises");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "enterprises");

            migrationBuilder.DropColumn(
                name: "State",
                table: "enterprises");
        }
    }
}
