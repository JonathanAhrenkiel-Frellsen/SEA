using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Survey.Api.UnitTests.Controllers.TestHelpers;
using Survey.API.Controllers;
using Survey.Application;
using Survey.Infrastructure.Data;
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
    }
}