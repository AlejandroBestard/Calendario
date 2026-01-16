using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calendario.Migrations
{
    /// <inheritdoc />
    public partial class AgregarColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlarmaYaSono",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "CodigoPais",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "EsTodoElDia",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "MinutosAntesAlarma",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "TieneAlarma",
                table: "Eventos");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Inicio",
                table: "Eventos",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fin",
                table: "Eventos",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Eventos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Eventos");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Inicio",
                table: "Eventos",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fin",
                table: "Eventos",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<bool>(
                name: "AlarmaYaSono",
                table: "Eventos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CodigoPais",
                table: "Eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsTodoElDia",
                table: "Eventos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MinutosAntesAlarma",
                table: "Eventos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "TieneAlarma",
                table: "Eventos",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
