using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nostalgame.Migrations
{
    /// <inheritdoc />
    public partial class ForeignKeysInVideogiocoAndRegistrazione : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IdUtente",
                table: "Registrazioni",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Videogiochi_IdGenere",
                table: "Videogiochi",
                column: "IdGenere");

            migrationBuilder.CreateIndex(
                name: "IX_Registrazioni_IdAbbonamento",
                table: "Registrazioni",
                column: "IdAbbonamento");

            migrationBuilder.CreateIndex(
                name: "IX_Registrazioni_IdUtente",
                table: "Registrazioni",
                column: "IdUtente");

            migrationBuilder.AddForeignKey(
                name: "FK_Registrazioni_Abbonamenti_IdAbbonamento",
                table: "Registrazioni",
                column: "IdAbbonamento",
                principalTable: "Abbonamenti",
                principalColumn: "IdAbbonamento",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrazioni_AspNetUsers_IdUtente",
                table: "Registrazioni",
                column: "IdUtente",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Videogiochi_Generi_IdGenere",
                table: "Videogiochi",
                column: "IdGenere",
                principalTable: "Generi",
                principalColumn: "IdGenere",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registrazioni_Abbonamenti_IdAbbonamento",
                table: "Registrazioni");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrazioni_AspNetUsers_IdUtente",
                table: "Registrazioni");

            migrationBuilder.DropForeignKey(
                name: "FK_Videogiochi_Generi_IdGenere",
                table: "Videogiochi");

            migrationBuilder.DropIndex(
                name: "IX_Videogiochi_IdGenere",
                table: "Videogiochi");

            migrationBuilder.DropIndex(
                name: "IX_Registrazioni_IdAbbonamento",
                table: "Registrazioni");

            migrationBuilder.DropIndex(
                name: "IX_Registrazioni_IdUtente",
                table: "Registrazioni");

            migrationBuilder.AlterColumn<string>(
                name: "IdUtente",
                table: "Registrazioni",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
