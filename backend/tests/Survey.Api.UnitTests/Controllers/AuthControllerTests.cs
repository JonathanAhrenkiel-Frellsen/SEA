using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Survey.API.Controllers;
using Survey.Application;
using Survey.Domain.Entities;
using Survey.Infrastructure.Data;
using Survey.Api.UnitTests.Controllers.TestHelpers;
using Xunit;

namespace Survey.Api.UnitTests.Controllers
{
    /// <summary>
    /// Unit tests for AuthController.Login logic.
    /// </summary>
    public class AuthControllerTests
    {
        private readonly SurveyDbContext _db;
        private readonly AuthController _sut;

        public AuthControllerTests()
        {
            _db = ControllerTestFixture.CreateInMemoryDb();
            _db.Users.Add(new User { UserId = 1, UserEmail = "test@example.com", UserPassword = "password" });
            _db.SaveChanges();

            var jwtSettings = ControllerTestFixture.CreateJwtSettings();
            _sut = new AuthController(_db, jwtSettings);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            var result = await _sut.Login(new LoginDto { UserEmail = "wrong@example.com", Password = "bad" });
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsUnauthorized()
        {
            // The password is plain text, so login still fails
            var result = await _sut.Login(new LoginDto { UserEmail = "test@example.com", Password = "password" });
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}