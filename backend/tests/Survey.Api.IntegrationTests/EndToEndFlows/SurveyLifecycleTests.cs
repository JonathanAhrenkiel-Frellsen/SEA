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

        private DesignedSurveyDto GetValidSurveyDto()
        {
            var now = DateTime.UtcNow;
            return new DesignedSurveyDto
            {
                SurveyTitle = "Integration Survey",
                SurveyDescription = "Desc",
                StartDate = now,
                EndDate = now.AddDays(1),
                SurveyTypeId = 1,
                Questionnaires = new List<QuestionnaireDto>()
            };
        }

        [Fact]
        public async Task Create_Succeeds()
        {
            var dto = GetValidSurveyDto();
            var resp = await _client.PostAsJsonAsync("/api/ExperimenterApp/surveys", dto);
            resp.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await resp.Content.ReadFromJsonAsync<DesignedSurveyDto>();
            created!.SurveyId.Should().HaveValue();
        }

        [Fact]
        public async Task Create_InvalidDates_ReturnsBadRequest()
        {
            var dto = GetValidSurveyDto();
            dto.StartDate = dto.EndDate.AddDays(1);
            var resp = await _client.PostAsJsonAsync("/api/ExperimenterApp/surveys", dto);
            resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Edit_Succeeds()
        {
            // first create
            var created = await (await _client.PostAsJsonAsync("/api/ExperimenterApp/surveys", GetValidSurveyDto()))
                                    .Content.ReadFromJsonAsync<DesignedSurveyDto>()
                                ?? throw new Exception();
            var id = created.SurveyId.Value;

            // edit title
            created.SurveyTitle = "Updated Title";
            var editResp = await _client.PostAsJsonAsync("/api/ExperimenterApp/surveys", created);
            editResp.StatusCode.Should().Be(HttpStatusCode.OK);

            var updated = await editResp.Content.ReadFromJsonAsync<DesignedSurveyDto>();
            updated!.SurveyTitle.Should().Be("Updated Title");
        }

        [Fact]
        public async Task Edit_NonExistent_ReturnsNotFound()
        {
            var dto = GetValidSurveyDto();
            dto.SurveyId = 9999;
            var resp = await _client.PostAsJsonAsync("/api/ExperimenterApp/surveys", dto);
            resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Edit_Published_ReturnsBadRequest()
        {
            // create & publish
            var created = await (await _client.PostAsJsonAsync("/api/ExperimenterApp/surveys", GetValidSurveyDto()))
                                    .Content.ReadFromJsonAsync<DesignedSurveyDto>()
                                ?? throw new Exception();
            var id = created.SurveyId.Value;
            await _client.PostAsync($"/api/ExperimenterApp/surveys/{id}/publish", null);

            // attempt edit
            created.SurveyDescription = "New Desc";
            var resp = await _client.PostAsJsonAsync("/api/ExperimenterApp/surveys", created);
            resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Publish_Succeeds()
        {
            var id = (await (await _client.PostAsJsonAsync("/api/ExperimenterApp/surveys", GetValidSurveyDto()))
                                .Content.ReadFromJsonAsync<DesignedSurveyDto>())!.SurveyId.Value;
            var resp = await _client.PostAsync($"/api/ExperimenterApp/surveys/{id}/publish", null);
            resp.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Publish_NonExistent_ReturnsNotFound()
        {
            var resp = await _client.PostAsync("/api/ExperimenterApp/surveys/9999/publish", null);
            resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Publish_AlreadyPublished_ReturnsBadRequest()
        {
            var id = (await (await _client.PostAsJsonAsync("/api/ExperimenterApp/surveys", GetValidSurveyDto()))
                                .Content.ReadFromJsonAsync<DesignedSurveyDto>())!.SurveyId.Value;
            await _client.PostAsync($"/api/ExperimenterApp/surveys/{id}/publish", null);
            var second = await _client.PostAsync($"/api/ExperimenterApp/surveys/{id}/publish", null);
            second.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetSurvey_Succeeds()
        {
            var dto = await (await _client.PostAsJsonAsync("/api/ExperimenterApp/surveys", GetValidSurveyDto()))
                                .Content.ReadFromJsonAsync<DesignedSurveyDto>()
                          ?? throw new Exception();
            var resp = await _client.GetAsync($"/api/ExperimenterApp/surveys/{dto.SurveyId}");
            resp.StatusCode.Should().Be(HttpStatusCode.OK);

            var fetched = await resp.Content.ReadFromJsonAsync<DesignedSurveyDto>();
            fetched!.SurveyId.Should().Be(dto.SurveyId);
        }

        [Fact]
        public async Task GetSurvey_NotFound()
        {
            var resp = await _client.GetAsync("/api/ExperimenterApp/surveys/9999");
            resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task LoadSurvey_Public_Succeeds()
        {
            var dto = await (await _client.PostAsJsonAsync("/api/ExperimenterApp/surveys", GetValidSurveyDto()))
                                .Content.ReadFromJsonAsync<DesignedSurveyDto>()
                          ?? throw new Exception();
            var resp = await _client.GetAsync($"/api/ExperimenterApp/LoadSurvey/{dto.SurveyId}");
            resp.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task LoadSurvey_Private_NoPin_ReturnsForbidden()
        {
            var dto = GetValidSurveyDto();
            dto.PrivateKey = "secret";
            var created = await (await _client.PostAsJsonAsync("/api/ExperimenterApp/surveys", dto))
                                    .Content.ReadFromJsonAsync<DesignedSurveyDto>()
                              ?? throw new Exception();
            var resp = await _client.GetAsync($"/api/ExperimenterApp/LoadSurvey/{created.SurveyId}");
            resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task LoadSurvey_InvalidId_ReturnsBadRequest()
        {
            var resp = await _client.GetAsync("/api/ExperimenterApp/LoadSurvey/0");
            resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PauseSurvey_Succeeds()
        {
            var id = (await (await _client.PostAsJsonAsync("/api/ExperimenterApp/surveys", GetValidSurveyDto()))
                                .Content.ReadFromJsonAsync<DesignedSurveyDto>())!.SurveyId.Value;
            var resp = await _client.PostAsync($"/api/ExperimenterApp/surveys/{id}/pause", null);
            resp.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task PauseSurvey_NotFound()
        {
            var resp = await _client.PostAsync("/api/ExperimenterApp/surveys/9999/pause", null);
            resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task PauseSurvey_AlreadyPaused_ReturnsBadRequest()
        {
            var id = (await (await _client.PostAsJsonAsync("/api/ExperimenterApp/surveys", GetValidSurveyDto()))
                                .Content.ReadFromJsonAsync<DesignedSurveyDto>())!.SurveyId.Value;
            await _client.PostAsync($"/api/ExperimenterApp/surveys/{id}/pause", null);
            var second = await _client.PostAsync($"/api/ExperimenterApp/surveys/{id}/pause", null);
            second.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ResumeSurvey_Succeeds()
        {
            var id = (await (await _client.PostAsJsonAsync("/api/ExperimenterApp/surveys", GetValidSurveyDto()))
                                .Content.ReadFromJsonAsync<DesignedSurveyDto>())!.SurveyId.Value;
            await _client.PostAsync($"/api/ExperimenterApp/surveys/{id}/pause", null);
            var resp = await _client.PostAsync($"/api/ExperimenterApp/surveys/{id}/resume", null);
            resp.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ResumeSurvey_NotFound()
        {
            var resp = await _client.PostAsync("/api/ExperimenterApp/surveys/9999/resume", null);
            resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ResumeSurvey_NotPaused_ReturnsBadRequest()
        {
            var id = (await (await _client.PostAsJsonAsync("/api/ExperimenterApp/surveys", GetValidSurveyDto()))
                                .Content.ReadFromJsonAsync<DesignedSurveyDto>())!.SurveyId.Value;
            var resp = await _client.PostAsync($"/api/ExperimenterApp/surveys/{id}/resume", null);
            resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteSurvey_Succeeds()
        {
            var id = (await (await _client.PostAsJsonAsync("/api/ExperimenterApp/surveys", GetValidSurveyDto()))
                                .Content.ReadFromJsonAsync<DesignedSurveyDto>())!.SurveyId.Value;
            var resp = await _client.DeleteAsync($"/api/ExperimenterApp/surveys/{id}");
            resp.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteSurvey_NotFound()
        {
            var resp = await _client.DeleteAsync("/api/ExperimenterApp/surveys/9999");
            resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
