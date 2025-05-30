using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTracking.Api.Models;
using SmartTracking.Api.Data;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace SmartTracking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Add authorization by default
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create user
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterModel model)
        {
            // 1. Check for existing email
            if (await _context.UserEntries.AnyAsync(u => u.Email == model.Email))
            {
                return BadRequest("Email address is already registered");
            }

            // 2. Validate role
            if (!IsValidRole(model.Role))
            {
                return BadRequest("Invalid role. Role must be 'Handler', 'Sender', or 'Admin'");
            }

            try
            {
                // Hash the password
                string hashedPassword = HashPassword(model.Password);

                // Create new user entry
                var userEntry = new UserEntry
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = model.Name,
                    Email = model.Email,
                    Password = hashedPassword,
                    Role = model.Role,
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserEntries.Add(userEntry);
                await _context.SaveChangesAsync();

                // Return success response
                return Ok(new
                {
                    message = "Registration successful",
                    user = new
                    {
                        id = userEntry.Id,
                        name = userEntry.FullName,
                        email = userEntry.Email,
                        role = userEntry.Role
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new[] { ex.Message });
            }
        }

        // Get all handlers
        [HttpGet("handlers")]
        public async Task<IActionResult> GetHandlers()
        {
            var handlers = await _context.UserEntries
                .Where(u => u.Role == UserRoles.Handler)
                .Select(u => new { u.Id, UserName = u.FullName, u.Email, u.Role })
                .ToListAsync();
            return Ok(handlers);
        }

        // Get all senders
        [HttpGet("senders")]
        public async Task<IActionResult> GetSenders()
        {
            var senders = await _context.UserEntries
                .Where(u => u.Role == UserRoles.Sender)
                .Select(u => new { u.Id, UserName = u.FullName, u.Email, u.Role })
                .ToListAsync();
            return Ok(senders);
        }

        // Get all admins
        [HttpGet("admins")]
        public async Task<IActionResult> GetAdmins()
        {
            var admins = await _context.UserEntries
                .Where(u => u.Role == UserRoles.Admin)
                .Select(u => new { u.Id, UserName = u.FullName, u.Email, u.Role })
                .ToListAsync();
            return Ok(admins);
        }

        // Get user by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _context.UserEntries.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(new { user.Id, UserName = user.FullName, user.Email, user.Role });
        }

        // Update user
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserModel model)
        {
            var existingUser = await _context.UserEntries.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(model.Name))
                existingUser.FullName = model.Name;

            if (!string.IsNullOrEmpty(model.Email))
                existingUser.Email = model.Email;

            if (!string.IsNullOrEmpty(model.Role) && IsValidRole(model.Role))
                existingUser.Role = model.Role;

            if (!string.IsNullOrEmpty(model.Password))
            {
                existingUser.Password = HashPassword(model.Password);
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { existingUser.Id, UserName = existingUser.FullName, existingUser.Email, existingUser.Role });
            }
            catch (Exception ex)
            {
                return BadRequest(new[] { ex.Message });
            }
        }

        // Delete user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.UserEntries.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.UserEntries.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully" });
        }

        private bool IsValidRole(string role)
        {
            return role == UserRoles.Handler ||
                   role == UserRoles.Sender ||
                   role == UserRoles.Admin;
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



