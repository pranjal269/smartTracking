using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;

namespace SmartTracking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger<BaseController> _logger;

        public BaseController(ILogger<BaseController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets the current user ID from the claims principal
        /// </summary>
        /// <returns>User ID or null if not authenticated</returns>
        protected string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        /// <summary>
        /// Gets the current user role from the claims principal
        /// </summary>
        /// <returns>User role or null if not authenticated</returns>
        protected string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value;
        }

        /// <summary>
        /// Creates a standard success response
        /// </summary>
        /// <param name="data">The data to return</param>
        /// <param name="message">Optional success message</param>
        /// <returns>An IActionResult with the standardized response</returns>
        protected IActionResult Success(object data = null, string message = "Operation completed successfully")
        {
            return Ok(new
            {
                success = true,
                message,
                data
            });
        }

        /// <summary>
        /// Creates a standard error response
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>An IActionResult with the standardized error response</returns>
        protected IActionResult Error(string message, int statusCode = 400)
        {
            return StatusCode(statusCode, new
            {
                success = false,
                message
            });
        }

        /// <summary>
        /// Creates a standard exception response and logs the error
        /// </summary>
        /// <param name="ex">The exception that occurred</param>
        /// <param name="customMessage">Optional custom message to return to the client</param>
        /// <returns>An IActionResult with the standardized error response</returns>
        protected IActionResult HandleException(Exception ex, string customMessage = null)
        {
            _logger.LogError(ex, ex.Message);
            
            return StatusCode(500, new
            {
                success = false,
                message = customMessage ?? "An error occurred while processing your request."
            });
        }
    }
}
