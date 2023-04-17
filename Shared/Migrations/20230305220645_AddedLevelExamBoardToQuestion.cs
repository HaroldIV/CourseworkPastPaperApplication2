using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseworkPastPaperApplication2.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddedLevelExamBoardToQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExamBoard",
                schema: "public",
                table: "Questions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Level",
                schema: "public",
                table: "Questions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamBoard",
                schema: "public",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Level",
                schema: "public",
                table: "Questions");
        }
    }
}
