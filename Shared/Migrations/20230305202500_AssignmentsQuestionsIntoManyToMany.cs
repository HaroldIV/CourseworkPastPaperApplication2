using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseworkPastPaperApplication2.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AssignmentsQuestionsIntoManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Assignments_AssignmentId",
                schema: "public",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_AssignmentId",
                schema: "public",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AssignmentId",
                schema: "public",
                table: "Questions");

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
                name: "IX_AssignmentQuestion_QuestionsId",
                schema: "public",
                table: "AssignmentQuestion",
                column: "QuestionsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignmentQuestion",
                schema: "public");

            migrationBuilder.AddColumn<Guid>(
                name: "AssignmentId",
                schema: "public",
                table: "Questions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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
        }
    }
}
