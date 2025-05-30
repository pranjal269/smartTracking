using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartTracking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParcelController : ControllerBase
    {
        // Mock data store
        private static readonly List<Parcel> _parcels = new List<Parcel>();

        [HttpPost]
        public IActionResult CreateParcel([FromBody] CreateParcelRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var parcel = new Parcel
            {
                Id = Guid.NewGuid().ToString(),
                TrackingNumber = GenerateTrackingNumber(),
                RecipientName = request.RecipientName,
                RecipientAddress = request.RecipientAddress,
                Status = "Created",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _parcels.Add(parcel);

            return Ok(new { success = true, data = parcel });
        }

        [HttpGet("mine")]
        public IActionResult GetMyParcels()
        {
            // In demo version, return all parcels
            return Ok(new { success = true, data = _parcels });
        }

        [HttpGet("track/{trackingNumber}")]
        public IActionResult TrackParcel(string trackingNumber)
        {
            var parcel = _parcels.Find(p => p.TrackingNumber == trackingNumber);
            
            if (parcel == null)
            {
                return NotFound(new { success = false, message = "Parcel not found" });
            }

            return Ok(new { success = true, data = parcel });
        }

        private string GenerateTrackingNumber()
        {
            // Format: ST-YYYYMMDD-XXXX
            var date = DateTime.UtcNow.ToString("yyyyMMdd");
            var random = new Random().Next(1000, 9999).ToString("D4");
            return $"ST-{date}-{random}";
        }
    }

    public class CreateParcelRequest
    {
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
    }

    public class Parcel
    {
        public string Id { get; set; }
        public string TrackingNumber { get; set; }
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CurrentLocation { get; set; }
    }
}