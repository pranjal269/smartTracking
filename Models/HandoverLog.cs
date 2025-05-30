using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SmartTracking.Api.Models
{
    public class HandoverLog
    {
        public int Id { get; set; }

        [Required]
        public string ParcelId { get; set; }
        
        [JsonIgnore]
        [ForeignKey("ParcelId")]
        public virtual Parcel Parcel { get; set; }

        public string Status { get; set; }
        
        [Required]
        public string HandlerId { get; set; }
        
        [JsonIgnore]
        [ForeignKey("HandlerId")]
        public virtual UserEntry Handler { get; set; }
        
        [Required]
        public DateTime HandoverTime { get; set; }
        
        public string Notes { get; set; }
    }
}