using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nostalgame.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNoleggioTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Noleggi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Noleggi",
                columns: table => new
                {
                    IdNoleggio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdVideogioco = table.Column<int>(type: "int", nullable: true),
                    NoleggianteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CarrelloNoleggioId = table.Column<int>(type: "int", nullable: true),
                    DataFine = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataInizio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IndirizzoSpedizione = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Restituito = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Noleggi", x => x.IdNoleggio);
                    table.ForeignKey(
                        name: "FK_Noleggi_AspNetUsers_NoleggianteId",
                        column: x => x.NoleggianteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Noleggi_CarrelliNoleggio_CarrelloNoleggioId",
                        column: x => x.CarrelloNoleggioId,
                        principalTable: "CarrelliNoleggio",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Noleggi_Videogiochi_IdVideogioco",
                        column: x => x.IdVideogioco,
                        principalTable: "Videogiochi",
                        principalColumn: "IdVideogioco");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Noleggi_CarrelloNoleggioId",
                table: "Noleggi",
                column: "CarrelloNoleggioId");

            migrationBuilder.CreateIndex(
                name: "IX_Noleggi_IdVideogioco",
                table: "Noleggi",
                column: "IdVideogioco");

            migrationBuilder.CreateIndex(
                name: "IX_Noleggi_NoleggianteId",
                table: "Noleggi",
                column: "NoleggianteId");
        }
    }
}
