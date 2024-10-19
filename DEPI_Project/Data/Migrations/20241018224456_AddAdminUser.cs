using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Collections.Generic;
using System.Security;

#nullable disable

namespace DEPI_Project.Data.Migrations
{
	/// <inheritdoc />
	public partial class AddAdminUser : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
                INSERT INTO [security].[Users]
                (
                    [Id], [UserType], [ProfilePicture], [CreatedAt], 
                    [UserName], [NormalizedUserName], [Email], [NormalizedEmail], 
                    [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], 
                    [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], 
                    [LockoutEnd], [LockoutEnabled], [AccessFailedCount]
                )
                VALUES
                (
                    N'13b18554-3e3d-4ec8-9f22-bd8ea8bf1d79', 
                    N'Regular', 
                    NULL, 
                    '0001-01-01T00:00:00', 
                    N'Admin0@test.com', 
                    N'ADMIN0@TEST.COM', 
                    N'Admin0@test.com', 
                    N'ADMIN0@TEST.COM', 
                    1, 
                    N'AQAAAAIAAYagAAAAEBq+fyHEVMlX93DVi2E6tA/65VV44bAhrEbwIWffPSebc9ca+pFHFSe+riw0fvsolA==', 
                    N'JP26LZDDJ7BQ6KVUYEGKJR2YAFSWCOBR', 
                    N'88d2bb80-06a8-49c2-93a5-f68889b6e576', 
                    NULL, 
                    0, 
                    0, 
                    NULL, 
                    1, 
                    0
                );
            ");


		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("DELETE FROM [security].[Users] WHERE Id = '13b18554-3e3d-4ec8-9f22-bd8ea8bf1d79'");
		}
    }
}
