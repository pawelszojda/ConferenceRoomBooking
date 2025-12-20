using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConferenceRoomBooking.Data;
using ConferenceRoomBooking.Models;

namespace ConferenceRoomBooking.Controllers
{
    public class ConferenceRoomsController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public ConferenceRoomsController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET: ConferenceRooms
        // Dostępne dla wszystkich (nawet niezalogowanych)
        public async Task<IActionResult> Index()
        {
            var rooms = await _context.ConferenceRooms
                .Where(r => r.IsAvailable)
                .ToListAsync();
            return View(rooms);
        }
        
        // GET: ConferenceRooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var room = await _context.ConferenceRooms
                .Include(r => r.Reservations)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (room == null)
            {
                return NotFound();
            }
            
            return View(room);
        }
        
        // GET: ConferenceRooms/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        
        // POST: ConferenceRooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Capacity,PricePerHour,Equipment,IsAvailable")] ConferenceRoom room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }
        
        // GET: ConferenceRooms/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var room = await _context.ConferenceRooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }
        
        // POST: ConferenceRooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Capacity,PricePerHour,Equipment,IsAvailable")] ConferenceRoom room)
        {
            if (id != room.Id)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConferenceRoomExists(room.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }
        
        // GET: ConferenceRooms/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var room = await _context.ConferenceRooms
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (room == null)
            {
                return NotFound();
            }
            
            return View(room);
        }
        
        // POST: ConferenceRooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.ConferenceRooms.FindAsync(id);
            if (room != null)
            {
                _context.ConferenceRooms.Remove(room);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        private bool ConferenceRoomExists(int id)
        {
            return _context.ConferenceRooms.Any(e => e.Id == id);
        }
    }
}