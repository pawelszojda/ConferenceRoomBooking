using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Models
{
    public class ConferenceRoom
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Room name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Range(1, 100, ErrorMessage = "Capacity must be between 1 and 100")]
        public int Capacity { get; set; }
        
        [Required]
        [Range(0, 10000, ErrorMessage = "Price must be positive")]
        public decimal PricePerHour { get; set; }
        
        // Equipment available (projector, whiteboard, etc.)
        public string Equipment { get; set; } = string.Empty;
        
        public bool IsAvailable { get; set; } = true;
        
        // Navigation property - one room can have many reservations
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}