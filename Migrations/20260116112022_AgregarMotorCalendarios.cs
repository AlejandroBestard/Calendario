using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Calendario.Migrations
{
    /// <inheritdoc />
    public partial class AgregarMotorCalendarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calendarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reglas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CalendarioId = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "interval", nullable: false),
                    DiasSemana = table.Column<List<int>>(type: "integer[]", nullable: false),
                    CalendarioDefinitionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reglas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reglas_Calendarios_CalendarioDefinitionId",
                        column: x => x.CalendarioDefinitionId,
                        principalTable: "Calendarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Excepciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReglaCalendarioId = table.Column<int>(type: "integer", nullable: false),
                    FechaOriginal = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    NuevaHoraInicio = table.Column<TimeSpan>(type: "interval", nullable: true),
                    NuevaHoraFin = table.Column<TimeSpan>(type: "interval", nullable: true),
                    NuevoTitulo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Excepciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Excepciones_Reglas_ReglaCalendarioId",
                        column: x => x.ReglaCalendarioId,
                        principalTable: "Reglas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Excepciones_ReglaCalendarioId",
                table: "Excepciones",
                column: "ReglaCalendarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Reglas_CalendarioDefinitionId",
                table: "Reglas",
                column: "CalendarioDefinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Excepciones");

            migrationBuilder.DropTable(
                name: "Reglas");

            migrationBuilder.DropTable(
                name: "Calendarios");
        }
    }
}
