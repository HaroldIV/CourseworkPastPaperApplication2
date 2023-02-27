using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseworkPastPaperApplication2.Shared.Migrations
{
    /// <inheritdoc />
    public partial class One : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "Students",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Set = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Due = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "public",
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classes_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalSchema: "public",
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaperResults",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaperResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaperResults_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalSchema: "public",
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaperResults_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "public",
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<byte[]>(type: "bytea", nullable: false),
                    ReadData = table.Column<string>(type: "text", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalSchema: "public",
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassStudent",
                schema: "public",
                columns: table => new
                {
                    CurrentClassesId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassStudent", x => new { x.CurrentClassesId, x.StudentsId });
                    table.ForeignKey(
                        name: "FK_ClassStudent_Classes_CurrentClassesId",
                        column: x => x.CurrentClassesId,
                        principalSchema: "public",
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassStudent_Students_StudentsId",
                        column: x => x.StudentsId,
                        principalSchema: "public",
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_Id",
                schema: "public",
                table: "Assignments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_StudentId",
                schema: "public",
                table: "Assignments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_Id",
                schema: "public",
                table: "Classes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_TeacherId",
                schema: "public",
                table: "Classes",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassStudent_StudentsId",
                schema: "public",
                table: "ClassStudent",
                column: "StudentsId");

            migrationBuilder.CreateIndex(
                name: "IX_PaperResults_AssignmentId",
                schema: "public",
                table: "PaperResults",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaperResults_Id",
                schema: "public",
                table: "PaperResults",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PaperResults_StudentId",
                schema: "public",
                table: "PaperResults",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_AssignmentId",
                schema: "public",
                table: "Questions",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_Id",
                schema: "public",
                table: "Questions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Students_Id",
                schema: "public",
                table: "Students",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_Id",
                schema: "public",
                table: "Teachers",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassStudent",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PaperResults",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Questions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Classes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Assignments",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Teachers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Students",
                schema: "public");
        }
    }
}
