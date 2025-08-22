using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feedbacktool.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIsTeacherToRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0); // Student by default

            // Migrate old values if IsTeacher still exists
            migrationBuilder.Sql("""
                                     UPDATE "Users"
                                     SET "Role" = CASE WHEN "IsTeacher" = TRUE THEN 1 ELSE 0 END
                                 """);

            migrationBuilder.DropColumn(
                name: "IsTeacher",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTeacher",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("""
                                     UPDATE "Users"
                                     SET "IsTeacher" = CASE WHEN "Role" = 1 THEN TRUE ELSE FALSE END
                                 """);

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");
        }

    }
}
