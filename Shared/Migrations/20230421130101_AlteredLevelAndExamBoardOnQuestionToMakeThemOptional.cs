using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseworkPastPaperApplication2.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AlteredLevelAndExamBoardOnQuestionToMakeThemOptional : Migration
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

            migrationBuilder.AlterColumn<short>(
                name: "LevelId",
                schema: "public",
                table: "Questions",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<short>(
                name: "ExamBoardId",
                schema: "public",
                table: "Questions",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_ExamBoard_ExamBoardId",
                schema: "public",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Level_LevelId",
                schema: "public",
                table: "Questions");

            migrationBuilder.AlterColumn<short>(
                name: "LevelId",
                schema: "public",
                table: "Questions",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "ExamBoardId",
                schema: "public",
                table: "Questions",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

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
    }
}
