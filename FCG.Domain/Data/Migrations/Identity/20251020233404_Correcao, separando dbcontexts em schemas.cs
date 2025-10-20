using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Domain.Data.Migrations.Identity
{
    /// <inheritdoc />
    public partial class Correçãoseparandodbcontextsemschemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.RenameTable(
                name: "UserTokens",
                newName: "UserTokens",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "UserRoles",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "UserLogins",
                newName: "UserLogins",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "UserClaims",
                newName: "UserClaims",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Roles",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "RoleClaims",
                newName: "RoleClaims",
                newSchema: "identity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UserTokens",
                schema: "identity",
                newName: "UserTokens");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "identity",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                schema: "identity",
                newName: "UserRoles");

            migrationBuilder.RenameTable(
                name: "UserLogins",
                schema: "identity",
                newName: "UserLogins");

            migrationBuilder.RenameTable(
                name: "UserClaims",
                schema: "identity",
                newName: "UserClaims");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "identity",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "RoleClaims",
                schema: "identity",
                newName: "RoleClaims");
        }
    }
}
