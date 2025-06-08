using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Survey.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Test123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SurveyAnswer_SurveyCompletion_SurveyCompletionId",
                table: "SurveyAnswer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SurveyCompletion",
                table: "SurveyCompletion");

            migrationBuilder.DropIndex(
                name: "IX_SurveyCompletion_SurveyId",
                table: "SurveyCompletion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SurveyAnswer",
                table: "SurveyAnswer");

            migrationBuilder.DropIndex(
                name: "IX_SurveyAnswer_SurveyCompletionId",
                table: "SurveyAnswer");

            migrationBuilder.DropColumn(
                name: "SurveyCompletionId",
                table: "SurveyCompletion");

            migrationBuilder.RenameColumn(
                name: "SurveyCompletionId",
                table: "SurveyAnswer",
                newName: "SurveyId");

            migrationBuilder.RenameColumn(
                name: "SurveyAnswerId",
                table: "SurveyAnswer",
                newName: "UserId");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaused",
                table: "Surveys",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Published",
                table: "Surveys",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SurveyCompletionTypeId1",
                table: "SurveyCompletion",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "SurveyAnswer",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SurveyCompletion",
                table: "SurveyCompletion",
                columns: new[] { "SurveyId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SurveyAnswer",
                table: "SurveyAnswer",
                columns: new[] { "UserId", "QuestionnaireId" });

            migrationBuilder.CreateIndex(
                name: "IX_SurveyCompletion_SurveyCompletionTypeId1",
                table: "SurveyCompletion",
                column: "SurveyCompletionTypeId1");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAnswer_SurveyId_UserId",
                table: "SurveyAnswer",
                columns: new[] { "SurveyId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyAnswer_SurveyCompletion_SurveyId_UserId",
                table: "SurveyAnswer",
                columns: new[] { "SurveyId", "UserId" },
                principalTable: "SurveyCompletion",
                principalColumns: new[] { "SurveyId", "UserId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyCompletion_SurveyCompletionTypes_SurveyCompletionTyp~1",
                table: "SurveyCompletion",
                column: "SurveyCompletionTypeId1",
                principalTable: "SurveyCompletionTypes",
                principalColumn: "SurveyCompletionTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SurveyAnswer_SurveyCompletion_SurveyId_UserId",
                table: "SurveyAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyCompletion_SurveyCompletionTypes_SurveyCompletionTyp~1",
                table: "SurveyCompletion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SurveyCompletion",
                table: "SurveyCompletion");

            migrationBuilder.DropIndex(
                name: "IX_SurveyCompletion_SurveyCompletionTypeId1",
                table: "SurveyCompletion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SurveyAnswer",
                table: "SurveyAnswer");

            migrationBuilder.DropIndex(
                name: "IX_SurveyAnswer_SurveyId_UserId",
                table: "SurveyAnswer");

            migrationBuilder.DropColumn(
                name: "IsPaused",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "Published",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "SurveyCompletionTypeId1",
                table: "SurveyCompletion");

            migrationBuilder.RenameColumn(
                name: "SurveyId",
                table: "SurveyAnswer",
                newName: "SurveyCompletionId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "SurveyAnswer",
                newName: "SurveyAnswerId");

            migrationBuilder.AddColumn<int>(
                name: "SurveyCompletionId",
                table: "SurveyCompletion",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "SurveyAnswerId",
                table: "SurveyAnswer",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SurveyCompletion",
                table: "SurveyCompletion",
                column: "SurveyCompletionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SurveyAnswer",
                table: "SurveyAnswer",
                column: "SurveyAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyCompletion_SurveyId",
                table: "SurveyCompletion",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAnswer_SurveyCompletionId",
                table: "SurveyAnswer",
                column: "SurveyCompletionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyAnswer_SurveyCompletion_SurveyCompletionId",
                table: "SurveyAnswer",
                column: "SurveyCompletionId",
                principalTable: "SurveyCompletion",
                principalColumn: "SurveyCompletionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
