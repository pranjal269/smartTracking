using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartTracking.Api.Models;

public class Parcel
{
    [Key]
    public string Id { get; set; }

    [Required]
    public string TrackingNumber { get; set; }

    [Required]
    public string RecipientName { get; set; }

    [Required]
    public string RecipientAddress { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Weight { get; set; }

    public string Status { get; set; } = "Created";

    public string SpecialInstructions { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [Required]
public string SenderId { get; set; } 


[ForeignKey("SenderId")]
public UserEntry Sender { get; set; }



}
