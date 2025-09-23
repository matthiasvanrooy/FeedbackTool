using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Feedbacktool.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "Exercises");

            migrationBuilder.RenameColumn(
                name: "UserScore",
                table: "Exercises",
                newName: "MaxScore");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Subjects",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Subjects",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Exercises",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Exercises",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "ExerciseItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Question = table.Column<string>(type: "text", nullable: false),
                    Answer = table.Column<string>(type: "text", nullable: false),
                    ExerciseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseItems_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScoreRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ExerciseId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoreRecords_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoreRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseItemResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExerciseItemId = table.Column<int>(type: "integer", nullable: false),
                    ScoreRecordId = table.Column<int>(type: "integer", nullable: false),
                    GivenAnswer = table.Column<string>(type: "text", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseItemResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseItemResult_ExerciseItems_ExerciseItemId",
                        column: x => x.ExerciseItemId,
                        principalTable: "ExerciseItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExerciseItemResult_ScoreRecords_ScoreRecordId",
                        column: x => x.ScoreRecordId,
                        principalTable: "ScoreRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseItemResult_ExerciseItemId",
                table: "ExerciseItemResult",
                column: "ExerciseItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseItemResult_ScoreRecordId",
                table: "ExerciseItemResult",
                column: "ScoreRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseItems_ExerciseId",
                table: "ExerciseItems",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreRecords_ExerciseId",
                table: "ScoreRecords",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreRecords_UserId_ExerciseId",
                table: "ScoreRecords",
                columns: new[] { "UserId", "ExerciseId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseItemResult");

            migrationBuilder.DropTable(
                name: "ExerciseItems");

            migrationBuilder.DropTable(
                name: "ScoreRecords");

            migrationBuilder.RenameColumn(
                name: "MaxScore",
                table: "Exercises",
                newName: "UserScore");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Subjects",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Subjects",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Exercises",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Exercises",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Exercises",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
