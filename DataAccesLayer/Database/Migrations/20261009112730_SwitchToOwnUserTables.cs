using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccesLayer.Database.Migrations
{
    /// <inheritdoc />
    public partial class SwitchToOwnUserTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM dbo.[Role] WHERE [Name] = N'User')
BEGIN
    INSERT INTO dbo.[Role] ([Name], [Description])
    VALUES (N'User', N'Default application role');
END;

INSERT INTO dbo.[User] (RoleId, [Name], Email, PasswordHash)
SELECT
    (SELECT TOP (1) RoleId FROM dbo.[Role] WHERE [Name] = N'User' ORDER BY RoleId),
    LEFT(COALESCE(NULLIF(aspUser.UserName, N''), aspUser.Email), 100),
    aspUser.Email,
    aspUser.PasswordHash
FROM dbo.AspNetUsers AS aspUser
WHERE aspUser.Email IS NOT NULL
  AND LEN(aspUser.Email) <= 255
  AND aspUser.PasswordHash IS NOT NULL
  AND NOT EXISTS (
      SELECT 1
      FROM dbo.[User] AS appUser
      WHERE appUser.Email = aspUser.Email
  );");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException(
                "This migration moves accounts into dbo.User and removes Identity tables; reversing it requires a deliberate data migration.");
        }
    }
}
