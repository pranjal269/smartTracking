using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTracking.Api.Models
{
    public class Parcel
    {
        public string Id { get; set; }

        public string TrackingNumber { get; set; }

        [Required]
        public string RecipientName { get; set; }

        [Required]
        public string RecipientAddress { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Weight { get; set; }

        // Basic status - keeping it simple
        public string Status { get; set; } = "Created";  // Created, In-Transit, Delivered
        
        // Additional details
        public string SpecialInstructions { get; set; }
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Sender relationship
        public string SenderId { get; set; }
        
        [ForeignKey("SenderId")]
        public UserEntry Sender { get; set; }
    }
}