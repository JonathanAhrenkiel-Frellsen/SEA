using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Survey.Infrastructure.Data;
using Survey.Application;
using Microsoft.Extensions.Logging;
using Moq;

namespace Survey.Api.UnitTests.Controllers.TestHelpers
{
    public static class ControllerTestFixture
    {
        public static SurveyDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<SurveyDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new SurveyDbContext(options);
        }

        public static JwtSettings CreateJwtSettings()
        {
            return new JwtSettings
            {
                Secret = "TestSecretKey1234567890",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpiryMinutes = 60
            };
        }

        // Unused but available if needed
        public static ILogger<T> CreateLogger<T>() => Mock.Of<ILogger<T>>();
    }
}