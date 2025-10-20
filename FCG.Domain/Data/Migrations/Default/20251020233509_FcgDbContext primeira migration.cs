using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Domain.Data.Migrations.Default
{
    /// <inheritdoc />
    public partial class FcgDbContextprimeiramigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "fcg");

            migrationBuilder.CreateTable(
                name: "Players",
                schema: "fcg",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                schema: "fcg",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Genre = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "fcg",
                        principalTable: "Players",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Promotion",
                schema: "fcg",
                columns: table => new
                {
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    PromocaoTipo = table.Column<int>(type: "integer", nullable: false),
                    PromocaoValor = table.Column<decimal>(type: "numeric", nullable: false),
                    PromocaoInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PromocaoFim = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Games_PlayerId",
                schema: "fcg",
                table: "Games",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Promotion",
                schema: "fcg");

            migrationBuilder.DropTable(
                name: "Games",
                schema: "fcg");

            migrationBuilder.DropTable(
                name: "Players",
                schema: "fcg");
        }
    }
}
