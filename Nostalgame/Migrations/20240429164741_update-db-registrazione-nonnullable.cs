using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nostalgame.Migrations
{
    /// <inheritdoc />
    public partial class updatedbregistrazionenonnullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdAbbonamentoAttuale",
                table: "Registrazioni",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdAbbonamentoAttuale",
                table: "Registrazioni",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
