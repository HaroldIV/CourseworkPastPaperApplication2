using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseworkPastPaperApplication2.Shared.Migrations
{
    /// <inheritdoc />
    public partial class ChangedQuestionsToContainAPaperResultPerEachStudentForEachAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "QuestionId",
                schema: "public",
                table: "PaperResults",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PaperResults_QuestionId",
                schema: "public",
                table: "PaperResults",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaperResults_Questions_QuestionId",
                schema: "public",
                table: "PaperResults",
                column: "QuestionId",
                principalSchema: "public",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaperResults_Questions_QuestionId",
                schema: "public",
                table: "PaperResults");

            migrationBuilder.DropIndex(
                name: "IX_PaperResults_QuestionId",
                schema: "public",
                table: "PaperResults");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                schema: "public",
                table: "PaperResults");
        }
    }
}
