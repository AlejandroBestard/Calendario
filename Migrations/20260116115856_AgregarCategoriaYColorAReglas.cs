using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calendario.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCategoriaYColorAReglas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Reglas",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Reglas",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Reglas");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Reglas");
        }
    }
}
