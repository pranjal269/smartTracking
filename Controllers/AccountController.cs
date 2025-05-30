using Microsoft.AspNetCore.Mvc;
using SmartTracking.Api.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SmartTracking.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace SmartTracking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _context.UserEntries.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                return BadRequest("Invalid email or password");
            }

            var hashedPassword = HashPassword(model.Password);
            if (user.Password != hashedPassword)
            {
                return BadRequest("Invalid email or password");
            }

            return Ok(new
            {
                Success = true,
                Message = "Login successful",
                email = user.Email,
                name = user.FullName,
                role = user.Role
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Since we're not using Identity, just return success
            return Ok(new { message = "Logged out successfully" });
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}