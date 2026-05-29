using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LojaDoImovel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoUrlToProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "properties",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "properties");
        }
    }
}
