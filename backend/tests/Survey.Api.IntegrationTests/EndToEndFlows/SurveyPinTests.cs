using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Survey.Api.IntegrationTests.Fixtures;
using Survey.Application;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Survey.Api.IntegrationTests.EndToEndFlows
{
    public class SurveyPinTests : IClassFixture<WebAppFactoryFixture>
    {
        private readonly WebAppFactoryFixture _factory;
        private readonly HttpClient _ownerClient;
        private readonly HttpClient _otherClient;

        public SurveyPinTests(WebAppFactoryFixture fixture)
        {
            _factory = fixture;
            _ownerClient = fixture.CreateClient();
            _otherClient = CreateClientFor(2, "3");
        }

        // Helper: build a client authenticated as a different user
        private HttpClient CreateClientFor(int userId, string userType)
        {
            var client = _factory.CreateClient();
            var jwt = GenerateJwt(userId, userType);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            return client;
        }

        private string GenerateJwt(int userId, string userType)
        {
            var settings = _factory.Services.GetRequiredService<JwtSettings>();
            var keyBytes = System.Text.Encoding.UTF8.GetBytes(settings.Secret);
            var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes),
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256
            );
            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: new[]
                {
                    new System.Security.Claims.Claim("UserId",   userId.ToString()),
                    new System.Security.Claims.Claim("UserType", userType)
                },
                expires: System.DateTime.UtcNow.AddMinutes(settings.ExpiryMinutes),
                signingCredentials: creds
            );
            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        }

        // Create + publish a survey with the given PIN (null => public)
        private async Task<DesignedSurveyDto> CreateAndPublish(string pin)
        {
            var dto = new DesignedSurveyDto
            {
                SurveyTitle = "PIN Test",
                SurveyDescription = "desc",
                StartDate = System.DateTime.UtcNow,
                EndDate = System.DateTime.UtcNow.AddDays(1),
                SurveyTypeId = 1,
                PrivateKey = pin,
                UserId = 1,
                Questionnaires = new System.Collections.Generic.List<QuestionnaireDto>()
            };

            // 1) create
            var create = await _ownerClient.PostAsJsonAsync("/api/ExperimenterApp/surveys", dto);
            create.StatusCode.Should().Be(HttpStatusCode.Created);
            var survey = await create.Content.ReadFromJsonAsync<DesignedSurveyDto>();

            // 2) publish
            var pub = await _ownerClient.PostAsync($"/api/ExperimenterApp/surveys/{survey.SurveyId}/publish", null);
            pub.StatusCode.Should().Be(HttpStatusCode.OK);

            return survey;
        }

        [Fact]
        public async Task Owner_CanLoadPrivateWithoutPin()
        {
            var survey = await CreateAndPublish("9999");
            var load = await _ownerClient.GetAsync($"/api/ExperimenteeApp/LoadSurvey/{survey.SurveyId}");
            load.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task OtherUser_CannotLoadPrivateWithoutPin()
        {
            var survey = await CreateAndPublish("9999");
            var load = await _otherClient.GetAsync($"/api/ExperimenteeApp/LoadSurvey/{survey.SurveyId}");
            load.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task OtherUser_CannotLoadPrivateWithWrongPin()
        {
            var survey = await CreateAndPublish("9999");
            _otherClient.DefaultRequestHeaders.Add("X-Survey-Pin", "0000");
            var load = await _otherClient.GetAsync($"/api/ExperimenteeApp/LoadSurvey/{survey.SurveyId}");
            load.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task OtherUser_CanLoadPrivateWithCorrectPin()
        {
            var survey = await CreateAndPublish("9999");
            _otherClient.DefaultRequestHeaders.Add("X-Survey-Pin", "9999");
            var load = await _otherClient.GetAsync($"/api/ExperimenteeApp/LoadSurvey/{survey.SurveyId}");
            load.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task PublicSurvey_OtherUser_CanLoadWithoutPin()
        {
            var survey = await CreateAndPublish(pin: null);
            var load = await _otherClient.GetAsync($"/api/ExperimenteeApp/LoadSurvey/{survey.SurveyId}");
            load.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
