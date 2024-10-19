using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEPI_Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class addBOtoRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.InsertData(
table: "Roles",
columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
values: new object[] { Guid.NewGuid().ToString(), "BusinessOwner", "BusinessOwner".ToUpper(), Guid.NewGuid().ToString() },
schema: "security"
);
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("DELETE FROM [security].[Roles]");
		}
    }
}
