using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConferenceRoomBooking.Data;
using ConferenceRoomBooking.Models;
using ConferenceRoomBooking.ViewModels;

namespace ConferenceRoomBooking.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        
        public ReservationsController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        // GET: Reservations - Moje rezerwacje
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            
            var reservations = await _context.Reservations
                .Include(r => r.Room)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
                
            return View(reservations);
        }
        
        // GET: Reservations/Create
        public async Task<IActionResult> Create(int? roomId)
        {
            var rooms = await _context.ConferenceRooms
                .Where(r => r.IsAvailable)
                .ToListAsync();
                
            ViewBag.Rooms = new SelectList(rooms, "Id", "Name", roomId);
            
            var model = new CreateReservationViewModel();
            if (roomId.HasValue)
            {
                model.RoomId = roomId.Value;
            }
            
            return View(model);
        }
        
        // POST: Reservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReservationViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Walidacja dat
                if (model.StartTime <= DateTime.Now)
                {
                    ModelState.AddModelError("StartTime", "Data rozpoczęcia musi być w przyszłości");
                }
                else if (model.EndTime <= model.StartTime)
                {
                    ModelState.AddModelError("EndTime", "Data zakończenia musi być późniejsza niż data rozpoczęcia");
                }
                else
                {
                    // Sprawdź czy sala jest wolna w tym czasie
                    var isRoomAvailable = !await _context.Reservations
                        .AnyAsync(r => r.RoomId == model.RoomId 
                                    && r.Status != ReservationStatus.Cancelled
                                    && r.Status != ReservationStatus.Rejected
                                    && ((model.StartTime >= r.StartTime && model.StartTime < r.EndTime)
                                        || (model.EndTime > r.StartTime && model.EndTime <= r.EndTime)
                                        || (model.StartTime <= r.StartTime && model.EndTime >= r.EndTime)));
                    
                    if (!isRoomAvailable)
                    {
                        ModelState.AddModelError("", "Sala jest już zarezerwowana w tym czasie");
                    }
                    else
                    {
                        var userId = _userManager.GetUserId(User);
                        
                        var reservation = new Reservation
                        {
                            StartTime = model.StartTime,
                            EndTime = model.EndTime,
                            Purpose = model.Purpose,
                            RoomId = model.RoomId,
                            UserId = userId!,
                            Status = ReservationStatus.Pending,
                            CreatedAt = DateTime.Now
                        };
                        
                        _context.Add(reservation);
                        await _context.SaveChangesAsync();
                        
                        TempData["SuccessMessage"] = "Rezerwacja została utworzona i oczekuje na zatwierdzenie przez administratora";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            
            // Jeśli walidacja nie przeszła, przeładuj listę sal
            var rooms = await _context.ConferenceRooms
                .Where(r => r.IsAvailable)
                .ToListAsync();
            ViewBag.Rooms = new SelectList(rooms, "Id", "Name", model.RoomId);
            
            return View(model);
        }
        
        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.Id == id);
                
            if (reservation == null)
            {
                return NotFound();
            }
            
            var userId = _userManager.GetUserId(User);
            
            // Tylko właściciel może edytować
            if (reservation.UserId != userId)
            {
                return Forbid();
            }
            
            // Nie można edytować zatwierdzonej lub odrzuconej rezerwacji
            if (reservation.Status == ReservationStatus.Approved || reservation.Status == ReservationStatus.Rejected)
            {
                TempData["ErrorMessage"] = "Nie można edytować zatwierdzonej lub odrzuconej rezerwacji";
                return RedirectToAction(nameof(Index));
            }
            
            var model = new EditReservationViewModel
            {
                Id = reservation.Id,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                Purpose = reservation.Purpose,
                RoomId = reservation.RoomId,
                RoomName = reservation.Room.Name
            };
            
            return View(model);
        }
        
        // POST: Reservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditReservationViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }
            
            var reservation = await _context.Reservations.FindAsync(id);
            
            if (reservation == null)
            {
                return NotFound();
            }
            
            var userId = _userManager.GetUserId(User);
            
            if (reservation.UserId != userId)
            {
                return Forbid();
            }
            
            if (ModelState.IsValid)
            {
                if (model.StartTime <= DateTime.Now)
                {
                    ModelState.AddModelError("StartTime", "Data rozpoczęcia musi być w przyszłości");
                }
                else if (model.EndTime <= model.StartTime)
                {
                    ModelState.AddModelError("EndTime", "Data zakończenia musi być późniejsza niż data rozpoczęcia");
                }
                else
                {
                    // Sprawdź czy sala jest wolna (z wyłączeniem obecnej rezerwacji)
                    var isRoomAvailable = !await _context.Reservations
                        .AnyAsync(r => r.Id != id
                                    && r.RoomId == model.RoomId 
                                    && r.Status != ReservationStatus.Cancelled
                                    && r.Status != ReservationStatus.Rejected
                                    && ((model.StartTime >= r.StartTime && model.StartTime < r.EndTime)
                                        || (model.EndTime > r.StartTime && model.EndTime <= r.EndTime)
                                        || (model.StartTime <= r.StartTime && model.EndTime >= r.EndTime)));
                    
                    if (!isRoomAvailable)
                    {
                        ModelState.AddModelError("", "Sala jest już zarezerwowana w tym czasie");
                    }
                    else
                    {
                        reservation.StartTime = model.StartTime;
                        reservation.EndTime = model.EndTime;
                        reservation.Purpose = model.Purpose;
                        reservation.Status = ReservationStatus.Pending; // Reset do Pending po edycji
                        
                        _context.Update(reservation);
                        await _context.SaveChangesAsync();
                        
                        TempData["SuccessMessage"] = "Rezerwacja została zaktualizowana";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            
            var room = await _context.ConferenceRooms.FindAsync(model.RoomId);
            model.RoomName = room?.Name ?? "";
            
            return View(model);
        }
        
        // POST: Reservations/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            
            if (reservation == null)
            {
                return NotFound();
            }
            
            var userId = _userManager.GetUserId(User);
            
            if (reservation.UserId != userId)
            {
                return Forbid();
            }
            
            reservation.Status = ReservationStatus.Cancelled;
            _context.Update(reservation);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Rezerwacja została anulowana";
            return RedirectToAction(nameof(Index));
        }
        
        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
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
            
            var userId = _userManager.GetUserId(User);
            
            // Tylko właściciel lub admin może zobaczyć szczegóły
            if (reservation.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            
            return View(reservation);
        }
    }
}