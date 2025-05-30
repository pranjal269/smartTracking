using System;
using System.Collections.Generic;

namespace SmartTracking.Api.Models
{
    public class HandlerDashboardStats
    {
        public int TotalParcelsToday { get; set; }
        public int PendingScans { get; set; }
        public List<DailyParcelCount> DailyHandledParcels { get; set; }
        public int TotalActiveAlerts { get; set; }
        public List<ParcelStatusCount> StatusCounts { get; set; }
    }

    public class DailyParcelCount
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class ParcelStatusCount
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }

    public class RecentActivity
    {
        public int Id { get; set; }
        public string ParcelId { get; set; }
        public string TrackingNumber { get; set; }
        public string RecipientName { get; set; }
        public string Action { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
    }

    public class HandlerAlert
    {
        public int Id { get; set; }
        public string Type { get; set; }  // e.g., "Delivery Delay", "Missing Scan", etc.
        public string Message { get; set; }
        public int ParcelId { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime Created { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string ResolvedBy { get; set; }
    }

    // Model for assigning handlers to specific locations/facilities
    public class HandlerLocation
    {
        public int Id { get; set; }
        public string HandlerId { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime AssignedDate { get; set; }

        // Navigation property
        public virtual UserEntry Handler { get; set; }
    }

    public class ParcelStatusUpdateModel
    {
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}
