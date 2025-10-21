using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Domain.Data.Migrations.Default
{
    /// <inheritdoc />
    public partial class addentitiesCartPurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Promotion",
                schema: "fcg");

            migrationBuilder.CreateTable(
                name: "CartItems",
                schema: "fcg",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "fcg",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "fcg",
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Promotions",
                schema: "fcg",
                columns: table => new
                {
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: false),
                    StartOf = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndOf = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.GameId);
                    table.ForeignKey(
                        name: "FK_Promotions_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "fcg",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Purchases",
                schema: "fcg",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Purchases_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "fcg",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Purchases_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "fcg",
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_GameId",
                schema: "fcg",
                table: "CartItems",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_PlayerId_GameId",
                schema: "fcg",
                table: "CartItems",
                columns: new[] { "PlayerId", "GameId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_GameId",
                schema: "fcg",
                table: "Purchases",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_PlayerId_GameId",
                schema: "fcg",
                table: "Purchases",
                columns: new[] { "PlayerId", "GameId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems",
                schema: "fcg");

            migrationBuilder.DropTable(
                name: "Promotions",
                schema: "fcg");

            migrationBuilder.DropTable(
                name: "Purchases",
                schema: "fcg");

            migrationBuilder.CreateTable(
                name: "Promotion",
                schema: "fcg",
                columns: table => new
                {
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    PromocaoFim = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PromocaoInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PromocaoTipo = table.Column<int>(type: "integer", nullable: false),
                    PromocaoValor = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotion", x => x.GameId);
                    table.ForeignKey(
                        name: "FK_Promotion_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "fcg",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
