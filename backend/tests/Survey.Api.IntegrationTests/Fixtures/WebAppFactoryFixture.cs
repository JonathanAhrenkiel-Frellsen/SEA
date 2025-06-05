using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Survey.API;
using Survey.Application;
using Survey.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace Survey.Api.IntegrationTests.Fixtures
{
    public class WebAppFactoryFixture : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // 1) Tell it to use the Integration settings file
            builder.UseEnvironment("Integration");

            // 2) Let Program.cs handle the EF registration—
            //    no need to swap providers here anymore.
        }

        public new HttpClient CreateClient()
        {
            var client = base.CreateClient();

            // Generate and attach a JWT on every client
            using var scope = Services.CreateScope();
            var jwtSettings = scope.ServiceProvider.GetRequiredService<JwtSettings>();
            var token = GenerateJwt(jwtSettings, userId: 1, userType: "1");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        public HttpClient CreateAnonymousClient()
        {
            return base.CreateClient();
        }

        private static string GenerateJwt(JwtSettings settings, int userId, string userType)
        {
            var key = Encoding.UTF8.GetBytes(settings.Secret);
            var claims = new[]
            {
                new Claim("UserId", userId.ToString()),
                new Claim("UserType", userType)
            };
            var creds = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            );
            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(settings.ExpiryMinutes),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
