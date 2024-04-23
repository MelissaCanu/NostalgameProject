using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nostalgame.Migrations
{
    /// <inheritdoc />
    public partial class addAvatarToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdAvatar",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IdAvatar",
                table: "AspNetUsers",
                column: "IdAvatar");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Avatars_IdAvatar",
                table: "AspNetUsers",
                column: "IdAvatar",
                principalTable: "Avatars",
                principalColumn: "IdAvatar");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Avatars_IdAvatar",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IdAvatar",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IdAvatar",
                table: "AspNetUsers");
        }
    }
}
