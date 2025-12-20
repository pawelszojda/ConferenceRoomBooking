using Microsoft.AspNetCore.Identity;

namespace ConferenceRoomBooking.Models
{
    // Extends IdentityUser to add custom properties if needed
    public class AppUser : IdentityUser
    {
        // Navigation property - one user can have many reservations
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}