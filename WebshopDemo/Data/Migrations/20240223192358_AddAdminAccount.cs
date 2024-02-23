using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using WebshopDemo.Models;

#nullable disable

namespace WebshopDemo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminAccount : Migration
    {
        const string adminUserGuid = "2B08E0D4-EEA7-46E7-8380-4D085694A54E";
        const string adminRoleGuid = "12FCADA8-F605-40CC-8831-84E032BB0E94";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var hasher = new PasswordHasher<ApplicationUser>();

            var passwordHash = hasher.HashPassword(null, "Password1234");

            migrationBuilder.Sql("INSERT INTO AspNetUsers(Id, UserName, NormalizedUserName,Email,EmailConfirmed," +
                "PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnabled,AccessFailedCount,NormalizedEmail,PasswordHash," +
                $"SecurityStamp,FirstName, LastName, Address) VALUES ('{adminUserGuid}', 'admin@admin.com', 'ADMIN@ADMIN.COM', 'admin@admin.com'," +
                $"0, 0, 0, 0, 0, 'ADMIN@ADMIN.COM', '{passwordHash}', '', 'Admin', 'Admin', 'AdminAddress')");

            migrationBuilder.Sql($"INSERT INTO AspNetRoles (Id, Name, NormalizedName) VALUES ('{adminRoleGuid}', 'Admin', 'ADMIN')");

            migrationBuilder.Sql($"INSERT INTO AspNetUserRoles (UserId, RoleId) Values ('{adminUserGuid}','{adminRoleGuid}')");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DELETE FROM AspNetUserRoles WHERE UserId = '{adminUserGuid}' AND RoleId = '{adminRoleGuid}'");

            migrationBuilder.Sql($"DELETE FROM AspNetUsers WHERE Id = '{adminUserGuid}'");

            migrationBuilder.Sql($"DELETE FROM AspNetRoles WHERE Id = '{adminRoleGuid}'");
        }
    }
}
