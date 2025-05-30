using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTracking.Api.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SmartTracking.Api.Data;


namespace SmartTracking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParcelController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ParcelController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Parcel
        // Get all parcels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Parcel>>> GetAllParcels()
        {
            try
            {
                var parcels = await _context.Set<Parcel>()
                    .Include(p => p.Sender)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    message = "Parcels retrieved successfully",
                    data = parcels,
                    count = parcels.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error retrieving parcels",
                    error = ex.Message
                });
            }
        }

        // GET: api/Parcel/{id}
        // Get specific parcel by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Parcel>> GetParcel(string id)
        {
            try
            {
                var parcel = await _context.Set<Parcel>()
                    .Include(p => p.Sender)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (parcel == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Parcel with ID '{id}' not found"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Parcel retrieved successfully",
                    data = parcel
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error retrieving parcel",
                    error = ex.Message
                });
            }
        }

        // GET: api/Parcel/track/{trackingNumber}
        // Track parcel by tracking number
        [HttpGet("track/{trackingNumber}")]
        public async Task<ActionResult<Parcel>> TrackParcel(string trackingNumber)
        {
            try
            {
                var parcel = await _context.Set<Parcel>()
                    .Include(p => p.Sender)
                    .FirstOrDefaultAsync(p => p.TrackingNumber == trackingNumber);

                if (parcel == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"No parcel found with tracking number '{trackingNumber}'"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Parcel tracked successfully",
                    data = parcel
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error tracking parcel",
                    error = ex.Message
                });
            }
        }

        // GET: api/Parcel/sender/{senderId}
        // Get all parcels for a specific sender
        [HttpGet("sender/{senderId}")]
        public async Task<ActionResult<IEnumerable<Parcel>>> GetParcelsBySender(string senderId)
        {
            try
            {
                var parcels = await _context.Set<Parcel>()
                    .Include(p => p.Sender)
                    .Where(p => p.SenderId == senderId) // senderId is string, no need for ToString()


                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    message = $"Parcels for sender retrieved successfully",
                    data = parcels,
                  count = parcels.Count()

                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error retrieving sender's parcels",
                    error = ex.Message
                });
            }
        }

        // POST: api/Parcel
        // Create new parcel
        [HttpPost]
        public async Task<ActionResult<Parcel>> CreateParcel(CreateParcelModel model)
        {
            try
            {
                // Validate the sender exists
                var sender = await _context.Set<UserEntry>().FindAsync(model.SenderId);
                if (sender == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid sender ID"
                    });
                }

                var parcelId = Guid.NewGuid().ToString();
var trackingNumber = GenerateTrackingNumber();

var parcel = new Parcel
{
    Id = parcelId,
    TrackingNumber = trackingNumber,
    RecipientName = model.RecipientName,
    RecipientAddress = model.RecipientAddress,
    Weight = model.Weight,
    Status = "Created",
    SenderId = model.SenderId,  // assuming model.SenderId is string
    CreatedAt = DateTime.UtcNow
};


_context.Set<Parcel>().Add(parcel);
await _context.SaveChangesAsync();


                // Return the created parcel with sender info
                var createdParcel = await _context.Set<Parcel>()
                    .Include(p => p.Sender)
                    .FirstOrDefaultAsync(p => p.Id == parcelId);

                return CreatedAtAction(
                    nameof(GetParcel), 
                    new { id = parcel.Id }, 
                    new
                    {
                        success = true,
                        message = "Parcel created successfully",
                        data = createdParcel
                    });
            }
            catch (Exception ex)
{
    var errorMessage = ex.InnerException?.InnerException?.Message 
                       ?? ex.InnerException?.Message 
                       ?? ex.Message;

    return StatusCode(500, new
    {
        success = false,
        message = "Error creating parcel",
        error = errorMessage
    });
}

        }

        // PUT: api/Parcel/{id}
        // Update existing parcel
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParcel(string id, UpdateParcelModel model)
        {
            try
            {
                var parcel = await _context.Set<Parcel>().FindAsync(id);
                if (parcel == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Parcel with ID '{id}' not found"
                    });
                }

                // Update fields if provided
                if (!string.IsNullOrEmpty(model.RecipientName))
                    parcel.RecipientName = model.RecipientName;
                
                if (!string.IsNullOrEmpty(model.RecipientAddress))
                    parcel.RecipientAddress = model.RecipientAddress;
                
                if (model.Weight.HasValue && model.Weight > 0)
                    parcel.Weight = model.Weight.Value;
                
                if (!string.IsNullOrEmpty(model.Status))
                    parcel.Status = model.Status;
                
                if (model.SpecialInstructions != null)
                    parcel.SpecialInstructions = model.SpecialInstructions;

                parcel.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Return updated parcel with sender info
                var updatedParcel = await _context.Set<Parcel>()
                    .Include(p => p.Sender)
                    .FirstOrDefaultAsync(p => p.Id == id);

                return Ok(new
                {
                    success = true,
                    message = "Parcel updated successfully",
                    data = updatedParcel
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error updating parcel",
                    error = ex.Message
                });
            }
        }

        // PATCH: api/Parcel/{id}/status
        // Update only parcel status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateParcelStatus(string id, [FromBody] ParcelStatusUpdateModel model)
        {
            try
            {
                var parcel = await _context.Set<Parcel>().FindAsync(id);
                if (parcel == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Parcel with ID '{id}' not found"
                    });
                }

                parcel.Status = model.Status;
                parcel.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Parcel status updated successfully",
                    data = new
                    {
                        Id = parcel.Id,
                        TrackingNumber = parcel.TrackingNumber,
                        Status = parcel.Status,
                        UpdatedAt = parcel.UpdatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error updating parcel status",
                    error = ex.Message
                });
            }
        }

        // DELETE: api/Parcel/{id}
        // Delete parcel
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParcel(string id)
        {
            try
            {
                var parcel = await _context.Set<Parcel>().FindAsync(id);
                if (parcel == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Parcel with ID '{id}' not found"
                    });
                }

                _context.Set<Parcel>().Remove(parcel);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Parcel deleted successfully",
                    data = new
                    {
                        Id = id,
                        TrackingNumber = parcel.TrackingNumber,
                        DeletedAt = DateTime.UtcNow
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error deleting parcel",
                    error = ex.Message
                });
            }
        }

        // Helper method to generate tracking number
        private string GenerateTrackingNumber()
        {
            var random = new Random();
            var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var numbers = "0123456789";
            
            var trackingNumber = "ST"; // SmartTracking prefix
            
            // Add 2 random letters
            for (int i = 0; i < 2; i++)
            {
                trackingNumber += letters[random.Next(letters.Length)];
            }
            
            // Add 8 random numbers
            for (int i = 0; i < 8; i++)
            {
                trackingNumber += numbers[random.Next(numbers.Length)];
            }
            
            return trackingNumber;
        }
    }

    // DTOs for request models
    public class CreateParcelModel
    {
        [Required(ErrorMessage = "Recipient name is required")]
        public string RecipientName { get; set; }

        [Required(ErrorMessage = "Recipient address is required")]
        public string RecipientAddress { get; set; }

        [Required(ErrorMessage = "Weight is required")]
        [Range(0.1, 1000, ErrorMessage = "Weight must be between 0.1 and 1000 kg")]
        public decimal Weight { get; set; }

        public string SpecialInstructions { get; set; }

        [Required(ErrorMessage = "Sender ID is required")]
        public string SenderId { get; set; }
    }

    public class UpdateParcelModel
    {
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
        public decimal? Weight { get; set; }
        public string Status { get; set; }
        public string SpecialInstructions { get; set; }
    }
}