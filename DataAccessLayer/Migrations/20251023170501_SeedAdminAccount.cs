using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "CreateDate", "Email", "IsDeleted", "Password", "Phone", "Role", "Status", "UpdateDate" },
                values: new object[] { 1, new DateTime(2025, 10, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@evrental.com", false, "Admin@123", "0123456789", 2, 0, new DateTime(2025, 10, 24, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
