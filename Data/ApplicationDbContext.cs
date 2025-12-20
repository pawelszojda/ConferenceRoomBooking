using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ConferenceRoomBooking.Models;

namespace ConferenceRoomBooking.Data
{
    // Inherits from IdentityDbContext to get user authentication tables
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
            
        // DbSet represents a table in the database
        public DbSet<ConferenceRoom> ConferenceRooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Configure relationships (EF Core usually infers these, but explicit is better)
            builder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Reservation>()
                .HasOne(r => r.Room)
                .WithMany(cr => cr.Reservations)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete room if it has reservations
            
            // Seed initial data
            builder.Entity<ConferenceRoom>().HasData(
                new ConferenceRoom
                {
                    Id = 1,
                    Name = "Main Conference Room",
                    Description = "Large room with projector and whiteboard",
                    Capacity = 20,
                    PricePerHour = 50.00m,
                    Equipment = "Projector, Whiteboard, Video Conference System",
                    IsAvailable = true
                },
                new ConferenceRoom
                {
                    Id = 2,
                    Name = "Small Meeting Room",
                    Description = "Cozy room for team meetings",
                    Capacity = 6,
                    PricePerHour = 25.00m,
                    Equipment = "TV Screen, Whiteboard",
                    IsAvailable = true
                },
                new ConferenceRoom
                {
                    Id = 3,
                    Name = "Executive Boardroom",
                    Description = "Premium room for executive meetings",
                    Capacity = 12,
                    PricePerHour = 75.00m,
                    Equipment = "4K Display, Premium Audio System, Catering Area",
                    IsAvailable = true
                }
            );
        }
    }
}