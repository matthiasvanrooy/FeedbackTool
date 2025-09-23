using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Feedbacktool.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseItemsAndFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseItemResult_ExerciseItems_ExerciseItemId",
                table: "ExerciseItemResult");

            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseItemResult_ScoreRecords_ScoreRecordId",
                table: "ExerciseItemResult");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExerciseItemResult",
                table: "ExerciseItemResult");

            migrationBuilder.RenameTable(
                name: "ExerciseItemResult",
                newName: "ExerciseItemResults");

            migrationBuilder.RenameIndex(
                name: "IX_ExerciseItemResult_ScoreRecordId",
                table: "ExerciseItemResults",
                newName: "IX_ExerciseItemResults_ScoreRecordId");

            migrationBuilder.RenameIndex(
                name: "IX_ExerciseItemResult_ExerciseItemId",
                table: "ExerciseItemResults",
                newName: "IX_ExerciseItemResults_ExerciseItemId");

            migrationBuilder.AddColumn<int>(
                name: "FeedbackRuleId",
                table: "Exercises",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExerciseItemResults",
                table: "ExerciseItemResults",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "FeedbackRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExerciseId = table.Column<int>(type: "integer", nullable: false),
                    Threshold = table.Column<int>(type: "integer", nullable: false),
                    FeedbackMessage = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedbackRules_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_FeedbackRuleId",
                table: "Exercises",
                column: "FeedbackRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackRules_ExerciseId",
                table: "FeedbackRules",
                column: "ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseItemResults_ExerciseItems_ExerciseItemId",
                table: "ExerciseItemResults",
                column: "ExerciseItemId",
                principalTable: "ExerciseItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseItemResults_ScoreRecords_ScoreRecordId",
                table: "ExerciseItemResults",
                column: "ScoreRecordId",
                principalTable: "ScoreRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Exercises_FeedbackRules_FeedbackRuleId",
                table: "Exercises",
                column: "FeedbackRuleId",
                principalTable: "FeedbackRules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseItemResults_ExerciseItems_ExerciseItemId",
                table: "ExerciseItemResults");

            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseItemResults_ScoreRecords_ScoreRecordId",
                table: "ExerciseItemResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Exercises_FeedbackRules_FeedbackRuleId",
                table: "Exercises");

            migrationBuilder.DropTable(
                name: "FeedbackRules");

            migrationBuilder.DropIndex(
                name: "IX_Exercises_FeedbackRuleId",
                table: "Exercises");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExerciseItemResults",
                table: "ExerciseItemResults");

            migrationBuilder.DropColumn(
                name: "FeedbackRuleId",
                table: "Exercises");

            migrationBuilder.RenameTable(
                name: "ExerciseItemResults",
                newName: "ExerciseItemResult");

            migrationBuilder.RenameIndex(
                name: "IX_ExerciseItemResults_ScoreRecordId",
                table: "ExerciseItemResult",
                newName: "IX_ExerciseItemResult_ScoreRecordId");

            migrationBuilder.RenameIndex(
                name: "IX_ExerciseItemResults_ExerciseItemId",
                table: "ExerciseItemResult",
                newName: "IX_ExerciseItemResult_ExerciseItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExerciseItemResult",
                table: "ExerciseItemResult",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseItemResult_ExerciseItems_ExerciseItemId",
                table: "ExerciseItemResult",
                column: "ExerciseItemId",
                principalTable: "ExerciseItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseItemResult_ScoreRecords_ScoreRecordId",
                table: "ExerciseItemResult",
                column: "ScoreRecordId",
                principalTable: "ScoreRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
