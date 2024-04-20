using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nostalgame.Migrations
{
    /// <inheritdoc />
    public partial class fknoleggiovideogioco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

          

            migrationBuilder.CreateIndex(
                name: "IX_Noleggi_IdVideogioco",
                table: "Noleggi",
                column: "IdVideogioco");

            migrationBuilder.AddForeignKey(
                name: "FK_Noleggi_Videogiochi_IdVideogioco",
                table: "Noleggi",
                column: "IdVideogioco",
                principalTable: "Videogiochi",
                principalColumn: "IdVideogioco");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropForeignKey(
                name: "FK_Noleggi_Videogiochi_IdVideogioco",
                table: "Noleggi");


            migrationBuilder.DropIndex(
                name: "IX_Noleggi_IdVideogioco",
                table: "Noleggi");

         

          
        }
    }
}
