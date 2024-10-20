using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEPI_Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class lala4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Products_ProductId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Users_UserId",
                table: "Comment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "comments");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_UserId",
                table: "comments",
                newName: "IX_comments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_ProductId",
                table: "comments",
                newName: "IX_comments_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_comments",
                table: "comments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_Products_ProductId",
                table: "comments",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_Users_UserId",
                table: "comments",
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
                name: "FK_comments_Products_ProductId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_Users_UserId",
                table: "comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_comments",
                table: "comments");

            migrationBuilder.RenameTable(
                name: "comments",
                newName: "Comment");

            migrationBuilder.RenameIndex(
                name: "IX_comments_UserId",
                table: "Comment",
                newName: "IX_Comment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_comments_ProductId",
                table: "Comment",
                newName: "IX_Comment_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Products_ProductId",
                table: "Comment",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Users_UserId",
                table: "Comment",
                column: "UserId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
