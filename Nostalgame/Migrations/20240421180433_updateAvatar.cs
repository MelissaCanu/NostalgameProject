using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nostalgame.Migrations
{
    /// <inheritdoc />
    public partial class updateAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avatars_AspNetUsers_IdUtente",
                table: "Avatars");

            migrationBuilder.DropIndex(
                name: "IX_Avatars_IdUtente",
                table: "Avatars");

            migrationBuilder.DropColumn(
                name: "IdUtente",
                table: "Avatars");

            migrationBuilder.AddColumn<int>(
                name: "IdGenere",
                table: "Avatars",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Avatars_IdGenere",
                table: "Avatars",
                column: "IdGenere");

            migrationBuilder.AddForeignKey(
                name: "FK_Avatars_Generi_IdGenere",
                table: "Avatars",
                column: "IdGenere",
                principalTable: "Generi",
                principalColumn: "IdGenere",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avatars_Generi_IdGenere",
                table: "Avatars");

            migrationBuilder.DropIndex(
                name: "IX_Avatars_IdGenere",
                table: "Avatars");

            migrationBuilder.DropColumn(
                name: "IdGenere",
                table: "Avatars");

            migrationBuilder.AddColumn<string>(
                name: "IdUtente",
                table: "Avatars",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Avatars_IdUtente",
                table: "Avatars",
                column: "IdUtente");

            migrationBuilder.AddForeignKey(
                name: "FK_Avatars_AspNetUsers_IdUtente",
                table: "Avatars",
                column: "IdUtente",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
