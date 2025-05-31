using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Survey.Api.UnitTests.Controllers.TestHelpers;
using Survey.API.Controllers;
using Survey.Application;
using Survey.Domain.Entities;
using Survey.Infrastructure.Data;
using Xunit;

namespace Survey.Api.UnitTests.Controllers
{
    public class UserAppControllerTests
    {
        private readonly SurveyDbContext _db;
        private readonly UserAppController _sut;

        public UserAppControllerTests()
        {
            _db = ControllerTestFixture.CreateInMemoryDb();
            _sut = new UserAppController(_db);
        }

        [Fact]
        public async void RegisterUser_Valid_ReturnsCreated()
        {
            var user = new User
            {
                UserName = "Alice",
                UserEmail = "alice@example.com",
                UserPassword = "secret",
                UserTypeId = 2
            };

            var result = await _sut.RegisterUser(user);
            result.Should().BeOfType<CreatedAtActionResult>();
            _db.Users.Count().Should().Be(1);
        }

        [Fact]
        public async void RegisterUser_DuplicateEmail_ReturnsBadRequest()
        {
            _db.Users.Add(new User
            {
                UserName = "Bob",
                UserEmail = "bob@example.com",
                UserPassword = "pw",
                UserTypeId = 2
            });
            _db.SaveChanges();

            var dup = new User
            {
                UserName = "Bob2",
                UserEmail = "bob@example.com",
                UserPassword = "pw2",
                UserTypeId = 2
            };
            var result = await _sut.RegisterUser(dup);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async void GetAllUsers_WhenEmpty_ReturnsNotFound()
        {
            var result = await _sut.GetAllUsers();
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async void GetAllUsers_WithData_ReturnsOkAndHidesPasswords()
        {
            _db.Users.AddRange(new[]
            {
                new User { UserName="U1",UserEmail="u1@e",UserPassword="p1",UserTypeId=2 },
                new User { UserName="U2",UserEmail="u2@e",UserPassword="p2",UserTypeId=2 }
            });
            _db.SaveChanges();

            var result = await _sut.GetAllUsers();
            var ok = result as OkObjectResult;
            var list = ok!.Value as List<User>;
            list.Should().HaveCount(2);
            list.All(u => u.UserPassword == "HIDDEN").Should().BeTrue();
        }

        [Fact]
        public async void DeleteUser_Valid_ReturnsOk()
        {
            _db.Users.Add(new User { UserId = 42, UserEmail = "x@e", UserName = "X", UserPassword = "p", UserTypeId = 2 });
            _db.SaveChanges();
            var dto = new UserDto { UserId = 42 };
            var result = await _sut.DeleteUser(dto);
            result.Should().BeOfType<OkObjectResult>();
            _db.Users.Find(42).Should().BeNull();
        }

        [Fact]
        public async void DeleteUser_NotFound_ReturnsNotFound()
        {
            var dto = new UserDto { UserId = 99 };
            var result = await _sut.DeleteUser(dto);
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
