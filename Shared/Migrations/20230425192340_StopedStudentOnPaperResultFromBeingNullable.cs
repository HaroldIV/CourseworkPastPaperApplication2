using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseworkPastPaperApplication2.Shared.Migrations
{
    /// <inheritdoc />
    public partial class StopedStudentOnPaperResultFromBeingNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaperResults_Students_StudentId",
                schema: "public",
                table: "PaperResults");

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId",
                schema: "public",
                table: "PaperResults",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaperResults_Students_StudentId",
                schema: "public",
                table: "PaperResults",
                column: "StudentId",
                principalSchema: "public",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaperResults_Students_StudentId",
                schema: "public",
                table: "PaperResults");

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId",
                schema: "public",
                table: "PaperResults",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_PaperResults_Students_StudentId",
                schema: "public",
                table: "PaperResults",
                column: "StudentId",
                principalSchema: "public",
                principalTable: "Students",
                principalColumn: "Id");
        }
    }
}
