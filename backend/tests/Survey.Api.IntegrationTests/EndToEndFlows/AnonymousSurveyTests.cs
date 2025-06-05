using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Survey.Api.IntegrationTests.Fixtures;
using Survey.Application;
using Xunit;
using System.Net.Http;

namespace Survey.Api.IntegrationTests.EndToEndFlows
{
    public class AnonymousSurveyTests : IClassFixture<WebAppFactoryFixture>
    {
        private readonly WebAppFactoryFixture _factory;
        private readonly HttpClient _ownerClient;
        private readonly HttpClient _anonClient;

        public AnonymousSurveyTests(WebAppFactoryFixture factory)
        {
            _factory = factory;
            _ownerClient = factory.CreateClient();
            _anonClient = factory.CreateAnonymousClient();
        }

        private async Task<DesignedSurveyDto> CreateAndPublishSurvey()
        {
            var dto = new DesignedSurveyDto
            {
                SurveyTitle = "AnonSurvey",
                SurveyDescription = "Desc",
                StartDate = System.DateTime.UtcNow,
                EndDate = System.DateTime.UtcNow.AddDays(1),
                SurveyTypeId = 1,
                UserId = 1,
                Questionnaires = new System.Collections.Generic.List<QuestionnaireDto>()
                {
                    new QuestionnaireDto
                    {
                        QuestionnairePos = 1,
                        QuestionnaireTitle = "Q1",
                        InputType = "text",
                        Range = ""
                    }
                }
            };
            var resp = await _ownerClient.PostAsJsonAsync("/api/ExperimenterApp/surveys", dto);
            var created = await resp.Content.ReadFromJsonAsync<DesignedSurveyDto>();
            await _ownerClient.PostAsync($"/api/ExperimenterApp/surveys/{created!.SurveyId}/publish", null);
            return created!;
        }

        [Fact]
        public async Task AnonymousUser_CanCompleteSurvey()
        {
            var survey = await CreateAndPublishSurvey();

            // Load survey anonymously -> gets anon user id
            var load = await _anonClient.GetAsync($"/api/ExperimenteeApp/LoadSurvey/{survey.SurveyId}");
            load.StatusCode.Should().Be(HttpStatusCode.OK);
            var dto = await load.Content.ReadFromJsonAsync<ExperimenteeAppDto>();
            dto!.UserId.Should().NotBeNull();

            _anonClient.DefaultRequestHeaders.Add("X-Anonymous-User", dto!.UserId!.ToString());

            var saveDto = new SurveySaveAnswerDto
            {
                SurveyId = survey.SurveyId!.Value,
                QuestionnaireId = survey.Questionnaires![0].QuestionnaireId,
                SurveyAnswer = "Answer"
            };
            var saveResp = await _anonClient.PostAsJsonAsync("/api/ExperimenteeApp/SaveSurveyAnswer", saveDto);
            saveResp.StatusCode.Should().Be(HttpStatusCode.OK);

            var compResp = await _anonClient.GetAsync($"/api/ExperimenteeApp/CompleteSurvey/{survey.SurveyId}");
            compResp.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
