using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Domain.Data.Migrations.Default
{
    /// <inheritdoc />
    public partial class FcgDbContextajustanaentidadegamerenomeadoapropriedadeNameparatitleeaddsubtitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                schema: "fcg",
                table: "Games");

            migrationBuilder.AlterColumn<string>(
                name: "Genre",
                schema: "fcg",
                table: "Games",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "fcg",
                table: "Games",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubTitle",
                schema: "fcg",
                table: "Games",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "fcg",
                table: "Games",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubTitle",
                schema: "fcg",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "fcg",
                table: "Games");

            migrationBuilder.AlterColumn<string>(
                name: "Genre",
                schema: "fcg",
                table: "Games",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "fcg",
                table: "Games",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "fcg",
                table: "Games",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
