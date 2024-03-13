using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebshopDemo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRole : Migration
    {
        const string userRoleGuid = "2D597D57-0A4B-4E8E-AC4C-E43DAAF9995E";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"INSERT INTO AspNetRoles (Id, Name, NormalizedName) VALUES ('{userRoleGuid}', 'User', 'USER')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DELETE FROM AspNetRoles WHERE Id = '{userRoleGuid}'");
        }
    }
}
