using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEPI_Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class lala5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_Users_UserId",
                schema: "security",
                table: "UserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTokens",
                schema: "security",
                table: "UserTokens");

            migrationBuilder.RenameTable(
                name: "UserTokens",
                schema: "security",
                newName: "Product");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Users_UserId",
                table: "Product",
                column: "UserId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Users_UserId",
                table: "Product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "UserTokens",
                newSchema: "security");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTokens",
                schema: "security",
                table: "UserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_Users_UserId",
                schema: "security",
                table: "UserTokens",
                column: "UserId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
