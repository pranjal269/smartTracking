using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartTracking.Api.Models;

namespace SmartTracking.Api.Data.Repository
{
    // Simplified HandlerRepository for the basic demo
    public class HandlerRepository : IHandlerRepository
    {
        private readonly ApplicationDbContext _context;

        public HandlerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HandlerDashboardStats> GetHandlerDashboardStats(string handlerId, int daysForTrend = 7)
        {
            // Simplified version just for demo
            var stats = new HandlerDashboardStats
            {
                TotalParcelsToday = 0,
                PendingScans = 0,
                DailyHandledParcels = new List<DailyParcelCount>(),
                TotalActiveAlerts = 0,
                StatusCounts = new List<ParcelStatusCount>()
            };
            
            // Add some demo status counts
            string[] demoStatuses = new[] { "Created", "In-Transit", "Delivered" };
            foreach (var status in demoStatuses)
            {
                stats.StatusCounts.Add(new ParcelStatusCount 
                { 
                    Status = status, 
                    Count = await _context.Parcels.CountAsync(p => p.Status == status) 
                });
            }
            
            return stats;
        }

        public async Task<List<Parcel>> GetPendingScanParcels(string handlerId, string location, int limit = 10)
        {
            // Simplified version - just return any parcels that aren't delivered
            return await _context.Parcels
                .Where(p => p.Status != "Delivered")
                .OrderByDescending(p => p.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<RecentActivity>> GetHandlerRecentActivity(string handlerId, int limit = 10)
        {
            // Get recent handover logs for this handler
            var recentLogs = await _context.HandoverLogs
                .Where(h => h.HandlerId == handlerId)
                .OrderByDescending(h => h.HandoverTime)
                .Take(limit)
                .Select(h => new
                {
                    Log = h,
                    ParcelId = h.ParcelId
                })
                .ToListAsync();

            // Get the related parcels
            var parcelIds = recentLogs.Select(r => r.ParcelId).Distinct().ToList();
            var parcels = await _context.Parcels
                .Where(p => parcelIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p);

            // Create the activity list
            var activities = new List<RecentActivity>();
            foreach (var log in recentLogs)
            {
                if (parcels.TryGetValue(log.ParcelId, out var parcel))
                {
                    activities.Add(new RecentActivity
                    {
                        Id = log.Log.Id,
                        ParcelId = parcel.Id,
                        TrackingNumber = parcel.TrackingNumber,
                        RecipientName = parcel.RecipientName,
                        Action = log.Log.Status,
                        Status = parcel.Status,
                        Timestamp = log.Log.HandoverTime,
                        Notes = log.Log.Notes
                    });
                }
            }

            return activities;
        }

        public async Task<bool> UpdateParcelStatus(string parcelId, string handlerId, string status, string location = null, string notes = null)
        {
            // Find the parcel
            var parcel = await _context.Parcels.FindAsync(parcelId);
            if (parcel == null)
                return false;

            // Update parcel status
            parcel.Status = status;
            parcel.UpdatedAt = DateTime.UtcNow;

            // Create handover log
            var handoverLog = new HandoverLog
            {
                ParcelId = parcel.Id,
                Status = status,
                HandlerId = handlerId,
                HandoverTime = DateTime.UtcNow,
                Notes = notes ?? $"Status updated to {status}"
            };

            _context.HandoverLogs.Add(handoverLog);
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<HandlerAlert>> GetActiveAlerts(string handlerId)
        {
            return await _context.HandlerAlerts
                .Where(a => !a.IsResolved)
                .OrderByDescending(a => a.Created)
                .ToListAsync();
        }

        public async Task<HandlerAlert> CreateAlert(HandlerAlert alert)
        {
            alert.Created = DateTime.UtcNow;
            alert.IsResolved = false;
            
            _context.HandlerAlerts.Add(alert);
            await _context.SaveChangesAsync();
            
            return alert;
        }

        public async Task<bool> ResolveAlert(int alertId, string resolvedBy)
        {
            var alert = await _context.HandlerAlerts.FindAsync(alertId);
            if (alert == null)
                return false;

            alert.IsResolved = true;
            alert.ResolvedAt = DateTime.UtcNow;
            alert.ResolvedBy = resolvedBy;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<HandlerLocation> GetHandlerLocation(string handlerId)
        {
            return await _context.HandlerLocations
                .FirstOrDefaultAsync(l => l.HandlerId == handlerId && l.IsActive);
        }
    }
}
