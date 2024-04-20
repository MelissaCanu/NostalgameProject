using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nostalgame.Migrations
{
    /// <inheritdoc />
    public partial class addNoleggioTableUpdateUtenteAndVideogioco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Noleggi",
                columns: table => new
                {
                    IdNoleggio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdVideogioco = table.Column<int>(type: "int", nullable: false),
                    IdUtenteNoleggiante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataInizio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFine = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IndirizzoSpedizione = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CostoNoleggio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StripePaymentId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Noleggi", x => x.IdNoleggio);
                    table.ForeignKey(
                        name: "FK_Noleggi_Videogiochi_IdVideogioco",
                        column: x => x.IdVideogioco,
                        principalTable: "Videogiochi",
                        principalColumn: "IdVideogioco",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Noleggi_IdVideogioco",
                table: "Noleggi",
                column: "IdVideogioco");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Noleggi");
        }
    }
}
