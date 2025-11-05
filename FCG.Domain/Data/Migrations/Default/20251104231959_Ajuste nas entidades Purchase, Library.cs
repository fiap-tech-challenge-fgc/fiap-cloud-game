using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Domain.Data.Migrations.Default
{
    /// <inheritdoc />
    public partial class AjustenasentidadesPurchaseLibrary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LibraryGames_Games_GameId",
                schema: "fcg",
                table: "LibraryGames");

            migrationBuilder.RenameColumn(
                name: "GameId",
                schema: "fcg",
                table: "LibraryGames",
                newName: "GalleryId");

            migrationBuilder.RenameIndex(
                name: "IX_LibraryGames_GameId",
                schema: "fcg",
                table: "LibraryGames",
                newName: "IX_LibraryGames_GalleryId");

            migrationBuilder.AddForeignKey(
                name: "FK_LibraryGames_GalleryGames_GalleryId",
                schema: "fcg",
                table: "LibraryGames",
                column: "GalleryId",
                principalSchema: "fcg",
                principalTable: "GalleryGames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LibraryGames_GalleryGames_GalleryId",
                schema: "fcg",
                table: "LibraryGames");

            migrationBuilder.RenameColumn(
                name: "GalleryId",
                schema: "fcg",
                table: "LibraryGames",
                newName: "GameId");

            migrationBuilder.RenameIndex(
                name: "IX_LibraryGames_GalleryId",
                schema: "fcg",
                table: "LibraryGames",
                newName: "IX_LibraryGames_GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_LibraryGames_Games_GameId",
                schema: "fcg",
                table: "LibraryGames",
                column: "GameId",
                principalSchema: "fcg",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
