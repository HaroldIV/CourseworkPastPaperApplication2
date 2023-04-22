using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseworkPastPaperApplication2.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddedFilterOptionsToModelBuilder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_ExamBoard_ExamBoardId",
                schema: "public",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Level_LevelId",
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

            migrationBuilder.RenameTable(
                name: "Level",
                schema: "public",
                newName: "Levels",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ExamBoard",
                schema: "public",
                newName: "ExamBoards",
                newSchema: "public");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Levels",
                schema: "public",
                table: "Levels",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExamBoards",
                schema: "public",
                table: "ExamBoards",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Levels_Id",
                schema: "public",
                table: "Levels",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ExamBoards_Id",
                schema: "public",
                table: "ExamBoards",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_ExamBoards_ExamBoardId",
                schema: "public",
                table: "Questions",
                column: "ExamBoardId",
                principalSchema: "public",
                principalTable: "ExamBoards",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Levels_LevelId",
                schema: "public",
                table: "Questions",
                column: "LevelId",
                principalSchema: "public",
                principalTable: "Levels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_ExamBoards_ExamBoardId",
                schema: "public",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Levels_LevelId",
                schema: "public",
                table: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Levels",
                schema: "public",
                table: "Levels");

            migrationBuilder.DropIndex(
                name: "IX_Levels_Id",
                schema: "public",
                table: "Levels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExamBoards",
                schema: "public",
                table: "ExamBoards");

            migrationBuilder.DropIndex(
                name: "IX_ExamBoards_Id",
                schema: "public",
                table: "ExamBoards");

            migrationBuilder.RenameTable(
                name: "Levels",
                schema: "public",
                newName: "Level",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ExamBoards",
                schema: "public",
                newName: "ExamBoard",
                newSchema: "public");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_ExamBoard_ExamBoardId",
                schema: "public",
                table: "Questions",
                column: "ExamBoardId",
                principalSchema: "public",
                principalTable: "ExamBoard",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Level_LevelId",
                schema: "public",
                table: "Questions",
                column: "LevelId",
                principalSchema: "public",
                principalTable: "Level",
                principalColumn: "Id");
        }
    }
}
