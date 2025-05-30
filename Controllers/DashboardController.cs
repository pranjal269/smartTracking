using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTracking.Api.Models;
using SmartTracking.Api.Data;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace SmartTracking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/dashboard/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                // Get current date
                var today = DateTime.UtcNow.Date;
                
                // Total parcels
                var totalParcels = await _context.Parcels.CountAsync();
                
                // Parcels created today
                var parcelsToday = await _context.Parcels
                    .CountAsync(p => p.CreatedAt.Date == today);
                
                // Parcels by status
                var statusCounts = await _context.Parcels
                    .GroupBy(p => p.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToListAsync();

                return Ok(new
                {
                    TotalParcels = totalParcels,
                    ParcelsToday = parcelsToday,
                    StatusBreakdown = statusCounts
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving dashboard stats", error = ex.Message });
            }
        }

        // GET: api/dashboard/recent
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentParcels(int limit = 10)
        {
            try
            {
                var recentParcels = await _context.Parcels
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(limit)
                    .ToListAsync();

                return Ok(recentParcels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving recent parcels", error = ex.Message });
            }
        }
    }
}
