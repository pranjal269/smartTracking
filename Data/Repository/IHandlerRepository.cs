using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartTracking.Api.Models;

namespace SmartTracking.Api.Data.Repository
{
    public interface IHandlerRepository
    {
        // Dashboard statistics
        Task<HandlerDashboardStats> GetHandlerDashboardStats(string handlerId, int daysForTrend = 7);
        
        // Parcel operations
        Task<List<Parcel>> GetPendingScanParcels(string handlerId, string location, int limit = 10);
        Task<List<RecentActivity>> GetHandlerRecentActivity(string handlerId, int limit = 10);
        Task<bool> UpdateParcelStatus(string parcelId, string handlerId, string status, string location = null, string notes = null);
        
        // Alert operations
        Task<List<HandlerAlert>> GetActiveAlerts(string handlerId);
        Task<HandlerAlert> CreateAlert(HandlerAlert alert);
        Task<bool> ResolveAlert(int alertId, string resolvedBy);
        
        // Location operations
        Task<HandlerLocation> GetHandlerLocation(string handlerId);
    }
}
