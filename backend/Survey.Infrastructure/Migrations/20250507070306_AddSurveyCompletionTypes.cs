using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Survey.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSurveyCompletionTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SurveyCompletion_SurveyCompletionType_SurveyCompletionTypeId",
                table: "SurveyCompletion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SurveyCompletionType",
                table: "SurveyCompletionType");

            migrationBuilder.RenameTable(
                name: "SurveyCompletionType",
                newName: "SurveyCompletionTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SurveyCompletionTypes",
                table: "SurveyCompletionTypes",
                column: "SurveyCompletionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyCompletion_SurveyCompletionTypes_SurveyCompletionType~",
                table: "SurveyCompletion",
                column: "SurveyCompletionTypeId",
                principalTable: "SurveyCompletionTypes",
                principalColumn: "SurveyCompletionTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SurveyCompletion_SurveyCompletionTypes_SurveyCompletionType~",
                table: "SurveyCompletion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SurveyCompletionTypes",
                table: "SurveyCompletionTypes");

            migrationBuilder.RenameTable(
                name: "SurveyCompletionTypes",
                newName: "SurveyCompletionType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SurveyCompletionType",
                table: "SurveyCompletionType",
                column: "SurveyCompletionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyCompletion_SurveyCompletionType_SurveyCompletionTypeId",
                table: "SurveyCompletion",
                column: "SurveyCompletionTypeId",
                principalTable: "SurveyCompletionType",
                principalColumn: "SurveyCompletionTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
