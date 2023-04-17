using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CourseworkPastPaperApplication2.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddedFilterOptionsEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamBoard",
                schema: "public",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Level",
                schema: "public",
                table: "Questions");

            migrationBuilder.AddColumn<short>(
                name: "ExamBoardId",
                schema: "public",
                table: "Questions",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "LevelId",
                schema: "public",
                table: "Questions",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "ValidExamBoards",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidExamBoards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ValidLevels",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidLevels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ExamBoardId",
                schema: "public",
                table: "Questions",
                column: "ExamBoardId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_LevelId",
                schema: "public",
                table: "Questions",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ValidExamBoards_Id",
                schema: "public",
                table: "ValidExamBoards",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ValidLevels_Id",
                schema: "public",
                table: "ValidLevels",
                column: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "ValidExamBoards",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ValidLevels",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_Questions_ExamBoardId",
                schema: "public",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_LevelId",
                schema: "public",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ExamBoardId",
                schema: "public",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "LevelId",
                schema: "public",
                table: "Questions");

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
    }
}
