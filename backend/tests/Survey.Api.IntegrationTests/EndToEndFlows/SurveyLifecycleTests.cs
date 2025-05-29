using FluentAssertions;
using Survey.Api.IntegrationTests.Fixtures;
using Survey.API;
using Survey.Application;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Survey.Api.IntegrationTests.EndToEndFlows
{
    public class SurveyLifecycleTests : IClassFixture<WebAppFactoryFixture>
    {
        private readonly HttpClient _client;

        public SurveyLifecycleTests(WebAppFactoryFixture factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Create_Publish_Delete_Works()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var createDto = new DesignedSurveyDto
            {
                SurveyTitle = "Integration Survey",
                SurveyDescription = "Desc",
                StartDate = now,
                EndDate = now.AddDays(1),
                SurveyTypeId = 1,
                Questionnaires = new List<QuestionnaireDto>()  // required
            };

            // Act: CREATE
            var createResp = await _client.PostAsJsonAsync(
                "/api/ExperimenterApp/surveys",
                createDto
            );
            createResp.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await createResp.Content.ReadFromJsonAsync<DesignedSurveyDto>();
            var id = created!.SurveyId.Value;

            // Act: PUBLISH
            var pubResp = await _client.PostAsync(
                $"/api/ExperimenterApp/surveys/{id}/publish",
                null
            );
            pubResp.StatusCode.Should().Be(HttpStatusCode.OK);

            // Act: DELETE
            var delResp = await _client.DeleteAsync(
                $"/api/ExperimenterApp/surveys/{id}"
            );
            delResp.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
