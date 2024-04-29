using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nostalgame.Migrations
{
    /// <inheritdoc />
    public partial class updateregistrazionemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AddColumn<int>(
                name: "IdAbbonamentoAttuale",
                table: "Registrazioni",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdAbbonamentoAttuale",
                table: "Registrazioni");

           
        }
    }
}
