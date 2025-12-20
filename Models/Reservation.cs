using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConferenceRoomBooking.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        
        [Required]
        public DateTime StartTime { get; set; }
        
        [Required]
        public DateTime EndTime { get; set; }
        
        [StringLength(500)]
        public string Purpose { get; set; } = string.Empty;
        
        [Required]
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Foreign key to User
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        // Navigation property
        [ForeignKey("UserId")]
        public AppUser User { get; set; } = null!;
        
        // Foreign key to Room
        [Required]
        public int RoomId { get; set; }
        
        // Navigation property
        [ForeignKey("RoomId")]
        public ConferenceRoom Room { get; set; } = null!;
    }
    
    public enum ReservationStatus
    {
        Pending,    // Waiting for admin approval
        Approved,   // Admin approved
        Rejected,   // Admin rejected
        Cancelled   // User cancelled
    }
}