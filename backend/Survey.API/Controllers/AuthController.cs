using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Survey.Application;
using Survey.Domain.Entities;
using SurveyDbContext = Survey.Infrastructure.Data.SurveyDbContext;

namespace Survey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SurveyDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public AuthController(SurveyDbContext context, JwtSettings jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (model == null || model.UserEmail == null || model.Password == null)
            {
                return BadRequest("Invalid client request");
            }

            var hasher = new PasswordHasher<string>();
            var hashedPassword = hasher.HashPassword(string.Empty, model.Password);

            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserEmail == model.UserEmail && u.UserPassword == hashedPassword);
            if (user == null) return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        [HttpPost("logout")]
        public Task<IActionResult> Logout()
        {
            // Discard the token at client side.
            return Task.FromResult<IActionResult>(Ok(new { message = "Discard the token at client side" }));
        }


        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserEmail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("UserType", user.UserTypeId.ToString()),
                new Claim("UserName", user.UserName),
                new Claim("UserEmail", user.UserEmail)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
