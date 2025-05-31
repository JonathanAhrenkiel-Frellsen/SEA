using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Survey.Api.IntegrationTests.Fixtures;
using Survey.Application;
using Xunit;

namespace Survey.Api.IntegrationTests.EndToEndFlows
{
    public class ExperimenteeSurveyTests : IClassFixture<WebAppFactoryFixture>
    {
        private readonly WebAppFactoryFixture _factory;
        private readonly HttpClient _ownerClient;
        private readonly HttpClient _userClient;

        public ExperimenteeSurveyTests(WebAppFactoryFixture factory)
        {
            _factory = factory;
            _ownerClient = factory.CreateClient();
            _userClient = ClientFor(userId: 3, userType: "3");
        }

        private string GenerateJwt(int userId, string userType)
        {
            var jwtSettings = _factory.Services.GetRequiredService<JwtSettings>();
            var key = System.Text.Encoding.UTF8.GetBytes(jwtSettings.Secret);
            var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                jwtSettings.Issuer, jwtSettings.Audience,
                new[]
                {
                    new System.Security.Claims.Claim("UserId", userId.ToString()),
                    new System.Security.Claims.Claim("UserType", userType)
                },
                expires: System.DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );
            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        private HttpClient ClientFor(int userId, string userType)
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", GenerateJwt(userId, userType));
            return client;
        }

        private async Task<DesignedSurveyDto> CreateAndPublishSurvey()
        {
            var dto = new DesignedSurveyDto
            {
                SurveyTitle = "Take Flow",
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
            await _ownerClient.PostAsync($"/api/ExperimenterApp/surveys/{created.SurveyId}/publish", null);
            return created;
        }

        [Fact]
        public async Task Experimentee_CanGetListOfNewSurveys()
        {
            var survey = await CreateAndPublishSurvey();
            var listResp = await _userClient.PostAsJsonAsync("/api/ExperimenteeApp/GetListofNewSurveys", new UserDto { UserId = 3 });
            listResp.StatusCode.Should().Be(HttpStatusCode.OK);
            var arr = await listResp.Content.ReadFromJsonAsync<DesignedSurveyDto[]>();
            arr.Should().ContainSingle(s => s.SurveyId == survey.SurveyId);
        }

        [Fact]
        public async Task Experimentee_CanSaveAndCompleteSurvey()
        {
            var survey = await CreateAndPublishSurvey();

            // Save answer
            var saveDto = new SurveySaveAnswerDto
            {
                SurveyId = survey.SurveyId.Value,
                QuestionnaireId = survey.Questionnaires![0].QuestionnaireId,
                SurveyAnswer = "Answer"
            };
            var saveResp = await _userClient.PostAsJsonAsync("/api/ExperimenteeApp/SaveSurveyAnswer", saveDto);
            saveResp.StatusCode.Should().Be(HttpStatusCode.OK);

            // Complete
            var compResp = await _userClient.GetAsync($"/api/ExperimenteeApp/CompleteSurvey/{survey.SurveyId}");
            compResp.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Experimentee_CannotLoadUnpublishedSurvey()
        {
            // Create but do NOT publish
            var dto = new DesignedSurveyDto
            {
                SurveyTitle = "NotPub",
                SurveyDescription = "Desc",
                StartDate = System.DateTime.UtcNow,
                EndDate = System.DateTime.UtcNow.AddDays(1),
                SurveyTypeId = 1,
                UserId = 1,
                Questionnaires = new System.Collections.Generic.List<QuestionnaireDto>()
            };
            var resp = await _ownerClient.PostAsJsonAsync("/api/ExperimenterApp/surveys", dto);
            var created = await resp.Content.ReadFromJsonAsync<DesignedSurveyDto>();

            // Try load as experimentee
            var load = await _userClient.GetAsync($"/api/ExperimenteeApp/LoadSurvey/{created.SurveyId}");
            // unpublished → BadRequest
            load.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
