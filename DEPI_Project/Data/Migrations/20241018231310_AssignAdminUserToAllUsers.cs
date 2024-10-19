using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEPI_Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class AssignAdminUserToAllUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Insert into security.UserRoles (UserId,RoleId) select '13b18554-3e3d-4ec8-9f22-bd8ea8bf1d79',Id from security.Roles");

		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Delete from security.UserRoles where UserId='13b18554-3e3d-4ec8-9f22-bd8ea8bf1d79'");

		}
    }
}
