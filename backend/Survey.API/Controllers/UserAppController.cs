using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Survey.Application;
using Survey.Domain.Entities;
using SurveyDbContext = Survey.Infrastructure.Data.SurveyDbContext;

namespace Survey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAppController : ControllerBase
    {
        private readonly SurveyDbContext _context;

        public UserAppController(SurveyDbContext context)
        {
            _context = context;
        }

        // GET: api/UserApp/GetAllUsers
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();

            if (users == null || users.Count == 0)
            {
                return NotFound("No users found.");
            }
            foreach (var user in users)
            {
                user.UserPassword = "HIDDEN"; // Hide password
            }
            return Ok(users);
        }

        // POST: api/UserApp/SaveUser
        [HttpPost("SaveUser")]
        public async Task<IActionResult> SaveUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User cannot be null.");
            }

            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.UserEmail) || user.UserTypeId <= 0 || string.IsNullOrEmpty(user.UserPassword))
            {
                return BadRequest("User name, email, user type and password are required.");
            }

            if (user.UserId <= 0)
            {
                var searchUser = await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == user.UserEmail);
                if (searchUser != null)
                {
                    return BadRequest("User email already exists.");
                }

                if (user.UserPassword == null || user.UserPassword == string.Empty)
                {
                    return BadRequest("User password is required.");
                }

                // Create new user
                _context.Entry(user).State = EntityState.Added;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    return BadRequest($"Error saving user: {ex.Message}");
                }

                return CreatedAtAction(nameof(GetAllUsers), new { id = user.UserId }, user);
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);

            if (existingUser != null)
            {
                // Update existing user
                existingUser.UserName = user.UserName;
                existingUser.UserEmail = user.UserEmail;

                if (user.UserPassword != null && user.UserPassword != string.Empty && user.UserPassword != "HIDDEN")
                {
                    existingUser.UserPassword = user.UserPassword;
                }
                existingUser.UserTypeId = user.UserTypeId;
                //_context.Entry(existingUser).State = EntityState.Modified;

                try
                { 
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return BadRequest($"Error updating user: {ex.Message}");
                }

                existingUser.UserPassword = "HIDDEN"; // Hide password

                return Ok(existingUser);
            }

            else
            {
                return BadRequest("Specific user ID doesn't exist");
            }
        }

        // POST: api/UserApp/GetUserByEmailId
        [HttpPost("GetUserByEmailId")]
        public async Task<IActionResult> GetUserByEmailId([FromBody] UserDto user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserEmail) || user.UserEmail == null)
            {
                return BadRequest("Invalid user email data.");
            }
            var existingUser = await _context.Users.FirstOrDefaultAsync(s => s.UserEmail == user.UserEmail);
            if (existingUser == null)
            {
                return NotFound("User not found.");
            }
            existingUser.UserPassword = "HIDDEN"; // Hide password

            return Ok(existingUser);
        }

        // POST: api/UserApp/GetUserById
        [HttpPost("GetUserById")]
        public async Task<IActionResult> GetUserById([FromBody] UserDto user)
        {
            if (user == null || user.UserId <= 0)
            {
                return BadRequest("Invalid user data.");
            }
            var existingUser = await _context.Users.FirstOrDefaultAsync(s => s.UserId == user.UserId);
            if (existingUser == null)
            {
                return NotFound("User not found.");
            }
            existingUser.UserPassword = "HIDDEN"; // Hide password
            return Ok(existingUser);
        }

        // POST: api/UserApp/DeleteUser
        [HttpPost("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromBody] UserDto user)
        {
            if (user == null || user.UserId == null)
            {
                return BadRequest("Invalid user data.");
            }
            var existingUser = await _context.Users.FirstOrDefaultAsync(s => s.UserId == user.UserId);
            if (existingUser == null)
            {
                return NotFound("User not found.");
            }
            _context.Users.Remove(existingUser);
            await _context.SaveChangesAsync();
            return Ok("User deleted successfully.");
        }
    }
}
