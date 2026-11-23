using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccesLayer.Database.Migrations
{
    /// <inheritdoc />
    public partial class RegistrationUsesActivitySchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registration_Activity_ActivityId",
                table: "Registration");

            migrationBuilder.RenameColumn(
                name: "ActivityId",
                table: "Registration",
                newName: "ActivityScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_Registration_ActivityId",
                table: "Registration",
                newName: "IX_Registration_ActivityScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Registration_ActivitySchedule_ActivityScheduleId",
                table: "Registration",
                column: "ActivityScheduleId",
                principalTable: "ActivitySchedule",
                principalColumn: "ActivityScheduleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.CreateIndex(
                name: "IX_Registration_UserId_ActivityScheduleId",
                table: "Registration",
                columns: new[] { "UserId", "ActivityScheduleId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registration_ActivitySchedule_ActivityScheduleId",
                table: "Registration");

            migrationBuilder.DropIndex(
                name: "IX_Registration_UserId_ActivityScheduleId",
                table: "Registration");

            migrationBuilder.RenameColumn(
                name: "ActivityScheduleId",
                table: "Registration",
                newName: "ActivityId");

            migrationBuilder.RenameIndex(
                name: "IX_Registration_ActivityScheduleId",
                table: "Registration",
                newName: "IX_Registration_ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Registration_Activity_ActivityId",
                table: "Registration",
                column: "ActivityId",
                principalTable: "Activity",
                principalColumn: "ActivityId",
                onDelete: ReferentialAction.Restrict);

        }
    }
}
