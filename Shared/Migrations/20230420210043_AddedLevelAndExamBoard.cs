using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseworkPastPaperApplication2.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddedLevelAndExamBoard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_ValidExamBoards_ExamBoardId",
                schema: "public",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_ValidLevels_LevelId",
                schema: "public",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "AssignmentQuestion",
                schema: "public");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ValidLevels",
                schema: "public",
                table: "ValidLevels");

            migrationBuilder.DropIndex(
                name: "IX_ValidLevels_Id",
                schema: "public",
                table: "ValidLevels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ValidExamBoards",
                schema: "public",
                table: "ValidExamBoards");

            migrationBuilder.DropIndex(
                name: "IX_ValidExamBoards_Id",
                schema: "public",
                table: "ValidExamBoards");

            migrationBuilder.RenameTable(
                name: "ValidLevels",
                schema: "public",
                newName: "Level",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ValidExamBoards",
                schema: "public",
                newName: "ExamBoard",
                newSchema: "public");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:fuzzystrmatch", ",,");

            migrationBuilder.AddColumn<Guid>(
                name: "AssignmentId",
                schema: "public",
                table: "Questions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Level",
                schema: "public",
                table: "Level",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExamBoard",
                schema: "public",
                table: "ExamBoard",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_AssignmentId",
                schema: "public",
                table: "Questions",
                column: "AssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Assignments_AssignmentId",
                schema: "public",
                table: "Questions",
                column: "AssignmentId",
                principalSchema: "public",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_ExamBoard_ExamBoardId",
                schema: "public",
                table: "Questions",
                column: "ExamBoardId",
                principalSchema: "public",
                principalTable: "ExamBoard",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Level_LevelId",
                schema: "public",
                table: "Questions",
                column: "LevelId",
                principalSchema: "public",
                principalTable: "Level",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Assignments_AssignmentId",
                schema: "public",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_ExamBoard_ExamBoardId",
                schema: "public",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Level_LevelId",
                schema: "public",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_AssignmentId",
                schema: "public",
                table: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Level",
                schema: "public",
                table: "Level");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExamBoard",
                schema: "public",
                table: "ExamBoard");

            migrationBuilder.DropColumn(
                name: "AssignmentId",
                schema: "public",
                table: "Questions");

            migrationBuilder.RenameTable(
                name: "Level",
                schema: "public",
                newName: "ValidLevels",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ExamBoard",
                schema: "public",
                newName: "ValidExamBoards",
                newSchema: "public");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:fuzzystrmatch", ",,");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ValidLevels",
                schema: "public",
                table: "ValidLevels",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ValidExamBoards",
                schema: "public",
                table: "ValidExamBoards",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AssignmentQuestion",
                schema: "public",
                columns: table => new
                {
                    AssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentQuestion", x => new { x.AssignmentId, x.QuestionsId });
                    table.ForeignKey(
                        name: "FK_AssignmentQuestion_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalSchema: "public",
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignmentQuestion_Questions_QuestionsId",
                        column: x => x.QuestionsId,
                        principalSchema: "public",
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ValidLevels_Id",
                schema: "public",
                table: "ValidLevels",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ValidExamBoards_Id",
                schema: "public",
                table: "ValidExamBoards",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentQuestion_QuestionsId",
                schema: "public",
                table: "AssignmentQuestion",
                column: "QuestionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_ValidExamBoards_ExamBoardId",
                schema: "public",
                table: "Questions",
                column: "ExamBoardId",
                principalSchema: "public",
                principalTable: "ValidExamBoards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_ValidLevels_LevelId",
                schema: "public",
                table: "Questions",
                column: "LevelId",
                principalSchema: "public",
                principalTable: "ValidLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
