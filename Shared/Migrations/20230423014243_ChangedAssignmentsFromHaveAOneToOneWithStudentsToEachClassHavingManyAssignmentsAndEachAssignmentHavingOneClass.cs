using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseworkPastPaperApplication2.Shared.Migrations
{
    /// <inheritdoc />
    public partial class ChangedAssignmentsFromHaveAOneToOneWithStudentsToEachClassHavingManyAssignmentsAndEachAssignmentHavingOneClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Students_StudentId",
                schema: "public",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_StudentId",
                schema: "public",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "StudentId",
                schema: "public",
                table: "Assignments");

            migrationBuilder.AddColumn<Guid>(
                name: "ClassId",
                schema: "public",
                table: "Assignments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_ClassId",
                schema: "public",
                table: "Assignments",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Classes_ClassId",
                schema: "public",
                table: "Assignments",
                column: "ClassId",
                principalSchema: "public",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Classes_ClassId",
                schema: "public",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_ClassId",
                schema: "public",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "ClassId",
                schema: "public",
                table: "Assignments");

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                schema: "public",
                table: "Assignments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_StudentId",
                schema: "public",
                table: "Assignments",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Students_StudentId",
                schema: "public",
                table: "Assignments",
                column: "StudentId",
                principalSchema: "public",
                principalTable: "Students",
                principalColumn: "Id");
        }
    }
}
