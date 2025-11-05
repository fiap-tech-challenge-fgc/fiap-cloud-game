using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Domain.Data.Migrations.Default
{
    /// <inheritdoc />
    public partial class AjustenasentidadesPlayerCartGallery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Games_GameId",
                schema: "fcg",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "GameId",
                schema: "fcg",
                table: "CartItems",
                newName: "GalleryId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_GameId",
                schema: "fcg",
                table: "CartItems",
                newName: "IX_CartItems_GalleryId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_CartId_GameId",
                schema: "fcg",
                table: "CartItems",
                newName: "IX_CartItems_CartId_GalleryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_GalleryGames_GalleryId",
                schema: "fcg",
                table: "CartItems",
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
                name: "FK_CartItems_GalleryGames_GalleryId",
                schema: "fcg",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "GalleryId",
                schema: "fcg",
                table: "CartItems",
                newName: "GameId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_GalleryId",
                schema: "fcg",
                table: "CartItems",
                newName: "IX_CartItems_GameId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_CartId_GalleryId",
                schema: "fcg",
                table: "CartItems",
                newName: "IX_CartItems_CartId_GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Games_GameId",
                schema: "fcg",
                table: "CartItems",
                column: "GameId",
                principalSchema: "fcg",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
