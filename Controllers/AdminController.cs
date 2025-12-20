using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConferenceRoomBooking.Data;
using ConferenceRoomBooking.Models;

namespace ConferenceRoomBooking.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var pendingReservations = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.User)
                .Where(r => r.Status == ReservationStatus.Pending)
                .OrderBy(r => r.StartTime)
                .ToListAsync();
            
            var approvedReservations = await _context.Reservations
                .Where(r => r.Status == ReservationStatus.Approved)
                .CountAsync();
            
            var totalRooms = await _context.ConferenceRooms.CountAsync();
            var availableRooms = await _context.ConferenceRooms
                .Where(r => r.IsAvailable)
                .CountAsync();
            
            var totalUsers = await _context.Users.CountAsync();
            
            ViewBag.PendingReservations = pendingReservations;
            ViewBag.ApprovedReservationsCount = approvedReservations;
            ViewBag.TotalRooms = totalRooms;
            ViewBag.AvailableRooms = availableRooms;
            ViewBag.TotalUsers = totalUsers;
            
            return View();
        }
        
        // GET: Admin/Reservations
        public async Task<IActionResult> Reservations(string status = "All")
        {
            var query = _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.User)
                .AsQueryable();
            
            if (status != "All")
            {
                if (Enum.TryParse<ReservationStatus>(status, out var statusEnum))
                {
                    query = query.Where(r => r.Status == statusEnum);
                }
            }
            
            var reservations = await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            
            ViewBag.CurrentStatus = status;
            return View(reservations);
        }
        
        // POST: Admin/ApproveReservation/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            
            if (reservation == null)
            {
                return NotFound();
            }
            
            // Sprawdź czy sala jest dostępna w tym czasie
            var conflicts = await _context.Reservations
                .Where(r => r.Id != id
                            && r.RoomId == reservation.RoomId
                            && r.Status == ReservationStatus.Approved
                            && ((reservation.StartTime >= r.StartTime && reservation.StartTime < r.EndTime)
                                || (reservation.EndTime > r.StartTime && reservation.EndTime <= r.EndTime)
                                || (reservation.StartTime <= r.StartTime && reservation.EndTime >= r.EndTime)))
                .ToListAsync();
            
            if (conflicts.Any())
            {
                TempData["ErrorMessage"] = "Nie można zatwierdzić - sala jest już zarezerwowana w tym czasie";
                return RedirectToAction(nameof(Dashboard));
            }
            
            reservation.Status = ReservationStatus.Approved;
            _context.Update(reservation);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Rezerwacja została zatwierdzona";
            return RedirectToAction(nameof(Dashboard));
        }
        
        // POST: Admin/RejectReservation/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            
            if (reservation == null)
            {
                return NotFound();
            }
            
            reservation.Status = ReservationStatus.Rejected;
            _context.Update(reservation);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Rezerwacja została odrzucona";
            return RedirectToAction(nameof(Dashboard));
        }
        
        // GET: Admin/ReservationDetails/5
        public async Task<IActionResult> ReservationDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (reservation == null)
            {
                return NotFound();
            }
            
            return View(reservation);
        }
        
        // GET: Admin/Statistics
        public async Task<IActionResult> Statistics()
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);
            
            // Statystyki tego miesiąca
            var monthlyReservations = await _context.Reservations
                .Where(r => r.CreatedAt >= startOfMonth && r.CreatedAt < endOfMonth)
                .GroupBy(r => r.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();
            
            // Najpopularniejsze sale
            var popularRooms = await _context.Reservations
                .Include(r => r.Room)
                .Where(r => r.Status == ReservationStatus.Approved)
                .GroupBy(r => r.Room)
                .Select(g => new { Room = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();
            
            ViewBag.MonthlyReservations = monthlyReservations;
            ViewBag.PopularRooms = popularRooms;
            ViewBag.CurrentMonth = startOfMonth.ToString("MMMM yyyy");
            
            return View();
        }
    }
}