using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feedbacktool.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixScoreGroupUsersNav : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassGroups_Subjects_SubjectId",
                table: "ClassGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_UserScoreGroups_ClassGroups_ScoreGroupId",
                table: "UserScoreGroups");

            migrationBuilder.DropIndex(
                name: "IX_ClassGroups_SubjectId",
                table: "ClassGroups");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "ClassGroups");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "ClassGroups");

            migrationBuilder.CreateTable(
                name: "ScoreGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoreGroups_ClassGroups_Id",
                        column: x => x.Id,
                        principalTable: "ClassGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoreGroups_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScoreGroups_SubjectId",
                table: "ScoreGroups",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserScoreGroups_ScoreGroups_ScoreGroupId",
                table: "UserScoreGroups",
                column: "ScoreGroupId",
                principalTable: "ScoreGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserScoreGroups_ScoreGroups_ScoreGroupId",
                table: "UserScoreGroups");

            migrationBuilder.DropTable(
                name: "ScoreGroups");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "ClassGroups",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "ClassGroups",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassGroups_SubjectId",
                table: "ClassGroups",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassGroups_Subjects_SubjectId",
                table: "ClassGroups",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserScoreGroups_ClassGroups_ScoreGroupId",
                table: "UserScoreGroups",
                column: "ScoreGroupId",
                principalTable: "ClassGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
