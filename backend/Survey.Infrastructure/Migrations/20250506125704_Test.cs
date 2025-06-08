using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Survey.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questionnaires_Surveys_SurveyId",
                table: "Questionnaires");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyAnswer_Questionnaires_QuestionnaireId",
                table: "SurveyAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyAnswer_SurveyCompletion_SurveyCompletionId1",
                table: "SurveyAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyAnswer_Surveys_SurveyId",
                table: "SurveyAnswer");

            migrationBuilder.DropTable(
                name: "Surveys_Ignore");

            migrationBuilder.DropIndex(
                name: "IX_SurveyAnswer_SurveyCompletionId1",
                table: "SurveyAnswer");

            migrationBuilder.DropIndex(
                name: "IX_SurveyAnswer_SurveyId",
                table: "SurveyAnswer");

            migrationBuilder.DropColumn(
                name: "SurveyCompletionId1",
                table: "SurveyAnswer");

            migrationBuilder.DropColumn(
                name: "SurveyId",
                table: "SurveyAnswer");

            migrationBuilder.AddColumn<string>(
                name: "PrivateKey",
                table: "Surveys",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SurveyTypeId",
                table: "Surveys",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SurveyCompletionTypeId",
                table: "SurveyCompletion",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SurveyCompletionType",
                columns: table => new
                {
                    SurveyCompletionTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SurveyCompletionTypeName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyCompletionType", x => x.SurveyCompletionTypeId);
                });

            migrationBuilder.CreateTable(
                name: "SurveyTypes",
                columns: table => new
                {
                    SurveyTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SurveyTypeName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyTypes", x => x.SurveyTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_SurveyTypeId",
                table: "Surveys",
                column: "SurveyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyCompletion_SurveyCompletionTypeId",
                table: "SurveyCompletion",
                column: "SurveyCompletionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyCompletion_SurveyId",
                table: "SurveyCompletion",
                column: "SurveyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questionnaires_Surveys_SurveyId",
                table: "Questionnaires",
                column: "SurveyId",
                principalTable: "Surveys",
                principalColumn: "SurveyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyAnswer_Questionnaires_QuestionnaireId",
                table: "SurveyAnswer",
                column: "QuestionnaireId",
                principalTable: "Questionnaires",
                principalColumn: "QuestionnaireId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyCompletion_SurveyCompletionType_SurveyCompletionTypeId",
                table: "SurveyCompletion",
                column: "SurveyCompletionTypeId",
                principalTable: "SurveyCompletionType",
                principalColumn: "SurveyCompletionTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyCompletion_Surveys_SurveyId",
                table: "SurveyCompletion",
                column: "SurveyId",
                principalTable: "Surveys",
                principalColumn: "SurveyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Surveys_SurveyTypes_SurveyTypeId",
                table: "Surveys",
                column: "SurveyTypeId",
                principalTable: "SurveyTypes",
                principalColumn: "SurveyTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questionnaires_Surveys_SurveyId",
                table: "Questionnaires");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyAnswer_Questionnaires_QuestionnaireId",
                table: "SurveyAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyCompletion_SurveyCompletionType_SurveyCompletionTypeId",
                table: "SurveyCompletion");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyCompletion_Surveys_SurveyId",
                table: "SurveyCompletion");

            migrationBuilder.DropForeignKey(
                name: "FK_Surveys_SurveyTypes_SurveyTypeId",
                table: "Surveys");

            migrationBuilder.DropTable(
                name: "SurveyCompletionType");

            migrationBuilder.DropTable(
                name: "SurveyTypes");

            migrationBuilder.DropIndex(
                name: "IX_Surveys_SurveyTypeId",
                table: "Surveys");

            migrationBuilder.DropIndex(
                name: "IX_SurveyCompletion_SurveyCompletionTypeId",
                table: "SurveyCompletion");

            migrationBuilder.DropIndex(
                name: "IX_SurveyCompletion_SurveyId",
                table: "SurveyCompletion");

            migrationBuilder.DropColumn(
                name: "PrivateKey",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "SurveyTypeId",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "SurveyCompletionTypeId",
                table: "SurveyCompletion");

            migrationBuilder.AddColumn<int>(
                name: "SurveyCompletionId1",
                table: "SurveyAnswer",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SurveyId",
                table: "SurveyAnswer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Surveys_Ignore",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surveys_Ignore", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAnswer_SurveyCompletionId1",
                table: "SurveyAnswer",
                column: "SurveyCompletionId1");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAnswer_SurveyId",
                table: "SurveyAnswer",
                column: "SurveyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questionnaires_Surveys_SurveyId",
                table: "Questionnaires",
                column: "SurveyId",
                principalTable: "Surveys",
                principalColumn: "SurveyId");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyAnswer_Questionnaires_QuestionnaireId",
                table: "SurveyAnswer",
                column: "QuestionnaireId",
                principalTable: "Questionnaires",
                principalColumn: "QuestionnaireId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyAnswer_SurveyCompletion_SurveyCompletionId1",
                table: "SurveyAnswer",
                column: "SurveyCompletionId1",
                principalTable: "SurveyCompletion",
                principalColumn: "SurveyCompletionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyAnswer_Surveys_SurveyId",
                table: "SurveyAnswer",
                column: "SurveyId",
                principalTable: "Surveys",
                principalColumn: "SurveyId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
