using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nostalgame.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCostoMensileToCostoAnnuale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameColumn(
                name: "CostoMensile",
                table: "Abbonamenti",
                newName: "CostoAnnuale");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PagamentiAbbonamenti_Abbonamenti_IdAbbonamento",
                table: "PagamentiAbbonamenti");

            migrationBuilder.DropIndex(
                name: "IX_PagamentiAbbonamenti_IdAbbonamento",
                table: "PagamentiAbbonamenti");

            migrationBuilder.RenameColumn(
                name: "CostoAnnuale",
                table: "Abbonamenti",
                newName: "CostoMensile");

            migrationBuilder.AddColumn<int>(
                name: "IdMetodoPagamento",
                table: "PagamentiAbbonamenti",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdMetodoPagamento",
                table: "Noleggi",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MetodoPagamento",
                columns: table => new
                {
                    IdMetodoPagamento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUtente = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Identificatore = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetodoPagamento", x => x.IdMetodoPagamento);
                    table.ForeignKey(
                        name: "FK_MetodoPagamento_AspNetUsers_IdUtente",
                        column: x => x.IdUtente,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PagamentiAbbonamenti_IdMetodoPagamento",
                table: "PagamentiAbbonamenti",
                column: "IdMetodoPagamento");

            migrationBuilder.CreateIndex(
                name: "IX_Noleggi_IdMetodoPagamento",
                table: "Noleggi",
                column: "IdMetodoPagamento");

            migrationBuilder.CreateIndex(
                name: "IX_MetodoPagamento_IdUtente",
                table: "MetodoPagamento",
                column: "IdUtente");

            migrationBuilder.AddForeignKey(
                name: "FK_Noleggi_MetodoPagamento_IdMetodoPagamento",
                table: "Noleggi",
                column: "IdMetodoPagamento",
                principalTable: "MetodoPagamento",
                principalColumn: "IdMetodoPagamento");

            migrationBuilder.AddForeignKey(
                name: "FK_PagamentiAbbonamenti_MetodoPagamento_IdMetodoPagamento",
                table: "PagamentiAbbonamenti",
                column: "IdMetodoPagamento",
                principalTable: "MetodoPagamento",
                principalColumn: "IdMetodoPagamento");
        }
    }
}
