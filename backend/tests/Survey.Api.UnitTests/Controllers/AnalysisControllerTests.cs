using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Survey.Api.UnitTests.Controllers.TestHelpers;
using Survey.API.Controllers;
using Survey.Application;
using Survey.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Survey.Api.UnitTests.Controllers
{
    /// <summary>
    /// Unit tests for AnalysisController: verifies parsing and empty-result behavior.
    /// </summary>
    public class AnalysisControllerTests
    {
        [Fact]
        public async Task GetSurveyResponseOverTime_InvalidSurveyId_ReturnsBadRequest()
        {
            var context = ControllerTestFixture.CreateInMemoryDb();
            var sut = new AnalysisController(context);

            var result = await sut.GetSurveyResponseOverTime("not-an-int");

            result.Result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Invalid surveyId");
        }


        [Fact]
        public async Task GetSurveyResponseOverTime_NoData_ReturnsEmptyOk()
        {
            // Arrange
            var context = ControllerTestFixture.CreateInMemoryDb();
            var sut = new AnalysisController(context);

            // Act
            var actionResult = await sut.GetSurveyResponseOverTime("1");

            // Assert: status 200 OK
            var ok = actionResult.Result
                .Should().BeOfType<OkObjectResult>().Subject;

            // Assert: an empty sequence, regardless of concrete type
            var items = ok.Value as IEnumerable<object>;
            items.Should().NotBeNull("the controller should return a collection, not null")
                 .And.BeEmpty("there are no SurveyCompletion entries in the DB");
        }


        [Fact]
        public async Task GetSurveyCompletionRate_InvalidSurveyId_ReturnsBadRequest()
        {
            var context = ControllerTestFixture.CreateInMemoryDb();
            var sut = new AnalysisController(context);

            var result = await sut.GetSurveyCompletionRate("xyz");

            result.Result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Invalid surveyId");
        }

        [Fact]
        public async Task GetSurveyCompletionRate_NoQuestions_ReturnsBadRequest()
        {
            var context = ControllerTestFixture.CreateInMemoryDb();
            var sut = new AnalysisController(context);

            var result = await sut.GetSurveyCompletionRate("2");

            result.Result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("No questions found for this survey.");
        }

        [Fact]
        public async Task GetSurveyCompletionRate_WithData_ReturnsOk()
        {
            // Arrange
            var context = ControllerTestFixture.CreateInMemoryDb();
            // 1) Seed one Survey, one Questionnaire, and one SurveyAnswer so totalQuestions = 1, and histogram has one entry.
            // NOTE: make sure you also seed User (if needed) and SurveyCompletion etc., so that SurveyAnswer.UserId references some user.
            var survey = new Domain.Entities.DesignedSurvey { SurveyId = 1, SurveyTitle = "T1", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), SurveyTypeId = 1, UserId = 1 };
            context.Surveys.Add(survey);
            context.SaveChanges();

            context.Questionnaires.Add(new Domain.Entities.Questionnaire
            {
                QuestionnaireId = 10,
                SurveyId = 1,
                QuestionnaireTitle = "Q1",
                InputType = "text",
                Range = "N/A",
                QuestionnairePos = 1
            });
            context.SaveChanges();

            // Create a SurveyCompletion and SurveyAnswer so that AnswerCount = 1 for one user
            context.Users.Add(new Domain.Entities.User { UserId = 1, UserEmail = "u@example.com", UserPassword = "pass", UserTypeId = 1 });
            context.SaveChanges();

            // Insert a SurveyCompletion row (composite key: SurveyId=1, UserId=1)
            context.SurveyCompletion.Add(new Domain.Entities.SurveyCompletion
            {
                SurveyId = 1,
                UserId = 1,
                SurveyCompletionDate = DateTime.UtcNow,
                SurveyCompletionTypeId = 2 // Completed; the type doesn't matter too much as long as there's an Answer linked
            });
            context.SaveChanges();

            // Insert a SurveyAnswer: (UserId=1, QuestionnaireId=10, SurveyId=1)
            context.SurveyAnswer.Add(new Domain.Entities.SurveyAnswer
            {
                QuestionnaireId = 10,
                SurveyId = 1,
                UserId = 1,
                Answer = "anything"
            });
            context.SaveChanges();

            var sut = new AnalysisController(context);

            // Act
            var actionResult = await sut.GetSurveyCompletionRate("1");

            // Assert
            var ok = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
            var data = ok.Value as SurveyCompletionRateDto;
            data.Should().NotBeNull();
            data.TotalQuestions.Should().Be(1);
            data.Histogram.Should().ContainSingle()
                .Which.AnsweredCount.Should().Be(1);
            data.Histogram[0].UserCount.Should().Be(1);
        }
        
        }
    }